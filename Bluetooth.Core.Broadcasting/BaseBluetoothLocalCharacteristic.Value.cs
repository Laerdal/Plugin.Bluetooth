namespace Bluetooth.Core.Broadcasting;

/// <summary>
///     Base class for Bluetooth broadcast characteristics.
/// </summary>
public abstract partial class BaseBluetoothLocalCharacteristic
{
    /// <inheritdoc />
    public ReadOnlySpan<byte> ValueSpan => Value.Span;

    /// <inheritdoc />
    public ReadOnlyMemory<byte> Value { get; private set; }

    /// <inheritdoc />
    public ValueTask UpdateValueAsync(ReadOnlyMemory<byte> value, bool notifyClients = true, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        Value = value;
        return NativeUpdateValueAsync(value, notifyClients, timeout, cancellationToken);
    }

    /// <summary>
    ///     Updates the value of the characteristic in the native Bluetooth stack.
    /// </summary>
    /// <param name="value">The new value to set for the characteristic.</param>
    /// <param name="notifyClients">Indicates whether to notify subscribed clients of the value change.</param>
    /// <param name="timeout">An optional timeout for the operation.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous update operation.</returns>
    protected abstract ValueTask NativeUpdateValueAsync(ReadOnlyMemory<byte> value, bool notifyClients, TimeSpan? timeout = null, CancellationToken cancellationToken = default);
}
