using Bluetooth.Abstractions.Scanning.Factories;
using Bluetooth.Core.Scanning.Factories;

namespace Bluetooth.Maui.Platforms.Apple.Scanning.Factories;

/// <inheritdoc />
public class AppleBluetoothServiceFactory : BaseBluetoothServiceFactory
{
    /// <inheritdoc />
    public AppleBluetoothServiceFactory(IBluetoothCharacteristicFactory characteristicFactory) : base(characteristicFactory)
    {
    }

    /// <inheritdoc />
    public override Abstractions.Scanning.IBluetoothRemoteService CreateService(IBluetoothRemoteDevice device, IBluetoothServiceFactory.BluetoothServiceFactoryRequest request)
    {
        return new AppleBluetoothRemoteService(device, request, CharacteristicFactory);
    }
}
