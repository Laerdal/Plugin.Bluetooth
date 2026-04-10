namespace Bluetooth.Avalonia.Platforms.Linux.Scanning.Factories;

/// <inheritdoc />
internal sealed class LinuxBluetoothRemoteDescriptorFactory : IBluetoothRemoteDescriptorFactory
{
    private readonly ILoggerFactory? _loggerFactory;

    public LinuxBluetoothRemoteDescriptorFactory(ILoggerFactory? loggerFactory = null)
    {
        _loggerFactory = loggerFactory;
    }

    /// <inheritdoc />
    public IBluetoothRemoteDescriptor Create(
        IBluetoothRemoteCharacteristic characteristic,
        IBluetoothRemoteDescriptorFactory.BluetoothRemoteDescriptorFactorySpec spec)
    {
        var logger = _loggerFactory?.CreateLogger<IBluetoothRemoteDescriptor>();
        return new LinuxBluetoothRemoteDescriptor(characteristic, spec, logger: logger);
    }
}
