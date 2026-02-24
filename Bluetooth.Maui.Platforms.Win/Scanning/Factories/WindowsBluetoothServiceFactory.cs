using Bluetooth.Core.Scanning.Factories;
using Microsoft.Extensions.Logging;

namespace Bluetooth.Maui.Platforms.Win.Scanning.Factories;

/// <inheritdoc/>
public class WindowsBluetoothServiceFactory : BaseBluetoothServiceFactory, Abstractions.Scanning.Factories.IBluetoothRemoteServiceFactory
{
    /// <inheritdoc/>
    public WindowsBluetoothServiceFactory(IBluetoothRemoteCharacteristicFactory characteristicFactory, ILoggerFactory? loggerFactory = null)
        : base(characteristicFactory, loggerFactory)
    {
    }

    /// <inheritdoc/>
    public override Abstractions.Scanning.IBluetoothRemoteService Create(
        IBluetoothRemoteDevice device,
        IBluetoothRemoteServiceFactory.BluetoothRemoteServiceFactorySpec spec)
    {
        var logger = LoggerFactory?.CreateLogger<IBluetoothRemoteService>();
        return new WindowsBluetoothRemoteService(device, spec, CharacteristicFactory, logger);
    }
}
