namespace Bluetooth.Abstractions.Scanning;

/// <summary>
///     Interface representing a Bluetooth characteristic, providing properties and methods for interacting with it.
/// </summary>
public partial interface IBluetoothRemoteCharacteristic
{
    /// <summary>
    ///     Gets the value of the characteristic as a read-only span. Useful for high-performance scenarios.
    /// </summary>
    ReadOnlySpan<byte> ValueSpan { get; }

    /// <summary>
    ///     Gets the value of the characteristic as a read-only memory segment. Useful for asynchronous operations.
    /// </summary>
    ReadOnlyMemory<byte> Value { get; }

    /// <summary>
    ///     Event raised when the value of the characteristic is updated, only triggered when IsListening is true.
    /// </summary>
    event EventHandler<ValueUpdatedEventArgs> ValueUpdated;

    /// <summary>
    ///     Waits for the value of the characteristic to change asynchronously.
    /// </summary>
    /// <param name="valueFilter">An optional filter function to apply to the value changes. If provided, the task completes only when the filter returns true.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the new value of the characteristic.</returns>
    ValueTask<ReadOnlyMemory<byte>> WaitForValueChangeAsync(Func<ReadOnlyMemory<byte>, bool>? valueFilter = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default);
}
