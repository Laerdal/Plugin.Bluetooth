namespace Bluetooth.Abstractions.Scanning.AccessService;

/// <summary>
/// Generic interface representing a service for accessing Bluetooth characteristics with different input and output types.
/// </summary>
public partial interface IBluetoothCharacteristicAccessService<TRead, TWrite>
{
    /// <summary>
    /// Delegate for handling notifications received from the characteristic.
    /// </summary>
    /// <param name="newValue">The new value received.</param>
    /// <returns>True if the listener should be kept, false if it should be removed.</returns>
    public delegate bool OnNotificationReceived(TRead newValue);

    /// <summary>
    /// Determines whether the specified device can listen to notifications from the characteristic asynchronously.
    /// </summary>
    /// <param name="device">The Bluetooth device.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the device can listen to notifications from the characteristic.</returns>
    ValueTask<bool> CanListenAsync(IBluetoothDevice device);

    /// <summary>
    /// Subscribes to notifications from the characteristic asynchronously.
    /// </summary>
    /// <param name="device">The Bluetooth device.</param>
    /// <param name="listener">The listener to handle notifications.</param>
    /// <param name="timeout">The timeout for this operation.</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask SubscribeAsync(IBluetoothDevice device, OnNotificationReceived listener, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unsubscribes from notifications from the characteristic asynchronously.
    /// </summary>
    /// <param name="device">The Bluetooth device.</param>
    /// <param name="listener">The listener to remove.</param>
    /// <param name="timeout">The timeout for this operation.</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UnsubscribeAsync(IBluetoothDevice device, OnNotificationReceived listener, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines whether the specified device is subscribed to notifications from the characteristic asynchronously.
    /// </summary>
    /// <param name="device">The Bluetooth device.</param>
    /// <param name="listener">The listener to check.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the device is subscribed to notifications from the characteristic.</returns>
    ValueTask<bool> IsSubscribedAsync(IBluetoothDevice device, OnNotificationReceived listener);

    /// <summary>
    /// Unsubscribes all listeners from the specified device.
    /// </summary>
    /// <param name="device">The Bluetooth device.</param>
    /// <param name="timeout">The timeout for this operation.</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UnsubscribeAllAsync(IBluetoothDevice device, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the listeners for the specified device.
    /// </summary>
    /// <param name="device">The Bluetooth device.</param>
    /// <returns>An enumerable of tuples containing the characteristic and the listener.</returns>
    IEnumerable<(IBluetoothCharacteristic, OnNotificationReceived)> GetListeners(IBluetoothDevice device);

    /// <summary>
    /// Gets the listeners for the specified characteristic.
    /// </summary>
    /// <param name="characteristic">The Bluetooth characteristic.</param>
    /// <returns>An enumerable of listeners.</returns>
    IEnumerable<OnNotificationReceived> GetListeners(IBluetoothCharacteristic characteristic);

    /// <summary>
    /// Determines whether any listeners are subscribed for the specified device.
    /// </summary>
    /// <param name="device">The Bluetooth device.</param>
    /// <returns>True if any listeners are subscribed; otherwise, false.</returns>
    bool HasListeners(IBluetoothDevice device);

    /// <summary>
    /// Determines whether the specified characteristic has any listeners.
    /// </summary>
    /// <param name="characteristic">The Bluetooth characteristic.</param>
    /// <returns>True if any listeners are subscribed; otherwise, false.</returns>
    bool HasListeners(IBluetoothCharacteristic characteristic);

    /// <summary>
    /// Gets the number of active listeners for the specified device.
    /// </summary>
    /// <param name="device">The Bluetooth device.</param>
    /// <returns>The number of active listeners.</returns>
    int GetListenerCount(IBluetoothDevice device);

}
