namespace Bluetooth.Core.Broadcasting;

/// <summary>
/// Base class for Bluetooth broadcaster devices.
/// </summary>
public abstract partial class BaseBluetoothConnectedDevice
{

    private readonly List<IBluetoothLocalCharacteristic> _subscribedCharacteristics = new List<IBluetoothLocalCharacteristic>();

    /// <inheritdoc />
    public IReadOnlyList<IBluetoothLocalCharacteristic> SubscribedCharacteristics => _subscribedCharacteristics.AsReadOnly();

    /// <inheritdoc />
    public void AddCharacteristicSubscription(IBluetoothLocalCharacteristic localCharacteristic)
    {
        _subscribedCharacteristics.Add(localCharacteristic);
        LogCharacteristicSubscriptionAdded(Id, localCharacteristic.Id, localCharacteristic.LocalService.Id);
    }

    /// <inheritdoc />
    public void RemoveCharacteristicSubscription(IBluetoothLocalCharacteristic localCharacteristic)
    {
        _subscribedCharacteristics.Remove(localCharacteristic);
        LogCharacteristicSubscriptionRemoved(Id, localCharacteristic.Id, localCharacteristic.LocalService.Id);
    }

    /// <inheritdoc />
    public void RemoveAllCharacteristicSubscriptions()
    {
        var count = _subscribedCharacteristics.Count;
        _subscribedCharacteristics.Clear();
        LogAllSubscriptionsCleared(Id, count);
    }
}
