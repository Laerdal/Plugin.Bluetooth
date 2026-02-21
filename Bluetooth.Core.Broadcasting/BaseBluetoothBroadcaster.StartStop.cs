namespace Bluetooth.Core.Broadcasting;

public abstract partial class BaseBluetoothBroadcaster
{

    #region Configuration

    /// <inheritdoc />
    public async ValueTask UpdateBroadcastingOptionsAsync(BroadcastingOptions options, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        LogUpdatingConfiguration();

        var old = CurrentBroadcastingOptions;
        try
        {
            if (IsRunning)
            {
                await StopBroadcastingAsync(timeout, cancellationToken).ConfigureAwait(false);
                await StartBroadcastingAsync(options, timeout, cancellationToken).ConfigureAwait(false);
            }

            LogConfigurationUpdated();
        }
        catch (Exception e)
        {
            CurrentBroadcastingOptions = old;
            LogConfigurationUpdateFailed(e);
            throw new BroadcasterConfigurationUpdateFailedException(this, innerException: e);
        }
    }

    #endregion

    #region Configuration

    /// <summary>
    ///     The default broadcasting options used when starting the broadcaster without specifying options.
    ///     This can be overridden by derived classes to provide platform-specific default options.
    /// </summary>
    private static BroadcastingOptions DefaultBroadcastingOptions { get; } = new BroadcastingOptions();

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
    public async ValueTask StartBroadcastingAsync(BroadcastingOptions options, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);

        // Ensure we are not already started
        BroadcasterIsAlreadyStartedException.ThrowIfIsStarted(this);

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
            CurrentBroadcastingOptions = options; // Set the options

            // Handle permissions based on strategy
            await HandlePermissions(options.PermissionStrategy, cancellationToken).ConfigureAwait(false);

            await NativeStartAsync(options, timeout, cancellationToken).ConfigureAwait(false); // actual start native call
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
    /// <param name="strategy">The permission request strategy defined in the broadcasting options.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the permission request operation.</param>
    protected async virtual ValueTask HandlePermissions(PermissionRequestStrategy strategy, CancellationToken cancellationToken)
    {
        switch (strategy)
        {
            case PermissionRequestStrategy.RequestAutomatically:
                await RequestBroadcasterPermissionsAsync(cancellationToken).ConfigureAwait(false);
                break;

            case PermissionRequestStrategy.ThrowIfNotGranted:
                var hasPermissions = await HasBroadcasterPermissionsAsync().ConfigureAwait(false);
                if (!hasPermissions)
                {
                    throw new BluetoothPermissionException("Broadcaster permissions not granted. Call broadcaster.RequestBroadcasterPermissionsAsync() before starting the broadcaster.");
                }
                break;

            case PermissionRequestStrategy.AssumeGranted:
                // Skip all permission checks
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(strategy), strategy, "Invalid permission request strategy.");
        }
    }

    /// <inheritdoc />
    public ValueTask StartBroadcastingIfNeededAsync(BroadcastingOptions options, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        return IsRunning && options == CurrentBroadcastingOptions ? ValueTask.CompletedTask : StartBroadcastingAsync(options, timeout, cancellationToken);
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

    /// <inheritdoc />
    public async ValueTask StopBroadcastingAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        // Ensure we are not already stopped
        BroadcasterIsAlreadyStoppedException.ThrowIfIsStopped(this);

        // Prevents multiple calls to StopAsync, if already stopping, we merge the calls
        if (StopTcs is { Task.IsCompleted: false })
        {
            LogMergingStopAttempts();
            await StopTcs.Task.ConfigureAwait(false);
            return;
        }

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
        return !IsRunning ? ValueTask.CompletedTask : StopBroadcastingAsync(timeout, cancellationToken);
    }

    /// <summary>
    ///     Stops the native Bluetooth broadcaster.
    ///     This method is called by <see cref="StopBroadcastingAsync" /> to perform platform-specific stop operations.
    /// </summary>
    protected abstract ValueTask NativeStopAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    #endregion

}