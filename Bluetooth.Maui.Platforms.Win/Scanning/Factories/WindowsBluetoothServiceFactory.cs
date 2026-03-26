using Microsoft.Extensions.Logging;

namespace Bluetooth.Maui.Platforms.Win.Scanning.Factories;

/// <inheritdoc/>
public class WindowsBluetoothServiceFactory : IBluetoothRemoteServiceFactory
{
    private readonly IBluetoothRemoteCharacteristicFactory _characteristicFactory;
    private readonly IBluetoothNameProvider? _nameProvider;
    private readonly ILoggerFactory? _loggerFactory;

    /// <inheritdoc/>
    public WindowsBluetoothServiceFactory(IBluetoothRemoteCharacteristicFactory characteristicFactory, IBluetoothNameProvider? nameProvider = null, ILoggerFactory? loggerFactory = null)
    {
        _characteristicFactory = characteristicFactory;
        _nameProvider = nameProvider;
        _loggerFactory = loggerFactory;
    }

    /// <inheritdoc/>
    public Abstractions.Scanning.IBluetoothRemoteService Create(
        IBluetoothRemoteDevice device,
        IBluetoothRemoteServiceFactory.BluetoothRemoteServiceFactorySpec spec)
    {
        var logger = _loggerFactory?.CreateLogger<IBluetoothRemoteService>();
        return new WindowsBluetoothRemoteService(device, spec, _characteristicFactory, _nameProvider, logger);
    }
}
