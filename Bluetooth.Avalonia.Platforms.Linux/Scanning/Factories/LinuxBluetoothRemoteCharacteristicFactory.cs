namespace Bluetooth.Avalonia.Platforms.Linux.Scanning.Factories;

/// <inheritdoc />
internal sealed class LinuxBluetoothRemoteCharacteristicFactory : IBluetoothRemoteCharacteristicFactory
{
    private readonly IBluetoothRemoteDescriptorFactory _descriptorFactory;
    private readonly IBluetoothNameProvider? _nameProvider;
    private readonly ILoggerFactory? _loggerFactory;

    public LinuxBluetoothRemoteCharacteristicFactory(
        IBluetoothRemoteDescriptorFactory descriptorFactory,
        IBluetoothNameProvider? nameProvider = null,
        ILoggerFactory? loggerFactory = null)
    {
        _descriptorFactory = descriptorFactory;
        _nameProvider = nameProvider;
        _loggerFactory = loggerFactory;
    }

    /// <inheritdoc />
    public IBluetoothRemoteCharacteristic Create(
        IBluetoothRemoteService service,
        IBluetoothRemoteCharacteristicFactory.BluetoothRemoteCharacteristicFactorySpec spec)
    {
        var logger = _loggerFactory?.CreateLogger<IBluetoothRemoteCharacteristic>();
        return new LinuxBluetoothRemoteCharacteristic(service, spec, _descriptorFactory, _nameProvider, logger);
    }
}
