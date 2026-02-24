using Bluetooth.Core.Scanning.Factories;

namespace Bluetooth.Maui.Platforms.Droid.Scanning.Factories;

/// <inheritdoc />
public class AndroidBluetoothServiceFactory : BaseBluetoothServiceFactory, IBluetoothRemoteServiceFactory
{
    /// <inheritdoc />
    public AndroidBluetoothServiceFactory(IBluetoothRemoteCharacteristicFactory characteristicFactory)
        : base(characteristicFactory)
    {
    }

    /// <inheritdoc />
    public override IBluetoothRemoteService Create(
        IBluetoothRemoteDevice device,
        IBluetoothRemoteServiceFactory.BluetoothRemoteServiceFactorySpec spec)
    {
        return new AndroidBluetoothRemoteService(device, spec, CharacteristicFactory);
    }
}
