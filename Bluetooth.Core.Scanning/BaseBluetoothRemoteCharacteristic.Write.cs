namespace Bluetooth.Core.Scanning;

public abstract partial class BaseBluetoothRemoteCharacteristic
{
    /// <summary>
    ///     Semaphore used to ensure only one write operation can occur at a time.
    ///     This prevents concurrent write operations that could interfere with each other and ensures proper queuing.
    /// </summary>
    private SemaphoreSlim WriteSemaphoreSlim { get; } = new SemaphoreSlim(1, 1);

    /// <summary>
    ///     Gets or sets the task completion source for the current write value operation.
    ///     Used to signal completion of asynchronous write value operations.
    /// </summary>
    private TaskCompletionSource? WriteValueTcs
    {
        get => GetValue<TaskCompletionSource?>(null);
        set => SetValue(value);
    }

    /// <summary>
    ///     Gets a value indicating whether a write value operation is currently in progress.
    ///     This flag helps prevent concurrent write operations and tracks the operation state.
    /// </summary>
    public bool IsWriting
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    /// <inheritdoc />
    /// <exception cref="DeviceNotConnectedException">Thrown when the device is not connected.</exception>
    /// <exception cref="CharacteristicCantWriteException">Thrown when the characteristic does not support write operations.</exception>
    /// <exception cref="CharacteristicAlreadyWritingException">Thrown when another write operation is already in progress despite semaphore protection.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled via the cancellation token.</exception>
    /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
    public async ValueTask WriteValueAsync(ReadOnlyMemory<byte> value, bool skipIfOldValueMatchesNewValue = false, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        // Ensure Device is Connected
        DeviceNotConnectedException.ThrowIfNotConnected(RemoteService.Device);

        // Ensure WRITE is supported
        CharacteristicCantWriteException.ThrowIfCantWrite(this);

        // Check if the value is already written and skipIfOldValueMatchesNewValue is true
        if (skipIfOldValueMatchesNewValue && Value.IsIdenticalTo(value))
        {
            return;
        }

        // Prevents multiple calls to WriteValueAsync, putting them in a queue. WARNING: If the queue gets too long, timeout might be reached
        LogQueuingWrite(Id, RemoteService.Device.Id);
        await WriteSemaphoreSlim.WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);

        // Should not happen because of the semaphore, but just in case
        if (WriteValueTcs is { Task.IsCompleted: false })
        {
            throw new CharacteristicAlreadyWritingException(this);
        }

        WriteValueTcs = new TaskCompletionSource(); // Reset the TCS
        IsWriting = true; // Set the writing flag

        LogWritingValue(Id, RemoteService.Device.Id, value.Length);

        try
        {
            // Actual start writing native call
            await NativeWriteValueAsync(value).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            // if exception is thrown during start, we trigger the failure
            OnWriteValueFailed(e);
        }

        // try-finally to ensure disposal and release of resources
        try
        {
            // Wait for OnWriteValueSuccess to be called
            await WriteValueTcs.Task.WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            // Reset the writing flag
            IsWriting = false;
            WriteValueTcs = null;
            WriteSemaphoreSlim.Release();
        }
    }

    /// <summary>
    ///     Platform-specific implementation to write the characteristic's value.
    ///     This method should initiate the platform-specific operation to write the value to the characteristic.
    /// </summary>
    /// <param name="value">The value to write to the characteristic.</param>
    /// <returns>A task that completes when the native write operation is initiated.</returns>
    /// <remarks>
    ///     Implementations should call <see cref="OnWriteValueSucceeded" /> when the operation succeeds
    ///     or <see cref="OnWriteValueFailed" /> when it fails.
    /// </remarks>
    protected abstract ValueTask NativeWriteValueAsync(ReadOnlyMemory<byte> value);

    /// <summary>
    ///     Called when writing the characteristic's value succeeds. Completes the task successfully.
    /// </summary>
    /// <exception cref="CharacteristicUnexpectedWriteException">Thrown when no pending write operation is found to complete.</exception>
    protected void OnWriteValueSucceeded()
    {
        LogWriteSucceeded(Id, RemoteService.Device.Id);

        // Attempt to dispatch success to the TaskCompletionSource
        var success = WriteValueTcs?.TrySetResult() ?? false;
        if (success)
        {
            return;
        }

        // Else throw an exception
        LogUnexpectedWrite(Id, RemoteService.Device.Id);
        throw new CharacteristicUnexpectedWriteException(this);
    }

    /// <summary>
    ///     Called when writing the characteristic's value fails. Completes the task with an exception or dispatches to the unhandled exception listener.
    /// </summary>
    /// <param name="e">The exception that occurred during the write operation.</param>
    /// <remarks>
    ///     If there's a pending write operation, the exception will be delivered to it. Otherwise, the exception
    ///     will be dispatched to the unhandled exception listener.
    /// </remarks>
    protected void OnWriteValueFailed(Exception e)
    {
        LogWriteFailed(Id, RemoteService.Device.Id, e);

        // Attempt to dispatch exception to the TaskCompletionSource
        var success = WriteValueTcs?.TrySetException(e) ?? false;
        if (success)
        {
            return;
        }

        // If the TaskCompletionSource was already completed, dispatch the exception to the listener
        BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
    }

    #region Write - Capabilities

    /// <summary>
    ///     Platform-specific implementation to determine if the characteristic can be written to.
    ///     This method should check the platform-specific properties to determine write capability.
    /// </summary>
    /// <returns>True if the characteristic supports write operations; otherwise, false.</returns>
    protected abstract bool NativeCanWrite();

    /// <summary>
    ///     Gets a value that determines if the characteristic supports write operations based on platform-specific properties.
    ///     This property is computed once and cached using Lazy initialization.
    /// </summary>
    private Lazy<bool> LazyCanWrite { get; }

    /// <inheritdoc />
    public bool CanWrite => LazyCanWrite.Value;

    #endregion
}
