namespace Bluetooth.Core.Abstractions;

/// <summary>
/// Interface representing a service for accessing Bluetooth characteristics, providing methods for reading, writing, and listening to characteristics.
/// </summary>
public interface IBluetoothCharacteristicAccessService
{
    #region Service

    /// <summary>
    /// Gets the unique identifier of the service.
    /// </summary>
    Guid ServiceId { get; }

    /// <summary>
    /// Gets the name of the service.
    /// </summary>
    string ServiceName { get; }

    /// <summary>
    /// Sets the service information.
    /// </summary>
    /// <param name="serviceId">The unique identifier of the service.</param>
    /// <param name="serviceName">The name of the service.</param>
    void SetServiceInformation(Guid serviceId, string serviceName);

    #endregion

    #region Characteristic

    /// <summary>
    /// Gets the unique identifier of the characteristic.
    /// </summary>
    Guid CharacteristicId { get; }

    /// <summary>
    /// Gets the name of the characteristic.
    /// </summary>
    string CharacteristicName { get; }

    /// <summary>
    /// Gets the characteristic associated with the specified device asynchronously.
    /// </summary>
    /// <param name="device">The Bluetooth device.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the Bluetooth characteristic.</returns>
    Task<IBluetoothCharacteristic> GetCharacteristicAsync(IBluetoothDevice device);

    /// <summary>
    /// Tries to get the characteristic associated with the specified device asynchronously.
    /// </summary>
    /// <param name="device">The Bluetooth device.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the Bluetooth characteristic, or null if not found.</returns>
    ValueTask<IBluetoothCharacteristic?> TryGetCharacteristicAsync(IBluetoothDevice device);

    /// <summary>
    /// Determines whether the specified device has the characteristic asynchronously.
    /// </summary>
    /// <param name="device">The Bluetooth device.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the device has the characteristic.</returns>
    ValueTask<bool> HasCharacteristicAsync(IBluetoothDevice device);

    #endregion
}

/// <summary>
/// Generic interface representing a service for accessing Bluetooth characteristics with specific input and output types.
/// </summary>
/// <typeparam name="T">The type of the characteristic value.</typeparam>
public interface IBluetoothCharacteristicAccessService<T> : IBluetoothCharacteristicAccessService<T, T>
{
}

/// <summary>
/// Generic interface representing a service for accessing Bluetooth characteristics with different input and output types.
/// </summary>
/// <typeparam name="TIn">The type of the input value.</typeparam>
/// <typeparam name="TOut">The type of the output value.</typeparam>
public interface IBluetoothCharacteristicAccessService<TIn, TOut> : IBluetoothCharacteristicAccessService
{
    /// <summary>
    /// Gets the type of the input value.
    /// </summary>
    Type ValueTypeIn { get; }

    /// <summary>
    /// Gets the type of the output value.
    /// </summary>
    Type ValueTypeOut { get; }

    #region Read

    /// <summary>
    /// Determines whether the specified device can read the characteristic asynchronously.
    /// </summary>
    /// <param name="device">The Bluetooth device.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the device can read the characteristic.</returns>
    ValueTask<bool> CanReadAsync(IBluetoothDevice device);

    /// <summary>
    /// Reads the value of the characteristic asynchronously.
    /// </summary>
    /// <param name="device">The Bluetooth device.</param>
    /// <param name="useLastValueIfPreviouslyRead">If true, uses the last value if it was previously read.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous read operation. The task result contains the value read.</returns>
    Task<TIn> ReadAsync(IBluetoothDevice device, bool useLastValueIfPreviouslyRead = false, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    #endregion

    #region Write

    /// <summary>
    /// Determines whether the specified device can write to the characteristic asynchronously.
    /// </summary>
    /// <param name="device">The Bluetooth device.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the device can write to the characteristic.</returns>
    ValueTask<bool> CanWriteAsync(IBluetoothDevice device);

    /// <summary>
    /// Writes a value to the characteristic asynchronously.
    /// </summary>
    /// <param name="device">The Bluetooth device.</param>
    /// <param name="value">The value to write.</param>
    /// <param name="skipIfOldValueMatchesNewValue">If true, skips writing if the old value matches the new value.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    Task WriteAsync(IBluetoothDevice device, TOut value, bool skipIfOldValueMatchesNewValue = false, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    #endregion

    #region Listen

    /// <summary>
    /// Delegate for handling notifications received from the characteristic.
    /// </summary>
    /// <param name="newValue">The new value received.</param>
    /// <returns>True if the listener should be kept, false if it should be removed.</returns>
    public delegate bool OnNotificationReceived(TIn newValue);

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
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SubscribeAsync(IBluetoothDevice device, OnNotificationReceived listener);

    /// <summary>
    /// Unsubscribes from notifications from the characteristic asynchronously.
    /// </summary>
    /// <param name="device">The Bluetooth device.</param>
    /// <param name="listener">The listener to remove.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UnsubscribeAsync(IBluetoothDevice device, OnNotificationReceived listener);

    /// <summary>
    /// Determines whether the specified device is subscribed to notifications from the characteristic asynchronously.
    /// </summary>
    /// <param name="device">The Bluetooth device.</param>
    /// <param name="listener">The listener to check.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the device is subscribed to notifications from the characteristic.</returns>
    ValueTask<bool> IsSubscribedAsync(IBluetoothDevice device, OnNotificationReceived listener);

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

    #endregion
}
