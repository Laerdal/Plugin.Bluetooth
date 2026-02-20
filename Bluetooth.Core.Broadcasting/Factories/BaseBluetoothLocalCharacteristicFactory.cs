namespace Bluetooth.Core.Broadcasting.Factories;

/// <inheritdoc />
public abstract class BaseBluetoothLocalCharacteristicFactory : IBluetoothLocalCharacteristicFactory
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseBluetoothLocalCharacteristicFactory" /> class.
    /// </summary>
    protected BaseBluetoothLocalCharacteristicFactory(IBluetoothLocalDescriptorFactory localDescriptorFactory)
    {
        LocalDescriptorFactory = localDescriptorFactory;
    }

    /// <summary>
    ///     Gets the descriptor factory to pass to the new Characteristic.
    /// </summary>
    protected IBluetoothLocalDescriptorFactory LocalDescriptorFactory { get; }

    /// <inheritdoc />
    public abstract IBluetoothLocalCharacteristic CreateCharacteristic(IBluetoothLocalService localService, IBluetoothLocalCharacteristicFactory.BluetoothLocalCharacteristicSpec spec);
}