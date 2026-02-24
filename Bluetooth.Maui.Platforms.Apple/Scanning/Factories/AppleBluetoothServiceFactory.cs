using Bluetooth.Core.Scanning.Factories;

namespace Bluetooth.Maui.Platforms.Apple.Scanning.Factories;

/// <inheritdoc />
public class AppleBluetoothServiceFactory : BaseBluetoothServiceFactory
{
    /// <inheritdoc />
    public AppleBluetoothServiceFactory(IBluetoothRemoteCharacteristicFactory characteristicFactory) : base(characteristicFactory)
    {
    }

    /// <inheritdoc />
    public override IBluetoothRemoteService Create(IBluetoothRemoteDevice device, IBluetoothRemoteServiceFactory.BluetoothRemoteServiceFactorySpec spec)
    {
        return new AppleBluetoothRemoteService(device, spec, CharacteristicFactory);
    }
}
