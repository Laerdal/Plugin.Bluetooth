namespace Bluetooth.Core.Scanning.Factories;

/// <inheritdoc />
public abstract class BaseBluetoothServiceFactory : IBluetoothServiceFactory
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseBluetoothServiceFactory" /> class.
    /// </summary>
    /// <param name="characteristicFactory">The characteristic factory to pass to the new Service.</param>
    protected BaseBluetoothServiceFactory(IBluetoothCharacteristicFactory characteristicFactory)
    {
        CharacteristicFactory = characteristicFactory;
    }

    /// <summary>
    ///     Gets the characteristic factory to pass to the new Service.
    /// </summary>
    protected IBluetoothCharacteristicFactory CharacteristicFactory { get; }

    /// <inheritdoc />
    public abstract IBluetoothRemoteService CreateService(IBluetoothRemoteDevice device, IBluetoothServiceFactory.BluetoothServiceFactoryRequest request);
}