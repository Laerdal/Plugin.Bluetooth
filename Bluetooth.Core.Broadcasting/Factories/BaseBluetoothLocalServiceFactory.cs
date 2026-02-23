namespace Bluetooth.Core.Broadcasting.Factories;

/// <inheritdoc />
public abstract class BaseBluetoothLocalServiceFactory : IBluetoothLocalServiceFactory
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseBluetoothLocalServiceFactory" /> class.
    /// </summary>
    protected BaseBluetoothLocalServiceFactory(IBluetoothLocalCharacteristicFactory localCharacteristicFactory)
    {
        LocalCharacteristicFactory = localCharacteristicFactory;
    }

    /// <summary>
    ///     Gets the characteristic factory to pass to the new Service.
    /// </summary>
    protected IBluetoothLocalCharacteristicFactory LocalCharacteristicFactory { get; }

    /// <inheritdoc />
    public abstract IBluetoothLocalService CreateService(IBluetoothBroadcaster broadcaster, IBluetoothLocalServiceFactory.BluetoothLocalServiceSpec spec);
}
