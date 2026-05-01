namespace Bluetooth.Linux.Scanning.Factories;

/// <inheritdoc />
public class LinuxBluetoothRemoteCharacteristicFactory : IBluetoothRemoteCharacteristicFactory
{
    private readonly IBluetoothRemoteDescriptorFactory _descriptorFactory;
    private readonly ILoggerFactory? _loggerFactory;

    /// <inheritdoc />
    public LinuxBluetoothRemoteCharacteristicFactory(
        IBluetoothRemoteDescriptorFactory descriptorFactory,
        ILoggerFactory? loggerFactory = null)
    {
        _descriptorFactory = descriptorFactory;
        _loggerFactory = loggerFactory;
    }

    /// <inheritdoc />
    public IBluetoothRemoteCharacteristic Create(IBluetoothRemoteService service, IBluetoothRemoteCharacteristicFactory.BluetoothRemoteCharacteristicFactorySpec spec)
    {
        var logger = _loggerFactory?.CreateLogger<IBluetoothRemoteCharacteristic>();
        return new LinuxBluetoothRemoteCharacteristic(service, spec, _descriptorFactory, logger: logger);
    }
}
