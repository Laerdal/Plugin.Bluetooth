using Bluetooth.Abstractions.Scanning.Factories;
using Bluetooth.Maui.Platforms.Droid.Exceptions;
using Bluetooth.Maui.Platforms.Droid.Scanning.NativeObjects;

using Microsoft.Extensions.Logging;

namespace Bluetooth.Maui.Platforms.Droid.Scanning;

/// <inheritdoc/>
public partial class BluetoothScanner : BaseBluetoothScanner, ScanCallbackProxy.IScanner
{
    /// <inheritdoc/>
    public BluetoothScanner(IBluetoothAdapter adapter,
        IBluetoothPermissionManager permissionManager,
        IBluetoothDeviceFactory deviceFactory,
        IBluetoothCharacteristicAccessServicesRepository knownServicesAndCharacteristicsRepository,
        ILogger? logger = null) : base(adapter,
                                       permissionManager,
                                       deviceFactory,
                                       knownServicesAndCharacteristicsRepository,
                                       logger)
    {
        ScanCallbackProxy = new ScanCallbackProxy(this);
    }

    /// <summary>
    /// Gets the BluetoothLeScanner instance from the Android Bluetooth adapter.
    /// </summary>
    private BluetoothLeScanner BluetoothLeScanner
    {
        get
        {
            if (Adapter is not BluetoothAdapter androidAdapter)
            {
                throw new InvalidOperationException("Adapter must be an Android BluetoothAdapter");
            }
            return androidAdapter.NativeBluetoothAdapter.BluetoothLeScanner
                ?? throw new InvalidOperationException("BluetoothLeScanner is not available");
        }
    }

    /// <summary>
    /// Gets the scan callback proxy that handles scan events.
    /// </summary>
    private ScanCallbackProxy ScanCallbackProxy { get; }

    /// <summary>
    /// Event to signal when scan start result is received.
    /// </summary>
    private AutoResetEvent InternalAndroidStartScanResultReceived { get; } = new AutoResetEvent(false);

    /// <summary>
    /// Timeout for waiting for scan to start. Defaults to 2 seconds.
    /// </summary>
    public static TimeSpan ScannerStartedTimeout { get; set; } = TimeSpan.FromSeconds(2);

    /// <inheritdoc/>
    /// <remarks>
    /// On Android, there is no direct way to check if BLE scanning is running.
    /// This is tracked manually when starting/stopping the scan and when receiving advertisements.
    /// </remarks>
    protected override void NativeRefreshIsRunning()
    {
        // On Android there is no way to check if Scan is running (IsDiscovering only refers to Classic Bluetooth)
        // Tracking of this is done manually when starting/stopping the scan and when receiving advertisements
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Starts BLE scanning using the Android BluetoothLeScanner.
    /// </remarks>
    protected async override ValueTask NativeStartAsync(IBluetoothScannerStartScanningOptions scanningOptions, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(BluetoothLeScanner);

        // Build scan settings and filters
        var settings = BuildScanSettings(scanningOptions);
        var filters = BuildScanFilters(scanningOptions);

        BluetoothLeScanner.StartScan(filters, settings, ScanCallbackProxy);

        try
        {
            // Wait for either scan to start or timeout
            await Task.Run(() => InternalAndroidStartScanResultReceived.WaitOne(ScannerStartedTimeout), cancellationToken).ConfigureAwait(false);
        }
        catch (TimeoutException)
        {
            // On Android, we only get feedback on "StartScanFailed" and on "AdvertisementReceived"
            // If no devices are around and scan started successfully, we don't receive anything
            // In timeout case, assume scan started successfully
            IsRunning = true;
        }
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Stops BLE scanning using the Android BluetoothLeScanner.
    /// </remarks>
    protected override ValueTask NativeStopAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(BluetoothLeScanner);
            BluetoothLeScanner.StopScan(ScanCallbackProxy);
            IsRunning = false;
        }
        catch (Exception e)
        {
            Logger?.LogError(e, "Error stopping Bluetooth scan");
            IsRunning = false;
        }

        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Builds Android scan settings from the scanner options.
    /// </summary>
    private ScanSettings BuildScanSettings(IBluetoothScannerStartScanningOptions options)
    {
        var builder = new ScanSettings.Builder();

        // Set scan mode for balanced power/performance
        builder.SetScanMode(Android.Bluetooth.LE.ScanMode.Balanced);

        // Set callback type
        if (options.IgnoreDuplicateAdvertisements)
        {
            builder.SetCallbackType(ScanCallbackType.AllMatches);
        }
        else
        {
            builder.SetCallbackType(ScanCallbackType.AllMatches);
        }

        return builder.Build() ?? throw new InvalidOperationException("Failed to build ScanSettings");
    }

    /// <summary>
    /// Builds Android scan filters from the scanner options.
    /// </summary>
    private IList<ScanFilter>? BuildScanFilters(IBluetoothScannerStartScanningOptions options)
    {
        // No service UUID filtering in base interface, so return null for now
        // This can be extended if needed
        return null;
    }

    #region ScanCallbackProxy.IScanner Implementation

    /// <summary>
    /// Handles scan failures from the Android Bluetooth LE scanner.
    /// </summary>
    public void OnScanFailed(ScanFailure errorCode)
    {
        if (errorCode == ScanFailure.AlreadyStarted)
        {
            IsRunning = true;
            InternalAndroidStartScanResultReceived.Set();
            return;
        }

        try
        {
            throw new AndroidNativeScanFailureException(errorCode);
        }
        catch (Exception e)
        {
            IsRunning = false;
            InternalAndroidStartScanResultReceived.Set();
            Logger?.LogError(e, "Bluetooth scan failed with error code: {ErrorCode}", errorCode);
        }
    }

    /// <summary>
    /// Handles batch scan results from the Android Bluetooth LE scanner.
    /// </summary>
    public void OnScanResult(ScanCallbackType callbackType, IEnumerable<ScanResult> results)
    {
        foreach (var result in results)
        {
            OnScanResult(callbackType, result);
        }
    }

    /// <summary>
    /// Handles individual scan results from the Android Bluetooth LE scanner.
    /// </summary>
    public void OnScanResult(ScanCallbackType callbackType, ScanResult result)
    {
        // Signal that scan has started successfully
        InternalAndroidStartScanResultReceived.Set();
        IsRunning = true;

        try
        {
            // Create advertisement from scan result
            var nativeDevice = result.Device;
            if (nativeDevice == null)
            {
                return;
            }

            var advertisement = new BluetoothAdvertisement(result);

            // Apply advertisement filter
            if (!ScanningOptions.AdvertisementFilter(advertisement))
            {
                return;
            }

            // Create or get device
            var deviceId = nativeDevice.Address ?? throw new InvalidOperationException("Device address is null");
            var device = GetDeviceOrDefault(deviceId);

            if (device == null)
            {
                var request = new BluetoothDeviceFactoryRequest
                {
                    Id = deviceId,
                    Manufacturer = advertisement.Manufacturer,
                };
                device = DeviceFactory.CreateDevice(this, request);
            }

            // Update device with advertisement
            if (device is BluetoothDevice androidDevice)
            {
                androidDevice.UpdateAdvertisement(advertisement, result.Rssi);
            }

            // Notify advertisement received
            OnAdvertisementReceived(device, advertisement);
        }
        catch (Exception e)
        {
            Logger?.LogError(e, "Error processing scan result");
        }
    }

    #endregion
}
