namespace Bluetooth.Avalonia.Platforms.Linux.Scanning.Factories;

/// <inheritdoc />
internal sealed class LinuxBluetoothRemoteServiceFactory : IBluetoothRemoteServiceFactory
{
    private readonly IBluetoothRemoteCharacteristicFactory _characteristicFactory;
    private readonly IBluetoothNameProvider? _nameProvider;
    private readonly ILoggerFactory? _loggerFactory;

    public LinuxBluetoothRemoteServiceFactory(
        IBluetoothRemoteCharacteristicFactory characteristicFactory,
        IBluetoothNameProvider? nameProvider = null,
        ILoggerFactory? loggerFactory = null)
    {
        _characteristicFactory = characteristicFactory;
        _nameProvider = nameProvider;
        _loggerFactory = loggerFactory;
    }

    /// <inheritdoc />
    public IBluetoothRemoteService Create(
        IBluetoothRemoteDevice device,
        IBluetoothRemoteServiceFactory.BluetoothRemoteServiceFactorySpec spec)
    {
        var logger = _loggerFactory?.CreateLogger<IBluetoothRemoteService>();
        return new LinuxBluetoothRemoteService(device, spec, _characteristicFactory, _nameProvider, logger);
    }
}
