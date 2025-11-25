

namespace Plugin.Bluetooth.BaseClasses;

/// <inheritdoc cref="IBluetoothActivity" />
public abstract class BaseBluetoothActivity : BaseBindableObject, IBluetoothActivity, IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseBluetoothActivity"/> class.
    /// Starts a periodic timer to refresh Bluetooth state values.
    /// </summary>
    protected BaseBluetoothActivity()
    {
        StartPeriodicTimer().StartAndForget(e => BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e));
    }

    /// <summary>
    /// Refreshes all native values from the underlying platform.
    /// Override this method to add additional values that need periodic refreshing.
    /// </summary>
    protected virtual void NativeRefreshAllValues()
    {
        NativeRefreshIsBluetoothOn();
        NativeRefreshIsRunning();
    }

    /// <summary>
    /// Releases the resources used by the <see cref="BaseBluetoothActivity"/>.
    /// </summary>
    /// <param name="disposing">True if called from Dispose; false if called from finalizer.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            StopPeriodicTimer();
        }
    }

    /// <summary>
    /// Initializes the activity.
    /// </summary>
    protected async virtual ValueTask InitializeAsync()
    {
        await NativeInitializeAsync().ConfigureAwait(false);
        NativeRefreshIsBluetoothOn();
    }

    /// <summary>
    /// Performs platform-specific initialization for the Bluetooth activity.
    /// </summary>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    protected abstract ValueTask NativeInitializeAsync();

    /// <summary>
    /// Disposes the Bluetooth manager and releases resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #region IsBluetoothOn

    /// <inheritdoc />
    public bool IsBluetoothOn
    {
        get => GetValue(false);
        set
        {
            if (SetValue(value))
            {
                OnBluetoothStateChanged(new BluetoothStateChangedEventArgs(value));
            }
        }
    }

    /// <summary>
    /// Refreshes the native adapter values asynchronously.
    /// </summary>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    protected abstract void NativeRefreshIsBluetoothOn();

    /// <inheritdoc />
    public event EventHandler<BluetoothStateChangedEventArgs>? BluetoothStateChanged;

    /// <summary>
    /// Raises the <see cref="BluetoothStateChanged"/> event.
    /// </summary>
    protected virtual void OnBluetoothStateChanged(BluetoothStateChangedEventArgs e)
    {
        BluetoothStateChanged?.Invoke(this, e);
    }

    /// <inheritdoc />
    public Task WaitForBluetoothToBeOnAsync(TimeSpan? timeout, CancellationToken cancellationToken = default)
    {
        return WaitForPropertyToBeOfValue(nameof(IsBluetoothOn), true, timeout, cancellationToken).AsTask();
    }

    #endregion

    #region IsRunning

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
    /// Called when the <see cref="IsRunning"/> property changes.
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
    }

    /// <summary>
    /// Refreshes the native running state from the underlying platform.
    /// </summary>
    protected abstract void NativeRefreshIsRunning();

    #endregion

    #region PeriodicTimer

    private PeriodicTimer? PeriodicTimer { get; set; }

    /// <summary>
    ///     Refresh loops cleans device list, checks for disappeared devices and triggers the devices refresh functions as well
    /// </summary>
    private TimeSpan TimeBetweenEachRefreshLoop { get; set; } = TimeSpan.FromSeconds(1);

    private async Task StartPeriodicTimer()
    {
        PeriodicTimer ??= new PeriodicTimer(TimeBetweenEachRefreshLoop);
        while (PeriodicTimer != null && await PeriodicTimer.WaitForNextTickAsync().ConfigureAwait(false))
        {
            // Do something with await
            // if it takes more than TimeBetweenEachRefreshLoop ms then it "skips a beat"
            NativeRefreshAllValues();
        }
    }

    private void StopPeriodicTimer()
    {
        if (PeriodicTimer == null)
        {
            return;
        }

        PeriodicTimer.Dispose();
        PeriodicTimer = null;
    }

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
    /// Called when the start operation has succeeded.
    /// Sets the TaskCompletionSource to signal completion of the start operation.
    /// </summary>
    /// <exception cref="ActivityUnexpectedStartException">Thrown when the activity starts unexpectedly without a pending start operation.</exception>
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
        throw new ActivityUnexpectedStartException(this);
    }

    /// <summary>
    /// Called when the start operation has failed.
    /// Sets the TaskCompletionSource exception or dispatches to the unhandled exception listener.
    /// </summary>
    /// <param name="e">The exception that caused the start to fail.</param>
    protected void OnStartFailed(Exception e)
    {
        // Attempt to dispatch exception to the TaskCompletionSource
        var success = StartTcs?.TrySetException(e) ?? false;
        if (success)
        {
            return;
        }

        // If the TaskCompletionSource was already completed, dispatch the exception to the listener
        BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
    }


    /// <inheritdoc/>
    public virtual ValueTask StartIfNeededAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        return IsRunning ? new ValueTask(StartAsync(timeout, cancellationToken)) : ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task StartAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        // Ensure we are not already started
        ActivityIsAlreadyStartedException.ThrowIfIsStarted(this);

        // Prevents multiple calls to StartAsync, if already starting, we merge the calls
        if (StartTcs is { Task.IsCompleted: false })
        {
            await StartTcs.Task.ConfigureAwait(false);
            return;
        }

        StartTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously); // Reset the TCS
        IsStarting = true; // Set the starting state to true
        Starting?.Invoke(this, System.EventArgs.Empty);

        try // try-catch to dispatch exceptions rising from start through OnStartFailed
        {
            if (!IsBluetoothOn)
            {
                await WaitForBluetoothToBeOnAsync(timeout, cancellationToken).ConfigureAwait(false);
            }
            BluetoothIsOffException.ThrowIfBluetoothIsOff(this);

            await NativeStartAsync(timeout, cancellationToken).ConfigureAwait(false); // actual start native call
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
                throw new ActivityFailedToStartException(this);
            }
        }
        finally
        {
            IsStarting = false; // Reset the starting state
            Started?.Invoke(this, System.EventArgs.Empty);
            StartTcs = null;
        }
    }

    /// <summary>
    /// Starts the native Bluetooth activity with the specified options.
    /// This method is called by <see cref="StartAsync"/> to perform platform-specific start operations.
    /// </summary>
    protected abstract ValueTask NativeStartAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

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
    /// Called when the stop operation has succeeded.
    /// Sets the TaskCompletionSource to signal completion of the stop operation.
    /// </summary>
    /// <exception cref="ActivityUnexpectedStopException">Thrown when the activity stops unexpectedly without a pending stop operation.</exception>
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
        throw new ActivityUnexpectedStopException(this);
    }

    /// <summary>
    /// Called when the stop operation has failed.
    /// Sets the TaskCompletionSource exception or dispatches to the unhandled exception listener.
    /// </summary>
    /// <param name="e">The exception that caused the stop to fail.</param>
    protected void OnStopFailed(Exception e)
    {
        // Attempt to dispatch exception to the TaskCompletionSource
        var success = StopTcs?.TrySetException(e) ?? false;
        if (success)
        {
            return;
        }

        // If the TaskCompletionSource was already completed, dispatch the exception to the listener
        BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
    }

    /// <inheritdoc/>
    public virtual ValueTask StopIfNeededAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        if (!IsRunning)
        {
            return ValueTask.CompletedTask;
        }

        return new ValueTask(StopAsync(timeout, cancellationToken));
    }

    /// <inheritdoc/>
    public async virtual Task StopAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        // Ensure we are not already stopped
        ActivityIsAlreadyStoppedException.ThrowIfIsStopped(this);

        // Prevents multiple calls to StopAsync, if already stopping, we merge the calls
        if (StopTcs is { Task.IsCompleted: false })
        {
            await StopTcs.Task.ConfigureAwait(false);
            return;
        }

        StopTcs = new TaskCompletionSource(); // Reset the TCS
        IsStopping = true; // Set the stopping state to true
        Stopping?.Invoke(this, System.EventArgs.Empty);

        try // try-catch to dispatch exceptions rising from start
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
            await StopTcs.Task.WaitBetterAsync(timeout, cancellationToken: cancellationToken).ConfigureAwait(false);

            if (IsRunning)
            {
                throw new ActivityFailedToStopException(this);
            }
        }
        finally
        {
            IsStopping = false; // Reset the stopping state
            Stopped?.Invoke(this, System.EventArgs.Empty);
            StopTcs = null;
        }
    }

    /// <summary>
    /// Stops the native Bluetooth activity.
    /// This method is called by <see cref="StopAsync"/> to perform platform-specific stop operations.
    /// </summary>
    protected abstract ValueTask NativeStopAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    #endregion

}
