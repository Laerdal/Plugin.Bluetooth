using Bluetooth.Abstractions.Broadcasting.Factories;
using Bluetooth.Core.Broadcasting.Factories;

namespace Bluetooth.Maui.Platforms.Droid.Broadcasting.Factories;

/// <inheritdoc />
public class BluetoothLocalServiceFactory : BaseBluetoothLocalServiceFactory
{
    /// <inheritdoc />
    public BluetoothLocalServiceFactory(IBluetoothLocalCharacteristicFactory localCharacteristicFactory) : base(localCharacteristicFactory)
    {
    }

    /// <inheritdoc />
    public override Abstractions.Broadcasting.IBluetoothLocalService CreateService(IBluetoothBroadcaster broadcaster, IBluetoothLocalServiceFactory.BluetoothLocalServiceSpec spec)
    {
        throw new NotImplementedException();
    }
}
