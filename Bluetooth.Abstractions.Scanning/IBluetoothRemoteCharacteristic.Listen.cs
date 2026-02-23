namespace Bluetooth.Abstractions.Scanning;

/// <summary>
///     Interface representing a Bluetooth characteristic, providing properties and methods for interacting with it.
/// </summary>
public partial interface IBluetoothRemoteCharacteristic
{
    #region Listen

    /// <summary>
    ///     Gets a value indicating whether the characteristic supports notifications.
    /// </summary>
    bool CanListen { get; }

    /// <summary>
    ///     Gets a value indicating whether the characteristic is currently listening for notifications.
    /// </summary>
    bool IsListening { get; }

    /// <summary>
    ///     Starts listening for notifications from the characteristic asynchronously.
    /// </summary>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="DeviceNotConnectedException">Thrown when the device is not connected.</exception>
    /// <exception cref="CharacteristicCantListenException">Thrown when the characteristic doesn't support notifications.</exception>
    /// <exception cref="CharacteristicAlreadyNotifyingException">Thrown when the characteristic is already listening for notifications.</exception>
    /// <exception cref="CharacteristicNotifyException">Thrown when the notify operation fails.</exception>
    /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
    ValueTask StartListeningAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Stops listening for notifications from the characteristic asynchronously.
    /// </summary>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="DeviceNotConnectedException">Thrown when the device is not connected.</exception>
    /// <exception cref="CharacteristicCantListenException">Thrown when the characteristic doesn't support notifications.</exception>
    /// <exception cref="CharacteristicAlreadyNotifyingException">Thrown when the characteristic is not currently listening.</exception>
    /// <exception cref="CharacteristicNotifyException">Thrown when the notify operation fails.</exception>
    /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
    ValueTask StopListeningAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    #endregion
}
