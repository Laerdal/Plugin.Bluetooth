namespace Bluetooth.Linux.Scanning.Factories;

/// <inheritdoc />
public class LinuxBluetoothRemoteServiceFactory : IBluetoothRemoteServiceFactory
{
    private readonly IBluetoothRemoteCharacteristicFactory _characteristicFactory;
    private readonly ILoggerFactory? _loggerFactory;

    /// <inheritdoc />
    public LinuxBluetoothRemoteServiceFactory(
        IBluetoothRemoteCharacteristicFactory characteristicFactory,
        ILoggerFactory? loggerFactory = null)
    {
        _characteristicFactory = characteristicFactory;
        _loggerFactory = loggerFactory;
    }

    /// <inheritdoc />
    public IBluetoothRemoteService Create(IBluetoothRemoteDevice device, IBluetoothRemoteServiceFactory.BluetoothRemoteServiceFactorySpec spec)
    {
        var logger = _loggerFactory?.CreateLogger<IBluetoothRemoteService>();
        return new LinuxBluetoothRemoteService(device, spec, _characteristicFactory, logger: logger);
    }
}
