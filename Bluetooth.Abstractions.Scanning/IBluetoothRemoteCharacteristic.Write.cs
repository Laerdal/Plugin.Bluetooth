namespace Bluetooth.Abstractions.Scanning;

/// <summary>
///     Interface representing a Bluetooth characteristic, providing properties and methods for interacting with it.
/// </summary>
public partial interface IBluetoothRemoteCharacteristic
{
    /// <summary>
    ///     Gets a value indicating whether the characteristic can be written to.
    /// </summary>
    bool CanWrite { get; }

    /// <summary>
    ///     Gets a value indicating whether the characteristic is currently writing its value.
    /// </summary>
    bool IsWriting { get; }

    /// <summary>
    ///     Writes a value to the characteristic asynchronously.
    /// </summary>
    /// <param name="value">The value to write.</param>
    /// <param name="skipIfOldValueMatchesNewValue">If true, skips writing if the old value matches the new value.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value" /> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the characteristic doesn't support write operations.</exception>
    /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
    ValueTask WriteValueAsync(ReadOnlyMemory<byte> value, bool skipIfOldValueMatchesNewValue = false, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    #region Reliable Write

    /// <summary>
    ///     Begins a reliable write transaction.
    ///     Reliable write allows you to queue multiple writes and execute them atomically.
    /// </summary>
    /// <param name="timeout">The timeout for this operation.</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when a reliable write transaction is already in progress.</exception>
    /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
    ValueTask BeginReliableWriteAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Executes all writes queued in the current reliable write transaction.
    /// </summary>
    /// <param name="timeout">The timeout for this operation.</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no reliable write transaction is in progress.</exception>
    /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
    ValueTask ExecuteReliableWriteAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Aborts the current reliable write transaction and discards all queued writes.
    /// </summary>
    /// <param name="timeout">The timeout for this operation.</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no reliable write transaction is in progress.</exception>
    /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
    ValueTask AbortReliableWriteAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    #endregion
}