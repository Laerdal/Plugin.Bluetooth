namespace Bluetooth.Abstractions.Broadcasting;

/// <summary>
///     Represents a characteristic in the context of bluetooth broadcasting.
/// </summary>
public partial interface IBluetoothLocalCharacteristic
{
    #region Value

    /// <summary>
    ///     Gets the value of the characteristic as a read-only span. Useful for high-performance scenarios.
    /// </summary>
    ReadOnlySpan<byte> ValueSpan { get; }

    /// <summary>
    ///     Gets the value of the characteristic as a read-only memory segment. Useful for asynchronous operations.
    /// </summary>
    ReadOnlyMemory<byte> Value { get; }

    /// <summary>
    ///     Updates the value of a hosted characteristic and optionally notifies/indicates subscribed clients.
    /// </summary>
    /// <param name="value">The new value.</param>
    /// <param name="notifyClients">Whether to notify subscribed clients of the value change.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UpdateValueAsync(ReadOnlyMemory<byte> value, bool notifyClients = true, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    #endregion
}
