namespace Bluetooth.Abstractions.Scanning.Profiles;

/// <summary>
///     Defines listener orchestration for a typed Bluetooth characteristic accessor.
/// </summary>
/// <typeparam name="TRead">The typed value produced from notification payloads.</typeparam>
public interface IBluetoothCharacteristicAccessorListener<TRead> : IBluetoothCharacteristicAccessor
{
    /// <summary>
    ///     Subscribes a typed listener to characteristic notifications.
    /// </summary>
    /// <param name="device">The connected remote device that owns the characteristic.</param>
    /// <param name="listener">The callback invoked for each decoded notification value.</param>
    /// <param name="timeout">Optional timeout for resolution and listening operations.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    ValueTask SubscribeAsync(
        IBluetoothRemoteDevice device,
        Action<TRead> listener,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Unsubscribes a typed listener from characteristic notifications.
    /// </summary>
    /// <param name="device">The connected remote device that owns the characteristic.</param>
    /// <param name="listener">The callback to remove.</param>
    /// <param name="timeout">Optional timeout for resolution and listening operations.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    ValueTask UnsubscribeAsync(
        IBluetoothRemoteDevice device,
        Action<TRead> listener,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Unsubscribes all listeners from characteristic notifications.
    /// </summary>
    /// <param name="device">The connected remote device that owns the characteristic.</param>
    /// <param name="timeout">Optional timeout for resolution and listening operations.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    ValueTask UnsubscribeAllAsync(
        IBluetoothRemoteDevice device,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default);
}
