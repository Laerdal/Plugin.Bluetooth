namespace Bluetooth.Core.Scanning;

public abstract partial class BaseBluetoothRemoteCharacteristic
{
    /// <inheritdoc />
    public bool IsListening
    {
        get => GetValue(false);
        protected set => SetValue(value);
    }

    /// <inheritdoc />
    /// <exception cref="DeviceNotConnectedException">Thrown when the device is not connected.</exception>
    /// <exception cref="CharacteristicCantListenException">Thrown when the characteristic does not support notifications or indications.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled via the cancellation token.</exception>
    /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
    public async ValueTask StartListeningAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        LogStartingNotifications(Id, RemoteService.Device.Id);

        await ReadIsListeningAsync(timeout, cancellationToken).ConfigureAwait(false);
        if (IsListening)
        {
            LogNotificationsStarted(Id, RemoteService.Device.Id);
            return;
        }

        await WriteIsListeningAsync(true, timeout, cancellationToken).ConfigureAwait(false);
        await ReadIsListeningAsync(timeout, cancellationToken).ConfigureAwait(false);

        LogNotificationsStarted(Id, RemoteService.Device.Id);
    }

    /// <inheritdoc />
    /// <exception cref="DeviceNotConnectedException">Thrown when the device is not connected.</exception>
    /// <exception cref="CharacteristicCantListenException">Thrown when the characteristic does not support notifications or indications.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled via the cancellation token.</exception>
    /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
    public async ValueTask StopListeningAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        LogStoppingNotifications(Id, RemoteService.Device.Id);

        await ReadIsListeningAsync(timeout, cancellationToken).ConfigureAwait(false);
        if (!IsListening)
        {
            LogNotificationsStopped(Id, RemoteService.Device.Id);
            return;
        }

        await WriteIsListeningAsync(false, timeout, cancellationToken).ConfigureAwait(false);
        await ReadIsListeningAsync(timeout, cancellationToken).ConfigureAwait(false);

        LogNotificationsStopped(Id, RemoteService.Device.Id);
    }


    /// <summary>
    ///     Invokes the ValueUpdated event when the characteristic's value changes.
    /// </summary>
    /// <param name="newValue">The new value of the characteristic.</param>
    /// <param name="oldValue">The old value of the characteristic.</param>
    private void OnValueUpdated(ReadOnlyMemory<byte> newValue, ReadOnlyMemory<byte> oldValue)
    {
        LogValueUpdated(Id, RemoteService.Device.Id, newValue.Length);
        ValueUpdated?.Invoke(this, new ValueUpdatedEventArgs(newValue, oldValue));
    }

    #region Listen - Capabilities

    /// <summary>
    ///     Platform-specific implementation to determine if the characteristic can listen for notifications.
    /// </summary>
    /// <returns>True if the characteristic supports listening for notifications; otherwise, false.</returns>
    protected abstract bool NativeCanListen();

    /// <summary>
    ///     Gets a value that determines if the characteristic supports notifications or indications based on platform-specific properties.
    ///     This property is computed once and cached using Lazy initialization.
    /// </summary>
    private Lazy<bool> LazyCanListen { get; }

    /// <inheritdoc />
    public bool CanListen => LazyCanListen.Value;

    #endregion


    #region ReadIsListening

    /// <summary>
    ///     Platform-specific implementation to read the current listening state of the characteristic.
    ///     This method should initiate the platform-specific operation to query whether notifications/indications are enabled.
    /// </summary>
    /// <returns>A task that completes when the native read operation is initiated.</returns>
    /// <remarks>
    ///     Implementations should call <see cref="OnReadIsListeningSucceeded" /> when the operation succeeds
    ///     or <see cref="OnReadIsListeningFailed" /> when it fails.
    /// </remarks>
    protected abstract ValueTask NativeReadIsListeningAsync();

    /// <summary>
    ///     Called when reading the listening state succeeds. Updates the IsListening property and completes the task.
    /// </summary>
    /// <param name="isListening">The current listening state returned from the native platform.</param>
    /// <exception cref="CharacteristicUnexpectedReadNotifyException">Thrown when no pending read operation is found to complete.</exception>
    protected void OnReadIsListeningSucceeded(bool isListening)
    {
        IsListening = isListening;

        // Attempt to dispatch success to the TaskCompletionSource
        var success = ReadIsListeningTcs?.TrySetResult(isListening) ?? false;
        if (success)
        {
            return;
        }

        // Else throw an exception
        throw new CharacteristicUnexpectedReadNotifyException(this);
    }

    /// <summary>
    ///     Called when reading the listening state fails. Completes the task with an exception or dispatches to the unhandled exception listener.
    /// </summary>
    /// <param name="e">The exception that occurred during the read operation.</param>
    /// <remarks>
    ///     If there's a pending read operation, the exception will be delivered to it. Otherwise, the exception
    ///     will be dispatched to the unhandled exception listener.
    /// </remarks>
    protected void OnReadIsListeningFailed(Exception e)
    {
        LogNotificationsStartFailed(Id, RemoteService.Device.Id, e);

        // Attempt to dispatch exception to the TaskCompletionSource
        var success = ReadIsListeningTcs?.TrySetException(e) ?? false;
        if (success)
        {
            return;
        }

        // If the TaskCompletionSource was already completed, dispatch the exception to the listener
        BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
    }

    /// <summary>
    ///     Gets or sets a value indicating whether a read listening operation is currently in progress.
    ///     This flag helps prevent concurrent read operations and tracks the operation state.
    /// </summary>
    public bool IsReadingIsListening
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    /// <summary>
    ///     Gets or sets the task completion source for the current read listening operation.
    ///     Used to signal completion of asynchronous read listening operations.
    /// </summary>
    private TaskCompletionSource<bool>? ReadIsListeningTcs
    {
        get => GetValue<TaskCompletionSource<bool>?>(null);
        set => SetValue(value);
    }

    /// <summary>
    ///     Reads the current listening state of the characteristic asynchronously, ensuring that only one read operation is in progress at a time.
    /// </summary>
    /// <param name="timeout">The timeout for this operation.</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="DeviceNotConnectedException">Thrown when the device is not connected.</exception>
    /// <exception cref="CharacteristicCantListenException">Thrown when the characteristic does not support notifications or indications.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled via the cancellation token.</exception>
    /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
    public async ValueTask<bool> ReadIsListeningAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        // Ensure Device is Connected
        DeviceNotConnectedException.ThrowIfNotConnected(RemoteService.Device);

        // Ensure LISTEN is supported
        CharacteristicCantListenException.ThrowIfCantListen(this);

        // Prevents multiple calls to ReadIsListeningAsync
        if (ReadIsListeningTcs is { Task.IsCompleted: false })
        {
            LogMergingNotificationAttempts(Id, RemoteService.Device.Id);
            return await ReadIsListeningTcs.Task.ConfigureAwait(false);
        }

        ReadIsListeningTcs = new TaskCompletionSource<bool>(); // Reset the TCS
        IsReadingIsListening = true; // Set the flag to true

        try // try-catch to dispatch exceptions rising from start reading
        {
            // Actual start reading native call
            await NativeReadIsListeningAsync().ConfigureAwait(false);
        }
        catch (Exception e)
        {
            // if exception is thrown during start, we trigger the failure
            OnReadIsListeningFailed(e);
        }

        // try-finally to ensure disposal and release of resources
        try
        {
            // Wait for OnReadValueSuccess to be called
            IsListening = await ReadIsListeningTcs.Task.WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);
            return IsListening;
        }
        finally
        {
            // Reset the reading flag
            IsReadingIsListening = false;
            ReadIsListeningTcs = null;
        }
    }

    #endregion

    #region WriteIsListening

    /// <summary>
    ///     Platform-specific implementation to write (set) the listening state of the characteristic.
    ///     This method should initiate the platform-specific operation to enable or disable notifications/indications.
    /// </summary>
    /// <param name="shouldBeListening">True to enable notifications/indications, false to disable them.</param>
    /// <returns>A task that completes when the native write operation is initiated.</returns>
    /// <remarks>
    ///     Implementations should call <see cref="OnWriteIsListeningSucceeded" /> when the operation succeeds
    ///     or <see cref="OnWriteIsListeningFailed" /> when it fails.
    /// </remarks>
    protected abstract ValueTask NativeWriteIsListeningAsync(bool shouldBeListening);

    /// <summary>
    ///     Called when writing the listening state succeeds. Completes the task successfully.
    /// </summary>
    /// <exception cref="CharacteristicUnexpectedWriteNotifyException">Thrown when no pending write operation is found to complete.</exception>
    protected void OnWriteIsListeningSucceeded()
    {
        // Attempt to dispatch success to the TaskCompletionSource
        var success = WriteIsListeningTcs?.TrySetResult() ?? false;
        if (success)
        {
            return;
        }

        // Else throw an exception
        throw new CharacteristicUnexpectedWriteNotifyException(this);
    }

    /// <summary>
    ///     Called when writing the listening state fails. Completes the task with an exception or dispatches to the unhandled exception listener.
    /// </summary>
    /// <param name="e">The exception that occurred during the write operation.</param>
    /// <remarks>
    ///     If there's a pending write operation, the exception will be delivered to it. Otherwise, the exception
    ///     will be dispatched to the unhandled exception listener.
    /// </remarks>
    protected void OnWriteIsListeningFailed(Exception e)
    {
        LogNotificationsStartFailed(Id, RemoteService.Device.Id, e);

        // Attempt to dispatch exception to the TaskCompletionSource
        var success = WriteIsListeningTcs?.TrySetException(e) ?? false;
        if (success)
        {
            return;
        }

        // If the TaskCompletionSource was already completed, dispatch the exception to the listener
        BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
    }

    /// <summary>
    ///     Semaphore used to ensure only one write listening operation can occur at a time.
    ///     This prevents concurrent write operations that could interfere with each other.
    /// </summary>
    private SemaphoreSlim WriteIsListeningSemaphoreSlim { get; } = new(1, 1);

    /// <summary>
    ///     Gets or sets a value indicating whether a write listening operation is currently in progress.
    ///     This flag helps prevent concurrent write operations and tracks the operation state.
    /// </summary>
    public bool IsWritingIsListening
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    /// <summary>
    ///     Gets or sets the task completion source for the current write listening operation.
    ///     Used to signal completion of asynchronous write listening operations.
    /// </summary>
    private TaskCompletionSource? WriteIsListeningTcs
    {
        get => GetValue<TaskCompletionSource?>(null);
        set => SetValue(value);
    }

    /// <summary>
    ///     Writes the desired listening state to the characteristic. This method ensures that only one write operation can occur at a time
    ///     and handles the asynchronous flow of starting the write operation and waiting for its completion.
    /// </summary>
    /// <param name="shouldBeListening">True to enable notifications/indications, false to disable them.</param>
    /// <param name="timeout">The timeout for this operation.</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="DeviceNotConnectedException">Thrown when the device is not connected.</exception>
    /// <exception cref="CharacteristicCantListenException">Thrown when the characteristic does not support notifications or indications.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled via the cancellation token.</exception>
    /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
    /// <exception cref="UnreachableException">Thrown when an already writing operation is detected despite semaphore protection.</exception>
    public async ValueTask WriteIsListeningAsync(bool shouldBeListening, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        // Ensure Device is Connected
        DeviceNotConnectedException.ThrowIfNotConnected(RemoteService.Device);

        // Ensure LISTEN is supported
        CharacteristicCantListenException.ThrowIfCantListen(this);

        // Check if the characteristic is already listened to
        if (IsListening == shouldBeListening)
        {
            return;
        }

        // Prevents multiple calls to WriteIsListeningAsync, putting them in a queue. WARNING: If the queue gets too long, timeout might be reached
        await WriteIsListeningSemaphoreSlim.WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);

        // Should not happen because of the semaphore, but just in case
        if (WriteIsListeningTcs is { Task.IsCompleted: false })
        {
            throw new UnreachableException("Already writing IsListening");
        }

        WriteIsListeningTcs = new TaskCompletionSource(); // Reset the TCS
        IsWritingIsListening = true; // Set the flag to true

        try // try-catch to dispatch exceptions rising from start reading
        {
            // Actual start writing native call
            await NativeWriteIsListeningAsync(shouldBeListening).ConfigureAwait(false);
        }
        catch (CharacteristicException e)
        {
            // if exception is thrown during start, we trigger the failure
            OnWriteIsListeningFailed(e);
        }
        catch (Exception e)
        {
            // if exception is thrown during start, we trigger the failure
            OnWriteIsListeningFailed(new CharacteristicNotifyException(this, innerException: e));
        }

        // try-finally to ensure disposal and release of resources
        try
        {
            // Wait for OnWriteIsListeningSuccess to be called
            await WriteIsListeningTcs.Task.WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            // Reset the writing flag
            IsWritingIsListening = false;
            WriteIsListeningTcs = null;
            WriteIsListeningSemaphoreSlim.Release();
        }
    }

    #endregion
}
