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
    ///     Guards <see cref="_pendingAdvertisements" />, <see cref="_deliveries" /> and <see cref="_acceptingMergedAdvertisements" />.
    ///     It is never held while a consumer callback runs.
    /// </summary>
    private readonly object _pendingGate = new();

    /// <summary>
    ///     Primary (ADV) advertisements awaiting either a correlated scan response or their merge
    ///     timeout, keyed by hex Bluetooth address, at most one per address. Only populated when
    ///     <c>WindowsScanningOptions.MergeScanResponses</c> is enabled.
    /// </summary>
    private readonly Dictionary<string, PendingAdvertisement> _pendingAdvertisements = new();

    // False from the moment a stop begins until the next start, so a late callback cannot buffer into a stopped scan.
    private bool _acceptingMergedAdvertisements;

    // A pending advertisement is identified by reference: the timer that resolves it must not resolve a newer one for the same address.
    private sealed class PendingAdvertisement(BluetoothLEAdvertisementReceivedEventArgs args)
    {
        public BluetoothLEAdvertisementReceivedEventArgs Args { get; } = args;

        public Timer? Timer { get; set; }
    }

    /// <summary>
    ///     What a scan delivers while merging, in the order it was decided. A single consumer, <see cref="ProcessDeliveriesAsync" />,
    ///     runs them one at a time, so deliveries for an address never overlap and an older advertisement cannot overwrite a newer one,
    ///     whichever thread (native callback or merge timer) produced them.
    /// </summary>
    private abstract record Delivery;

    private sealed record AdvertisementDelivery(PendingAdvertisement Pending, BluetoothLEAdvertisementReceivedEventArgs? ScanResponse) : Delivery;

    private sealed record ScanResponseDelivery(BluetoothLEAdvertisementReceivedEventArgs ScanResponse) : Delivery;

    private Channel<Delivery>? _deliveries;

    private Task _deliveryLoop = Task.CompletedTask;

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

        var isScanResponse = argsAdvertisement.AdvertisementType == BluetoothLEAdvertisementType.ScanResponse;

        if (!_mergeScanResponses)
        {
            // Every PDU, scan responses included, goes through the normal pipeline, as it always has: data that only a scan
            // response carries (a name, service data) would otherwise never reach AdvertisementReceived or LastAdvertisement.
            DispatchAdvertisement(new WindowsBluetoothAdvertisement(argsAdvertisement));

            if (isScanResponse)
            {
                DispatchScanResponseToExistingDevice(argsAdvertisement);
            }

            return;
        }

        var hexAddress = WindowsBluetoothAdvertisement.ConvertNumericBleAddressToHexBleAddress(argsAdvertisement.BluetoothAddress);

        if (isScanResponse)
        {
            RememberScanResponse(hexAddress, argsAdvertisement);

            lock (_pendingGate)
            {
                if (!_acceptingMergedAdvertisements)
                {
                    return; // The scanner is stopping.
                }

                if (_pendingAdvertisements.Remove(hexAddress, out var pending))
                {
                    Enqueue(new AdvertisementDelivery(pending, argsAdvertisement));
                }

                // Whether or not a pending merge was resolved above, also surface the raw scan response on the device if it is
                // already known - a late scan response (buffer already timed out) falls back to the same path used when merging is
                // off. Queued after the advertisement above, so the device exists by then when that advertisement created it.
                Enqueue(new ScanResponseDelivery(argsAdvertisement));
            }

            return;
        }

        // Primary ADV PDU: buffer it until its scan response arrives or the merge window ends.
        var entry = new PendingAdvertisement(argsAdvertisement);
#pragma warning disable CA2000 // Disposed when the entry is delivered, or by NativeStopAsync / below when it never is - not leaked.
        entry.Timer = new Timer(_ => ResolvePendingAdvertisement(hexAddress, entry), null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
#pragma warning restore CA2000

        lock (_pendingGate)
        {
            if (!_acceptingMergedAdvertisements)
            {
                entry.Timer.Dispose(); // The scanner is stopping.
                return;
            }

            if (_pendingAdvertisements.TryGetValue(hexAddress, out var previous))
            {
                // A newer advertisement from the same address must not postpone the older one's delivery: with an advertising
                // interval shorter than the merge window and no scan response, the deadline would be pushed back forever.
                Enqueue(new AdvertisementDelivery(previous, GetRememberedScanResponse(hexAddress)));
            }

            _pendingAdvertisements[hexAddress] = entry;

            // Armed only once the entry can be found, so even a zero window cannot fire before it is registered.
            entry.Timer.Change(_scanResponseMergeWindow, Timeout.InfiniteTimeSpan);
        }
    }

    /// <summary>
    ///     Queues a delivery. Called under <see cref="_pendingGate" />, which is what keeps the queue in the order decisions were made.
    /// </summary>
    private void Enqueue(Delivery delivery)
    {
        _deliveries?.Writer.TryWrite(delivery);
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
            Enqueue(new AdvertisementDelivery(entry, GetRememberedScanResponse(hexAddress)));
        }
    }

    /// <summary>
    ///     Runs the deliveries one at a time until the queue is completed. A failure in a consumer callback goes to the unhandled
    ///     exception listener, like those of native reception, and does not end the loop.
    /// </summary>
    private async Task ProcessDeliveriesAsync(ChannelReader<Delivery> reader)
    {
        await foreach (var delivery in reader.ReadAllAsync().ConfigureAwait(false))
        {
            try
            {
                switch (delivery)
                {
                    case AdvertisementDelivery advertisement:
                        if (advertisement.Pending.Timer is { } timer)
                        {
                            await timer.DisposeAsync().ConfigureAwait(false);
                        }

                        DispatchAdvertisement(new WindowsBluetoothAdvertisement(advertisement.Pending.Args, advertisement.ScanResponse));
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

    private void DispatchAdvertisement(WindowsBluetoothAdvertisement advertisement)
    {
        var deviceName = string.IsNullOrEmpty(advertisement.DeviceName) ? "(no name)" : advertisement.DeviceName;
        Logger?.LogDeviceDiscovered(advertisement.BluetoothAddress, deviceName, advertisement.Manufacturer, advertisement.SignalStrengthInDBm);
        OnAdvertisementReceived(advertisement); // Base class method
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
            windowsDevice.OnScanResponseReceived(new WindowsBluetoothAdvertisement(scanResponseArgs));
        }
    }

    /// <summary>
    ///     Called when the advertisement watcher is stopped.
    /// </summary>
    /// <param name="argsError">The error code, if any.</param>
    public void OnAdvertisementWatcherStopped(BluetoothError argsError)
    {
        if (argsError != BluetoothError.Success)
        {
            Logger?.LogScanError(argsError.ToString(), new WindowsNativeBluetoothErrorException(argsError));
            OnStopFailed(new WindowsNativeBluetoothErrorException(argsError));
        }
        else
        {
            Logger?.LogScanStopped();
            OnStopSucceeded();
        }
    }

    #endregion

    #region Abstract Method Implementations

    /// <inheritdoc />
    /// <remarks>
    ///     On Windows, this checks if the advertisement watcher status is <see cref="BluetoothLEAdvertisementWatcherStatus.Started" />.
    ///     Uses the wrapper's observable Status property which is automatically refreshed by the ticker.
    /// </remarks>
    protected override void NativeRefreshIsRunning()
    {
        IsRunning = _watcher != null && _watcher.Status == BluetoothLEAdvertisementWatcherStatus.Started;
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
        _lastScanResponses.Clear(); // nothing remembered from an earlier scan, whatever its options were

        if (_mergeScanResponses)
        {
            var deliveries = Channel.CreateUnbounded<Delivery>(new UnboundedChannelOptions { SingleReader = true });
            lock (_pendingGate)
            {
                _deliveries = deliveries;
                _acceptingMergedAdvertisements = true;
            }

            _deliveryLoop = Task.Run(() => ProcessDeliveriesAsync(deliveries.Reader), CancellationToken.None);
        }

        var nativeWatcher = Watcher.BluetoothLeAdvertisementWatcher;
        ConfigureNativeWatcher(nativeWatcher, scanningOptions, windowsOptions);

        // Start watcher (status change callback will call OnStartSucceeded)
        try
        {
            nativeWatcher.Start();
            Logger?.LogScanStarted();
        }
        catch (COMException e)
        {
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

        // Nothing is buffered from this moment: a callback that was already running finds the gate closed. Advertisements still waiting for
        // their scan response are dropped; the deliveries already queued are run before the stop returns, so nothing from this scan
        // is dispatched afterwards (a restart clears the device list).
        List<PendingAdvertisement> abandoned;
        Channel<Delivery>? deliveries;
        lock (_pendingGate)
        {
            _acceptingMergedAdvertisements = false;
            abandoned = [.. _pendingAdvertisements.Values];
            _pendingAdvertisements.Clear();
            deliveries = _deliveries;
            _deliveries = null;
        }

        _watcher?.BluetoothLeAdvertisementWatcher.Stop();

        foreach (var entry in abandoned)
        {
            if (entry.Timer is { } timer)
            {
                await timer.DisposeAsync().ConfigureAwait(false);
            }
        }

        deliveries?.Writer.TryComplete();

        try
        {
            // Bounded, so a handler that never returns cannot keep the scanner from stopping.
            await _deliveryLoop.WaitAsync(TimeSpan.FromSeconds(5), cancellationToken).ConfigureAwait(false);
        }
        catch (TimeoutException)
        {
            // Stopping goes on; the handler keeps the loop busy until it returns.
        }

        _lastScanResponses.Clear();
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
