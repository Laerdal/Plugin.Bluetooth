namespace Bluetooth.Abstractions.Broadcasting;

/// <summary>
///     Represents a characteristic in the context of bluetooth broadcasting.
/// </summary>
public partial interface IBluetoothLocalCharacteristic
{
    #region Listen

    /// <summary>
    ///     Gets the list of client devices that are currently subscribed to notifications/indications from this characteristic.
    /// </summary>
    IReadOnlyList<IBluetoothConnectedDevice> SubscribedDevices { get; }

    /// <summary>
    ///     Event raised when a client device subscribes to this characteristic.
    /// </summary>
    event EventHandler<CharacteristicSubscriptionChangedEventArgs>? ClientSubscribed;

    /// <summary>
    ///     Event raised when a client device unsubscribes from this characteristic.
    /// </summary>
    event EventHandler<CharacteristicSubscriptionChangedEventArgs>? ClientUnsubscribed;

    // TODO : Implement Notify/Indicate send API once all platforms are investigated.

    #endregion
}
