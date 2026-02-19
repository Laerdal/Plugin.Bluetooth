namespace Bluetooth.Core.Scanning;

public abstract partial class BaseBluetoothRemoteDescriptor
{
    /// <summary>
    /// Gets a value indicating whether a read value operation is currently in progress.
    /// </summary>
    public bool IsReadingValue
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    private TaskCompletionSource<ReadOnlyMemory<byte>>? ReadValueTcs
    {
        get => GetValue<TaskCompletionSource<ReadOnlyMemory<byte>>?>(null);
        set => SetValue(value);
    }

    /// <inheritdoc/>
    public async ValueTask<ReadOnlyMemory<byte>> ReadValueAsync(bool skipIfPreviouslyRead = false, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        // Ensure Device is Connected
        DeviceNotConnectedException.ThrowIfNotConnected(RemoteCharacteristic.RemoteService.Device);

        // Ensure READ is supported
        if (!CanRead)
        {
            throw new DescriptorCantReadException(this);
        }

        // Check if the value is already read and skipIfPreviouslyRead is true
        if (skipIfPreviouslyRead && Value.Length != 0)
        {
            return Value;
        }

        // Prevents multiple calls to ReadValueAsync
        if (ReadValueTcs is { Task.IsCompleted: false })
        {
            return await ReadValueTcs.Task.ConfigureAwait(false);
        }

        ReadValueTcs = new TaskCompletionSource<ReadOnlyMemory<byte>>();
        IsReadingValue = true;

        try
        {
            await NativeReadValueAsync().ConfigureAwait(false);
        }
        catch (Exception e)
        {
            OnReadValueFailed(e);
        }

        try
        {
            return await ReadValueTcs.Task.WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            IsReadingValue = false;
            ReadValueTcs = null;
        }
    }

    /// <summary>
    /// Platform-specific implementation to read the descriptor's value.
    /// </summary>
    protected abstract ValueTask NativeReadValueAsync();

    /// <summary>
    /// Called when reading the descriptor's value succeeds.
    /// </summary>
    protected void OnReadValueSucceeded(ReadOnlyMemory<byte> value)
    {
        var old = Value;
        Value = value;

        if (ReadValueTcs != null && ReadValueTcs.TrySetResult(value))
        {
            return;
        }

        OnValueUpdated(value, old);
    }

    /// <summary>
    /// Called when reading the descriptor's value fails.
    /// </summary>
    protected void OnReadValueFailed(Exception e)
    {
        var success = ReadValueTcs?.TrySetException(e) ?? false;
        if (success)
        {
            return;
        }

        BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
    }

    #region CanRead

    private Lazy<bool> LazyCanRead { get; }

    /// <inheritdoc/>
    public bool CanRead => LazyCanRead.Value;

    /// <summary>
    /// Platform-specific implementation to determine if the descriptor can be read.
    /// </summary>
    protected abstract bool NativeCanRead();

    #endregion
}
