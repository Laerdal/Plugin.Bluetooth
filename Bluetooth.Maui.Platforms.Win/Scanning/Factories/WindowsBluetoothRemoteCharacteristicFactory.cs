using Microsoft.Extensions.Logging;

namespace Bluetooth.Maui.Platforms.Win.Scanning.Factories;

/// <inheritdoc/>
public class WindowsBluetoothRemoteCharacteristicFactory : IBluetoothRemoteCharacteristicFactory
{
    private readonly IBluetoothRemoteDescriptorFactory _descriptorFactory;
    private readonly IBluetoothNameProvider? _nameProvider;
    private readonly ILoggerFactory? _loggerFactory;

    /// <inheritdoc/>
    public WindowsBluetoothRemoteCharacteristicFactory(IBluetoothRemoteDescriptorFactory descriptorFactory, IBluetoothNameProvider? nameProvider = null, ILoggerFactory? loggerFactory = null)
    {
        _descriptorFactory = descriptorFactory;
        _nameProvider = nameProvider;
        _loggerFactory = loggerFactory;
    }

    /// <inheritdoc/>
    public Abstractions.Scanning.IBluetoothRemoteCharacteristic Create(
        Abstractions.Scanning.IBluetoothRemoteService service,
        IBluetoothRemoteCharacteristicFactory.BluetoothRemoteCharacteristicFactorySpec spec)
    {
        var logger = _loggerFactory?.CreateLogger<IBluetoothRemoteCharacteristic>();
        return new WindowsBluetoothRemoteCharacteristic(service, spec, _descriptorFactory, _nameProvider, logger);
    }
}
