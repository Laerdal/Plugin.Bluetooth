using Microsoft.Extensions.Logging;

namespace Bluetooth.Maui.Platforms.Droid.Scanning.Factories;

/// <inheritdoc />
public class AndroidBluetoothServiceFactory : IBluetoothRemoteServiceFactory
{
    private readonly IBluetoothRemoteCharacteristicFactory _characteristicFactory;
    private readonly ILoggerFactory? _loggerFactory;

    /// <inheritdoc />
    public AndroidBluetoothServiceFactory(
        IBluetoothRemoteCharacteristicFactory characteristicFactory,
        ILoggerFactory? loggerFactory = null)
    {
        _characteristicFactory = characteristicFactory;
        _loggerFactory = loggerFactory;
    }

    /// <inheritdoc />
    public IBluetoothRemoteService Create(
        IBluetoothRemoteDevice device,
        IBluetoothRemoteServiceFactory.BluetoothRemoteServiceFactorySpec spec)
    {
        var logger = _loggerFactory?.CreateLogger<IBluetoothRemoteService>();
        return new AndroidBluetoothRemoteService(device, spec, _characteristicFactory, logger);
    }
}
