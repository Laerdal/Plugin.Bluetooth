namespace Bluetooth.Maui.Platforms.Apple.Scanning.Factories;

/// <summary>
///     Apple implementation of <see cref="IBluetoothRemoteDeviceFactory" />.
///     Creates <see cref="AppleBluetoothRemoteDevice" /> instances from Apple-specific factory specs.
/// </summary>
public class AppleBluetoothRemoteDeviceFactory : IBluetoothRemoteDeviceFactory
{
    private readonly IBluetoothRemoteServiceFactory _serviceFactory;
    private readonly IBluetoothRssiToSignalStrengthConverter _rssiToSignalStrengthConverter;
    private readonly ILoggerFactory? _loggerFactory;

    /// <summary>
    ///     Initializes a new instance of the <see cref="AppleBluetoothRemoteDeviceFactory" /> class.
    /// </summary>
    /// <param name="serviceFactory">The factory for creating remote services.</param>
    /// <param name="rssiToSignalStrengthConverter">The converter for RSSI to signal strength.</param>
    /// <param name="loggerFactory">Optional logger factory for creating loggers.</param>
    public AppleBluetoothRemoteDeviceFactory(
        IBluetoothRemoteServiceFactory serviceFactory,
        IBluetoothRssiToSignalStrengthConverter rssiToSignalStrengthConverter,
        ILoggerFactory? loggerFactory = null)
    {
        _serviceFactory = serviceFactory;
        _rssiToSignalStrengthConverter = rssiToSignalStrengthConverter;
        _loggerFactory = loggerFactory;
    }

    /// <inheritdoc />
    public IBluetoothRemoteDevice Create(IBluetoothScanner scanner, IBluetoothRemoteDeviceFactory.BluetoothRemoteDeviceFactorySpec spec)
    {
        if (spec is not AppleBluetoothRemoteDeviceFactorySpec appleSpec)
        {
            throw new ArgumentException($"Spec must be {nameof(AppleBluetoothRemoteDeviceFactorySpec)}", nameof(spec));
        }

        var logger = _loggerFactory?.CreateLogger<IBluetoothRemoteDevice>();
        return new AppleBluetoothRemoteDevice(scanner, appleSpec, _serviceFactory, _rssiToSignalStrengthConverter, logger);
    }
}
