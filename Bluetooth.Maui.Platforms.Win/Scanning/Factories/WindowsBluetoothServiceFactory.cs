using Bluetooth.Core.Scanning.Factories;

namespace Bluetooth.Maui.Platforms.Win.Scanning.Factories;

/// <inheritdoc/>
public class WindowsBluetoothServiceFactory : BaseBluetoothServiceFactory, Abstractions.Scanning.Factories.IBluetoothServiceFactory
{
    /// <inheritdoc/>
    public WindowsBluetoothServiceFactory(IBluetoothCharacteristicFactory characteristicFactory)
        : base(characteristicFactory)
    {
    }

    /// <inheritdoc/>
    public override Abstractions.Scanning.IBluetoothRemoteService CreateService(
        IBluetoothRemoteDevice device,
        IBluetoothServiceFactory.BluetoothServiceFactoryRequest request)
    {
        return new WindowsBluetoothRemoteService(device, request, CharacteristicFactory);
    }
}