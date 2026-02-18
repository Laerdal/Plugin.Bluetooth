using Plugin.BaseTypeExtensions;

namespace Bluetooth.Core.Scanning;

public abstract partial class BaseBluetoothRemoteDescriptor
{
    /// <summary>
    /// Gets a value indicating whether a write value operation is currently in progress.
    /// </summary>
    public bool IsWritingValue
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    private TaskCompletionSource? WriteValueTcs
    {
        get => GetValue<TaskCompletionSource?>(null);
        set => SetValue(value);
    }

    #region IBluetoothDescriptor Members

    /// <inheritdoc/>
    public async ValueTask WriteValueAsync(ReadOnlyMemory<byte> value, bool skipIfOldValueMatchesNewValue = false, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        // Ensure Device is Connected
        DeviceNotConnectedException.ThrowIfNotConnected(RemoteCharacteristic.RemoteService.Device);

        // Ensure WRITE is supported
        if (!CanWrite)
        {
            throw new DescriptorCantWriteException(this);
        }

        // Check if the value matches the current value and skipIfOldValueMatchesNewValue is true
        if (skipIfOldValueMatchesNewValue && Value.Span.SequenceEqual(value.Span))
        {
            return;
        }

        // Prevents multiple calls to WriteValueAsync
        if (WriteValueTcs is { Task.IsCompleted: false })
        {
            await WriteValueTcs.Task.ConfigureAwait(false);
            return;
        }

        WriteValueTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        IsWritingValue = true;

        try
        {
            await NativeWriteValueAsync(value).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            OnWriteValueFailed(e);
        }

        try
        {
            await WriteValueTcs.Task.WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            IsWritingValue = false;
            WriteValueTcs = null;
        }
    }

    #endregion

    /// <summary>
    /// Platform-specific implementation to write the descriptor's value.
    /// </summary>
    protected abstract ValueTask NativeWriteValueAsync(ReadOnlyMemory<byte> value);

    /// <summary>
    /// Called when writing the descriptor's value succeeds.
    /// </summary>
    protected void OnWriteValueSucceeded()
    {
        WriteValueTcs?.TrySetResult();
    }

    /// <summary>
    /// Called when writing the descriptor's value fails.
    /// </summary>
    protected void OnWriteValueFailed(Exception e)
    {
        var success = WriteValueTcs?.TrySetException(e) ?? false;
        if (success)
        {
            return;
        }

        BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
    }

    #region CanWrite

    private Lazy<bool> LazyCanWrite { get; }

    /// <inheritdoc/>
    public bool CanWrite => LazyCanWrite.Value;

    /// <summary>
    /// Platform-specific implementation to determine if the descriptor can be written.
    /// </summary>
    protected abstract bool NativeCanWrite();

    #endregion
}
