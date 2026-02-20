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

    // TODO : Implement Notify/Indicate support once all platforms are investigated.

    #endregion
}