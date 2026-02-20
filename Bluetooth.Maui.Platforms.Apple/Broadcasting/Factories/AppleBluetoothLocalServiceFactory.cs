using Bluetooth.Core.Broadcasting.Factories;

namespace Bluetooth.Maui.Platforms.Apple.Broadcasting.Factories;

/// <inheritdoc />
public class AppleBluetoothLocalServiceFactory : BaseBluetoothLocalServiceFactory
{
    /// <inheritdoc />
    public AppleBluetoothLocalServiceFactory(IBluetoothLocalCharacteristicFactory localCharacteristicFactory) : base(localCharacteristicFactory)
    {
    }

    /// <inheritdoc />
    public override IBluetoothLocalService CreateService(IBluetoothBroadcaster broadcaster, IBluetoothLocalServiceFactory.BluetoothLocalServiceSpec spec)
    {
        return new AppleBluetoothLocalService(broadcaster, spec, LocalCharacteristicFactory);
    }
}