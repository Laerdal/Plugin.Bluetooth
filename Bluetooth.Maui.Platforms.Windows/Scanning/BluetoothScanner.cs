using Bluetooth.Maui.Platforms.Windows.Exceptions;
using Bluetooth.Maui.Platforms.Windows.Scanning.Factories;
using Bluetooth.Maui.Platforms.Windows.Scanning.NativeObjects;

namespace Bluetooth.Maui.Platforms.Windows.Scanning;

/// <summary>
///     Windows implementation of the Bluetooth scanner using Windows.Devices.Bluetooth APIs.
/// </summary>
/// <remarks>
///     This implementation uses <see cref="BluetoothLEAdvertisementWatcher" /> to monitor BLE advertisements.
/// </remarks>
public class BluetoothScanner : BaseBluetoothScanner,
    BluetoothLeAdvertisementWatcherWrapper.IBluetoothLeAdvertisementWatcherProxyDelegate
{
    private BluetoothLeAdvertisementWatcherWrapper? _watcher;

    /// <inheritdoc />
    public BluetoothScanner(
        IBluetoothAdapter adapter,
        IBluetoothPermissionManager permissionManager,
        IBluetoothDeviceFactory deviceFactory,
        ITicker ticker,
        IBluetoothRssiToSignalStrengthConverter rssiToSignalStrengthConverter,
        ILogger<IBluetoothScanner>? logger = null)
        : base(adapter, permissionManager, deviceFactory, rssiToSignalStrengthConverter, ticker, logger)
    {
    }

    #region Delegate Callbacks

    /// <summary>
    ///     Called when a Bluetooth LE advertisement is received.
    /// </summary>
    /// <param name="argsAdvertisement">The advertisement event arguments.</param>
    public void OnAdvertisementReceived(BluetoothLEAdvertisementReceivedEventArgs argsAdvertisement)
    {
        var advertisement = new BluetoothAdvertisement(argsAdvertisement);
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
    /// </remarks>
    protected override void NativeRefreshIsRunning()
    {
        if (_watcher?.BluetoothLeAdvertisementWatcher is { } watcher)
        {
            IsRunning = watcher.Status == BluetoothLEAdvertisementWatcherStatus.Started;
        }
        else
        {
            IsRunning = false;
        }
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
        // Create wrapper if needed
        _watcher ??= new BluetoothLeAdvertisementWatcherWrapper(this);

        // Start watcher (status change callback will call OnStartSucceeded)
        _watcher.BluetoothLeAdvertisementWatcher.Start();

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
        if (advertisement is not BluetoothAdvertisement windowsAdvertisement)
        {
            throw new ArgumentException(
                $"Expected advertisement of type {typeof(BluetoothAdvertisement)}",
                nameof(advertisement));
        }

        return new BluetoothDeviceFactoryRequest(windowsAdvertisement);
    }

    #endregion
}