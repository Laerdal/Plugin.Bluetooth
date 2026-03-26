using Microsoft.Extensions.Logging;

namespace Bluetooth.Maui.Platforms.Droid.Scanning.Factories;

/// <inheritdoc />
public class AndroidBluetoothRemoteCharacteristicFactory : IBluetoothRemoteCharacteristicFactory
{
    private readonly IBluetoothRemoteDescriptorFactory _descriptorFactory;
    private readonly IBluetoothNameProvider? _nameProvider;
    private readonly ILoggerFactory? _loggerFactory;

    /// <inheritdoc />
    public AndroidBluetoothRemoteCharacteristicFactory(
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
        return new AndroidBluetoothRemoteCharacteristic(service, spec, _descriptorFactory, _nameProvider, logger);
    }
}
