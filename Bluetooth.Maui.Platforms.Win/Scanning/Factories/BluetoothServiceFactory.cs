using Bluetooth.Core.Scanning.Factories;

namespace Bluetooth.Maui.Platforms.Win.Scanning.Factories;

/// <inheritdoc />
public class BluetoothServiceFactory : BaseBluetoothServiceFactory, IBluetoothServiceFactory
{
    /// <inheritdoc />
    public BluetoothServiceFactory(IBluetoothCharacteristicFactory characteristicFactory)
        : base(characteristicFactory)
    {
    }

    /// <inheritdoc />
    public override IBluetoothRemoteService CreateService(
        IBluetoothRemoteDevice device,
        IBluetoothServiceFactory.BluetoothServiceFactoryRequest request)
    {
        return new WindowsBluetoothRemoteService(device, request, CharacteristicFactory);
    }
}