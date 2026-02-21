namespace Bluetooth.Core.Scanning.Factories;

/// <inheritdoc />
public abstract class BaseBluetoothCharacteristicFactory : IBluetoothCharacteristicFactory
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseBluetoothCharacteristicFactory" /> class.
    /// </summary>
    /// <param name="descriptorFactory">The descriptor factory to pass to the new Characteristic.</param>
    protected BaseBluetoothCharacteristicFactory(IBluetoothDescriptorFactory descriptorFactory)
    {
        DescriptorFactory = descriptorFactory;
    }

    /// <summary>
    ///     Gets the descriptor factory to pass to the new Characteristic.
    /// </summary>
    protected IBluetoothDescriptorFactory DescriptorFactory { get; }

    /// <inheritdoc />
    public abstract IBluetoothRemoteCharacteristic CreateCharacteristic(IBluetoothRemoteService remoteService, IBluetoothCharacteristicFactory.BluetoothCharacteristicFactoryRequest request);
}