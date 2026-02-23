namespace Bluetooth.Abstractions.Broadcasting;

/// <summary>
///     Represents a descriptor in the context of Bluetooth broadcasting.
///     Descriptors provide additional information about a characteristic.
/// </summary>
public partial interface IBluetoothLocalDescriptor
{
    #region Value

    /// <summary>
    ///     Gets the value of the descriptor as a read-only span. Useful for high-performance scenarios.
    /// </summary>
    ReadOnlySpan<byte> ValueSpan { get; }

    /// <summary>
    ///     Gets the value of the descriptor as a read-only memory segment. Useful for asynchronous operations.
    /// </summary>
    ReadOnlyMemory<byte> Value { get; }

    /// <summary>
    ///     Updates the value of a hosted descriptor.
    /// </summary>
    /// <param name="value">The new value.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UpdateValueAsync(ReadOnlyMemory<byte> value, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    #endregion
}
