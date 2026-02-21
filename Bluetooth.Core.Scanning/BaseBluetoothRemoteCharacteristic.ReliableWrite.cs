namespace Bluetooth.Core.Scanning;

public abstract partial class BaseBluetoothRemoteCharacteristic
{
    private TaskCompletionSource? BeginReliableWriteTcs
    {
        get => GetValue<TaskCompletionSource?>(null);
        set => SetValue(value);
    }

    private TaskCompletionSource? ExecuteReliableWriteTcs
    {
        get => GetValue<TaskCompletionSource?>(null);
        set => SetValue(value);
    }

    private TaskCompletionSource? AbortReliableWriteTcs
    {
        get => GetValue<TaskCompletionSource?>(null);
        set => SetValue(value);
    }

    /// <inheritdoc />
    public async ValueTask BeginReliableWriteAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        // Ensure Device is Connected
        DeviceNotConnectedException.ThrowIfNotConnected(RemoteService.Device);

        // Prevents multiple calls
        if (BeginReliableWriteTcs is { Task.IsCompleted: false })
        {
            await BeginReliableWriteTcs.Task.ConfigureAwait(false);
            return;
        }

        BeginReliableWriteTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        try
        {
            await NativeBeginReliableWriteAsync().ConfigureAwait(false);
        }
        catch (Exception e)
        {
            OnBeginReliableWriteFailed(e);
        }

        try
        {
            await BeginReliableWriteTcs.Task.WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            BeginReliableWriteTcs = null;
        }
    }

    /// <inheritdoc />
    public async ValueTask ExecuteReliableWriteAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        // Ensure Device is Connected
        DeviceNotConnectedException.ThrowIfNotConnected(RemoteService.Device);

        // Prevents multiple calls
        if (ExecuteReliableWriteTcs is { Task.IsCompleted: false })
        {
            await ExecuteReliableWriteTcs.Task.ConfigureAwait(false);
            return;
        }

        ExecuteReliableWriteTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        try
        {
            await NativeExecuteReliableWriteAsync().ConfigureAwait(false);
        }
        catch (Exception e)
        {
            OnExecuteReliableWriteFailed(e);
        }

        try
        {
            await ExecuteReliableWriteTcs.Task.WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            ExecuteReliableWriteTcs = null;
        }
    }

    /// <inheritdoc />
    public async ValueTask AbortReliableWriteAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        // Ensure Device is Connected
        DeviceNotConnectedException.ThrowIfNotConnected(RemoteService.Device);

        // Prevents multiple calls
        if (AbortReliableWriteTcs is { Task.IsCompleted: false })
        {
            await AbortReliableWriteTcs.Task.ConfigureAwait(false);
            return;
        }

        AbortReliableWriteTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        try
        {
            await NativeAbortReliableWriteAsync().ConfigureAwait(false);
        }
        catch (Exception e)
        {
            OnAbortReliableWriteFailed(e);
        }

        try
        {
            await AbortReliableWriteTcs.Task.WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            AbortReliableWriteTcs = null;
        }
    }

    #region Native Abstracts

    /// <summary>
    ///     Platform-specific implementation to begin a reliable write transaction.
    /// </summary>
    protected abstract ValueTask NativeBeginReliableWriteAsync();

    /// <summary>
    ///     Platform-specific implementation to execute a reliable write transaction.
    /// </summary>
    protected abstract ValueTask NativeExecuteReliableWriteAsync();

    /// <summary>
    ///     Platform-specific implementation to abort a reliable write transaction.
    /// </summary>
    protected abstract ValueTask NativeAbortReliableWriteAsync();

    #endregion

    #region Callbacks

    /// <summary>
    ///     Called when begin reliable write succeeds.
    /// </summary>
    protected void OnBeginReliableWriteSucceeded()
    {
        BeginReliableWriteTcs?.TrySetResult();
    }

    /// <summary>
    ///     Called when begin reliable write fails.
    /// </summary>
    protected void OnBeginReliableWriteFailed(Exception e)
    {
        var success = BeginReliableWriteTcs?.TrySetException(e) ?? false;
        if (success)
        {
            return;
        }

        BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
    }

    /// <summary>
    ///     Called when execute reliable write succeeds.
    /// </summary>
    protected void OnExecuteReliableWriteSucceeded()
    {
        ExecuteReliableWriteTcs?.TrySetResult();
    }

    /// <summary>
    ///     Called when execute reliable write fails.
    /// </summary>
    protected void OnExecuteReliableWriteFailed(Exception e)
    {
        var success = ExecuteReliableWriteTcs?.TrySetException(e) ?? false;
        if (success)
        {
            return;
        }

        BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
    }

    /// <summary>
    ///     Called when abort reliable write succeeds.
    /// </summary>
    protected void OnAbortReliableWriteSucceeded()
    {
        AbortReliableWriteTcs?.TrySetResult();
    }

    /// <summary>
    ///     Called when abort reliable write fails.
    /// </summary>
    protected void OnAbortReliableWriteFailed(Exception e)
    {
        var success = AbortReliableWriteTcs?.TrySetException(e) ?? false;
        if (success)
        {
            return;
        }

        BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
    }

    #endregion
}