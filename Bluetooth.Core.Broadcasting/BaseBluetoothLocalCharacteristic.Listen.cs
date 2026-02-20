namespace Bluetooth.Core.Broadcasting;

/// <summary>
///     Base class for Bluetooth broadcast characteristics.
/// </summary>
public abstract partial class BaseBluetoothLocalCharacteristic
{
    /// <inheritdoc />
    public IReadOnlyList<IBluetoothConnectedDevice> SubscribedDevices => LocalService.Broadcaster.GetClientDevices(d => d.SubscribedCharacteristics.Contains(this)).ToList().AsReadOnly();

    /// <summary>
    ///     Called when a client device subscribes to the characteristic.
    /// </summary>
    /// <param name="device">The client device that subscribed.</param>
    protected void OnCharacteristicSubscribed(IBluetoothConnectedDevice device)
    {
        ArgumentNullException.ThrowIfNull(device);

        LogClientSubscribed(Id, LocalService.Id, device.Id);
        // TODO : EVENT
        device.AddCharacteristicSubscription(this);
    }

    /// <summary>
    ///     Called when a client device unsubscribes from the characteristic.
    /// </summary>
    /// <param name="device">The client device that unsubscribed.</param>
    protected void OnCharacteristicUnsubscribed(IBluetoothConnectedDevice device)
    {
        ArgumentNullException.ThrowIfNull(device);

        LogClientUnsubscribed(Id, LocalService.Id, device.Id);
        // TODO : EVENT
        device.RemoveCharacteristicSubscription(this);
    }
}