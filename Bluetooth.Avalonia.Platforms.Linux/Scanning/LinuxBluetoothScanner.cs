namespace Bluetooth.Avalonia.Platforms.Linux.Scanning;

/// <inheritdoc />
public class LinuxBluetoothScanner : BaseBluetoothScanner
{

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    public LinuxBluetoothScanner(IBluetoothAdapter adapter,
        IBluetoothRssiToSignalStrengthConverter rssiToSignalStrengthConverter,
        ITicker ticker,
        IBluetoothNameProvider? nameProvider = null,
        ILoggerFactory? loggerFactory = null) : base(adapter,
                                                     rssiToSignalStrengthConverter,
                                                     ticker,
                                                     nameProvider,
                                                     loggerFactory)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. Linux / Desktop BLE is not yet implemented. BlueZ integration is planned.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override void NativeRefreshIsRunning()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. Linux / Desktop BLE is not yet implemented. BlueZ integration is planned.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask NativeStartAsync(ScanningOptions scanningOptions, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. Linux / Desktop BLE is not yet implemented. BlueZ integration is planned.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask NativeStopAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. Linux / Desktop BLE is not yet implemented. BlueZ integration is planned.");
    }
    
    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask<bool> NativeHasScannerPermissionsAsync()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. Linux / Desktop BLE is not yet implemented. BlueZ integration is planned.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask NativeRequestScannerPermissionsAsync(bool requireBackgroundLocation, CancellationToken cancellationToken)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. Linux / Desktop BLE is not yet implemented. BlueZ integration is planned.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override IBluetoothRemoteDevice NativeCreateDeviceFromAdvertisement(IBluetoothAdvertisement advertisement)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. Linux / Desktop BLE is not yet implemented. BlueZ integration is planned.");
    }
}
