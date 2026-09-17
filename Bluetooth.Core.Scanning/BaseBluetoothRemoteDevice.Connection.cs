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
        await NativeRefreshIsConnectedAsync(cancellationToken).AsTask().WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);
        await WaitForPropertyToBeOfValue(nameof(IsConnected), isConnected, timeout, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    ///     Synchronizes the check-and-set of <see cref="ConnectionTcs" />/<see cref="DisconnectionTcs" />
    ///     in <see cref="ConnectAsync" />/<see cref="DisconnectAsync" />. The refresh those methods await
    ///     immediately beforehand is a real yield point (a main-thread dispatch on Apple), so without this
    ///     lock two concurrent calls could both observe no pending TCS and both issue separate native
    ///     connect/disconnect requests instead of merging onto one. The critical section never awaits, so
    ///     a plain lock is sufficient - no need for a SemaphoreSlim here.
    /// </summary>
    private readonly object _connectionOperationLock = new();

    /// <summary>
    ///     Platform-specific implementation to refresh the current connection state from the native platform.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the refresh operation.</param>
    /// <returns>A task that represents the asynchronous refresh operation.</returns>
    /// <remarks>
    ///     Not every platform can offer a real freshness guarantee: Apple and Windows perform a synchronous
    ///     native query, but Android relies entirely on the <c>OnConnectionStateChange</c> callback and only
    ///     clears <see cref="IsConnected" /> here when its GATT proxy is null - see ADR 0003.
    /// </remarks>
    protected abstract ValueTask NativeRefreshIsConnectedAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Reports an exception to <see cref="BluetoothUnhandledExceptionListener" /> without letting its
    ///     own throw-when-unobserved behavior propagate to the caller.
    /// </summary>
    /// <param name="exception">The exception to report.</param>
    /// <remarks>
    ///     <see cref="BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException" /> rethrows when no
    ///     listener is registered. Callers that must guarantee they reach code after this call regardless
    ///     (e.g. completing a captured <see cref="TaskCompletionSource" /> for a best-effort refresh failure
    ///     that is secondary to the operation's real outcome) need this instead of calling it directly.
    /// </remarks>
    private void ReportBestEffortFailure(Exception exception)
    {
        try
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, exception);
        }
        catch
        {
            // Intentionally swallowed - see remarks above.
        }
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
    /// <param name="cancellationToken">Token to cancel the refresh operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected async ValueTask OnConnectSucceededAsync(CancellationToken cancellationToken = default)
    {
        // Capture the TCS for *this* attempt before awaiting below - ConnectAsync's own finally
        // can null out (or replace with a new attempt's) the live ConnectionTcs property while
        // this await is in flight (e.g. if the caller already gave up on this attempt via
        // timeout/cancellation), so completing whatever ConnectionTcs happens to be live *after*
        // the await would risk resolving a later, unrelated connect attempt instead of this one.
        var connectionTcs = ConnectionTcs;

        // Best-effort: a failed/cancelled refresh must not prevent the captured TCS below from
        // being completed - ConnectAsync's "merge concurrent attempts" branch awaits this exact
        // TCS with no timeout of its own, so leaving it uncompleted here would hang that caller
        // forever once the owning ConnectAsync's finally clears the live ConnectionTcs property.
        try
        {
            await NativeRefreshIsConnectedAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception refreshException)
        {
            ReportBestEffortFailure(refreshException);
        }

        // Attempt to dispatch success to the TaskCompletionSource
        var success = connectionTcs?.TrySetResult() ?? false;
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
    /// <param name="cancellationToken">Token to cancel the refresh operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected async ValueTask OnConnectFailedAsync(Exception e, CancellationToken cancellationToken = default)
    {
        LogDeviceConnectionFailed(Id, e);

        // Capture both TCS instances before awaiting below - see OnConnectSucceededAsync for why.
        var connectionTcs = ConnectionTcs;
        var disconnectionTcs = DisconnectionTcs;

        // Best-effort: see OnConnectSucceededAsync for why a failed/cancelled refresh must not
        // prevent the captured TCS below from being completed.
        try
        {
            await NativeRefreshIsConnectedAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception refreshException)
        {
            ReportBestEffortFailure(refreshException);
        }

        // Attempt to dispatch exception to the TaskCompletionSource
        var success = (connectionTcs?.TrySetException(e) ?? false) || (disconnectionTcs?.TrySetException(e) ?? false);
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
        await NativeRefreshIsConnectedAsync(cancellationToken).AsTask().WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);
        if (IsConnected)
        {
            return;
        }

        await ConnectAsync(connectionOptions, timeout, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async virtual ValueTask ConnectAsync(ConnectionOptions? connectionOptions = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        // Refresh before the already-connected guard below - callers invoking ConnectAsync
        // directly (rather than through ConnectIfNeededAsync, which already refreshes) would
        // otherwise have this guard decide against a stale cached IsConnected value.
        await NativeRefreshIsConnectedAsync(cancellationToken).AsTask().WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);

        // Ensure we are not already connected
        if (IsConnected)
        {
            LogDeviceAlreadyConnected(Id);
        }

        DeviceIsAlreadyConnectedException.ThrowIfAlreadyConnected(this);
        connectionOptions ??= new ConnectionOptions();

        // Prevents multiple calls to ConnectAsync, if already starting, we merge the calls.
        // The refresh above just awaited a real yield point, so the check-and-set below needs
        // _connectionOperationLock to stay atomic across concurrent callers - see its doc comment.
        // ownConnectionTcs (rather than re-reading the live ConnectionTcs property later) is what
        // lets the finally below tell whether it still owns the live TCS or a newer, concurrent
        // attempt has already replaced it.
        var ownConnectionTcs = new TaskCompletionSource();
        Task? pendingConnectionTask;
        lock (_connectionOperationLock)
        {
            if (ConnectionTcs is { Task.IsCompleted: false })
            {
                pendingConnectionTask = ConnectionTcs.Task;
            }
            else
            {
                pendingConnectionTask = null;
                ConnectionTcs = ownConnectionTcs; // Reset the TCS
            }
        }

        if (pendingConnectionTask != null)
        {
            LogMergingConnectionAttempts(Id);
            // Bounded by this caller's own timeout/cancellationToken, not just the owning
            // attempt's - otherwise a merged caller would wait on the owner's TCS indefinitely
            // if the owner gives up (or its refresh keeps it pending) without ever completing it.
            await pendingConnectionTask.WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);
            return;
        }

        IsConnecting = true; // Set the connecting state to true
        Connecting?.Invoke(this, EventArgs.Empty);

        // Single outer try/finally so state cleanup below always runs, even if OnConnectFailedAsync
        // itself throws (e.g. cancellation racing NativeRefreshIsConnectedAsync's own await) - without
        // this, that exception would escape past this finally and leave IsConnecting/ConnectionTcs
        // permanently stuck, wedging every subsequent ConnectAsync call on this device.
        try
        {
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
                await OnConnectFailedAsync(e, cancellationToken).ConfigureAwait(false); // if exception is thrown during start, we trigger the failure
            }

            // Wait for OnConnectSucceeded to be called. NOTE: WaitBetterAsync throws
            // TimeoutException/OperationCanceledException directly on timeout/cancellation - it does
            // NOT return normally past the deadline - so this line itself is where a timed-out
            // connect attempt is detected, not the IsConnected check below. Awaits the captured
            // ownConnectionTcs, not the live ConnectionTcs property, which a concurrent attempt may
            // have already replaced by the time this runs.
            await ownConnectionTcs.Task.WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);

            await NativeRefreshIsConnectedAsync(cancellationToken).AsTask().WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);
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
            // Only reset IsConnecting/ConnectionTcs if this attempt still owns the live
            // operation - a concurrent attempt may have already replaced ConnectionTcs and set
            // IsConnecting back to true for its own in-flight connect (see the merge branch
            // above); resetting unconditionally here would corrupt that newer attempt's state,
            // not just its TCS.
            lock (_connectionOperationLock)
            {
                if (ReferenceEquals(ConnectionTcs, ownConnectionTcs))
                {
                    IsConnecting = false; // Set the connecting state to false
                    ConnectionTcs = null;
                }
            }
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
    /// <param name="cancellationToken">Token to cancel the refresh operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected async ValueTask OnDisconnectAsync(Exception? e = null, CancellationToken cancellationToken = default)
    {
        // Capture both TCS instances before awaiting below - see OnConnectSucceededAsync for why.
        var disconnectionTcs = DisconnectionTcs;
        var connectionTcs = ConnectionTcs;

        // Best-effort: see OnConnectSucceededAsync for why a failed/cancelled refresh must not
        // prevent the captured TCS below from being completed.
        try
        {
            await NativeRefreshIsConnectedAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception refreshException)
        {
            ReportBestEffortFailure(refreshException);
        }

        // Attempt to dispatch success/failure to a pending explicit Connect/Disconnect await.
        var success = (disconnectionTcs?.TrySetResultOrException(e) ?? false) || (connectionTcs?.TrySetResultOrException(e) ?? false);
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

    /// <inheritdoc />
    public async ValueTask DisconnectIfNeededAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        await NativeRefreshIsConnectedAsync(cancellationToken).AsTask().WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);
        if (!IsConnected)
        {
            return;
        }

        await DisconnectAsync(timeout, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask DisconnectAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        // Refresh before the already-disconnected guard below - callers invoking DisconnectAsync
        // directly (rather than through DisconnectIfNeededAsync, which already refreshes) would
        // otherwise have this guard decide against a stale cached IsConnected value.
        await NativeRefreshIsConnectedAsync(cancellationToken).AsTask().WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);

        // Ensure we are not already disconnected
        if (!IsConnected)
        {
            LogDeviceAlreadyDisconnected(Id);
        }

        DeviceIsAlreadyDisconnectedException.ThrowIfAlreadyDisconnected(this);

        // Prevents multiple calls to ConnectAsync, if already starting, we merge the calls.
        // The refresh above just awaited a real yield point, so the check-and-set below needs
        // _connectionOperationLock to stay atomic across concurrent callers - see its doc comment.
        // ownDisconnectionTcs (rather than re-reading the live DisconnectionTcs property later) is
        // what lets the finally below tell whether it still owns the live TCS or a newer,
        // concurrent attempt has already replaced it.
        var ownDisconnectionTcs = new TaskCompletionSource();
        Task? pendingDisconnectionTask;
        lock (_connectionOperationLock)
        {
            if (DisconnectionTcs is { Task.IsCompleted: false })
            {
                pendingDisconnectionTask = DisconnectionTcs.Task;
            }
            else
            {
                pendingDisconnectionTask = null;
                DisconnectionTcs = ownDisconnectionTcs; // Reset the TCS
            }
        }

        if (pendingDisconnectionTask != null)
        {
            LogMergingDisconnectionAttempts(Id);
            // Bounded by this caller's own timeout/cancellationToken, not just the owning
            // attempt's - otherwise a merged caller would wait on the owner's TCS indefinitely
            // if the owner gives up (or its refresh keeps it pending) without ever completing it.
            await pendingDisconnectionTask.WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);
            return;
        }

        IsDisconnecting = true; // Set the disconnecting state to true
        Disconnecting?.Invoke(this, EventArgs.Empty);

        // Single outer try/finally so state cleanup below always runs, even if OnDisconnectAsync
        // itself throws (e.g. cancellation racing NativeRefreshIsConnectedAsync's own await) - without
        // this, that exception would escape past this finally and leave IsDisconnecting/DisconnectionTcs
        // permanently stuck, wedging every subsequent DisconnectAsync call on this device.
        try
        {
            try // try-catch to dispatch exceptions rising from start
            {
                LogDeviceDisconnecting(Id);
                await NativeDisconnectAsync(timeout, cancellationToken).ConfigureAwait(false); // actual start native call
            }
            catch (Exception e)
            {
                await OnDisconnectAsync(e, cancellationToken).ConfigureAwait(false); // if exception is thrown during start, we trigger the failure
            }

            // Wait for OnDisconnection to be called. Awaits the captured ownDisconnectionTcs, not
            // the live DisconnectionTcs property, which a concurrent attempt may have already
            // replaced by the time this runs.
            await ownDisconnectionTcs.Task.WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);
            await ClearServicesAsync().ConfigureAwait(false);
            await NativeRefreshIsConnectedAsync(cancellationToken).AsTask().WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);
            if (IsConnected)
            {
                throw new DeviceFailedToDisconnectException(this);
            }
        }
        finally
        {
            // Only reset IsDisconnecting/DisconnectionTcs if this attempt still owns the live
            // operation - a concurrent attempt may have already replaced DisconnectionTcs and set
            // IsDisconnecting back to true for its own in-flight disconnect (see the merge branch
            // above); resetting unconditionally here would corrupt that newer attempt's state,
            // not just its TCS.
            lock (_connectionOperationLock)
            {
                if (ReferenceEquals(DisconnectionTcs, ownDisconnectionTcs))
                {
                    IsDisconnecting = false; // Set the disconnecting state to false
                    DisconnectionTcs = null;
                }
            }
        }
    }

    /// <summary>
    ///     Platform-specific implementation to initiate a disconnection from the device.
    /// </summary>
    /// <param name="timeout">Optional timeout for the operation.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    protected abstract ValueTask NativeDisconnectAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

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
