namespace Bluetooth.Abstractions.Broadcasting;

/// <summary>
///     Represents a Bluetooth device that has connected to a broadcast (GATT server).
/// </summary>
public partial interface IBluetoothConnectedDevice
{
    #region Subscription Management

    /// <summary>
    ///     Gets the list of broadcast characteristics this client device is subscribed to.
    /// </summary>
    IReadOnlyList<IBluetoothLocalCharacteristic> SubscribedCharacteristics { get; }

    /// <summary>
    ///     Adds a subscription to a broadcast characteristic for this client device.
    /// </summary>
    void AddCharacteristicSubscription(IBluetoothLocalCharacteristic localCharacteristic);

    /// <summary>
    ///     Removes a subscription to a broadcast characteristic for this client device.
    /// </summary>
    void RemoveCharacteristicSubscription(IBluetoothLocalCharacteristic localCharacteristic);

    /// <summary>
    ///     Removes all characteristic subscriptions for this client device.
    /// </summary>
    void RemoveAllCharacteristicSubscriptions();

    #endregion
}
