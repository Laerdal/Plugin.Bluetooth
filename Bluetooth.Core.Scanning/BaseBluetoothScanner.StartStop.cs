namespace Bluetooth.Core.Scanning;

public abstract partial class BaseBluetoothScanner
{
    #region Configuration

    /// <inheritdoc />
    public async ValueTask UpdateScannerOptionsAsync(ScanningOptions options, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        var old = CurrentScanningOptions;
        try
        {
            LogUpdatingScannerConfiguration();
            if (IsRunning)
            {
                await StopScanningAsync(timeout, cancellationToken).ConfigureAwait(false);
                await StartScanningAsync(options, timeout, cancellationToken).ConfigureAwait(false);
            }
        }
        catch (Exception e)
        {
            CurrentScanningOptions = old;
            LogScannerConfigurationUpdateFailed(e);
            throw new ScannerConfigurationUpdateFailedException(this, innerException: e);
        }
    }

    #endregion

    #region Configuration

    /// <summary>
    ///     Gets the default scanning options used when starting the scanner without specifying options.
    /// </summary>
    public static ScanningOptions DefaultScanningOptions { get; } = new();

    /// <inheritdoc />
    public ScanningOptions CurrentScanningOptions
    {
        get => GetValue(DefaultScanningOptions);
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
    /// <exception cref="ScannerUnexpectedStartException">Thrown when the scanner starts unexpectedly without a pending start operation.</exception>
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
        LogScannerUnexpectedStart();
        throw new ScannerUnexpectedStartException(this);
    }

    /// <summary>
    ///     Called when the start operation has failed.
    ///     Sets the Task Completion Source exception or dispatches to the unhandled exception listener.
    /// </summary>
    /// <param name="e">The exception that caused the start to fail.</param>
    protected void OnStartFailed(Exception e)
    {
        LogScannerStartFailed(e);

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
    public ValueTask StartScanningIfNeededAsync(ScanningOptions options, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        return IsRunning && options == CurrentScanningOptions ? ValueTask.CompletedTask : new ValueTask(StartScanningAsync(options, timeout, cancellationToken));
    }

    /// <inheritdoc />
    public async Task StartScanningAsync(ScanningOptions options, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);

        // Ensure we are not already started
        if (IsRunning)
        {
            LogScannerAlreadyStarted();
            throw new ScannerIsAlreadyStartedException(this);
        }

        // Prevents multiple calls to StartAsync, if already starting, we merge the calls
        if (StartTcs is { Task.IsCompleted: false })
        {
            LogMergingStartOperation();
            await StartTcs.Task.ConfigureAwait(false);
            return;
        }

        StartTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously); // Reset the TCS
        IsStarting = true; // Set the starting state to true
        Starting?.Invoke(this, EventArgs.Empty);

        try // try-catch to dispatch exceptions rising from start through OnStartFailed
        {
            CurrentScanningOptions = options; // Set the configuration

            // Handle permissions based on strategy
            await HandlePermissionsAsync(options.PermissionStrategy, options.RequireBackgroundLocation, cancellationToken).ConfigureAwait(false);

            LogScannerStarting(options.ServiceUuids?.Count ?? 0);
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
                throw new ScannerFailedToStartException(this);
            }
        }
        finally
        {
            IsStarting = false; // Reset the starting state
            Started?.Invoke(this, EventArgs.Empty);
            StartTcs = null;
            if (IsRunning)
            {
                LogScannerStarted();
            }
        }
    }

    /// <summary>
    ///     Handles permissions based on the specified strategy.
    ///     This method is called by <see cref="StartScanningAsync" /> to manage permissions before starting the scanner.
    /// </summary>
    /// <param name="permissionStrategy">The strategy to use for handling permissions.</param>
    /// <param name="requireBackgroundLocation">Indicates whether background location permission is required.</param>
    /// <param name="cancellationToken">An optional cancellation token to cancel the permission handling operation.</param>
    protected async virtual Task HandlePermissionsAsync(
        PermissionRequestStrategy permissionStrategy,
        bool requireBackgroundLocation, CancellationToken cancellationToken = default)
    {
        switch (permissionStrategy)
        {
            case PermissionRequestStrategy.RequestAutomatically:
                await RequestScannerPermissionsAsync(requireBackgroundLocation, cancellationToken).ConfigureAwait(false);
                break;

            case PermissionRequestStrategy.ThrowIfNotGranted:
                var hasPermissions = await HasScannerPermissionsAsync().ConfigureAwait(false);
                if (!hasPermissions)
                {
                    throw new BluetoothPermissionException(
                                                           "Scanner permissions not granted. Call scanner.RequestScannerPermissionsAsync() before starting the scanner.");
                }
                break;

            case PermissionRequestStrategy.AssumeGranted:
                // Skip all permission checks
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(permissionStrategy), permissionStrategy, "Invalid permission request strategy.");
        }
    }

    /// <summary>
    ///     Starts the native Bluetooth scanner with the specified options.
    ///     This method is called by <see cref="StartScanningAsync" /> to perform platform-specific start operations.
    /// </summary>
    /// <param name="scanningOptions">The scanning options to use when starting the scanner.</param>
    /// <param name="timeout">An optional timeout for the start operation.</param>
    /// <param name="cancellationToken">An optional cancellation token to cancel the start operation.</param>
    protected abstract ValueTask NativeStartAsync(ScanningOptions scanningOptions, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

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
    /// <exception cref="ScannerUnexpectedStopException">Thrown when the scanner stops unexpectedly without a pending stop operation.</exception>
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
        LogScannerUnexpectedStop();
        throw new ScannerUnexpectedStopException(this);
    }

    /// <summary>
    ///     Called when the stop operation has failed.
    ///     Sets the TaskCompletionSource exception or dispatches to the unhandled exception listener.
    /// </summary>
    /// <param name="e">The exception that caused the stop to fail.</param>
    protected void OnStopFailed(Exception e)
    {
        LogScannerStopFailed(e);

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
    public virtual ValueTask StopScanningIfNeededAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        if (!IsRunning)
        {
            return ValueTask.CompletedTask;
        }

        return new ValueTask(StopScanningAsync(timeout, cancellationToken));
    }

    /// <inheritdoc />
    public async virtual Task StopScanningAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        // Ensure we are not already stopped
        if (!IsRunning)
        {
            LogScannerAlreadyStopped();
            throw new ScannerIsAlreadyStoppedException(this);
        }

        // Prevents multiple calls to StopAsync, if already stopping, we merge the calls
        if (StopTcs is { Task.IsCompleted: false })
        {
            LogMergingStopOperation();
            await StopTcs.Task.ConfigureAwait(false);
            return;
        }

        StopTcs = new TaskCompletionSource(); // Reset the TCS
        IsStopping = true; // Set the stopping state to true
        Stopping?.Invoke(this, EventArgs.Empty);

        try // try-catch to dispatch exceptions rising from start
        {
            LogScannerStopping();
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
                throw new ScannerFailedToStopException(this);
            }
        }
        finally
        {
            IsStopping = false; // Reset the stopping state
            Stopped?.Invoke(this, EventArgs.Empty);
            StopTcs = null;
            if (!IsRunning)
            {
                LogScannerStopped();
            }
        }
    }

    /// <summary>
    ///     Stops the native Bluetooth scanner.
    ///     This method is called by <see cref="StopScanningAsync" /> to perform platform-specific stop operations.
    /// </summary>
    protected abstract ValueTask NativeStopAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    #endregion
}