using Bluetooth.Abstractions.Broadcasting;
using Bluetooth.Core.Broadcasting.Exceptions;
using Bluetooth.Core.Exceptions;

using Plugin.BaseTypeExtensions;

namespace Bluetooth.Core.Broadcasting;

public abstract partial class BaseBluetoothBroadcaster
{

    #region Configuration

    /// <inheritdoc/>
    public async Task UpdateBroadcasterOptionsAsync(IBluetoothBroadcasterStartBroadcastingOptions options, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        var old = StartBroadcastingOptions;
        try
        {
            if (IsRunning)
            {
                await StopBroadcastingAsync(timeout, cancellationToken).ConfigureAwait(false);
                await StartBroadcastingAsync(options, timeout, cancellationToken).ConfigureAwait(false);
            }
        }
        catch (Exception e)
        {
            StartBroadcastingOptions = old;
            throw new BroadcasterConfigurationUpdateFailedException(this, innerException: e);
        }
    }

    /// <inheritdoc/>
    public IBluetoothBroadcasterStartBroadcastingOptions StartBroadcastingOptions { get; private set; } = new DefaultBluetoothBroadcasterStartBroadcastingOptions();

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
        RunningStateChanged?.Invoke(this, System.EventArgs.Empty);
    }

    /// <summary>
    /// Refreshes the native running state from the underlying platform.
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
    /// Called when the start operation has succeeded.
    /// Sets the TaskCompletionSource to signal completion of the start operation.
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
        throw new BroadcasterUnexpectedStartException(this);
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
    public async Task StartBroadcastingAsync(IBluetoothBroadcasterStartBroadcastingOptions options, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        // Ensure we are not already started
        BroadcasterIsAlreadyStartedException.ThrowIfIsStarted(this);

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
            StartBroadcastingOptions = options; // Set the options

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
        }
        finally
        {
            IsStarting = false; // Reset the starting state
            Started?.Invoke(this, System.EventArgs.Empty);
            StartTcs = null;
        }
    }

    /// <inheritdoc/>
    public ValueTask StartBroadcastingIfNeededAsync(IBluetoothBroadcasterStartBroadcastingOptions options, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        return (IsRunning && options == StartBroadcastingOptions) ? ValueTask.CompletedTask : new ValueTask(StartBroadcastingAsync(options, timeout, cancellationToken));
    }

    /// <summary>
    /// Starts the native Bluetooth scanner with the specified options.
    /// This method is called by <see cref="StartBroadcastingAsync"/> to perform platform-specific start operations.
    /// </summary>
    protected abstract ValueTask NativeStartAsync(IBluetoothBroadcasterStartBroadcastingOptions options, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

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
        throw new BroadcasterUnexpectedStopException(this);
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
    public async Task StopBroadcastingAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        // Ensure we are not already stopped
        BroadcasterIsAlreadyStoppedException.ThrowIfIsStopped(this);

        // Prevents multiple calls to StopAsync, if already stopping, we merge the calls
        if (StopTcs is { Task.IsCompleted: false })
        {
            await StopTcs.Task.ConfigureAwait(false);
            return;
        }

        StopTcs = new TaskCompletionSource(); // Reset the TCS
        IsStopping = true; // Set the stopping state to true
        Stopping?.Invoke(this, System.EventArgs.Empty);

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
            await StopTcs.Task.WaitBetterAsync(timeout, cancellationToken: cancellationToken).ConfigureAwait(false);

            if (IsRunning)
            {
                throw new BroadcasterFailedToStopException(this);
            }
        }
        finally
        {
            IsStopping = false; // Reset the stopping state
            Stopped?.Invoke(this, System.EventArgs.Empty);
            StopTcs = null;
        }
    }

    /// <inheritdoc/>
    public ValueTask StopBroadcastingIfNeededAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        if (!IsRunning)
        {
            return ValueTask.CompletedTask;
        }

        return new ValueTask(StopBroadcastingAsync(timeout, cancellationToken));
    }

    /// <summary>
    /// Stops the native Bluetooth scanner.
    /// This method is called by <see cref="StopBroadcastingAsync"/> to perform platform-specific stop operations.
    /// </summary>
    protected abstract ValueTask NativeStopAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    #endregion

}
