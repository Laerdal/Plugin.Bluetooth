namespace Bluetooth.Abstractions.Scanning;

public partial interface IBluetoothRemoteDescriptor
{
    #region Write

    /// <summary>
    ///     Gets a value indicating whether the descriptor can be written to.
    /// </summary>
    bool CanWrite { get; }

    /// <summary>
    ///     Gets a value indicating whether the descriptor is currently writing its value.
    /// </summary>
    bool IsWritingValue { get; }

    /// <summary>
    ///     Writes a value to the descriptor asynchronously.
    /// </summary>
    /// <param name="value">The value to write.</param>
    /// <param name="skipIfOldValueMatchesNewValue">If true, skips writing if the old value matches the new value.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value" /> is null.</exception>
    /// <exception cref="DeviceNotConnectedException">Thrown when the device is not connected.</exception>
    /// <exception cref="DescriptorCantWriteException">Thrown when the descriptor doesn't support write operations.</exception>
    /// <exception cref="DescriptorException">Thrown when the write operation fails.</exception>
    /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
    ValueTask WriteValueAsync(ReadOnlyMemory<byte> value, bool skipIfOldValueMatchesNewValue = false, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    #endregion
}
