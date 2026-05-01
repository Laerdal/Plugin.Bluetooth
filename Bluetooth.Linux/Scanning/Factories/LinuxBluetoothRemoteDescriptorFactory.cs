namespace Bluetooth.Linux.Scanning.Factories;

/// <inheritdoc />
public class LinuxBluetoothRemoteDescriptorFactory : IBluetoothRemoteDescriptorFactory
{
    private readonly ILoggerFactory? _loggerFactory;

    /// <inheritdoc />
    public LinuxBluetoothRemoteDescriptorFactory(ILoggerFactory? loggerFactory = null)
    {
        _loggerFactory = loggerFactory;
    }

    /// <inheritdoc />
    public IBluetoothRemoteDescriptor Create(IBluetoothRemoteCharacteristic characteristic, IBluetoothRemoteDescriptorFactory.BluetoothRemoteDescriptorFactorySpec spec)
    {
        var logger = _loggerFactory?.CreateLogger<IBluetoothRemoteDescriptor>();
        return new LinuxBluetoothRemoteDescriptor(characteristic, spec, logger);
    }
}
