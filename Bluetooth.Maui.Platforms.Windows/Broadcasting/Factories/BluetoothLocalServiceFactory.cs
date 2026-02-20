using Bluetooth.Core.Broadcasting.Factories;

namespace Bluetooth.Maui.Platforms.Windows.Broadcasting.Factories;

/// <inheritdoc />
public class BluetoothLocalServiceFactory : BaseBluetoothLocalServiceFactory
{
    /// <inheritdoc />
    public BluetoothLocalServiceFactory(IBluetoothLocalCharacteristicFactory localCharacteristicFactory) : base(localCharacteristicFactory)
    {
    }

    /// <inheritdoc />
    public override IBluetoothLocalService CreateService(IBluetoothBroadcaster broadcaster, IBluetoothLocalServiceFactory.BluetoothLocalServiceSpec spec)
    {
        throw new NotImplementedException();
    }
}