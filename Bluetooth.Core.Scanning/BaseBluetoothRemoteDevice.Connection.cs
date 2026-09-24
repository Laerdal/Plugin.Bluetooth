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
        // WaitBetterAsync's own documented contract (see its callers throughout this repo) treats
        // null, zero, AND negative - including Timeout.InfiniteTimeSpan (-1ms), this repo's
        // documented "wait forever" sentinel per Docs/API-Reference/README.md - as "no timeout".
        // Only checking for null here would let Timeout.InfiniteTimeSpan fall into the timed branch
        // below, where subtracting elapsed time from an already-negative budget always leaves
        // `remaining <= TimeSpan.Zero` - throwing TimeoutException immediately instead of waiting
        // indefinitely as documented.
        if (timeout is not { } timeoutValue || timeoutValue <= TimeSpan.Zero)
        {
            await RefreshIsConnectedAsync(null, cancellationToken).ConfigureAwait(false);
            await WaitForPropertyToBeOfValue(nameof(IsConnected), isConnected, null, cancellationToken).ConfigureAwait(false);
            return;
        }

        // Passing the same timeout to both awaits would let this method run for up to ~2x
        // timeout when the refresh takes non-zero time and the property isn't already at the
        // target value - the documented contract is one timeout for the whole operation, not one
        // each. Track elapsed time across the refresh and give the property wait only what's left.
        // WaitBetterAsync treats a zero/negative timeout as "no timeout" (waits forever), not "time
        // already up" - so an exhausted budget must throw directly here instead of being passed
        // through as-is.
        var stopwatch = Stopwatch.StartNew();
        await RefreshIsConnectedAsync(timeoutValue, cancellationToken).ConfigureAwait(false);

        var remaining = timeoutValue - stopwatch.Elapsed;
        if (remaining <= TimeSpan.Zero)
        {
            throw new TimeoutException($"{nameof(WaitForIsConnectedAsync)} timed out after {timeoutValue}.");
        }

        await WaitForPropertyToBeOfValue(nameof(IsConnected), isConnected, remaining, cancellationToken).ConfigureAwait(false);
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
    ///     The <see cref="ConnectionTcs" /> instance that was live when <see cref="ConnectAsync" /> most
    ///     recently started a genuinely new (non-merged) connect attempt - captured separately from
    ///     <see cref="ConnectionTcs" /> itself so it can be retired (set to <c>null</c>) the moment
    ///     <see cref="ConnectAsync" /> gives up on an attempt whose native connect request could not be
    ///     synchronously retracted, *before* <see cref="ConnectionTcs" /> itself is cleared in that
    ///     method's <c>finally</c>. Every native-callback-driven completion path
    ///     (<see cref="OnConnectSucceededAsync" />/<see cref="OnConnectFailedAsync" />/the connect-attempt
    ///     branch of <see cref="OnDisconnectAsync" />) validates this against the live
    ///     <see cref="ConnectionTcs" /> via <see cref="TryClaimPendingConnectAttempt" /> before touching
    ///     any state - a mismatch means the signal is stale (superseded by a newer attempt, or explicitly
    ///     retired after abandonment) and must not resolve whatever attempt happens to be live now. See
    ///     ADR 0003.
    /// </summary>
    private TaskCompletionSource? _connectAttemptToken;

    /// <summary>Disconnect equivalent of <see cref="_connectAttemptToken" />, validated against
    /// <see cref="DisconnectionTcs" /> inside <see cref="OnDisconnectAsync" />.</summary>
    private TaskCompletionSource? _disconnectAttemptToken;

    /// <summary>
    ///     Atomically checks whether a connect attempt is still pending (its <see cref="ConnectionTcs" />
    ///     matches the token captured when <see cref="ConnectAsync" /> actually issued the native connect
    ///     call, and no terminal signal has been claimed for it yet) and, if so, claims it - in one lock
    ///     acquisition, closing the two-separate-lock-acquisitions gap that previously let a concurrent
    ///     newer attempt install its own <see cref="ConnectionTcs" /> between a caller's check and its
    ///     subsequent capture. Used by <see cref="OnConnectSucceededAsync" />, <see cref="OnConnectFailedAsync" />,
    ///     <see cref="OnDisconnectAsync" />'s own routing, and platform disconnect callbacks that need
    ///     platform-specific failure detail (e.g. Android's <c>OnConnectionStateChange</c>) via
    ///     <see cref="CompleteClaimedConnectFailureAsync" />.
    /// </summary>
    /// <returns>
    ///     The claimed <see cref="TaskCompletionSource" /> if a connect attempt was genuinely pending and
    ///     is now claimed by this call; <c>null</c> if there is nothing to claim - either no attempt is in
    ///     progress (including the automatic/late-connection case where no explicit <see cref="ConnectAsync" />
    ///     caller exists), it was already claimed by another signal, or this signal is stale.
    /// </returns>
    protected TaskCompletionSource? TryClaimPendingConnectAttempt()
    {
        lock (_connectionOperationLock)
        {
            if (!ReferenceEquals(_connectAttemptToken, ConnectionTcs) || ConnectionTcs is not { Task.IsCompleted: false } || _connectAttemptTerminalSignalReceived)
            {
                return null;
            }

            _connectAttemptTerminalSignalReceived = true;
            return ConnectionTcs;
        }
    }

    /// <summary>
    ///     Returns the live <see cref="DisconnectionTcs" /> if it still matches <see cref="_disconnectAttemptToken" />
    ///     (a genuinely current, non-stale disconnect attempt), or <c>null</c> otherwise - see
    ///     <see cref="TryClaimPendingConnectAttempt" />'s remarks for why the same validation applies
    ///     symmetrically to the disconnect side. A mismatch means either no explicit
    ///     <see cref="DisconnectAsync" /> caller exists, or this signal is stale/superseded.
    /// </summary>
    /// <remarks>
    ///     Captures <see cref="DisconnectionTcs" /> into a local exactly once, used for both the
    ///     comparison and the return value - reading the property a second time for the return
    ///     (as an inline <c>DisconnectionTcs is match ? DisconnectionTcs : null</c> would) risked a
    ///     concurrent <see cref="DisconnectAsync" /> replacing it between the two reads, letting a
    ///     check that validated one instance return a completely different one.
    /// </remarks>
    private TaskCompletionSource? TryGetLiveDisconnectionTcs()
    {
        var disconnectionTcs = DisconnectionTcs;
        return ReferenceEquals(_disconnectAttemptToken, disconnectionTcs) ? disconnectionTcs : null;
    }

    /// <summary>
    ///     Completes an already-claimed pending connect attempt (see <see cref="TryClaimPendingConnectAttempt" />)
    ///     as a failure. Exposed as a lower-level primitive (rather than requiring callers to go through
    ///     <see cref="OnConnectFailedAsync" />, which performs its own claim) for platform disconnect
    ///     callbacks that need to attach platform-specific failure detail - e.g. Android's
    ///     <c>OnConnectionStateChange</c> attaching the native GATT status - instead of the generic default
    ///     <see cref="OnDisconnectAsync" /> would otherwise use.
    /// </summary>
    /// <param name="claimedConnectionTcs">The instance returned by a prior, successful <see cref="TryClaimPendingConnectAttempt" /> call.</param>
    /// <param name="e">The exception describing why the connect attempt failed.</param>
    /// <param name="cancellationToken">Token to cancel the refresh operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected ValueTask CompleteClaimedConnectFailureAsync(TaskCompletionSource claimedConnectionTcs, Exception e, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(claimedConnectionTcs);
        LogDeviceConnectionFailed(Id, e);
        return CompleteConnectFailureAsync(claimedConnectionTcs, TryGetLiveDisconnectionTcs(), e, cancellationToken);
    }

    /// <summary>
    ///     Shared refresh-then-complete logic for a connect failure, used by both
    ///     <see cref="OnConnectFailedAsync" /> and <see cref="OnDisconnectAsync" />'s connect-attempt-pending
    ///     routing (and, via <see cref="CompleteClaimedConnectFailureAsync" />, by platform disconnect
    ///     callbacks) - factored out so none of them re-validate a claim that was already made
    ///     atomically by <see cref="TryClaimPendingConnectAttempt" />.
    /// </summary>
    private async ValueTask CompleteConnectFailureAsync(TaskCompletionSource? connectionTcs, TaskCompletionSource? disconnectionTcs, Exception e, CancellationToken cancellationToken)
    {
        // Best-effort: a failed/cancelled refresh must not prevent connectionTcs from being
        // completed with e below - ConnectAsync's "merge concurrent attempts" branch awaits this
        // exact TCS with no timeout of its own, so leaving it uncompleted here would hang that
        // caller forever once the owning call's finally clears the live property.
        Exception? refreshFailure = null;
        try
        {
            await RefreshIsConnectedAsync(CallbackRefreshTimeout, cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // The caller's own requested cancellation is a normal, documented outcome, not a
            // fault - proceed to the TCS completion below silently instead of reporting it as an
            // unhandled Bluetooth exception to every registered listener.
        }
        catch (Exception ex)
        {
            refreshFailure = ex;
        }

        // connectionTcs always faults with e - the connect failure's own reason - regardless of
        // whether the refresh above succeeded; that refresh is only relevant to the disconnect
        // side below.
        var connectionTcsCompleted = connectionTcs?.TrySetException(e) ?? false;

        // The disconnect side isn't failing *because of* this unrelated connect failure - if a
        // live DisconnectAsync call is racing the same moment, its own outcome is independent: a
        // device that's actually disconnected (confirmed by the refresh above) represents a
        // *successful* disconnect regardless of why the concurrent connect attempt failed - mirrors
        // OnDisconnectAsync's own e==null gating. Blindly faulting it with e (as this used to)
        // contradicted that same principle - e.g. Android's OnConnectionStateChange explicitly
        // documents that a non-Success GATT status here must not fault an explicit DisconnectAsync,
        // yet this path did exactly that. If the refresh itself failed, IsConnected can no longer be
        // trusted either way, so fault with that refresh failure instead of guessing.
        var disconnectOutcome = refreshFailure ?? (IsConnected ? new DeviceFailedToDisconnectException(this) : null);
        var disconnectionTcsCompleted = disconnectionTcs?.TrySetResultOrException(disconnectOutcome) ?? false;

        if (!disconnectionTcsCompleted && refreshFailure != null)
        {
            // No live DisconnectAsync caller to deliver the refresh failure to directly - report it
            // so it isn't silently lost.
            ReportBestEffortFailure(refreshFailure);
        }

        if (connectionTcsCompleted || disconnectionTcsCompleted)
        {
            return;
        }

        // Neither TaskCompletionSource was live (e.g. a native callback for an already-abandoned
        // attempt) - report the connect failure too, via ReportBestEffortFailure rather than
        // calling the listener directly: every native callback wraps its call to this method's
        // callers in its own StartAndForget(ex => BluetoothUnhandledExceptionListener...), so
        // letting this rethrow (the listener's documented behavior when nothing is registered)
        // would let that wrapper deliver the same failure to the listener a second time.
        ReportBestEffortFailure(e);
    }

    /// <summary>
    ///     Platform-specific implementation to refresh the current connection state from the native platform.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the refresh operation.</param>
    /// <returns>A task that represents the asynchronous refresh operation.</returns>
    /// <remarks>
    ///     Not every platform can offer a real freshness guarantee: Windows performs a synchronous
    ///     native query, and Apple dispatches to the main thread and awaits completion (not
    ///     synchronous, but still a real, awaited query rather than a fire-and-forget one) - see
    ///     ADR 0003. Android relies entirely on the <c>OnConnectionStateChange</c> callback and only
    ///     clears <see cref="IsConnected" /> here when its GATT proxy is null.
    /// </remarks>
    protected abstract ValueTask NativeRefreshIsConnectedAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Safety-valve bound applied to the refresh inside native-callback-driven completion
    ///     methods (<see cref="OnConnectSucceededAsync" />/<see cref="OnConnectFailedAsync" />/
    ///     <see cref="OnDisconnectAsync" />), which are always invoked with
    ///     <c>cancellationToken: default</c> and have no caller-supplied timeout of their own to
    ///     bound this refresh with otherwise.
    /// </summary>
    /// <remarks>
    ///     Before this change, a stalled Apple main-thread queue here could hang
    ///     <see cref="ConnectAsync" />/<see cref="DisconnectAsync" /> indefinitely even when called
    ///     with their documented default (no timeout) - awaiting this refresh is now on the
    ///     operation-completion path (needed to validate the refreshed state before declaring
    ///     success/failure), unlike the pre-ADR-0003 fire-and-forget refresh this replaced, which
    ///     could never block a caller no matter how long it took. Five seconds is generous for what
    ///     is normally a near-instant local property read (Apple's <c>CBPeripheral.state</c> or a
    ///     synchronous Android/Windows check) - this is a backstop against a genuinely stuck main
    ///     thread, not a normal-path timing constraint.
    /// </remarks>
    private static readonly TimeSpan CallbackRefreshTimeout = TimeSpan.FromSeconds(5);

    /// <summary>
    ///     Awaits <see cref="NativeRefreshIsConnectedAsync" />, bounding it by <paramref name="timeout" />.
    /// </summary>
    /// <param name="timeout">Optional timeout for the refresh.</param>
    /// <param name="cancellationToken">Token to cancel the refresh operation.</param>
    /// <returns>A task that represents the asynchronous, timeout-bounded refresh operation.</returns>
    /// <remarks>
    ///     Wrapping the call in an outer <c>WaitBetterAsync(timeout, cancellationToken)</c> (as every
    ///     call site used to) bounds how long the *caller* waits, but never actually cancels the
    ///     token handed to the platform implementation - so a platform that only reports a late fault
    ///     once its own token is cancelled (see <c>AppleBluetoothRemoteDevice.NativeRefreshIsConnectedAsync</c>)
    ///     never learns that a purely timeout-triggered abandonment happened, and a fault arriving
    ///     after that point goes unreported. Linking <paramref name="timeout" /> directly into the
    ///     token passed down makes a timeout indistinguishable from an explicit cancellation to the
    ///     platform implementation, closing that gap without any platform-specific timeout plumbing.
    /// </remarks>
    private async ValueTask RefreshIsConnectedAsync(TimeSpan? timeout, CancellationToken cancellationToken)
    {
        // null, zero, and negative all mean "no timeout" per WaitBetterAsync's own documented
        // contract (see its callers throughout this repo). Without this, CancelAfter(TimeSpan.Zero)
        // below would schedule near-immediate cancellation - the opposite of what a caller passing
        // TimeSpan.Zero (or Timeout.InfiniteTimeSpan) to ConnectAsync/DisconnectAsync intends.
        if (timeout is not { } timeoutValue || timeoutValue <= TimeSpan.Zero)
        {
            await NativeRefreshIsConnectedAsync(cancellationToken).ConfigureAwait(false);
            return;
        }

        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        linkedCts.CancelAfter(timeoutValue);
        try
        {
            await NativeRefreshIsConnectedAsync(linkedCts.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            // The caller's own cancellationToken was checked above, so linkedCts can only have been
            // cancelled by CancelAfter here - surface this as a timeout rather than an ambiguous
            // OperationCanceledException that callers would otherwise mistake for their own
            // requested cancellation.
            throw new TimeoutException($"{nameof(NativeRefreshIsConnectedAsync)} timed out after {timeoutValue}.");
        }
    }

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
    ///     Whether a native connect-attempt-terminal signal (a "connected" or "connect failed"
    ///     callback) has been *claimed* for the current connect attempt - set atomically alongside
    ///     the claim inside <see cref="TryClaimPendingConnectAttempt" />, not once the claimant
    ///     finishes processing it.
    /// </summary>
    /// <remarks>
    ///     Needed because the claimants (<see cref="OnConnectSucceededAsync" />/<see cref="OnConnectFailedAsync" />/
    ///     <see cref="OnDisconnectAsync" />) await their own refresh after claiming but before completing
    ///     <see cref="ConnectionTcs" />: without this flag, a second signal arriving mid-refresh could
    ///     also pass <see cref="TryClaimPendingConnectAttempt" />'s <c>ConnectionTcs</c>-completion check
    ///     (still incomplete) and race to complete the same <see cref="ConnectionTcs" /> a second time.
    ///     A plain field, not a bindable property: it's set inside <see cref="_connectionOperationLock" />
    ///     (both when claiming and when starting an attempt), and a bindable property's setter would
    ///     synchronously raise the public <see cref="BaseBindableObject.PropertyChanged" /> event to
    ///     arbitrary external subscribers while that lock is held - letting a handler re-enter or
    ///     block connection operations. Nothing outside this class observes this flag's changes.
    /// </remarks>
    private bool _connectAttemptTerminalSignalReceived;

    /// <summary>
    ///     Called when a connection attempt succeeds. Updates the connection state and completes the connection task.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the refresh operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected async ValueTask OnConnectSucceededAsync(CancellationToken cancellationToken = default)
    {
        // Claim atomically before awaiting below - see TryClaimPendingConnectAttempt's remarks.
        // Returns null both for the automatic/late-connection case (no explicit ConnectAsync
        // caller ever installed a token to claim - connectionTcs stays null throughout, and every
        // use below is a safe no-op via ?.) and for a stale signal (superseded by a newer attempt,
        // or retired after this attempt's own ConnectAsync gave up) - either way, this method must
        // not resolve whatever attempt happens to be live *now* instead of the one this signal
        // actually belongs to.
        var connectionTcs = TryClaimPendingConnectAttempt();

        // Best-effort: a failed/cancelled refresh must not prevent the captured TCS below from
        // being completed - ConnectAsync's "merge concurrent attempts" branch awaits this exact
        // TCS with no timeout of its own, so leaving it uncompleted here would hang that caller
        // forever once the owning ConnectAsync's finally clears the live ConnectionTcs property.
        try
        {
            await RefreshIsConnectedAsync(CallbackRefreshTimeout, cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // The caller's own requested cancellation is a normal, documented outcome, not a
            // fault - proceed to the TCS completion below silently instead of reporting it as an
            // unhandled Bluetooth exception to every registered listener.
        }
        catch (Exception refreshException)
        {
            // The refresh itself failed, so IsConnected can no longer be trusted to reflect
            // reality - fail the captured TCS with that failure rather than falling through to
            // report a success we can no longer verify. If a live TCS absorbs it, ConnectAsync's
            // own caller receives it directly via that captured task - no need to also notify
            // BluetoothUnhandledExceptionListener here. If there's no live TCS (e.g. an
            // automatic/late connection with no explicit ConnectAsync caller), just throw and let
            // it reach that listener the one time via the calling native callback's own
            // StartAndForget(onException) wrapper - reporting here too would deliver the same
            // failure to that listener twice.
            if (connectionTcs?.TrySetException(refreshException) ?? false)
            {
                return;
            }

            throw new DeviceFailedToConnectException(this, innerException: refreshException);
        }

        if (IsConnected)
        {
            // The just-awaited refresh confirms the device is actually connected - safe to
            // declare success. connectionTcs may be null/already-completed for an
            // automatic/late connection with no live ConnectAsync caller; nothing more to do
            // in that case either.
            if (connectionTcs?.TrySetResult() ?? false)
            {
                LogDeviceConnected(Id);
            }
            return;
        }

        // The just-awaited refresh confirms the device is NOT actually connected, despite the
        // native "connected" callback that led here - it must have disconnected again
        // immediately. Fault the captured TCS instead of declaring success: a merged caller (see
        // ConnectAsync's merge branch) awaits this exact TCS and returns as soon as it completes,
        // so leaving this to be discovered only by the owning ConnectAsync's own later
        // IsConnected check would let a merged caller observe success for a connection that's
        // already known to be dead.
        if (connectionTcs?.TrySetException(new DeviceFailedToConnectException(this)) ?? false)
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

        // Claim atomically - see OnConnectSucceededAsync/TryClaimPendingConnectAttempt for why.
        var connectionTcs = TryClaimPendingConnectAttempt();

        // Only also look at DisconnectionTcs when this callback genuinely claimed a live connect
        // attempt - not merely when one happens to be live right now. A live, validated
        // DisconnectionTcs is only relevant here for the specific scenario this dual-fail exists
        // for: a disconnect arriving while a connect attempt is pending, itself concurrent with an
        // explicit DisconnectAsync call at that *same* live moment (see CompleteConnectFailureAsync).
        // If the claim above failed - this signal is stale, or there was never a connect attempt at
        // all - there is no such moment to be concurrent with, and touching whatever DisconnectionTcs
        // happens to be live *now* would let a stale, unrelated connect failure fault a genuinely
        // current DisconnectAsync call that has nothing to do with it.
        var disconnectionTcs = connectionTcs != null ? TryGetLiveDisconnectionTcs() : null;

        await CompleteConnectFailureAsync(connectionTcs, disconnectionTcs, e, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async virtual ValueTask ConnectIfNeededAsync(ConnectionOptions? connectionOptions = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        // Delegates entirely to ConnectAsync (which already refreshes before its own
        // already-connected guard) instead of refreshing here too - a caller going through this
        // method would otherwise pay for two main-thread dispatches on Apple for one conditional
        // connect. The only difference from ConnectAsync is silently no-op'ing when already
        // connected instead of throwing.
        try
        {
            await ConnectAsync(connectionOptions, timeout, cancellationToken).ConfigureAwait(false);
        }
        catch (DeviceIsAlreadyConnectedException)
        {
            // Already connected - a no-op for this method, unlike ConnectAsync's explicit throw.
        }
    }

    /// <inheritdoc />
    public async virtual ValueTask ConnectAsync(ConnectionOptions? connectionOptions = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        // Refresh before the already-connected guard below - callers invoking ConnectAsync
        // directly (rather than through ConnectIfNeededAsync, which already refreshes) would
        // otherwise have this guard decide against a stale cached IsConnected value.
        await RefreshIsConnectedAsync(timeout, cancellationToken).ConfigureAwait(false);

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
        // attempt has already replaced it. RunContinuationsAsynchronously matters here because
        // finally completes this TCS while holding _connectionOperationLock (see its own comment) -
        // without it, a merged caller's continuation could resume inline on that same thread, still
        // inside the lock, instead of being scheduled to run after it's released.
        var ownConnectionTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
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
                _connectAttemptTerminalSignalReceived = false; // New attempt - no terminal signal received yet.
                _connectAttemptToken = ownConnectionTcs; // This attempt now owns the native connect call about to be issued below - see TryClaimPendingConnectAttempt.
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

            await RefreshIsConnectedAsync(timeout, cancellationToken).ConfigureAwait(false);
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
            //
            // Only if this attempt still owns _connectAttemptToken: retire it *before* issuing that
            // cancellation - not in the finally below, which only clears it if ConnectionTcs still
            // points at ownConnectionTcs at that later point. A new ConnectAsync call cannot start
            // until this call returns to its own caller (retiring here happens-before that), so a
            // stale native callback for the request just cancelled can never find a *newer*
            // attempt's token here by coincidence - closing the gap ADR 0003 previously left open.
            // See TryClaimPendingConnectAttempt.
            //
            // Ownership can already have moved to a newer attempt by this point even though this
            // method hasn't reached its own finally yet: ownConnectionTcs.Task may have already
            // completed *successfully* (e.g. OnConnectSucceededAsync ran, confirmed the device was
            // genuinely connected, and resolved it) before this method's own subsequent refresh/
            // IsConnected check decided to give up anyway (e.g. the device disconnected again
            // immediately after) - a fresh ConnectAsync call reads a completed ConnectionTcs.Task as
            // "no pending attempt" and installs its own, entirely independently of this method still
            // running. If that happened, the native connect request this method issued is no longer
            // "stuck" (it already resolved) and calling NativeDisconnectAsync unconditionally here
            // would tear down whatever that newer, unrelated attempt has since established instead
            // of retracting anything of this attempt's own.
            bool stillOwnsAttempt;
            lock (_connectionOperationLock)
            {
                stillOwnsAttempt = ReferenceEquals(_connectAttemptToken, ownConnectionTcs);
                if (stillOwnsAttempt)
                {
                    _connectAttemptToken = null;
                }
            }

            if (stillOwnsAttempt)
            {
                try { await NativeDisconnectAsync(timeout: null, cancellationToken: CancellationToken.None).ConfigureAwait(false); }
                catch { /* best-effort - we are already about to report the connection as failed */ }
            }

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
            // Cancelling ownConnectionTcs and clearing ConnectionTcs/_connectAttemptToken (if still
            // owned) must happen under one lock acquisition, not two separate ones - a new
            // ConnectAsync call's own merge-check (also under this lock) reads ConnectionTcs.Task's
            // completion state, which TrySetCanceled below changes. Splitting the cancel from the
            // clear would let that new call's check-and-install run *between* them, atomically as
            // far as this lock is concerned but not as far as the overall cleanup below is - see
            // TryClaimPendingConnectAttempt for why _connectAttemptToken must stay in lockstep with
            // ConnectionTcs at every observable point, not just at the start and end of this method.
            //
            // IsConnecting (a public, bindable property) is deliberately set *outside* this lock,
            // below - its setter synchronously raises PropertyChanged to arbitrary external
            // subscribers, and a handler reacting by calling ConnectAsync/DisconnectAsync again
            // while this thread still held the lock could re-enter or corrupt state still being
            // cleaned up. Safe to defer: nothing in the correlation logic above reads IsConnecting.
            bool stillOwnsAttempt;
            lock (_connectionOperationLock)
            {
                // Guarantee ownConnectionTcs reaches a terminal state no matter how we're leaving
                // this method. A caller that merged onto it above holds a direct reference to its
                // Task, bounded only by *its own* timeout/cancellationToken - if that caller used the
                // documented defaults (no timeout, no cancellation) and this attempt is giving up
                // here (e.g. timeout/cancellation) before any native callback ever completed
                // ownConnectionTcs, clearing the live ConnectionTcs below would strand that merged
                // caller forever, since a later native callback reads whatever's live *then*, not this
                // specific instance. A harmless no-op if it already completed normally.
                // CancellationToken.None here is deliberate: this is a generic "the attempt is over"
                // signal for whichever reason (timeout, cancellation, or native failure), not specifically
                // this method's own cancellationToken.
                ownConnectionTcs.TrySetCanceled(CancellationToken.None);

                // Only reset ConnectionTcs/_connectAttemptToken if this attempt still owns the live
                // operation - a concurrent attempt may have already replaced them for its own
                // in-flight connect (see the merge branch above); resetting unconditionally here
                // would corrupt that newer attempt's state, not just its TCS.
                stillOwnsAttempt = ReferenceEquals(ConnectionTcs, ownConnectionTcs);
                if (stillOwnsAttempt)
                {
                    ConnectionTcs = null;
                    if (ReferenceEquals(_connectAttemptToken, ownConnectionTcs))
                    {
                        _connectAttemptToken = null;
                    }
                }
            }

            if (stillOwnsAttempt)
            {
                IsConnecting = false; // Set the connecting state to false
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
        // A disconnect signal that arrives before any terminal native signal (connected/failed)
        // has been claimed for a pending connect attempt is itself the terminal result of a
        // *failed* connect, not a completed disconnect - the device never actually finished
        // connecting. Route through the connect-failure path instead - otherwise
        // TrySetResultOrException(null) below would complete the still-pending ConnectionTcs as a
        // success. TryClaimPendingConnectAttempt's single lock acquisition makes this check and its
        // capture atomic - Android's OnConnectionStateChange performs the equivalent claim itself
        // (to attach its native GATT status as the failure reason) via the same method, using
        // CompleteClaimedConnectFailureAsync instead of routing through here.
        var connectionTcs = TryClaimPendingConnectAttempt();
        if (connectionTcs != null)
        {
            var connectFailure = e ?? new DeviceFailedToConnectException(this, "Device disconnected while a connection attempt was in progress");
            LogDeviceConnectionFailed(Id, connectFailure);
            // See OnConnectFailedAsync for why DisconnectionTcs must be validated here too, not
            // read raw - this disconnect signal is real-time (it's what triggered this very call),
            // but the connect attempt it's failing may not be, and a genuinely live, validated
            // DisconnectionTcs at this exact instant is still the right thing to also fail.
            await CompleteConnectFailureAsync(connectionTcs, TryGetLiveDisconnectionTcs(), connectFailure, cancellationToken).ConfigureAwait(false);
            return;
        }

        // Validate via TryGetLiveDisconnectionTcs before capturing - see
        // TryClaimPendingConnectAttempt's remarks on why the connect side needs this; the same
        // reasoning applies here. A mismatch (null for an unsolicited disconnect with no explicit
        // DisconnectAsync caller, or non-null but stale/superseded) means there is no live
        // DisconnectionTcs this specific signal may complete - IsConnected is still refreshed below
        // either way, since the device's native state is real regardless of who's tracking it.
        var disconnectionTcs = TryGetLiveDisconnectionTcs();

        // Best-effort: see OnConnectSucceededAsync for why a failed/cancelled refresh must not
        // prevent the captured TCS below from being completed.
        try
        {
            await RefreshIsConnectedAsync(CallbackRefreshTimeout, cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // The caller's own requested cancellation is a normal, documented outcome, not a
            // fault - proceed to the TCS completion below silently instead of reporting it as an
            // unhandled Bluetooth exception to every registered listener.
        }
        catch (Exception refreshException)
        {
            // The refresh itself failed, so IsConnected can no longer be trusted to reflect
            // reality - fail the captured TCS with that failure instead of completing it with e
            // below, which may represent a success outcome we can no longer verify. If a live TCS
            // absorbs it, DisconnectAsync's own caller receives it directly via that captured
            // task - no need to also notify BluetoothUnhandledExceptionListener here (see
            // OnConnectSucceededAsync for why).
            if (disconnectionTcs?.TrySetException(refreshException) ?? false)
            {
                return;
            }

            // No live TCS to deliver this to directly (an unexpected disconnection, e.g. a
            // device rebooting mid-DFU) - this branch doesn't throw, so report directly here;
            // nothing else downstream would otherwise ever notify
            // BluetoothUnhandledExceptionListener about it.
            ReportBestEffortFailure(refreshException);
            OnUnexpectedDisconnection(e);
            return;
        }

        // Attempt to dispatch success/failure to a pending explicit DisconnectAsync await. When e
        // is null (a "clean disconnect" signal), gate success on the just-refreshed IsConnected
        // actually confirming the device is disconnected - a merged caller (see DisconnectAsync's
        // merge branch) awaits this exact TCS and returns as soon as it completes, so declaring
        // success without checking IsConnected could have it observe success for a "disconnect"
        // that, per the refresh just awaited, never actually took effect - something the owning
        // DisconnectAsync call would otherwise only discover moments later via its own
        // post-native refresh/check.
        var tcsOutcome = e ?? (IsConnected ? new DeviceFailedToDisconnectException(this) : null);
        var success = disconnectionTcs?.TrySetResultOrException(tcsOutcome) ?? false;
        if (success)
        {
            // Explicitly requested (someone is awaiting DisconnectAsync/ConnectAsync) - log its
            // outcome directly. This is not an unexpected disconnection, regardless of tcsOutcome.
            if (tcsOutcome == null)
            {
                LogDeviceDisconnected(Id);
            }
            else
            {
                LogDeviceDisconnectionFailed(Id, tcsOutcome);
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
        // Delegates entirely to DisconnectAsync (which already refreshes before its own
        // already-disconnected guard) instead of refreshing here too - see ConnectIfNeededAsync
        // for why. The only difference from DisconnectAsync is silently no-op'ing when already
        // disconnected instead of throwing.
        try
        {
            await DisconnectAsync(timeout, cancellationToken).ConfigureAwait(false);
        }
        catch (DeviceIsAlreadyDisconnectedException)
        {
            // Already disconnected - a no-op for this method, unlike DisconnectAsync's explicit throw.
        }
    }

    /// <inheritdoc />
    public async ValueTask DisconnectAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        // Refresh before the already-disconnected guard below - callers invoking DisconnectAsync
        // directly (rather than through DisconnectIfNeededAsync, which already refreshes) would
        // otherwise have this guard decide against a stale cached IsConnected value.
        await RefreshIsConnectedAsync(timeout, cancellationToken).ConfigureAwait(false);

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
        // concurrent attempt has already replaced it. RunContinuationsAsynchronously matters here
        // for the same reason as ConnectAsync's ownConnectionTcs - finally completes this TCS while
        // holding _connectionOperationLock.
        var ownDisconnectionTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
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
                _disconnectAttemptToken = ownDisconnectionTcs; // This attempt now owns the native disconnect call about to be issued below.
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
            await RefreshIsConnectedAsync(timeout, cancellationToken).ConfigureAwait(false);
            if (IsConnected)
            {
                throw new DeviceFailedToDisconnectException(this);
            }
        }
        finally
        {
            // Cancelling ownDisconnectionTcs and clearing DisconnectionTcs/_disconnectAttemptToken
            // (if still owned) happen under one lock acquisition - see ConnectAsync's finally for
            // why splitting them into two separate lock acquisitions would reopen this same window.
            // IsDisconnecting (public, bindable) is set outside the lock below - see ConnectAsync's
            // finally for why.
            bool stillOwnsAttempt;
            lock (_connectionOperationLock)
            {
                // Guarantee ownDisconnectionTcs reaches a terminal state - see ConnectAsync's finally
                // for why a merged caller using the documented defaults would otherwise be stranded.
                // CancellationToken.None here is deliberate - see ConnectAsync's finally.
                ownDisconnectionTcs.TrySetCanceled(CancellationToken.None);

                // Only reset DisconnectionTcs/_disconnectAttemptToken if this attempt still owns the
                // live operation - a concurrent attempt may have already replaced them for its own
                // in-flight disconnect (see the merge branch above); resetting unconditionally here
                // would corrupt that newer attempt's state, not just its TCS.
                stillOwnsAttempt = ReferenceEquals(DisconnectionTcs, ownDisconnectionTcs);
                if (stillOwnsAttempt)
                {
                    DisconnectionTcs = null;
                    if (ReferenceEquals(_disconnectAttemptToken, ownDisconnectionTcs))
                    {
                        _disconnectAttemptToken = null;
                    }
                }
            }

            if (stillOwnsAttempt)
            {
                IsDisconnecting = false; // Set the disconnecting state to false
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
