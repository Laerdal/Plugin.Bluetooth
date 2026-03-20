namespace Bluetooth.Maui.Platforms.Droid.Broadcasting.Factories;

/// <inheritdoc />
public class AndroidBluetoothLocalCharacteristicFactory : IBluetoothLocalCharacteristicFactory
{
    private readonly IBluetoothLocalDescriptorFactory _localDescriptorFactory;

    /// <inheritdoc />
    public AndroidBluetoothLocalCharacteristicFactory(IBluetoothLocalDescriptorFactory localDescriptorFactory)
    {
        _localDescriptorFactory = localDescriptorFactory;
    }

    /// <inheritdoc />
    public IBluetoothLocalCharacteristic Create(IBluetoothLocalService service, IBluetoothLocalCharacteristicFactory.BluetoothLocalCharacteristicSpec spec)
    {
        return new AndroidBluetoothLocalCharacteristic(service, spec, _localDescriptorFactory);
    }
}
