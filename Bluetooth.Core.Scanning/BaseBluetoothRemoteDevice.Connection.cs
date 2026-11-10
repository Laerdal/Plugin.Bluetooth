namespace Bluetooth.Core.Scanning;

public abstract partial class BaseBluetoothRemoteDevice
{
    #region ConnectionState

    /// <inheritdoc />
    public event EventHandler? Connected;

    /// <inheritdoc />
    public event EventHandler? Disconnected;

    /// <inheritdoc />
    public event EventHandler<DeviceConnectionStateChangedEventArgs>? ConnectionStateChanged;

    /// <inheritdoc />
    public bool IsConnected
    {
        get => GetValue(false);
        protected set
        {
            if (SetValue(value))
            {
                if (value)
                {
                    Connected?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    Disconnected?.Invoke(this, EventArgs.Empty);
                }

                ConnectionStateChanged?.Invoke(this, new DeviceConnectionStateChangedEventArgs(this, value));
            }
        }
    }

    /// <inheritdoc />
    public async ValueTask WaitForIsConnectedAsync(bool isConnected, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        await WaitForPropertyToBeOfValue(nameof(IsConnected), isConnected, timeout, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    ///     Platform-specific implementation to refresh the current connection state from the native platform.
    /// </summary>
    protected abstract void NativeRefreshIsConnected();

    /// <summary>
    ///     Refreshes the connection state from the native platform and completes once <see cref="IsConnected" />
    ///     reflects it. Callers that read <see cref="IsConnected" /> right after refreshing must await this rather than
    ///     call <see cref="NativeRefreshIsConnected" />, which may only schedule the update. The default runs the
    ///     synchronous refresh; platforms whose refresh completes asynchronously override it.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the wait.</param>
    /// <returns>A task that completes once <see cref="IsConnected" /> reflects the native connection state.</returns>
    /// <exception cref="OperationCanceledException">
    ///     Thrown by overrides that complete asynchronously when <paramref name="cancellationToken" /> is cancelled before the refresh
    ///     completes. The default implementation completes synchronously and does not observe the token.
    /// </exception>
    protected virtual ValueTask NativeRefreshIsConnectedAsync(CancellationToken cancellationToken = default)
    {
        NativeRefreshIsConnected();
        return ValueTask.CompletedTask;
    }

    #endregion

    #region Connecting

    /// <summary>
    ///     Gets a value indicating whether a connection operation is currently in progress.
    /// </summary>
    public bool IsConnecting
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    /// <inheritdoc />
    public event EventHandler? Connecting;

    private TaskCompletionSource? ConnectionTcs
    {
        get => GetValue<TaskCompletionSource?>(null);
        set => SetValue(value);
    }

    /// <summary>
    ///     Called when a connection attempt succeeds. Updates the connection state and completes the connection task.
    /// </summary>
    protected void OnConnectSucceeded()
    {
        NativeRefreshIsConnected();

        // Attempt to dispatch success to the TaskCompletionSource
        var success = ConnectionTcs?.TrySetResult() ?? false;
        if (success)
        {
            LogDeviceConnected(Id);
            return;
        }

        if (IsConnected)
        {
            return;
        }

        // Else throw an exception
        throw new DeviceFailedToConnectException(this);
    }

    /// <summary>
    ///     Called when a connection attempt fails. Completes the connection task with an exception or dispatches to the unhandled exception listener.
    /// </summary>
    /// <param name="e">The exception that occurred during the connection attempt.</param>
    protected void OnConnectFailed(Exception e)
    {
        LogDeviceConnectionFailed(Id, e);
        NativeRefreshIsConnected();

        // Attempt to dispatch exception to the TaskCompletionSource
        var success = (ConnectionTcs?.TrySetException(e) ?? false) || (DisconnectionTcs?.TrySetException(e) ?? false);
        if (success)
        {
            return;
        }

        // If the TaskCompletionSource was already completed, dispatch the exception to the listener
        BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
    }

    /// <inheritdoc />
    public async virtual ValueTask ConnectIfNeededAsync(ConnectionOptions? connectionOptions = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        connectionOptions ??= new ConnectionOptions();
        await NativeRefreshIsConnectedAsync(cancellationToken).ConfigureAwait(false);
        if (IsConnected)
        {
            return;
        }

        await ConnectAsync(connectionOptions, timeout, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async virtual ValueTask ConnectAsync(ConnectionOptions? connectionOptions = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        // Ensure we are not already connected
        if (IsConnected)
        {
            LogDeviceAlreadyConnected(Id);
        }

        DeviceIsAlreadyConnectedException.ThrowIfAlreadyConnected(this);
        connectionOptions ??= new ConnectionOptions();

        // Asynchronous continuations: a merged caller released by this attempt must not run its own code inside this one's cleanup.
        var ownConnection = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource? pendingConnection = null;
        lock (_attemptGate)
        {
            // Prevents multiple calls to ConnectAsync, if already starting, we merge the calls
            var current = ConnectionTcs;
            if (current is { Task.IsCompleted: false })
            {
                pendingConnection = current;
            }
            else
            {
                ConnectionTcs = ownConnection; // Reset the TCS
            }
        }

        if (pendingConnection is not null)
        {
            LogMergingConnectionAttempts(Id);
            await WaitForMergedConnectionAsync(pendingConnection.Task, timeout, cancellationToken).ConfigureAwait(false);
            return;
        }

        RefreshIsConnecting(); // Set the connecting state to true
        Connecting?.Invoke(this, EventArgs.Empty);

        try // try-catch to dispatch exceptions rising from start
        {
            LogDeviceConnecting(Id);
            if (connectionOptions.WaitForAdvertisementBeforeConnecting)
            {
                LogWaitingForAdvertisement(Id);
                await WaitForAdvertisementAsync(timeout, cancellationToken).ConfigureAwait(false);
            }

            await NativeConnectAsync(connectionOptions, timeout, cancellationToken).ConfigureAwait(false); // actual start native call
        }
        catch (Exception e)
        {
            OnConnectFailed(e); // if exception is thrown during start, we trigger the failure
        }

        // try-finally to ensure disposal and release of resources
        try
        {
            // Wait for OnConnectSucceeded to be called. NOTE: WaitBetterAsync throws
            // TimeoutException/OperationCanceledException directly on timeout/cancellation - it does
            // NOT return normally past the deadline - so this line itself is where a timed-out
            // connect attempt is detected, not the IsConnected check below.
            await ownConnection.Task.WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);

            await NativeRefreshIsConnectedAsync(cancellationToken).ConfigureAwait(false);
            if (!IsConnected)
            {
                throw new DeviceFailedToConnectException(this);
            }
        }
        catch (Exception e)
        {
            // Giving up here (timeout, cancellation, or the "wait completed but IsConnected is still
            // false" edge case above) does NOT retract the native connect request already issued by
            // NativeConnectAsync above (e.g. CoreBluetooth's connectPeripheral: has no concept of a
            // timeout - once called, iOS keeps trying indefinitely until explicitly told to stop).
            // Left alone, that request can succeed or fail later, completely detached from this call
            // and its caller - confirmed against real hardware via a Legacy DFU rediscovery flow that
            // abandoned a timed-out ConnectAsync(timeout:) and then connected to a second, different
            // device shortly after: the first device's stale connect request finally completed ~29s
            // afterwards, on its own, with nothing left to observe it - and the second, legitimate
            // connect failed right around the same time, consistent with two concurrent connection
            // attempts confusing the shared central manager. Cancel the native request so giving up
            // here actually means the device stops trying, not just that this caller stops watching.
            try { await NativeDisconnectAsync(timeout: null, cancellationToken: CancellationToken.None).ConfigureAwait(false); }
            catch { /* best-effort - we are already about to report the connection as failed */ }

            // Callers merged into this attempt wait on its task, which no native callback completes
            // once this attempt is over: end it here, after the cleanup above so that the
            // disconnection callback it may raise is still absorbed by the pending attempt instead of
            // being reported as an unexpected disconnection.
            AbandonAttempt(ownConnection, new DeviceFailedToConnectException(this, innerException: e));

            // Timeout/cancellation are documented, BCL-recognized outcomes of this method (see
            // IBluetoothRemoteDevice.Connection.cs) - callers following the standard .NET cancellation
            // idiom (catch (OperationCanceledException)) depend on the exact type surviving unwrapped.
            // Only normalize genuinely opaque failures (e.g. a raw native exception from
            // NativeConnectAsync) into DeviceFailedToConnectException.
            if (e is DeviceFailedToConnectException or TimeoutException or OperationCanceledException)
            {
                throw;
            }

            throw new DeviceFailedToConnectException(this, innerException: e);
        }
        finally
        {
            lock (_attemptGate)
            {
                // A caller released by AbandonAttempt, or one that arrived once this attempt had completed, may
                // already have started the next attempt: only release what still belongs to this one.
                if (ReferenceEquals(ConnectionTcs, ownConnection))
                {
                    ConnectionTcs = null;
                }
            }

            RefreshIsConnecting(); // Set the connecting state to false, unless the next attempt is already registered
        }
    }

    // IsConnecting follows whether an attempt is registered. A claim or a release can land between reading that and writing
    // the flag, so look again until the two agree.
    private void RefreshIsConnecting()
    {
        bool connecting;
        do
        {
            connecting = ConnectionTcs is not null;
            IsConnecting = connecting;
        }
        while (connecting != (ConnectionTcs is not null));
    }

    /// <summary>
    ///     Waits for the connection attempt another caller owns. The merged caller's own
    ///     <paramref name="timeout" /> and <paramref name="cancellationToken" /> bound the wait, including the refresh of
    ///     the connection state that follows it; giving up does not cancel the owner's attempt.
    /// </summary>
    private async ValueTask WaitForMergedConnectionAsync(Task attempt, TimeSpan? timeout, CancellationToken cancellationToken)
    {
        var started = Stopwatch.GetTimestamp();
        try
        {
            await WaitForAttemptAsync(attempt, timeout, cancellationToken).ConfigureAwait(false);

            // A disconnection callback also completes the pending attempt, so completion alone does not
            // mean the device is connected. The refresh gets what is left of the caller's timeout.
            using var remaining = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            if (timeout is { } limit && limit != Timeout.InfiniteTimeSpan)
            {
                var left = limit - Stopwatch.GetElapsedTime(started);
                remaining.CancelAfter(left > TimeSpan.Zero ? left : TimeSpan.Zero);
            }

            try
            {
                await NativeRefreshIsConnectedAsync(remaining.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested && remaining.IsCancellationRequested)
            {
                throw new TimeoutException("The connection state could not be refreshed within the timeout.");
            }

            if (!IsConnected)
            {
                throw new DeviceFailedToConnectException(this);
            }
        }
        catch (Exception e) when (e is not (DeviceFailedToConnectException or TimeoutException or OperationCanceledException))
        {
            // Same outcome as the owner: only genuinely opaque failures are normalized.
            throw new DeviceFailedToConnectException(this, innerException: e);
        }
    }

    /// <summary>
    ///     Platform-specific implementation to initiate a connection to the device.
    /// </summary>
    /// <param name="connectionOptions"></param>
    /// <param name="timeout">Optional timeout for the operation.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    protected abstract ValueTask NativeConnectAsync(ConnectionOptions connectionOptions, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    #endregion

    #region Disconnecting

    /// <summary>
    ///     Gets a value indicating whether a disconnection operation is currently in progress.
    /// </summary>
    public bool IsDisconnecting
    {
        get => GetValue(false);
        private set => SetValue(value);
    }


    /// <inheritdoc />
    public event EventHandler? Disconnecting;

    private TaskCompletionSource? DisconnectionTcs
    {
        get => GetValue<TaskCompletionSource?>(null);
        set => SetValue(value);
    }

    /// <summary>
    ///     Called when a disconnection occurs, either intentionally or unexpectedly. Completes the disconnection task.
    /// </summary>
    /// <param name="e">Optional exception that caused the disconnection.</param>
    protected void OnDisconnect(Exception? e = null)
    {
        NativeRefreshIsConnected();

        // Attempt to dispatch success/failure to a pending explicit Connect/Disconnect await.
        var success = (DisconnectionTcs?.TrySetResultOrException(e) ?? false) || (ConnectionTcs?.TrySetResultOrException(e) ?? false);
        if (success)
        {
            // Explicitly requested (someone is awaiting DisconnectAsync/ConnectAsync) - log its
            // outcome directly. This is not an unexpected disconnection, regardless of e.
            if (e == null)
            {
                LogDeviceDisconnected(Id);
            }
            else
            {
                LogDeviceDisconnectionFailed(Id, e);
            }
            return;
        }

        // No pending explicit await - this is an unexpected disconnection (e.g. a device rebooting
        // mid-DFU). OnUnexpectedDisconnection() owns all logging for this path: WARNING normally, or
        // a quiet DEBUG entry when IgnoreNextUnexpectedDisconnection suppresses it. Do NOT also log a
        // second, always-ERROR entry here - that bypassed IgnoreNextUnexpectedDisconnection entirely
        // and turned every expected DFU-reboot disconnection into ERROR-level noise.
        OnUnexpectedDisconnection(e);
    }

    /// <summary>
    ///     Completes a pending explicit <see cref="DisconnectAsync" /> without ever treating the call as an
    ///     unexpected disconnection. Intended for platforms where releasing the native resources in
    ///     <see cref="NativeDisconnectAsync" /> is itself the disconnection and no native callback follows
    ///     (e.g. Windows, where the disposed device no longer raises its connection-status event).
    /// </summary>
    /// <returns>
    ///     <c>true</c> if this call completed the pending disconnection; <c>false</c> if none was pending
    ///     (e.g. cleanup after a failed connection attempt) or a native callback had already completed it.
    /// </returns>
    protected bool TryCompleteDisconnection()
    {
        NativeRefreshIsConnected();

        var success = DisconnectionTcs?.TrySetResult() ?? false;
        if (success)
        {
            LogDeviceDisconnected(Id);
        }

        return success;
    }

    /// <inheritdoc />
    public async ValueTask DisconnectIfNeededAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        await NativeRefreshIsConnectedAsync(cancellationToken).ConfigureAwait(false);
        if (!IsConnected)
        {
            return;
        }

        await DisconnectAsync(timeout, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask DisconnectAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        // Ensure we are not already disconnected
        if (!IsConnected)
        {
            LogDeviceAlreadyDisconnected(Id);
        }

        DeviceIsAlreadyDisconnectedException.ThrowIfAlreadyDisconnected(this);

        var ownDisconnection = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource? pendingDisconnection = null;
        lock (_attemptGate)
        {
            // Prevents multiple calls to DisconnectAsync, if already disconnecting, we merge the calls
            var current = DisconnectionTcs;
            if (current is { Task.IsCompleted: false })
            {
                pendingDisconnection = current;
            }
            else
            {
                DisconnectionTcs = ownDisconnection; // Reset the TCS
            }
        }

        if (pendingDisconnection is not null)
        {
            // The merged caller's own timeout and cancellation token bound the wait; giving up does not
            // cancel the owner's attempt.
            LogMergingDisconnectionAttempts(Id);
            await WaitForAttemptAsync(pendingDisconnection.Task, timeout, cancellationToken).ConfigureAwait(false);
            return;
        }

        RefreshIsDisconnecting(); // Set the disconnecting state to true
        Disconnecting?.Invoke(this, EventArgs.Empty);

        try // try-catch to dispatch exceptions rising from start
        {
            LogDeviceDisconnecting(Id);
            await NativeDisconnectAsync(timeout, cancellationToken).ConfigureAwait(false); // actual start native call
        }
        catch (Exception e)
        {
            OnDisconnect(e); // if exception is thrown during start, we trigger the failure
        }

        // try-finally to ensure disposal and release of resources
        try
        {
            // Wait for OnDisconnection to be called
            await ownDisconnection.Task.WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);
            await ClearServicesAsync().ConfigureAwait(false);
            await NativeRefreshIsConnectedAsync(cancellationToken).ConfigureAwait(false);
            if (IsConnected)
            {
                throw new DeviceFailedToDisconnectException(this);
            }
        }
        catch (Exception e)
        {
            // Callers merged into this attempt wait on its task, which no native callback completes
            // once this attempt is over (timeout or cancellation here): end it so they stop waiting.
            AbandonAttempt(ownDisconnection, new DeviceFailedToDisconnectException(this, innerException: e));
            throw;
        }
        finally
        {
            lock (_attemptGate)
            {
                // See ConnectAsync: only release what still belongs to this attempt.
                if (ReferenceEquals(DisconnectionTcs, ownDisconnection))
                {
                    DisconnectionTcs = null;
                }
            }

            RefreshIsDisconnecting(); // Set the disconnecting state to false, unless the next attempt is already registered
        }
    }

    // See RefreshIsConnecting.
    private void RefreshIsDisconnecting()
    {
        bool disconnecting;
        do
        {
            disconnecting = DisconnectionTcs is not null;
            IsDisconnecting = disconnecting;
        }
        while (disconnecting != (DisconnectionTcs is not null));
    }

    /// <summary>
    ///     Platform-specific implementation to initiate a disconnection from the device.
    /// </summary>
    /// <param name="timeout">Optional timeout for the operation.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    protected abstract ValueTask NativeDisconnectAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    #endregion

    #region Connection - Merged attempts

    // Guards the decision to join or own an attempt and the release of ConnectionTcs/DisconnectionTcs, each one step. Only the
    // attempt fields are touched under it: events are raised and flags are written outside.
    private readonly object _attemptGate = new();

    // Task.WaitAsync rather than WaitBetterAsync: the latter ignores the cancellation token when no timeout is given.
    private static Task WaitForAttemptAsync(Task attempt, TimeSpan? timeout, CancellationToken cancellationToken)
    {
        return timeout is { } t ? attempt.WaitAsync(t, cancellationToken) : attempt.WaitAsync(cancellationToken);
    }

    /// <summary>
    ///     Ends a connect or disconnect attempt its owner is giving up on, so every caller merged into it
    ///     stops waiting. Does nothing when the attempt already completed.
    /// </summary>
    private static void AbandonAttempt(TaskCompletionSource attempt, Exception reason)
    {
        if (attempt.TrySetException(reason))
        {
            // Nobody may be merged into the attempt: read the fault so it is not reported as an unobserved task exception.
            _ = attempt.Task.Exception;
        }
    }

    #endregion

    #region Connection - UnexpectedDisconnection

    /// <inheritdoc />
    public event EventHandler<DeviceUnexpectedDisconnectionEventArgs>? UnexpectedDisconnection;

    /// <summary>
    ///     Gets or sets a value indicating whether the next unexpected disconnection should be ignored.
    /// </summary>
    public bool IgnoreNextUnexpectedDisconnection { get; set; }

    /// <summary>
    ///     Called when an unexpected disconnection occurs. Clears services and raises the UnexpectedDisconnection event.
    /// </summary>
    /// <param name="e">Optional exception that caused the unexpected disconnection.</param>
    protected virtual void OnUnexpectedDisconnection(Exception? e = null)
    {
        ClearServicesAsync().StartAndForget(ex => BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, ex));
        if (IgnoreNextUnexpectedDisconnection)
        {
            LogUnexpectedDisconnectionIgnored(Id);
            IgnoreNextUnexpectedDisconnection = false;
        }
        else // unexpected disconnection
        {
            LogUnexpectedDisconnection(Id, e);
            UnexpectedDisconnection?.Invoke(this, new DeviceUnexpectedDisconnectionEventArgs(e));
        }
    }

    #endregion

    #region Connection Priority

    /// <inheritdoc />
    public async ValueTask RequestConnectionPriorityAsync(ConnectionPriority priority, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        if (!IsConnected)
        {
            LogConnectionPriorityNotConnected(Id);
            throw new DeviceNotConnectedException(this, "Device must be connected to request connection priority change.");
        }

        LogRequestingConnectionPriority(Id, priority);
        await NativeRequestConnectionPriorityAsync(priority, timeout, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    ///     Platform-specific implementation to request a connection priority change.
    /// </summary>
    /// <param name="priority">The desired connection priority mode.</param>
    /// <param name="timeout">Optional timeout for the operation.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <remarks>
    ///     On platforms that don't support this feature (iOS, Windows), this should be a no-op.
    /// </remarks>
    protected abstract ValueTask NativeRequestConnectionPriorityAsync(ConnectionPriority priority, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    #endregion
}
