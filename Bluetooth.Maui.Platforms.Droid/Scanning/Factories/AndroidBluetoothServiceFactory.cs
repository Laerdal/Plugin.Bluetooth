using Bluetooth.Core.Scanning.Factories;
using Microsoft.Extensions.Logging;

namespace Bluetooth.Maui.Platforms.Droid.Scanning.Factories;

/// <inheritdoc />
public class AndroidBluetoothServiceFactory : BaseBluetoothServiceFactory, IBluetoothRemoteServiceFactory
{
    /// <inheritdoc />
    public AndroidBluetoothServiceFactory(
        IBluetoothRemoteCharacteristicFactory characteristicFactory,
        ILoggerFactory? loggerFactory = null)
        : base(characteristicFactory, loggerFactory)
    {
    }

    /// <inheritdoc />
    public override IBluetoothRemoteService Create(
        IBluetoothRemoteDevice device,
        IBluetoothRemoteServiceFactory.BluetoothRemoteServiceFactorySpec spec)
    {
        var logger = LoggerFactory?.CreateLogger<IBluetoothRemoteService>();
        return new AndroidBluetoothRemoteService(device, spec, CharacteristicFactory, logger);
    }
}
