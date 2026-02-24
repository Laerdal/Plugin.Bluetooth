namespace Bluetooth.Core.Scanning.Factories;

/// <inheritdoc />
public abstract class BaseBluetoothCharacteristicFactory : IBluetoothRemoteCharacteristicFactory
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseBluetoothCharacteristicFactory" /> class.
    /// </summary>
    /// <param name="descriptorFactory">The descriptor factory to pass to the new Characteristic.</param>
    protected BaseBluetoothCharacteristicFactory(IBluetoothRemoteDescriptorFactory descriptorFactory)
    {
        DescriptorFactory = descriptorFactory;
    }

    /// <summary>
    ///     Gets the descriptor factory to pass to the new Characteristic.
    /// </summary>
    protected IBluetoothRemoteDescriptorFactory DescriptorFactory { get; }

    /// <inheritdoc />
    public abstract IBluetoothRemoteCharacteristic Create(IBluetoothRemoteService remoteService, IBluetoothRemoteCharacteristicFactory.BluetoothRemoteCharacteristicFactorySpec spec);
}
