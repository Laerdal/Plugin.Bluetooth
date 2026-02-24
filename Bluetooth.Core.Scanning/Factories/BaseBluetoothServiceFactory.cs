namespace Bluetooth.Core.Scanning.Factories;

/// <inheritdoc />
public abstract class BaseBluetoothServiceFactory : IBluetoothRemoteServiceFactory
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseBluetoothServiceFactory" /> class.
    /// </summary>
    /// <param name="characteristicFactory">The characteristic factory to pass to the new Service.</param>
    protected BaseBluetoothServiceFactory(IBluetoothRemoteCharacteristicFactory characteristicFactory)
    {
        CharacteristicFactory = characteristicFactory;
    }

    /// <summary>
    ///     Gets the characteristic factory to pass to the new Service.
    /// </summary>
    protected IBluetoothRemoteCharacteristicFactory CharacteristicFactory { get; }

    /// <inheritdoc />
    public abstract IBluetoothRemoteService Create(IBluetoothRemoteDevice device, IBluetoothRemoteServiceFactory.BluetoothRemoteServiceFactorySpec spec);
}
