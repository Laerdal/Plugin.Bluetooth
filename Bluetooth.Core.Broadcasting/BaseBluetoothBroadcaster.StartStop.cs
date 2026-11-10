namespace Bluetooth.Core.Broadcasting;

public abstract partial class BaseBluetoothBroadcaster
{

    #region Configuration

    /// <summary>
    ///     The default broadcasting options used when starting the broadcaster without specifying options.
    ///     This can be overridden by derived classes to provide platform-specific default options.
    /// </summary>
    protected static BroadcastingOptions DefaultBroadcastingOptions { get; } = new BroadcastingOptions();

    /// <summary>
    ///     The default permission options used when starting the broadcaster without specifying options.
    ///     This can be overridden by derived classes to provide platform-specific default options.
    /// </summary>
    protected static PermissionOptions DefaultPermissionOptions { get; } = new PermissionOptions();

    /// <inheritdoc />
    public BroadcastingOptions CurrentBroadcastingOptions
    {
        get => GetValue(DefaultBroadcastingOptions);
        private set => SetValue(value);
    }

    #endregion

    #region IsRunning

    /// <inheritdoc />
    public event EventHandler? RunningStateChanged;

    /// <inheritdoc />
    public bool IsRunning
    {
        get => GetValue(false);
        protected set
        {
            if (SetValue(value))
            {
                OnIsRunningChanged(value);
            }
        }
    }

    /// <summary>
    ///     Called when the <see cref="IsRunning" /> property changes.
    /// </summary>
    /// <param name="value">The new running state value.</param>
    protected virtual void OnIsRunningChanged(bool value)
    {
        if (value)
        {
            // Started
            OnStartSucceeded();
        }
        else
        {
            // Stopped
            OnStopSucceeded();
        }

        RunningStateChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    ///     Refreshes the native running state from the underlying platform.
    /// </summary>
    protected abstract void NativeRefreshIsRunning();

    #endregion

    #region Start

    /// <inheritdoc />
    public bool IsStarting
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    /// <inheritdoc />
    public event EventHandler? Starting;

    /// <inheritdoc />
    public event EventHandler? Started;

    private TaskCompletionSource? StartTcs
    {
        get => GetValue<TaskCompletionSource?>(null);
        set => SetValue(value);
    }

    /// <summary>
    ///     Called when the start operation has succeeded.
    ///     Sets the TaskCompletionSource to signal completion of the start operation.
    /// </summary>
    /// <exception cref="BroadcasterUnexpectedStartException">Thrown when the broadcaster starts unexpectedly without a pending start operation.</exception>
    protected void OnStartSucceeded()
    {
        // Attempt to dispatch success to the TaskCompletionSource
        var success = StartTcs?.TrySetResult() ?? false;
        if (success)
        {
            return; // expected path
        }

        // If the process is already running, we don't need to do anything
        if (IsRunning)
        {
            return;
        }

        // Else throw an exception
        LogUnexpectedStart();
        throw new BroadcasterUnexpectedStartException(this);
    }

    /// <summary>
    ///     Called when the start operation has failed.
    ///     Sets the TaskCompletionSource exception or dispatches to the unhandled exception listener.
    /// </summary>
    /// <param name="e">The exception that caused the start to fail.</param>
    protected void OnStartFailed(Exception e)
    {
        LogBroadcasterStartFailed(e);

        // Attempt to dispatch exception to the TaskCompletionSource
        var success = StartTcs?.TrySetException(e) ?? false;
        if (success)
        {
            return;
        }

        // If the TaskCompletionSource was already completed, dispatch the exception to the listener
        BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
    }

    /// <inheritdoc />
    public async ValueTask StartBroadcastingAsync(BroadcastingOptions? broadcastingOptions = null,
        PermissionOptions? permissionOptions = null,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        broadcastingOptions ??= DefaultBroadcastingOptions;
        permissionOptions ??= DefaultPermissionOptions;

        // A stop that is still draining has already cleared IsRunning; starting now would run beside it, so wait for it, and look again
        // afterwards.
        bool isRunning;
        while (true)
        {
            var (stopping, running) = GetStopState();
            isRunning = running;
            if (stopping is null)
            {
                break;
            }

            // A stop that failed is its own callers' business, so it is turned into a task that always completes: the only
            // exceptions this wait can throw are this call's own timeout and cancellation.
            var stopped = stopping.ContinueWith(static _ => { }, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
            await WaitForStopAsync(stopped, timeout, cancellationToken).ConfigureAwait(false);
        }

        // Ensure we are not already started
        if (isRunning)
        {
            LogBroadcasterAlreadyStarted();
            throw new BroadcasterIsAlreadyStartedException(this);
        }

        // Prevents multiple calls to StartAsync, if already starting, we merge the calls
        if (StartTcs is { Task.IsCompleted: false })
        {
            LogMergingStartAttempts();
            await StartTcs.Task.ConfigureAwait(false);
            return;
        }

        StartTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously); // Reset the TCS
        IsStarting = true; // Set the starting state to true
        Starting?.Invoke(this, EventArgs.Empty);

        LogBroadcasterStarting();

        try // try-catch to dispatch exceptions rising from start through OnStartFailed
        {
            CurrentBroadcastingOptions = broadcastingOptions; // Set the options

            // Handle permissions based on strategy
            await HandlePermissionsAsync(permissionOptions, cancellationToken).ConfigureAwait(false);

            await NativeStartAsync(broadcastingOptions, timeout, cancellationToken).ConfigureAwait(false); // actual start native call
        }
        catch (Exception e)
        {
            // if exception is thrown during start, we trigger the failure
            OnStartFailed(e);
        }

        // try-finally to ensure disposal and release of resources
        try
        {
            // Wait for OnStartSucceeded to be called
            await StartTcs.Task.WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);

            if (!IsRunning)
            {
                throw new BroadcasterFailedToStartException(this);
            }

            LogBroadcasterStarted();
        }
        finally
        {
            IsStarting = false; // Reset the starting state
            Started?.Invoke(this, EventArgs.Empty);
            StartTcs = null;
        }
    }

    /// <summary>
    ///     Handles permission requests based on the specified strategy in the broadcasting options.
    ///     This method is called by <see cref="StartBroadcastingAsync" /> before starting the broadcaster to ensure necessary permissions are granted.
    /// </summary>
    /// <param name="permissionOptions">The permission options containing the strategy for handling permissions.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the permission request operation.</param>
    protected async virtual ValueTask HandlePermissionsAsync(PermissionOptions permissionOptions, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(permissionOptions);
        switch (permissionOptions.PermissionStrategy)
        {
            case PermissionRequestStrategy.RequestAutomatically:
                await RequestBroadcasterPermissionsAsync(cancellationToken).ConfigureAwait(false);
                break;

            case PermissionRequestStrategy.ThrowIfNotGranted:
                var hasPermissions = await HasBroadcasterPermissionsAsync().ConfigureAwait(false);
                if (!hasPermissions)
                {
                    throw new
                        BluetoothPermissionException("Broadcaster permissions not granted. Call broadcaster.RequestBroadcasterPermissionsAsync() before starting the broadcaster.");
                }
                break;

            case PermissionRequestStrategy.AssumeGranted:
                // Skip all permission checks
                break;

            default:
                throw new ArgumentException($"Unsupported permission strategy: {permissionOptions.PermissionStrategy}");
        }
    }

    /// <inheritdoc />
    public ValueTask StartBroadcastingIfNeededAsync(BroadcastingOptions? broadcastingOptions = null,
        PermissionOptions? permissionOptions = null,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        broadcastingOptions ??= DefaultBroadcastingOptions;
        permissionOptions ??= DefaultPermissionOptions;
        return IsRunning && broadcastingOptions == CurrentBroadcastingOptions ?
                   ValueTask.CompletedTask :
                   StartBroadcastingAsync(broadcastingOptions, permissionOptions, timeout, cancellationToken);
    }

    /// <summary>
    ///     Starts the native Bluetooth broadcaster with the specified options.
    ///     This method is called by <see cref="StartBroadcastingAsync" /> to perform platform-specific start operations.
    /// </summary>
    protected abstract ValueTask NativeStartAsync(BroadcastingOptions options, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    #endregion

    #region Stop

    /// <inheritdoc />
    public bool IsStopping
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    /// <inheritdoc />
    public event EventHandler? Stopping;

    /// <inheritdoc />
    public event EventHandler? Stopped;

    private TaskCompletionSource? StopTcs
    {
        get => GetValue<TaskCompletionSource?>(null);
        set => SetValue(value);
    }

    /// <summary>
    ///     Called when the stop operation has succeeded.
    ///     Sets the TaskCompletionSource to signal completion of the stop operation.
    /// </summary>
    /// <exception cref="BroadcasterUnexpectedStopException">Thrown when the scanner stops unexpectedly without a pending stop operation.</exception>
    protected void OnStopSucceeded()
    {
        // Attempt to dispatch success to the TaskCompletionSource
        var success = StopTcs?.TrySetResult() ?? false;
        if (success)
        {
            return; // expected path
        }

        // If the process is already stopped, we don't need to do anything
        if (!IsRunning)
        {
            return;
        }

        // Else throw an exception
        LogUnexpectedStop();
        throw new BroadcasterUnexpectedStopException(this);
    }

    /// <summary>
    ///     Called when the stop operation has failed.
    ///     Sets the TaskCompletionSource exception or dispatches to the unhandled exception listener.
    /// </summary>
    /// <param name="e">The exception that caused the stop to fail.</param>
    protected void OnStopFailed(Exception e)
    {
        LogBroadcasterStopFailed(e);

        // Attempt to dispatch exception to the TaskCompletionSource
        var success = StopTcs?.TrySetException(e) ?? false;
        if (success)
        {
            return;
        }

        // If the TaskCompletionSource was already completed, dispatch the exception to the listener
        BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
    }

    private readonly object _stopGate = new();

    /// <summary>
    ///     Completes when the whole stop operation is over: the native stop, the wait for the platform to report it, and the
    ///     bookkeeping that follows. <see cref="StopTcs" /> only says that the platform reported "stopped", which can come
    ///     before that, so every caller waits on this one instead.
    /// </summary>
    private TaskCompletionSource? _stopOperation;

    /// <summary>
    ///     The stop in flight, if any, and whether the broadcaster runs, read together under <see cref="_stopGate" />. A stop publishes its
    ///     operation under the same gate before it clears IsRunning, so it cannot begin between the two reads: a broadcaster that no longer
    ///     runs is never seen without the stop that is still going on, and a stop that was in flight is never seen as finished with
    ///     the broadcaster still running.
    /// </summary>
    private (Task? Stopping, bool IsRunning) GetStopState()
    {
        lock (_stopGate)
        {
            var stopping = _stopOperation is { Task.IsCompleted: false } operation ? operation.Task : null;
            var isRunning = IsRunning;
            return (stopping, isRunning);
        }
    }

    /// <summary>
    ///     Waits for a stop that someone else started, for at most <paramref name="timeout" /> and until <paramref name="cancellationToken" />
    ///     is cancelled. Unlike <c>WaitBetterAsync</c>, which ignores the token when there is no timeout, a caller can always cancel here.
    /// </summary>
    private static Task WaitForStopAsync(Task stop, TimeSpan? timeout, CancellationToken cancellationToken)
    {
        return timeout is { } limit ? stop.WaitAsync(limit, cancellationToken) : stop.WaitAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask StopBroadcastingAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        TaskCompletionSource operation;
        bool owner;
        lock (_stopGate)
        {
            if (_stopOperation is { Task.IsCompleted: false } running)
            {
                // Prevents multiple calls to StopAsync, if already stopping, we merge the calls
                operation = running;
                owner = false;
            }
            else if (!IsRunning)
            {
                // Ensure we are not already stopped
                LogBroadcasterAlreadyStopped();
                throw new BroadcasterIsAlreadyStoppedException(this);
            }
            else
            {
                operation = _stopOperation = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
                owner = true;
            }
        }

        if (!owner)
        {
            LogMergingStopAttempts();
            await WaitForStopAsync(operation.Task, timeout, cancellationToken).ConfigureAwait(false);
            return;
        }

        try
        {
            await StopCoreAsync(timeout, cancellationToken).ConfigureAwait(false);
            operation.TrySetResult();
        }
        catch (Exception e)
        {
            operation.TrySetException(e);
            _ = operation.Task.Exception; // observed here: nobody else may be waiting on it
            throw;
        }
    }

    private async ValueTask StopCoreAsync(TimeSpan? timeout, CancellationToken cancellationToken)
    {
        StopTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously); // Reset the TCS
        IsStopping = true; // Set the stopping state to true
        Stopping?.Invoke(this, EventArgs.Empty);

        LogBroadcasterStopping();

        try // try-catch to dispatch exceptions rising from stop
        {
            await NativeStopAsync(timeout, cancellationToken).ConfigureAwait(false); // actual stop native call
        }
        catch (Exception e)
        {
            OnStopFailed(e); // if exception is thrown during stop, we trigger the failure
        }

        // try-finally to ensure disposal and release of resources
        try
        {
            // Wait for OnStopSucceeded to be called
            await StopTcs.Task.WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);

            if (IsRunning)
            {
                throw new BroadcasterFailedToStopException(this);
            }

            LogBroadcasterStopped();
        }
        finally
        {
            IsStopping = false; // Reset the stopping state
            Stopped?.Invoke(this, EventArgs.Empty);
            StopTcs = null;
        }
    }

    /// <inheritdoc />
    public ValueTask StopBroadcastingIfNeededAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        // A stop that is still draining has already cleared IsRunning, but the broadcaster is not stopped yet.
        var (inFlight, isRunning) = GetStopState();
        if (inFlight is not null)
        {
            LogMergingStopAttempts();
            return new ValueTask(WaitForStopAsync(inFlight, timeout, cancellationToken));
        }

        return !isRunning ? ValueTask.CompletedTask : StopWhileRunningAsync(timeout, cancellationToken);
    }

    /// <summary>
    ///     Stops a broadcaster that was running when <see cref="StopBroadcastingIfNeededAsync" /> looked. If another caller finished stopping it
    ///     in the meantime there is nothing left to do, and "if needed" must not throw for that.
    /// </summary>
    private async ValueTask StopWhileRunningAsync(TimeSpan? timeout, CancellationToken cancellationToken)
    {
        try
        {
            await StopBroadcastingAsync(timeout, cancellationToken).ConfigureAwait(false);
        }
        catch (BroadcasterIsAlreadyStoppedException)
        {
        }
    }

    /// <summary>
    ///     Stops the native Bluetooth broadcaster.
    ///     This method is called by <see cref="StopBroadcastingAsync" /> to perform platform-specific stop operations.
    /// </summary>
    protected abstract ValueTask NativeStopAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    #endregion

}
