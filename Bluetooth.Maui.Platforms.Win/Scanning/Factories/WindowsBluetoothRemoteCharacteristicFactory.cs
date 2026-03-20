using Microsoft.Extensions.Logging;

namespace Bluetooth.Maui.Platforms.Win.Scanning.Factories;

/// <inheritdoc/>
public class WindowsBluetoothRemoteCharacteristicFactory : IBluetoothRemoteCharacteristicFactory
{
    private readonly IBluetoothRemoteDescriptorFactory _descriptorFactory;
    private readonly ILoggerFactory? _loggerFactory;

    /// <inheritdoc/>
    public WindowsBluetoothRemoteCharacteristicFactory(IBluetoothRemoteDescriptorFactory descriptorFactory, ILoggerFactory? loggerFactory = null)
    {
        _descriptorFactory = descriptorFactory;
        _loggerFactory = loggerFactory;
    }

    /// <inheritdoc/>
    public Abstractions.Scanning.IBluetoothRemoteCharacteristic Create(
        Abstractions.Scanning.IBluetoothRemoteService service,
        IBluetoothRemoteCharacteristicFactory.BluetoothRemoteCharacteristicFactorySpec spec)
    {
        var logger = _loggerFactory?.CreateLogger<IBluetoothRemoteCharacteristic>();
        return new WindowsBluetoothRemoteCharacteristic(service, spec, _descriptorFactory, logger);
    }
}
