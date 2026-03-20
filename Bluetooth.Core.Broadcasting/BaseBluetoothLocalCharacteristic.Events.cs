namespace Bluetooth.Core.Broadcasting;

/// <summary>
///     Base class for Bluetooth broadcast characteristics.
/// </summary>
public abstract partial class BaseBluetoothLocalCharacteristic
{
    /// <inheritdoc />
    public event EventHandler<CharacteristicReadRequestEventArgs>? ReadRequested;

    /// <inheritdoc />
    public event EventHandler<CharacteristicWriteRequestEventArgs>? WriteRequested;

    /// <inheritdoc />
    public event EventHandler<CharacteristicSubscriptionChangedEventArgs>? ClientSubscribed;

    /// <inheritdoc />
    public event EventHandler<CharacteristicSubscriptionChangedEventArgs>? ClientUnsubscribed;
}
