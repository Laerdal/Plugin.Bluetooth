using System.Runtime.InteropServices;

using Bluetooth.Maui.Platforms.Win.Exceptions;
using Bluetooth.Maui.Platforms.Win.Scanning.Factories;

namespace Bluetooth.Maui.Platforms.Win.Scanning;

/// <summary>
///     Windows implementation of the Bluetooth scanner using Windows.Devices.Bluetooth APIs.
/// </summary>
/// <remarks>
///     This implementation uses <see cref="BluetoothLEAdvertisementWatcher" /> to monitor BLE advertisements.
/// </remarks>
public class WindowsBluetoothScanner : BaseBluetoothScanner,
    NativeObjects.BluetoothLeAdvertisementWatcherWrapper.IBluetoothLeAdvertisementWatcherProxyDelegate
{
    private NativeObjects.BluetoothLeAdvertisementWatcherWrapper? _watcher;
    private readonly ITicker _ticker;

    /// <summary>
    ///     Gets the advertisement watcher wrapper, creating it lazily with this scanner as the delegate.
    /// </summary>
    private NativeObjects.BluetoothLeAdvertisementWatcherWrapper Watcher =>
        _watcher ??= new NativeObjects.BluetoothLeAdvertisementWatcherWrapper(this, _ticker);

    /// <inheritdoc />
    public WindowsBluetoothScanner(
        IBluetoothAdapter adapter,
        IBluetoothDeviceFactory deviceFactory,
        ITicker ticker,
        IBluetoothRssiToSignalStrengthConverter rssiToSignalStrengthConverter,
        ILogger<IBluetoothScanner>? logger = null)
        : base(adapter, deviceFactory, rssiToSignalStrengthConverter, ticker, logger)
    {
        _ticker = ticker;
    }

    #region Delegate Callbacks

    /// <summary>
    ///     Called when a Bluetooth LE advertisement is received.
    /// </summary>
    /// <param name="argsAdvertisement">The advertisement event arguments.</param>
    public void OnAdvertisementReceived(BluetoothLEAdvertisementReceivedEventArgs argsAdvertisement)
    {
        var advertisement = new WindowsBluetoothAdvertisement(argsAdvertisement);
        OnAdvertisementReceived(advertisement); // Base class method
    }

    /// <summary>
    ///     Called when the advertisement watcher is stopped.
    /// </summary>
    /// <param name="argsError">The error code, if any.</param>
    public void OnAdvertisementWatcherStopped(BluetoothError argsError)
    {
        if (argsError != BluetoothError.Success)
        {
            OnStopFailed(new WindowsNativeBluetoothErrorException(argsError));
        }
        else
        {
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
    protected override ValueTask NativeStartAsync(
        ScanningOptions scanningOptions,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        // Start watcher (status change callback will call OnStartSucceeded)
        try
        {
            Watcher.BluetoothLeAdvertisementWatcher.Start();
        }
        catch (COMException e)
        {
            // Check if it's a permission-related error
            const int eAccessdenied = unchecked((int) 0x80070005);
            if (e.HResult == eAccessdenied)
            {
                throw new BluetoothPermissionException(
                    "Access denied when starting Bluetooth scanner. Ensure 'bluetooth' capability is declared in Package.appxmanifest and Bluetooth radio is enabled. " +
                    "You may need to call IBluetoothPermissionManager.RequestBluetoothPermissionsAsync() to check and enable the radio.",
                    e);
            }

            throw new WindowsNativeBluetoothException("Failed to start Bluetooth LE advertisement watcher.", e);
        }

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    /// <remarks>
    ///     Stops the Windows Bluetooth LE advertisement watcher.
    /// </remarks>
    protected override ValueTask NativeStopAsync(
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        _watcher?.BluetoothLeAdvertisementWatcher.Stop();
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    /// <remarks>
    ///     Creates a Windows-specific device factory request from the advertisement.
    /// </remarks>
    protected override IBluetoothDeviceFactory.BluetoothDeviceFactoryRequest
        CreateDeviceFactoryRequestFromAdvertisement(IBluetoothAdvertisement advertisement)
    {
        if (advertisement is not WindowsBluetoothAdvertisement windowsAdvertisement)
        {
            throw new ArgumentException(
                $"Expected advertisement of type {typeof(WindowsBluetoothAdvertisement)}",
                nameof(advertisement));
        }

        return new WindowsBluetoothDeviceFactoryRequest(windowsAdvertisement);
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
    ///     On Windows, no runtime permission request is needed. Bluetooth permissions are
    ///     declared in Package.appxmanifest and granted at install time.
    ///     The <paramref name="requireBackgroundLocation"/> parameter is ignored on Windows.
    /// </remarks>
    protected override ValueTask NativeRequestScannerPermissionsAsync(bool requireBackgroundLocation, CancellationToken cancellationToken)
    {
        // No runtime request needed on Windows - permissions are declared at install time
        return ValueTask.CompletedTask;
    }

    #endregion
}
