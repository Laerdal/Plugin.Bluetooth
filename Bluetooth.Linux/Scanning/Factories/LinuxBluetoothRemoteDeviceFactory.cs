namespace Bluetooth.Linux.Scanning.Factories;

/// <inheritdoc />
public class LinuxBluetoothRemoteDeviceFactory : IBluetoothRemoteDeviceFactory
{
    private readonly IBluetoothRemoteServiceFactory _serviceFactory;
    private readonly IBluetoothRssiToSignalStrengthConverter _rssiConverter;
    private readonly ILoggerFactory? _loggerFactory;

    /// <inheritdoc />
    public LinuxBluetoothRemoteDeviceFactory(
        IBluetoothRemoteServiceFactory serviceFactory,
        IBluetoothRssiToSignalStrengthConverter rssiConverter,
        ILoggerFactory? loggerFactory = null)
    {
        _serviceFactory = serviceFactory;
        _rssiConverter = rssiConverter;
        _loggerFactory = loggerFactory;
    }

    /// <inheritdoc />
    public IBluetoothRemoteDevice Create(IBluetoothScanner scanner, IBluetoothRemoteDeviceFactory.BluetoothRemoteDeviceFactorySpec spec)
    {
        var logger = _loggerFactory?.CreateLogger<IBluetoothRemoteDevice>();
        return new LinuxBluetoothRemoteDevice(scanner, spec, _serviceFactory, _rssiConverter, logger);
    }
}
