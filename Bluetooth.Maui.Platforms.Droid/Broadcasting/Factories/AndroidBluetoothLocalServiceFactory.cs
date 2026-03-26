namespace Bluetooth.Maui.Platforms.Droid.Broadcasting.Factories;

/// <inheritdoc />
public class AndroidBluetoothLocalServiceFactory : IBluetoothLocalServiceFactory
{
    /// <inheritdoc />
    public IBluetoothLocalService Create(IBluetoothBroadcaster broadcaster, IBluetoothLocalServiceFactory.BluetoothLocalServiceSpec spec)
    {
        return new AndroidBluetoothLocalService(broadcaster, spec);
    }
}
