namespace Bluetooth.Core.Scanning;

public abstract partial class BaseBluetoothRemoteDescriptor
{
    /// <inheritdoc/>
    public ReadOnlySpan<byte> ValueSpan => Value.Span;

    /// <inheritdoc/>
    public ReadOnlyMemory<byte> Value
    {
        get => GetValue(ReadOnlyMemory<byte>.Empty);
        protected set => SetValue(value);
    }

    /// <inheritdoc/>
    public event EventHandler<ValueUpdatedEventArgs>? ValueUpdated;

    /// <summary>
    /// Raises the ValueUpdated event.
    /// </summary>
    /// <param name="newValue">The new value of the descriptor.</param>
    /// <param name="oldValue">The previous value of the descriptor.</param>
    protected void OnValueUpdated(ReadOnlyMemory<byte> newValue, ReadOnlyMemory<byte> oldValue)
    {
        ValueUpdated?.Invoke(this, new ValueUpdatedEventArgs(newValue, oldValue));
    }

    /// <inheritdoc/>
    public async ValueTask<ReadOnlyMemory<byte>> WaitForValueChangeAsync(Func<ReadOnlyMemory<byte>, bool>? valueFilter = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        var tcs = new TaskCompletionSource<ReadOnlyMemory<byte>>();

        ValueUpdated += EventCallback;

        return await tcs.Task.WaitBetterAsync(timeout, cancellationToken: cancellationToken).ConfigureAwait(false);

        void EventCallback(object? sender, ValueUpdatedEventArgs valueUpdatedEventArgs)
        {
            if (valueFilter != null && !valueFilter.Invoke(valueUpdatedEventArgs.NewValue))
            {
                return;
            }

            ValueUpdated -= EventCallback;
            tcs.TrySetResult(valueUpdatedEventArgs.NewValue);
        }
    }
}
