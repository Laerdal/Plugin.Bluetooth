using System.Runtime.InteropServices;
using System.Threading.Channels;

using Bluetooth.Maui.Platforms.Win.Exceptions;
using Bluetooth.Maui.Platforms.Win.Logging;
using Bluetooth.Maui.Platforms.Win.Scanning.Factories;

namespace Bluetooth.Maui.Platforms.Win.Scanning;

/// <summary>
///     Windows implementation of the Bluetooth scanner using Windows.Devices.Bluetooth APIs.
/// </summary>
/// <remarks>
///     This implementation uses <see cref="BluetoothLEAdvertisementWatcher" /> to monitor BLE advertisements.
///     <seealso href="https://learn.microsoft.com/en-us/uwp/api/windows.devices.bluetooth.advertisement.bluetoothleadvertisementwatcher">BluetoothLEAdvertisementWatcher</seealso>
///     <seealso href="https://learn.microsoft.com/en-us/uwp/api/windows.devices.bluetooth.advertisement.bluetoothleadvertisementreceivedeventargs">BluetoothLEAdvertisementReceivedEventArgs</seealso>
/// </remarks>
public class WindowsBluetoothScanner : BaseBluetoothScanner, NativeObjects.BluetoothLeAdvertisementWatcherWrapper.IBluetoothLeAdvertisementWatcherProxyDelegate
{
    private NativeObjects.BluetoothLeAdvertisementWatcherWrapper? _watcher;

    private readonly ITicker _ticker;
    private readonly IBluetoothRemoteDeviceFactory _deviceFactory;

    /// <summary>
    ///     Gets the advertisement watcher wrapper, creating it lazily with this scanner as the delegate.
    /// </summary>
    private NativeObjects.BluetoothLeAdvertisementWatcherWrapper Watcher =>
        _watcher ??= new NativeObjects.BluetoothLeAdvertisementWatcherWrapper(this, _ticker);

    #region Scan-Response Merging (ADR 0003)

    private bool _mergeScanResponses;
    private TimeSpan _scanResponseMergeWindow = TimeSpan.FromMilliseconds(500);

    /// <summary>
    ///     Guards <see cref="_session" />, <see cref="_pendingAdvertisements" /> and the remembered scan responses.
    ///     It is never held while a consumer callback runs.
    /// </summary>
    private readonly object _pendingGate = new();

    /// <summary>
    ///     Primary (ADV) advertisements awaiting either a correlated scan response or their merge
    ///     timeout, keyed by hex Bluetooth address, at most one per address. Only populated when
    ///     <c>WindowsScanningOptions.MergeScanResponses</c> is enabled.
    /// </summary>
    private readonly Dictionary<string, PendingAdvertisement> _pendingAdvertisements = new();

    // A pending advertisement is identified by reference: the timer that resolves it must not resolve a newer one for the same address.
    private sealed class PendingAdvertisement(BluetoothLEAdvertisementReceivedEventArgs args)
    {
        public BluetoothLEAdvertisementReceivedEventArgs Args { get; } = args;

        public Timer? Timer { get; set; }

        /// <summary>Whether the advertisement already had a service UUID match when it was received, before merging held it back.</summary>
        public bool MatchedWhenReceived { get; init; }
    }

    /// <summary>
    ///     What a scan delivers while merging, in the order it was decided. A single consumer, <see cref="ProcessDeliveriesAsync" />,
    ///     runs them one at a time, so deliveries for an address never overlap and an older advertisement cannot overwrite a newer one,
    ///     whichever thread (native callback or merge timer) produced them.
    /// </summary>
    private abstract record Delivery;

    private sealed record AdvertisementDelivery(PendingAdvertisement Pending, BluetoothLEAdvertisementReceivedEventArgs? ScanResponse) : Delivery;

    private sealed record ScanResponseDelivery(BluetoothLEAdvertisementReceivedEventArgs ScanResponse) : Delivery;

    // The longest a stop waits for the deliveries still queued, whatever timeout the caller gives.
    private static readonly TimeSpan MaxDrainWait = TimeSpan.FromSeconds(5);

    // The most deliveries a scan holds while its consumer is busy; past it the oldest are dropped, so a callback that blocks cannot make
    // the scanner grow without limit.
    private const int MaxQueuedDeliveries = 2048;

    /// <summary>
    ///     One scan's merge machinery: its queue and its consumer. A scan that is stopped, fails to start or is stopped by the system
    ///     closes its session; a session that is retired (a consumer callback that would not return) skips what is still queued, so
    ///     nothing from it reaches a later scan.
    /// </summary>
    private sealed class MergeSession
    {
        public Channel<Delivery> Deliveries { get; } = Channel.CreateBounded<Delivery>(
            new BoundedChannelOptions(MaxQueuedDeliveries) { SingleReader = true, FullMode = BoundedChannelFullMode.DropOldest },
            static dropped =>
            {
                if (dropped is AdvertisementDelivery advertisement)
                {
                    advertisement.Pending.Timer?.Dispose();
                }
            });

        public Task Loop { get; set; } = Task.CompletedTask;

        public volatile bool Retired;
    }

    // The session of the running merging scan, or null when none runs: null is what keeps a late callback from buffering anything.
    private MergeSession? _session;

    private TimeSpan _scanResponseMaxAge = TimeSpan.FromSeconds(10);

    /// <summary>
    ///     The last scan response received from each address, keyed by hex Bluetooth address, with the time it arrived
    ///     (<see cref="Environment.TickCount64" />). A peripheral does not answer every scan request, so an advertisement whose
    ///     own scan response was missed is completed from this one while it is younger than
    ///     <c>WindowsScanningOptions.ScanResponseMaxAge</c>. Only populated when merging is enabled.
    /// </summary>
    private readonly ConcurrentDictionary<string, RememberedScanResponse> _lastScanResponses = new();

    private sealed record RememberedScanResponse(BluetoothLEAdvertisementReceivedEventArgs Args, long ReceivedAtTickCount);

    // Addresses can rotate (private addresses), so the dictionary is swept of expired entries once it grows past this.
    private const int MaxRememberedScanResponsesBeforeSweep = 256;

    private void RememberScanResponse(string hexAddress, BluetoothLEAdvertisementReceivedEventArgs scanResponseArgs)
    {
        if (_scanResponseMaxAge <= TimeSpan.Zero)
        {
            return;
        }

        _lastScanResponses[hexAddress] = new RememberedScanResponse(scanResponseArgs, Environment.TickCount64);

        if (_lastScanResponses.Count > MaxRememberedScanResponsesBeforeSweep)
        {
            var oldestUsable = Environment.TickCount64 - (long) _scanResponseMaxAge.TotalMilliseconds;
            foreach (var (address, remembered) in _lastScanResponses)
            {
                if (remembered.ReceivedAtTickCount < oldestUsable)
                {
                    _lastScanResponses.TryRemove(address, out _);
                }
            }
        }
    }

    private BluetoothLEAdvertisementReceivedEventArgs? GetRememberedScanResponse(string hexAddress)
    {
        if (_scanResponseMaxAge <= TimeSpan.Zero || !_lastScanResponses.TryGetValue(hexAddress, out var remembered))
        {
            return null;
        }

        return Environment.TickCount64 - remembered.ReceivedAtTickCount <= (long) _scanResponseMaxAge.TotalMilliseconds
            ? remembered.Args
            : null;
    }

    #endregion

    #region Service UUID Filter

    // ScanningOptions.ServiceUuids, applied in software: Windows delivers an advertisement and its scan response as separate PDUs and
    // the UUID can be in either of them. A scan's UUIDs and its matches are one object that a scan start replaces, so a reception
    // callback still running from the previous scan can only write into that scan's matches. Null when the scan has no service UUIDs.
    private sealed class ServiceUuidFilterState(Guid[] uuids)
    {
        public Guid[] Uuids { get; } = uuids;

        /// <summary>
        ///     The last time a PDU from each address, keyed by hex Bluetooth address, listed one of the service UUIDs
        ///     (<see cref="Environment.TickCount64" />), noted when the PDU is received. The other PDUs of that device do not have to
        ///     list it again.
        /// </summary>
        public ConcurrentDictionary<string, long> Matches { get; } = new();
    }

    private volatile ServiceUuidFilterState? _serviceUuidFilter;

    // How long a match lets the other PDUs of the same address through.
    private const long ServiceUuidMatchLifetimeMs = 10_000;

    // Addresses can rotate (private addresses), so the dictionary is swept of expired entries once it grows past this.
    private const int MaxServiceUuidMatchesBeforeSweep = 256;

    /// <summary>
    ///     Tells whether an advertisement passes <see cref="ScanningOptions.ServiceUuids" />: it lists at least one of them, or an
    ///     advertisement or scan response of the same address did in the last 10 seconds. Without service UUIDs everything passes.
    /// </summary>
    /// <param name="advertisement">The advertisement to check.</param>
    /// <param name="matchedWhenReceived">Whether the advertisement already had a match when it was received, before merging held it back.</param>
    /// <remarks>
    ///     The second condition is what keeps the filter from cutting a matching device in half: with the UUID in the advertisement,
    ///     its scan response does not repeat it, and the other way round. It also means a device is never created from a PDU that
    ///     has no UUID and no match behind it. With merging, the advertisement already carries the UUIDs of both PDUs. The match
    ///     is noted when a PDU is received (<see cref="NoteServiceUuidMatch" />), so a remembered scan response that is merged
    ///     into a later advertisement does not extend it, and a scan response of a device that is not known yet still counts. An
    ///     advertisement that merging holds back keeps the eligibility it had when it was received.
    /// </remarks>
    private bool PassesServiceUuidFilter(WindowsBluetoothAdvertisement advertisement, bool matchedWhenReceived = false)
    {
        var filter = _serviceUuidFilter;
        if (filter == null)
        {
            return true;
        }

        foreach (var advertised in advertisement.ServicesGuids)
        {
            if (Array.IndexOf(filter.Uuids, advertised) >= 0)
            {
                return true;
            }
        }

        return matchedWhenReceived || HasRecentMatch(filter, advertisement.BluetoothAddress);
    }

    private static bool HasRecentMatch(ServiceUuidFilterState filter, string address)
    {
        return filter.Matches.TryGetValue(address, out var matchedAt) && Environment.TickCount64 - matchedAt <= ServiceUuidMatchLifetimeMs;
    }

    /// <summary>
    ///     Whether a PDU of this address listed one of the service UUIDs in the last 10 seconds; <c>false</c> without service UUIDs.
    /// </summary>
    private bool HasRecentServiceUuidMatch(string address)
    {
        return _serviceUuidFilter is { } filter && HasRecentMatch(filter, address);
    }

    /// <summary>
    ///     Notes that a received PDU lists one of the service UUIDs, whichever PDU it is and whether or not the device exists yet.
    /// </summary>
    private void NoteServiceUuidMatch(BluetoothLEAdvertisementReceivedEventArgs args)
    {
        var filter = _serviceUuidFilter;
        if (filter == null)
        {
            return;
        }

        foreach (var advertised in args.Advertisement.ServiceUuids)
        {
            if (Array.IndexOf(filter.Uuids, advertised) >= 0)
            {
                RememberServiceUuidMatch(filter, WindowsBluetoothAdvertisement.ConvertNumericBleAddressToHexBleAddress(args.BluetoothAddress));
                return;
            }
        }
    }

    private static void RememberServiceUuidMatch(ServiceUuidFilterState filter, string address)
    {
        var now = Environment.TickCount64;
        filter.Matches[address] = now;

        if (filter.Matches.Count > MaxServiceUuidMatchesBeforeSweep)
        {
            foreach (var (matchedAddress, matchedAt) in filter.Matches)
            {
                if (now - matchedAt > ServiceUuidMatchLifetimeMs)
                {
                    filter.Matches.TryRemove(matchedAddress, out _);
                }
            }
        }
    }

    #endregion

    /// <inheritdoc />
    public WindowsBluetoothScanner(IBluetoothAdapter adapter,
        IBluetoothRssiToSignalStrengthConverter rssiToSignalStrengthConverter,
        ITicker ticker,
        IBluetoothRemoteDeviceFactory deviceFactory,
        IBluetoothNameProvider? nameProvider = null,
        ILoggerFactory? loggerFactory = null) : base(adapter,
                                                       rssiToSignalStrengthConverter,
                                                       ticker,
                                                       nameProvider,
                                                       loggerFactory)
    {
        _ticker = ticker;
        _deviceFactory = deviceFactory;
    }

    #region Delegate Callbacks

    /// <summary>
    ///     Called when a Bluetooth LE advertisement is received.
    /// </summary>
    /// <param name="argsAdvertisement">The advertisement event arguments.</param>
    /// <remarks>
    ///     See ADR 0003 (<c>Docs/Architecture/ADR/0003-windows-scan-response-handling.md</c>) for the
    ///     full behavior this implements.
    /// </remarks>
    public void OnAdvertisementReceived(BluetoothLEAdvertisementReceivedEventArgs argsAdvertisement)
    {
        ArgumentNullException.ThrowIfNull(argsAdvertisement);

        NoteServiceUuidMatch(argsAdvertisement);

        // An extended scan response has the advertisement type Extended and is told apart by its own flag; Windows before build 19041 has only
        // the legacy type.
        var isScanResponse = argsAdvertisement.AdvertisementType == BluetoothLEAdvertisementType.ScanResponse
                             || (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 19041) && argsAdvertisement.IsScanResponse);

        if (!_mergeScanResponses)
        {
            // Every PDU, scan responses included, goes through the normal pipeline, as it always has: data that only a scan
            // response carries (a name, service data) would otherwise never reach AdvertisementReceived or LastAdvertisement.
            var dispatched = DispatchAdvertisement(new WindowsBluetoothAdvertisement(argsAdvertisement));

            if (dispatched && isScanResponse)
            {
                DispatchScanResponseToExistingDevice(argsAdvertisement);
            }

            return;
        }

        var hexAddress = WindowsBluetoothAdvertisement.ConvertNumericBleAddressToHexBleAddress(argsAdvertisement.BluetoothAddress);

        if (isScanResponse)
        {
            lock (_pendingGate)
            {
                if (_session is not { } session)
                {
                    return; // No merging scan runs: the scanner is stopping or stopped.
                }

                RememberScanResponse(hexAddress, argsAdvertisement);

                if (_pendingAdvertisements.Remove(hexAddress, out var pending))
                {
                    Enqueue(session, new AdvertisementDelivery(pending, argsAdvertisement));
                }

                // Whether or not a pending merge was resolved above, also surface the raw scan response on the device if it is
                // already known - a late scan response (buffer already timed out) falls back to the same path used when merging is
                // off. Queued after the advertisement above, so the device exists by then when that advertisement created it.
                Enqueue(session, new ScanResponseDelivery(argsAdvertisement));
            }

            return;
        }

        // Primary ADV PDU: buffer it until its scan response arrives or the merge window ends.
        var entry = new PendingAdvertisement(argsAdvertisement) { MatchedWhenReceived = HasRecentServiceUuidMatch(hexAddress) };
#pragma warning disable CA2000 // Disposed when the entry is delivered, dropped or abandoned, or below when there is no scan - not leaked.
        entry.Timer = new Timer(_ => ResolvePendingAdvertisement(hexAddress, entry), null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
#pragma warning restore CA2000

        lock (_pendingGate)
        {
            if (_session is not { } session)
            {
                entry.Timer.Dispose(); // No merging scan runs: the scanner is stopping or stopped.
                return;
            }

            if (_pendingAdvertisements.TryGetValue(hexAddress, out var previous))
            {
                // A newer advertisement from the same address must not postpone the older one's delivery: with an advertising
                // interval shorter than the merge window and no scan response, the deadline would be pushed back forever.
                Enqueue(session, new AdvertisementDelivery(previous, GetRememberedScanResponse(hexAddress)));
            }

            _pendingAdvertisements[hexAddress] = entry;

            // Armed only once the entry can be found, so even a zero window cannot fire before it is registered.
            entry.Timer.Change(_scanResponseMergeWindow, Timeout.InfiniteTimeSpan);
        }
    }

    /// <summary>
    ///     Queues a delivery. Called under <see cref="_pendingGate" />, which is what keeps the queue in the order decisions were made.
    /// </summary>
    private static void Enqueue(MergeSession session, Delivery delivery)
    {
        session.Deliveries.Writer.TryWrite(delivery);
    }

    /// <summary>
    ///     Fires once <see cref="Abstractions.Scanning.Options.Windows.WindowsScanningOptions.ScanResponseMergeWindow" />
    ///     elapses for a buffered advertisement with no scan response received - queues it merged with the last scan
    ///     response remembered for the address when that one is recent enough, ADV-only otherwise.
    /// </summary>
    /// <remarks>
    ///     It resolves exactly <paramref name="entry" />: a newer advertisement for the same address that replaced it is left alone.
    /// </remarks>
    private void ResolvePendingAdvertisement(string hexAddress, PendingAdvertisement entry)
    {
        lock (_pendingGate)
        {
            if (!_pendingAdvertisements.TryGetValue(hexAddress, out var current) || !ReferenceEquals(current, entry))
            {
                return; // Already resolved by a scan response, replaced by a newer advertisement, or dropped by a stop.
            }

            _pendingAdvertisements.Remove(hexAddress);

            if (_session is { } session)
            {
                Enqueue(session, new AdvertisementDelivery(entry, GetRememberedScanResponse(hexAddress)));
            }
        }
    }

    /// <summary>
    ///     Runs the deliveries of a session one at a time until its queue is completed. A failure in a consumer callback goes to the
    ///     unhandled exception listener, like those of native reception, and does not end the loop.
    /// </summary>
    private async Task ProcessDeliveriesAsync(MergeSession session)
    {
        await foreach (var delivery in session.Deliveries.Reader.ReadAllAsync().ConfigureAwait(false))
        {
            try
            {
                if (delivery is AdvertisementDelivery { Pending.Timer: { } timer })
                {
                    await timer.DisposeAsync().ConfigureAwait(false);
                }

                if (session.Retired)
                {
                    continue; // This scan was given up on: what is still queued must not reach a later one.
                }

                switch (delivery)
                {
                    case AdvertisementDelivery advertisement:
                        DispatchAdvertisement(new WindowsBluetoothAdvertisement(advertisement.Pending.Args, advertisement.ScanResponse), advertisement.Pending.MatchedWhenReceived);
                        break;
                    case ScanResponseDelivery scanResponse:
                        DispatchScanResponseToExistingDevice(scanResponse.ScanResponse);
                        break;
                }
            }
            catch (Exception e)
            {
                BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
            }
        }
    }

    /// <summary>
    ///     Closes the merging scan, if one runs: no further advertisement is admitted, the advertisements still waiting for a scan
    ///     response are dropped and the remembered scan responses are forgotten. Returns the session so that the caller can wait for
    ///     its queue to drain; <c>null</c> when there was none.
    /// </summary>
    private MergeSession? CloseMergeSession(out List<PendingAdvertisement> abandoned)
    {
        lock (_pendingGate)
        {
            var session = _session;
            _session = null;
            abandoned = [.. _pendingAdvertisements.Values];
            _pendingAdvertisements.Clear();
            _lastScanResponses.Clear();
            session?.Deliveries.Writer.TryComplete();
            return session;
        }
    }

    /// <summary>
    ///     Closes the merging scan without waiting for anything: for a start that failed and for a watcher that the system stopped.
    /// </summary>
    private void AbandonMergeSession()
    {
        var session = CloseMergeSession(out var abandoned);
        foreach (var entry in abandoned)
        {
            entry.Timer?.Dispose();
        }

        if (session is not null)
        {
            session.Retired = true;
        }
    }

    /// <returns><c>false</c> when the advertisement does not pass the service UUID filter and was dropped.</returns>
    private bool DispatchAdvertisement(WindowsBluetoothAdvertisement advertisement, bool matchedWhenReceived = false)
    {
        if (!PassesServiceUuidFilter(advertisement, matchedWhenReceived))
        {
            return false;
        }

        var deviceName = string.IsNullOrEmpty(advertisement.DeviceName) ? "(no name)" : advertisement.DeviceName;
        Logger?.LogDeviceDiscovered(advertisement.BluetoothAddress, deviceName, advertisement.Manufacturer, advertisement.SignalStrengthInDBm);
        OnAdvertisementReceived(advertisement); // Base class method
        return true;
    }

    /// <summary>
    ///     Correlates a scan-response PDU to an already-known device and raises
    ///     <c>WindowsBluetoothRemoteDevice.ScanResponseReceived</c> on it. A device is never created
    ///     from a scan response alone - if none is found for this address yet, the PDU is dropped.
    /// </summary>
    private void DispatchScanResponseToExistingDevice(BluetoothLEAdvertisementReceivedEventArgs scanResponseArgs)
    {
        var hexAddress = WindowsBluetoothAdvertisement.ConvertNumericBleAddressToHexBleAddress(scanResponseArgs.BluetoothAddress);

        if (GetDeviceOrDefault(hexAddress)?.UnderlyingPlatformDevice is WindowsBluetoothRemoteDevice windowsDevice)
        {
            // A device of an earlier scan is still known, so the service UUID filter decides here as well.
            var scanResponse = new WindowsBluetoothAdvertisement(scanResponseArgs);
            if (PassesServiceUuidFilter(scanResponse))
            {
                windowsDevice.OnScanResponseReceived(scanResponse);
            }
        }
    }

    /// <summary>
    ///     Called when the advertisement watcher is stopped.
    /// </summary>
    /// <param name="argsError">The error code, if any.</param>
    public void OnAdvertisementWatcherStopped(BluetoothError argsError)
    {
        // However the watcher stopped, a merging scan that is still open is over (a stop that this scanner asked for has closed it already).
        AbandonMergeSession();

        if (argsError != BluetoothError.Success)
        {
            Logger?.LogScanError(argsError.ToString(), new WindowsNativeBluetoothErrorException(argsError));
            OnStopFailed(new WindowsNativeBluetoothErrorException(argsError));
        }
        else
        {
            Logger?.LogScanStopped();

            // IsRunning is otherwise refreshed only by the periodic tick. Until then it would still read true, and the
            // StopScanningAsync that is waiting for this callback would fail with ScannerFailedToStopException.
            IsRunning = false;
            OnStopSucceeded();
        }
    }

    #endregion

    #region Abstract Method Implementations

    /// <inheritdoc />
    /// <remarks>
    ///     On Windows, this checks if the advertisement watcher status is <see cref="BluetoothLEAdvertisementWatcherStatus.Started" />.
    ///     It reads the native watcher's own status, not the wrapper's observable copy: that copy is refreshed by a separate
    ///     tick, so right after the watcher stopped it can still say Started and would turn <c>IsRunning</c> back on.
    /// </remarks>
    protected override void NativeRefreshIsRunning()
    {
        IsRunning = _watcher != null && _watcher.BluetoothLeAdvertisementWatcher.Status == BluetoothLEAdvertisementWatcherStatus.Started;
    }

    /// <inheritdoc />
    /// <remarks>
    ///     Starts the Windows Bluetooth LE advertisement watcher.
    ///     The watcher will begin receiving advertisements and call <see cref="OnAdvertisementReceived" /> for each one.
    /// </remarks>
    protected override ValueTask NativeStartAsync(ScanningOptions scanningOptions, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(scanningOptions);
        Logger?.LogScanStarting(scanningOptions.ScanMode, scanningOptions.CallbackType);

        var windowsOptions = scanningOptions.Windows as Abstractions.Scanning.Options.Windows.WindowsScanningOptions;
        _mergeScanResponses = windowsOptions?.MergeScanResponses ?? false;
        _scanResponseMergeWindow = windowsOptions?.ScanResponseMergeWindow ?? TimeSpan.FromMilliseconds(500);
        _scanResponseMaxAge = windowsOptions?.ScanResponseMaxAge ?? TimeSpan.FromSeconds(10);

        // A scan gets its own filter state, so nothing matched in an earlier scan counts in this one.
        _serviceUuidFilter = scanningOptions.ServiceUuids is { Count: > 0 } serviceUuids ? new ServiceUuidFilterState(serviceUuids.ToArray()) : null;

        var nativeWatcher = Watcher.BluetoothLeAdvertisementWatcher;
        ConfigureNativeWatcher(nativeWatcher, scanningOptions, windowsOptions);

        // A merging scan gets its session, and nothing remembered from an earlier scan survives into it. It is closed again below if
        // the watcher does not start.
        lock (_pendingGate)
        {
            _lastScanResponses.Clear();
            if (_mergeScanResponses)
            {
                var session = new MergeSession();
                session.Loop = Task.Run(() => ProcessDeliveriesAsync(session), CancellationToken.None);
                _session = session;
            }
        }

        // Start watcher (status change callback will call OnStartSucceeded)
        try
        {
            nativeWatcher.Start();
            Logger?.LogScanStarted();
        }
        catch (COMException e)
        {
            AbandonMergeSession();

            // Check if it's a permission-related error
            const int eAccessdenied = unchecked((int) 0x80070005);
            if (e.HResult == eAccessdenied)
            {
                throw new BluetoothPermissionException("Access denied when starting Bluetooth scanner. Ensure 'bluetooth' capability is declared in Package.appxmanifest and Bluetooth radio is enabled. "
                                                     + "You may need to call IBluetoothPermissionManager.RequestBluetoothPermissionsAsync() to check and enable the radio.",
                                                       e);
            }

            throw new WindowsNativeBluetoothException("Failed to start Bluetooth LE advertisement watcher.", e);
        }
        catch (Exception)
        {
            AbandonMergeSession();
            throw;
        }

        return ValueTask.CompletedTask;
    }

    /// <summary>
    ///     Applies <see cref="ScanningOptions" /> and Windows-specific options to the native watcher
    ///     before it starts. Covers cross-platform properties Windows already documents support for
    ///     (<see cref="ScanningOptions.ScanMode" />, <see cref="ScanningOptions.RssiThreshold" />,
    ///     <see cref="ScanningOptions.EnableExtendedAdvertising" />) but never previously wired up,
    ///     plus Windows-only escape hatches with no cross-platform equivalent.
    /// </summary>
    private static void ConfigureNativeWatcher(BluetoothLEAdvertisementWatcher nativeWatcher, ScanningOptions scanningOptions, Abstractions.Scanning.Options.Windows.WindowsScanningOptions? windowsOptions)
    {
        nativeWatcher.ScanningMode = ToNativeScanningMode(windowsOptions?.ScanningMode, scanningOptions.ScanMode);

        if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 19041))
        {
            nativeWatcher.AllowExtendedAdvertisements = windowsOptions?.AllowExtendedAdvertisements ?? scanningOptions.EnableExtendedAdvertising;
        }

        // Always reassign (rather than only when a value is set) so a filter configured on a
        // previous StartScanningAsync call doesn't linger once options no longer request it - the
        // native watcher instance is created once and reused across start/stop cycles.
        nativeWatcher.SignalStrengthFilter = new BluetoothSignalStrengthFilter
        {
            InRangeThresholdInDBm = scanningOptions.RssiThreshold.HasValue ? (short) scanningOptions.RssiThreshold.Value : null,
            OutOfRangeThresholdInDBm = windowsOptions?.SignalStrengthOutOfRangeThresholdInDBm,
            SamplingInterval = windowsOptions?.SignalStrengthSamplingInterval,
            OutOfRangeTimeout = windowsOptions?.SignalStrengthOutOfRangeTimeout
        };
    }

    /// <summary>
    ///     Maps the Windows-specific <see cref="Abstractions.Scanning.Options.Windows.WindowsScanningMode" />
    ///     escape hatch (when set) or the cross-platform <see cref="BluetoothScanMode" /> to a native
    ///     scanning mode. Active scanning
    ///     is preserved as the default for <see cref="BluetoothScanMode.Balanced" /> to match this
    ///     plugin's existing (pre-ADR-0003) behavior. <see cref="BluetoothScanMode.LowPower" /> trades
    ///     away scan-response reception entirely for reduced radio activity.
    /// </summary>
    private static BluetoothLEScanningMode ToNativeScanningMode(Abstractions.Scanning.Options.Windows.WindowsScanningMode? windowsScanningMode, BluetoothScanMode crossPlatformScanMode)
    {
        if (windowsScanningMode.HasValue)
        {
            return windowsScanningMode.Value == Abstractions.Scanning.Options.Windows.WindowsScanningMode.Passive
                ? BluetoothLEScanningMode.Passive
                : BluetoothLEScanningMode.Active;
        }

        return crossPlatformScanMode == BluetoothScanMode.LowPower
            ? BluetoothLEScanningMode.Passive
            : BluetoothLEScanningMode.Active;
    }

    /// <inheritdoc />
    /// <remarks>
    ///     Stops the Windows Bluetooth LE advertisement watcher and discards any advertisements still
    ///     buffered awaiting a scan response.
    /// </remarks>
    protected async override ValueTask NativeStopAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        Logger?.LogScanStopping();

        // Nothing is buffered from this moment: a callback that was already running finds no session. Advertisements still waiting for
        // their scan response are dropped; the deliveries already queued are run before the stop returns, so nothing from this scan
        // is dispatched afterwards (a restart clears the device list).
        var session = CloseMergeSession(out var abandoned);
        _watcher?.BluetoothLeAdvertisementWatcher.Stop();

        foreach (var entry in abandoned)
        {
            if (entry.Timer is { } timer)
            {
                await timer.DisposeAsync().ConfigureAwait(false);
            }
        }

        if (session is not null)
        {
            try
            {
                // Bounded, so a handler that never returns cannot keep the scanner from stopping: by the caller's timeout when it gives
                // a finite one, and in any case by a safety limit.
                var drainLimit = timeout is { } requested && requested >= TimeSpan.Zero && requested < MaxDrainWait ? requested : MaxDrainWait;
                await session.Loop.WaitAsync(drainLimit, cancellationToken).ConfigureAwait(false);
            }
            catch (TimeoutException)
            {
                // Stopping goes on. The session is given up on: what is still queued is skipped, so it cannot reach a later scan.
                session.Retired = true;
            }
            catch (OperationCanceledException)
            {
                session.Retired = true; // The same, when the caller gave up waiting.
                throw;
            }
        }
    }

    #endregion

    #region Permission Methods

    /// <inheritdoc />
    /// <remarks>
    ///     On Windows, Bluetooth permissions are capability-based and granted at install time
    ///     if the 'bluetooth' capability is declared in Package.appxmanifest.
    ///     This method always returns true.
    /// </remarks>
    protected override ValueTask<bool> NativeHasScannerPermissionsAsync()
    {
        // On Windows, Bluetooth permissions are capability-based and granted at install time
        return ValueTask.FromResult(true);
    }

    /// <inheritdoc />
    /// <remarks>
    ///     On Windows, no runtime permission spec is needed. Bluetooth permissions are
    ///     declared in Package.appxmanifest and granted at install time.
    ///     The <paramref name="requireBackgroundLocation"/> parameter is ignored on Windows.
    /// </remarks>
    protected override ValueTask NativeRequestScannerPermissionsAsync(bool requireBackgroundLocation, CancellationToken cancellationToken)
    {
        // No runtime spec needed on Windows - permissions are declared at install time
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    protected override IBluetoothRemoteDevice NativeCreateDeviceFromAdvertisement(IBluetoothAdvertisement advertisement)
    {
        ArgumentNullException.ThrowIfNull(advertisement);

        if (advertisement is not WindowsBluetoothAdvertisement windowsAd)
        {
            throw new ArgumentException($"Expected advertisement of type {typeof(WindowsBluetoothAdvertisement)}, but got {advertisement.GetType()}", nameof(advertisement));
        }

        var spec = new WindowsBluetoothRemoteDeviceFactorySpec(windowsAd);
        return _deviceFactory.Create(this, spec);
    }

    #endregion

}
