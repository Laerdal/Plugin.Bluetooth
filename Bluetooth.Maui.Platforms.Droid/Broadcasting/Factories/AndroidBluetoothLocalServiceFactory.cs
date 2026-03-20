namespace Bluetooth.Maui.Platforms.Droid.Broadcasting.Factories;

/// <inheritdoc />
public class AndroidBluetoothLocalServiceFactory : IBluetoothLocalServiceFactory
{
    private readonly IBluetoothLocalCharacteristicFactory _characteristicFactory;

    /// <inheritdoc />
    public AndroidBluetoothLocalServiceFactory(IBluetoothLocalCharacteristicFactory characteristicFactory)
    {
        _characteristicFactory = characteristicFactory;
    }

    /// <inheritdoc />
    public IBluetoothLocalService Create(IBluetoothBroadcaster broadcaster, IBluetoothLocalServiceFactory.BluetoothLocalServiceSpec spec)
    {
        return new AndroidBluetoothLocalService(broadcaster, spec, _characteristicFactory);
    }
}
