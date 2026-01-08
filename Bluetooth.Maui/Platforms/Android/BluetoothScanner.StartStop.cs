using Bluetooth.Maui.PlatformSpecific;
using Bluetooth.Maui.PlatformSpecific.Exceptions;
using Bluetooth.Maui.PlatformSpecific.NativeOptions;

using Plugin.BaseTypeExtensions;

namespace Bluetooth.Maui;

public partial class BluetoothScanner
{
    /// <inheritdoc/>
    /// <remarks>
    /// On Android, there is no direct way to check if BLE scanning is running (IsDiscovering only refers to Classic Bluetooth).
    /// This is tracked manually when starting/stopping the scan and when receiving advertisements.
    /// </remarks>
    protected override void NativeRefreshIsRunning()
    {
        //IsRunning = BluetoothAdapterProxy.BluetoothAdapter.IsDiscovering;
        //On android there is no way to check if Scan is running ... IsDiscovering only refers to Classic Bluetooth ...
        // Tracking of this is done manually when starting/stopping the scan and when receiving advertisements
    }

    /// <summary>
    /// Gets or sets the scan options for Android BLE scanning.
    /// </summary>
    /// <remarks>
    /// These options control scan filters, scan mode, callback type, and other scanning parameters.
    /// </remarks>
    public StartScanningOptions? StartScanningOptions { get; set; }

    private AutoResetEvent InternalAndroidStartScanResultReceived { get; set; }

    /// <summary>
    /// On Android, we only get feedback on "StartScanFailed" (see below) and on "AdvertisementReceived" (see DeviceExploration) ...
    /// If no bluetooth devices are around and the scan has started successfully : we don't receive anything.
    /// Adjust this value to wait for longer or shorter.
    /// </summary>
    public static TimeSpan ScannerStartedTimeout { get; set; } = TimeSpan.FromSeconds(2);

    /// <inheritdoc/>
    /// <remarks>
    /// Starts BLE scanning using the Android <see cref="BluetoothLeScanner"/> with the configured <see cref="StartScanningOptions"/>.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <see cref="BluetoothLeScannerProxy.BluetoothLeScanner"/> is <c>null</c>.</exception>
    protected async override ValueTask NativeStartAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(BluetoothLeScannerProxy.BluetoothLeScanner, nameof(BluetoothLeScannerProxy.BluetoothLeScanner));
        BluetoothLeScannerProxy.BluetoothLeScanner.StartScan(StartScanningOptions.ToNativeScanFilters(), StartScanningOptions.ToNativeScanSettings(), ScanCallbackProxy);
        try
        {
            await Task.Run(() => InternalAndroidStartScanResultReceived.WaitOne(ScannerStartedTimeout), cancellationToken).ConfigureAwait(false);
        }
        catch (TimeoutException)
        {
            // On Android, we only get feedback on "StartScanFailed" (see below) and on "AdvertisementReceived" (see DeviceExploration) ...
            // If no bluetooth devices are around and the scan has started successfully : we don't receive anything.
            // In this timeout case, we ignore timeout and we sent a StartScanSuccess :
            OnStartSucceeded();
        }
    }

    /// <summary>
    /// Handles scan failures from the Android Bluetooth LE scanner.
    /// </summary>
    /// <param name="errorCode">The error code indicating the scan failure reason.</param>
    /// <exception cref="AndroidNativeScanFailureException">Thrown when a scan failure occurs (unless the error is <see cref="ScanFailure.AlreadyStarted"/>).</exception>
    public virtual void OnScanFailed(ScanFailure errorCode)
    {
        if (errorCode == ScanFailure.AlreadyStarted)
        {
            IsRunning = true;
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
            OnStartFailed(e);
        }
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Stops BLE scanning using the Android <see cref="BluetoothLeScanner"/>.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <see cref="BluetoothLeScannerProxy.BluetoothLeScanner"/> is <c>null</c>.</exception>
    protected override ValueTask NativeStopAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(BluetoothLeScannerProxy.BluetoothLeScanner, nameof(BluetoothLeScannerProxy.BluetoothLeScanner));
            BluetoothLeScannerProxy.BluetoothLeScanner.StopScan(ScanCallbackProxy);
            IsRunning = false;
            OnStopSucceeded();
        }
        catch (Exception e)
        {
            IsRunning = false;
            OnStopFailed(e);
        }
        return ValueTask.CompletedTask;
    }

}
