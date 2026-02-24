using Bluetooth.Core.Scanning.Factories;
using Microsoft.Extensions.Logging;

namespace Bluetooth.Maui.Platforms.Apple.Scanning.Factories;

/// <inheritdoc />
public class AppleBluetoothServiceFactory : BaseBluetoothServiceFactory
{
    /// <inheritdoc />
    public AppleBluetoothServiceFactory(
        IBluetoothRemoteCharacteristicFactory characteristicFactory,
        ILoggerFactory? loggerFactory = null)
        : base(characteristicFactory, loggerFactory)
    {
    }

    /// <inheritdoc />
    public override IBluetoothRemoteService Create(IBluetoothRemoteDevice device, IBluetoothRemoteServiceFactory.BluetoothRemoteServiceFactorySpec spec)
    {
        var logger = LoggerFactory?.CreateLogger<IBluetoothRemoteService>();
        return new AppleBluetoothRemoteService(device, spec, CharacteristicFactory, logger);
    }
}
