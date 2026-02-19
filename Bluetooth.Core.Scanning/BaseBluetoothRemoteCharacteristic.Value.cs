namespace Bluetooth.Core.Scanning;

public abstract partial class BaseBluetoothRemoteCharacteristic
{
    /// <inheritdoc/>
    public ReadOnlySpan<byte> ValueSpan => Value.Span;

    /// <inheritdoc/>
    public ReadOnlyMemory<byte> Value
    {
        get => GetValue<ReadOnlyMemory<byte>>(default);
        private set => SetValue(value);
    }

    /// <inheritdoc/>
    public event EventHandler<ValueUpdatedEventArgs>? ValueUpdated;

    /// <summary>
    /// Waits for the characteristic's value to change and optionally applies a filter to the new value.
    /// </summary>
    /// <param name="valueFilter">Optional filter to apply to the new value. If null, any value change will trigger completion.</param>
    /// <param name="timeout">Optional timeout for the operation.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The new value that triggered the completion.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled via the cancellation token.</exception>
    /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
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
