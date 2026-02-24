using Bluetooth.Core.Scanning.Factories;

namespace Bluetooth.Maui.Platforms.Win.Scanning.Factories;

/// <inheritdoc/>
public class WindowsBluetoothServiceFactory : BaseBluetoothServiceFactory, Abstractions.Scanning.Factories.IBluetoothRemoteServiceFactory
{
    /// <inheritdoc/>
    public WindowsBluetoothServiceFactory(IBluetoothRemoteCharacteristicFactory characteristicFactory)
        : base(characteristicFactory)
    {
    }

    /// <inheritdoc/>
    public override Abstractions.Scanning.IBluetoothRemoteService Create(
        IBluetoothRemoteDevice device,
        IBluetoothRemoteServiceFactory.BluetoothRemoteServiceFactorySpec spec)
    {
        return new WindowsBluetoothRemoteService(device, spec, CharacteristicFactory);
    }
}
