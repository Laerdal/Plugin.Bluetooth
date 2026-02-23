using Bluetooth.Core.Scanning.Factories;

namespace Bluetooth.Maui.Platforms.Droid.Scanning.Factories;

/// <inheritdoc />
public class AndroidBluetoothServiceFactory : BaseBluetoothServiceFactory, IBluetoothServiceFactory
{
    /// <inheritdoc />
    public AndroidBluetoothServiceFactory(IBluetoothCharacteristicFactory characteristicFactory)
        : base(characteristicFactory)
    {
    }

    /// <inheritdoc />
    public override IBluetoothRemoteService CreateService(
        IBluetoothRemoteDevice device,
        IBluetoothServiceFactory.BluetoothServiceFactoryRequest request)
    {
        return new AndroidBluetoothRemoteService(device, request, CharacteristicFactory);
    }
}
