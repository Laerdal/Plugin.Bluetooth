using System.Runtime.InteropServices;

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
    ///     Primary (ADV) advertisements awaiting either a correlated scan response or their merge
    ///     timeout, keyed by hex Bluetooth address. Only populated when
    ///     <c>WindowsScanningOptions.MergeScanResponses</c> is enabled.
    /// </summary>
    private readonly ConcurrentDictionary<string, PendingAdvertisement> _pendingAdvertisements = new();

    private sealed record PendingAdvertisement(BluetoothLEAdvertisementReceivedEventArgs Args, Timer Timer);

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
            if (isScanResponse)
            {
                DispatchScanResponseToExistingDevice(argsAdvertisement);
                return;
            }

            DispatchAdvertisement(new WindowsBluetoothAdvertisement(argsAdvertisement));
            return;
        }

        var hexAddress = WindowsBluetoothAdvertisement.ConvertNumericBleAddressToHexBleAddress(argsAdvertisement.BluetoothAddress);

        if (isScanResponse)
        {
            if (_pendingAdvertisements.TryRemove(hexAddress, out var pending))
            {
                pending.Timer.Dispose();
                DispatchAdvertisement(new WindowsBluetoothAdvertisement(pending.Args, argsAdvertisement));
            }

            // Whether or not a pending merge was resolved above, also surface the raw scan response
            // on the device if it's already known - a late scan response (buffer already timed out)
            // falls back to the same path used when merging is off.
            DispatchScanResponseToExistingDevice(argsAdvertisement);
            return;
        }

        // Primary ADV PDU: buffer it, replacing (and disposing) any stale pending entry for this
        // address that never resolved.
#pragma warning disable CA2000 // Stored in _pendingAdvertisements and disposed by ResolvePendingAdvertisement, the scan-response branch above, or NativeStopAsync - not leaked.
        var timer = new Timer(_ => ResolvePendingAdvertisement(hexAddress), null, _scanResponseMergeWindow, Timeout.InfiniteTimeSpan);
#pragma warning restore CA2000
        var entry = new PendingAdvertisement(argsAdvertisement, timer);
        if (_pendingAdvertisements.TryGetValue(hexAddress, out var stale))
        {
            stale.Timer.Dispose();
        }

        _pendingAdvertisements[hexAddress] = entry;
    }

    /// <summary>
    ///     Fires once <see cref="Abstractions.Scanning.Options.Windows.WindowsScanningOptions.ScanResponseMergeWindow" />
    ///     elapses for a buffered advertisement with no scan response received - dispatches it ADV-only.
    /// </summary>
    private void ResolvePendingAdvertisement(string hexAddress)
    {
        if (!_pendingAdvertisements.TryRemove(hexAddress, out var pending))
        {
            return; // Already resolved by a scan response arriving concurrently.
        }

        pending.Timer.Dispose();
        DispatchAdvertisement(new WindowsBluetoothAdvertisement(pending.Args, null));
    }

    private void DispatchAdvertisement(WindowsBluetoothAdvertisement advertisement)
    {
        Logger?.LogDeviceDiscovered(advertisement.BluetoothAddress, advertisement.RawSignalStrengthInDBm);
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
    protected override ValueTask NativeStopAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        Logger?.LogScanStopping();
        _watcher?.BluetoothLeAdvertisementWatcher.Stop();

        // In-memory cleanup only (no I/O) on an infrequently-called stop path - the async
        // alternatives these analyzers suggest buy nothing here.
#pragma warning disable CA1849
        foreach (var hexAddress in _pendingAdvertisements.Keys.ToArray())
        {
            if (_pendingAdvertisements.TryRemove(hexAddress, out var pending))
            {
                pending.Timer.Dispose();
            }
        }
#pragma warning restore CA1849

        return ValueTask.CompletedTask;
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
