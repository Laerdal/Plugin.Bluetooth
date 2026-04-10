namespace Bluetooth.Avalonia.Platforms.Linux.Scanning.Factories;

/// <inheritdoc />
internal sealed class LinuxBluetoothRemoteDeviceFactory : IBluetoothRemoteDeviceFactory
{
    private readonly IBluetoothRemoteServiceFactory _serviceFactory;
    private readonly IBluetoothRssiToSignalStrengthConverter _rssiConverter;
    private readonly LinuxBluetoothAdapter _adapter;
    private readonly ILoggerFactory? _loggerFactory;

    public LinuxBluetoothRemoteDeviceFactory(
        IBluetoothRemoteServiceFactory serviceFactory,
        IBluetoothRssiToSignalStrengthConverter rssiConverter,
        LinuxBluetoothAdapter adapter,
        ILoggerFactory? loggerFactory = null)
    {
        _serviceFactory = serviceFactory;
        _rssiConverter = rssiConverter;
        _adapter = adapter;
        _loggerFactory = loggerFactory;
    }

    /// <inheritdoc />
    public IBluetoothRemoteDevice Create(
        IBluetoothScanner scanner,
        IBluetoothRemoteDeviceFactory.BluetoothRemoteDeviceFactorySpec spec)
    {
        var logger = _loggerFactory?.CreateLogger<IBluetoothRemoteDevice>();
        return new LinuxBluetoothRemoteDevice(scanner, spec, _serviceFactory, _rssiConverter, _adapter, logger);
    }
}
