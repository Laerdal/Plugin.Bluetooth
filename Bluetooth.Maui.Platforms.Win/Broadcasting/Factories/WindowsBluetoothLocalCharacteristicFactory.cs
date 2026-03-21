namespace Bluetooth.Maui.Platforms.Win.Broadcasting.Factories;

/// <inheritdoc />
public class WindowsBluetoothLocalCharacteristicFactory : IBluetoothLocalCharacteristicFactory
{
    private readonly IBluetoothLocalDescriptorFactory _localDescriptorFactory;

    /// <inheritdoc />
    public WindowsBluetoothLocalCharacteristicFactory(IBluetoothLocalDescriptorFactory localDescriptorFactory)
    {
        _localDescriptorFactory = localDescriptorFactory;
    }

    /// <inheritdoc />
    public IBluetoothLocalCharacteristic Create(IBluetoothLocalService service, IBluetoothLocalCharacteristicFactory.BluetoothLocalCharacteristicSpec spec)
    {
        throw new NotSupportedException("Windows local GATT characteristic creation is not implemented yet.");
    }
}
