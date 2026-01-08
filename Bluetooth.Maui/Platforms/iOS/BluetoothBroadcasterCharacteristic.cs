using Bluetooth.Maui.PlatformSpecific;

namespace Bluetooth.Maui;

/// <summary>
/// iOS implementation of a mutable Bluetooth characteristic for the broadcaster/peripheral role.
/// </summary>
/// <remarks>
/// This implementation wraps iOS's <see cref="CBMutableCharacteristic"/> for hosting GATT characteristics.
/// Unlike <see cref="BluetoothCharacteristic"/>, this is used when the device acts as a peripheral.
/// </remarks>
public partial class BluetoothBroadcasterCharacteristic : BaseBluetoothCharacteristic
{
    /// <summary>
    /// Gets the native iOS Core Bluetooth mutable characteristic.
    /// </summary>
    public CBMutableCharacteristic NativeMutableCharacteristic { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BluetoothBroadcasterCharacteristic"/> class.
    /// </summary>
    /// <param name="service">The Bluetooth service associated with this characteristic.</param>
    /// <param name="characteristicId">The unique identifier of the characteristic.</param>
    /// <param name="properties">The properties of the characteristic.</param>
    /// <param name="permissions">The permissions required to access the characteristic.</param>
    /// <param name="initialValue">The initial value of the characteristic.</param>
    public BluetoothBroadcasterCharacteristic(
        IBluetoothService service,
        Guid characteristicId,
        CBCharacteristicProperties properties,
        CBAttributePermissions permissions,
        byte[]? initialValue = null)
        : base(service, characteristicId)
    {
        var value = initialValue != null ? NSData.FromArray(initialValue) : null;
        NativeMutableCharacteristic = new CBMutableCharacteristic(
            CBUUID.FromString(characteristicId.ToString()),
            properties,
            value,
            permissions);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BluetoothBroadcasterCharacteristic"/> class with an existing mutable characteristic.
    /// </summary>
    /// <param name="service">The Bluetooth service associated with this characteristic.</param>
    /// <param name="characteristicId">The unique identifier of the characteristic.</param>
    /// <param name="nativeMutableCharacteristic">The native iOS Core Bluetooth mutable characteristic.</param>
    public BluetoothBroadcasterCharacteristic(
        IBluetoothService service,
        Guid characteristicId,
        CBMutableCharacteristic nativeMutableCharacteristic)
        : base(service, characteristicId)
    {
        NativeMutableCharacteristic = nativeMutableCharacteristic;
    }

    /// <summary>
    /// Updates the value of this characteristic.
    /// </summary>
    /// <param name="value">The new value.</param>
    public void UpdateValue(byte[] value)
    {
        ArgumentNullException.ThrowIfNull(value);
        NativeMutableCharacteristic.Value = NSData.FromArray(value);
        Value = value;
    }

    /// <inheritdoc/>
    protected override bool NativeCanRead()
    {
        return NativeMutableCharacteristic.Properties.HasFlag(CBCharacteristicProperties.Read);
    }

    /// <inheritdoc/>
    protected override bool NativeCanWrite()
    {
        return NativeMutableCharacteristic.Properties.HasFlag(CBCharacteristicProperties.Write) ||
               NativeMutableCharacteristic.Properties.HasFlag(CBCharacteristicProperties.WriteWithoutResponse);
    }

    /// <inheritdoc/>
    protected override bool NativeCanListen()
    {
        return NativeMutableCharacteristic.Properties.HasFlag(CBCharacteristicProperties.Notify) ||
               NativeMutableCharacteristic.Properties.HasFlag(CBCharacteristicProperties.Indicate);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// For broadcaster characteristics, reading returns the locally stored value.
    /// </remarks>
    protected override ValueTask NativeReadValueAsync()
    {
        // For broadcaster/peripheral role, we provide the value, not read it from remote
        var value = NativeMutableCharacteristic.Value?.ToArray() ?? Array.Empty<byte>();
        Value = value;
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// For broadcaster characteristics, writing updates the local value.
    /// </remarks>
    protected override ValueTask NativeWriteValueAsync(ReadOnlyMemory<byte> value)
    {
        UpdateValue(value.ToArray());
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// For broadcaster characteristics, this queries if centrals have subscribed.
    /// </remarks>
    protected override ValueTask NativeReadIsListeningAsync()
    {
        // For broadcaster/peripheral role, this would indicate if any centrals are subscribed
        // We can't easily determine this from CBMutableCharacteristic alone
        // This would need to be tracked at the broadcaster level
        IsListening = false; // Default to false
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// For broadcaster characteristics, this is not directly applicable.
    /// Subscription state is managed by centrals, not by the peripheral.
    /// </remarks>
    protected override ValueTask NativeWriteIsListeningAsync(bool shouldBeListening)
    {
        // For broadcaster/peripheral role, we can't control whether centrals subscribe
        // Centrals decide if they want to subscribe or not
        // This is a no-op for peripheral role
        IsListening = shouldBeListening;
        return ValueTask.CompletedTask;
    }
}
