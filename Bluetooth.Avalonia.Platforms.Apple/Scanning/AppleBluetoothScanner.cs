namespace Bluetooth.Avalonia.Platforms.Apple.Scanning;

/// <inheritdoc />
public class AppleBluetoothScanner : BaseBluetoothScanner
{

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    public AppleBluetoothScanner(IBluetoothAdapter adapter,
        IBluetoothRssiToSignalStrengthConverter rssiToSignalStrengthConverter,
        ITicker ticker,
        IBluetoothNameProvider? nameProvider = null,
        ILoggerFactory? loggerFactory = null) : base(adapter,
                                                     rssiToSignalStrengthConverter,
                                                     ticker,
                                                     nameProvider,
                                                     loggerFactory)
    {
        throw new PlatformNotSupportedException("Apple BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Apple.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override void NativeRefreshIsRunning()
    {
        throw new PlatformNotSupportedException("Apple BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Apple.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask NativeStartAsync(ScanningOptions scanningOptions, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new PlatformNotSupportedException("Apple BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Apple.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask NativeStopAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new PlatformNotSupportedException("Apple BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Apple.");
    }
    
    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask<bool> NativeHasScannerPermissionsAsync()
    {
        throw new PlatformNotSupportedException("Apple BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Apple.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask NativeRequestScannerPermissionsAsync(bool requireBackgroundLocation, CancellationToken cancellationToken)
    {
        throw new PlatformNotSupportedException("Apple BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Apple.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override IBluetoothRemoteDevice NativeCreateDeviceFromAdvertisement(IBluetoothAdvertisement advertisement)
    {
        throw new PlatformNotSupportedException("Apple BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Apple.");
    }
}
