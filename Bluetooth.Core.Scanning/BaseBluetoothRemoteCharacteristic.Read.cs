namespace Bluetooth.Core.Scanning;

public abstract partial class BaseBluetoothRemoteCharacteristic
{
    /// <summary>
    /// Gets a value indicating whether a read value operation is currently in progress.
    /// This flag helps prevent concurrent read operations and tracks the operation state.
    /// </summary>
    public bool IsReading
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the task completion source for the current read value operation.
    /// Used to signal completion of asynchronous read value operations.
    /// </summary>
    private TaskCompletionSource<ReadOnlyMemory<byte>>? ReadValueTcs
    {
        get => GetValue<TaskCompletionSource<ReadOnlyMemory<byte>>?>(null);
        set => SetValue(value);
    }

    /// <inheritdoc/>
    /// <exception cref="DeviceNotConnectedException">Thrown when the device is not connected.</exception>
    /// <exception cref="CharacteristicCantReadException">Thrown when the characteristic does not support read operations.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled via the cancellation token.</exception>
    /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
    public async ValueTask<ReadOnlyMemory<byte>> ReadValueAsync(bool skipIfPreviouslyRead = false, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        // Ensure Device is Connected
        DeviceNotConnectedException.ThrowIfNotConnected(RemoteService.Device);

        // Ensure READ is supported
        CharacteristicCantReadException.ThrowIfCantRead(this);

        // Check if the value is already read and skipIfPreviouslyRead is true
        if (skipIfPreviouslyRead && Value.Length != 0)
        {
            LogMergingReadAttempts(Id, RemoteService.Device.Id);
            return Value;
        }

        // Prevents multiple calls to ReadValueAsync
        if (ReadValueTcs is { Task.IsCompleted: false })
        {
            LogMergingReadAttempts(Id, RemoteService.Device.Id);
            return await ReadValueTcs.Task.ConfigureAwait(false);
        }

        ReadValueTcs = new TaskCompletionSource<ReadOnlyMemory<byte>>(); // Reset the TCS
        IsReading = true; // Set the flag to true

        LogReadingValue(Id, RemoteService.Device.Id);

        try // try-catch to dispatch exceptions rising from start reading
        {
            await NativeReadValueAsync().ConfigureAwait(false); // Actual start reading native call
        }
        catch (Exception e)
        {
            // if exception is thrown during start, we trigger the failure
            OnReadValueFailed(e);
        }

        // try-finally to ensure disposal and release of resources
        try
        {
            // Wait for OnReadValueSuccess to be called or timeout
            return await ReadValueTcs.Task.WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            // Reset the reading flag
            IsReading = false;
            ReadValueTcs = null;
        }
    }

    /// <summary>
    /// Platform-specific implementation to read the characteristic's value.
    /// This method should initiate the platform-specific operation to read the characteristic value.
    /// </summary>
    /// <returns>A task that completes when the native read operation is initiated.</returns>
    /// <remarks>
    /// Implementations should call <see cref="OnReadValueSucceeded"/> when the operation succeeds
    /// or <see cref="OnReadValueFailed"/> when it fails.
    /// </remarks>
    protected abstract ValueTask NativeReadValueAsync();

    /// <summary>
    /// Called when reading the characteristic's value succeeds. Updates the Value property and completes the task.
    /// </summary>
    /// <param name="value">The value read from the characteristic.</param>
    /// <exception cref="CharacteristicUnexpectedReadException">Thrown when no pending read operation is found to complete and the characteristic is not listening.</exception>
    protected void OnReadValueSucceeded(ReadOnlyMemory<byte> value)
    {
        var old = Value;
        Value = value;

        LogReadSucceeded(Id, RemoteService.Device.Id, value.Length);

        // Attempt to dispatch success to the TaskCompletionSource
        if (ReadValueTcs != null && ReadValueTcs.TrySetResult(value))
        {
            return;
        }

        // If the TaskCompletionSource wasn't available, but we are listening, dispatch the value to the listener
        if (IsListening)
        {
            OnValueUpdated(value, old);
            return;
        }

        // Else throw an exception
        LogUnexpectedRead(Id, RemoteService.Device.Id);
        throw new CharacteristicUnexpectedReadException(this);
    }

    /// <summary>
    /// Called when reading the characteristic's value fails. Completes the task with an exception or dispatches to the unhandled exception listener.
    /// </summary>
    /// <param name="e">The exception that occurred during the read operation.</param>
    /// <remarks>
    /// If there's a pending read operation, the exception will be delivered to it. Otherwise, the exception
    /// will be dispatched to the unhandled exception listener.
    /// </remarks>
    protected void OnReadValueFailed(Exception e)
    {
        LogReadFailed(Id, RemoteService.Device.Id, e);

        // Attempt to dispatch exception to the TaskCompletionSource
        var success = ReadValueTcs?.TrySetException(e) ?? false;
        if (success)
        {
            return;
        }

        // If the TaskCompletionSource was already completed, dispatch the exception to the listener
        BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
    }

    #region Read - Capabilities

    /// <summary>
    /// Platform-specific implementation to determine if the characteristic can be read.
    /// This method should check the platform-specific properties to determine read capability.
    /// </summary>
    /// <returns>True if the characteristic supports read operations; otherwise, false.</returns>
    protected abstract bool NativeCanRead();

    /// <summary>
    /// Gets a value that determines if the characteristic supports read operations based on platform-specific properties.
    /// This property is computed once and cached using Lazy initialization.
    /// </summary>
    private Lazy<bool> LazyCanRead { get; }

    /// <inheritdoc/>
    public bool CanRead => LazyCanRead.Value;

    #endregion
}
