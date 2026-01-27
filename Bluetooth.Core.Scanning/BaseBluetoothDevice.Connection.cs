using Bluetooth.Abstractions.Scanning;
using Bluetooth.Abstractions.Scanning.EventArgs;
using Bluetooth.Core.Exceptions;
using Bluetooth.Core.Scanning.Exceptions;

using Plugin.BaseTypeExtensions;

namespace Bluetooth.Core.Scanning;

public abstract partial class BaseBluetoothDevice
{
    /// <inheritdoc/>
    public bool IsConnected
    {
        get => GetValue(false);
        set
        {
            if (SetValue(value))
            {
                if (value)
                {
                    Connected?.Invoke(this, System.EventArgs.Empty);
                }
                else
                {
                    Disconnected?.Invoke(this, System.EventArgs.Empty);
                }
            }
        }
    }

    /// <summary>
    /// Waits for the device's connection status to reach the specified state.
    /// </summary>
    /// <param name="isConnected">The desired connection state to wait for.</param>
    /// <param name="timeout">Optional timeout for the operation.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that completes when the device reaches the desired connection state.</returns>
    public async ValueTask WaitForIsConnectedAsync(bool isConnected, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        await WaitForPropertyToBeOfValue(nameof(IsConnected), isConnected, timeout, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Platform-specific implementation to refresh the current connection state from the native platform.
    /// </summary>
    protected abstract void NativeRefreshIsConnected();

    #region Connection - Connect

    /// <summary>
    /// Gets a value indicating whether a connection operation is currently in progress.
    /// </summary>
    public bool IsConnecting
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    /// <inheritdoc/>
    public event EventHandler? Connected;

    /// <inheritdoc/>
    public event EventHandler? Connecting;

    private TaskCompletionSource? ConnectionTcs
    {
        get => GetValue<TaskCompletionSource?>(null);
        set => SetValue(value);
    }

    /// <summary>
    /// Called when a connection attempt succeeds. Updates the connection state and completes the connection task.
    /// </summary>
    protected void OnConnectSucceeded()
    {
        NativeRefreshIsConnected();

        // Attempt to dispatch success to the TaskCompletionSource
        var success = ConnectionTcs?.TrySetResult() ?? false;
        if (success)
        {
            return;
        }

        if(IsConnected)
        {
            return;
        }

        // Else throw an exception
        throw new DeviceFailedToConnectException(this);
    }

    /// <summary>
    /// Called when a connection attempt fails. Completes the connection task with an exception or dispatches to the unhandled exception listener.
    /// </summary>
    /// <param name="e">The exception that occurred during the connection attempt.</param>
    protected void OnConnectFailed(Exception e)
    {
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

    /// <inheritdoc/>
    public async virtual ValueTask ConnectIfNeededAsync(IBluetoothDeviceConnectionOptions connectionOptions, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        NativeRefreshIsConnected();
        if (IsConnected)
        {
            return;
        }

        await ConnectAsync(connectionOptions, timeout, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async virtual Task ConnectAsync(IBluetoothDeviceConnectionOptions connectionOptions, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        // Ensure we are not already connected
        DeviceIsAlreadyConnectedException.ThrowIfAlreadyConnected(this);
        ArgumentNullException.ThrowIfNull(connectionOptions);

        // Prevents multiple calls to ConnectAsync, if already starting, we merge the calls
        if (ConnectionTcs is { Task.IsCompleted: false })
        {
            await ConnectionTcs.Task.ConfigureAwait(false);
            return;
        }

        ConnectionTcs = new TaskCompletionSource(); // Reset the TCS
        IsConnecting = true; // Set the connecting state to true
        Connecting?.Invoke(this, System.EventArgs.Empty);

        try // try-catch to dispatch exceptions rising from start
        {
            if(connectionOptions.WaitForAdvertisementBeforeConnecting)
            {
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
            // Wait for OnConnectSucceeded to be called
            await ConnectionTcs.Task.WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);

            NativeRefreshIsConnected();
            if (!IsConnected)
            {
                throw new DeviceFailedToConnectException(this);
            }
        }
        finally
        {
            IsConnecting = false; // Set the connecting state to false
            ConnectionTcs = null;
        }
    }

    /// <summary>
    /// Platform-specific implementation to initiate a connection to the device.
    /// </summary>
    /// <param name="connectionOptions"></param>
    /// <param name="timeout">Optional timeout for the operation.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    protected abstract ValueTask NativeConnectAsync(IBluetoothDeviceConnectionOptions connectionOptions, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    #endregion

    #region Connection - Disconnect

    /// <summary>
    /// Gets a value indicating whether a disconnection operation is currently in progress.
    /// </summary>
    public bool IsDisconnecting
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    /// <inheritdoc/>
    public event EventHandler? Disconnected;

    /// <inheritdoc/>
    public event EventHandler? Disconnecting;

    private TaskCompletionSource? DisconnectionTcs
    {
        get => GetValue<TaskCompletionSource?>(null);
        set => SetValue(value);
    }

    /// <summary>
    /// Called when a disconnection occurs, either intentionally or unexpectedly. Completes the disconnection task.
    /// </summary>
    /// <param name="e">Optional exception that caused the disconnection.</param>
    protected void OnDisconnect(Exception? e = null)
    {
        NativeRefreshIsConnected();

        // Attempt to dispatch success to the TaskCompletionSource
        var success = (DisconnectionTcs?.TrySetResultOrException(e) ?? false) || (ConnectionTcs?.TrySetResultOrException(e) ?? false);
        if (success)
        {
            return;
        }

        OnUnexpectedDisconnection(e);
    }

    /// <inheritdoc/>
    public async ValueTask DisconnectIfNeededAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        NativeRefreshIsConnected();
        if (!IsConnected)
        {
            return;
        }

        await DisconnectAsync(timeout, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task DisconnectAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        // Ensure we are not already disconnected
        DeviceIsAlreadyDisconnectedException.ThrowIfAlreadyDisconnected(this);

        // Prevents multiple calls to ConnectAsync, if already starting, we merge the calls
        if (DisconnectionTcs is { Task.IsCompleted: false })
        {
            await DisconnectionTcs.Task.ConfigureAwait(false);
            return;
        }

        DisconnectionTcs = new TaskCompletionSource(); // Reset the TCS
        IsDisconnecting = true; // Set the disconnecting state to true
        Disconnecting?.Invoke(this, System.EventArgs.Empty);

        try // try-catch to dispatch exceptions rising from start
        {
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
            await DisconnectionTcs.Task.WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);
            await ClearServicesAsync().ConfigureAwait(false);
            NativeRefreshIsConnected();
            if (IsConnected)
            {
                throw new DeviceFailedToDisconnectException(this);
            }
        }
        finally
        {
            IsDisconnecting = false; // Set the disconnecting state to false
            DisconnectionTcs = null;
        }
    }

    /// <summary>
    /// Platform-specific implementation to initiate a disconnection from the device.
    /// </summary>
    /// <param name="timeout">Optional timeout for the operation.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    protected abstract ValueTask NativeDisconnectAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    #endregion

    #region Connection - UnexpectedDisconnection

    /// <inheritdoc/>
    public event EventHandler<DeviceUnexpectedDisconnectionEventArgs>? UnexpectedDisconnection;

    /// <summary>
    /// Gets or sets a value indicating whether the next unexpected disconnection should be ignored.
    /// </summary>
    public bool IgnoreNextUnexpectedDisconnection { get; set; }

    /// <summary>
    /// Called when an unexpected disconnection occurs. Clears services and raises the UnexpectedDisconnection event.
    /// </summary>
    /// <param name="e">Optional exception that caused the unexpected disconnection.</param>
    protected virtual void OnUnexpectedDisconnection(Exception? e = null)
    {
        ClearServicesAsync().StartAndForget(ex => BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, ex));
        if (IgnoreNextUnexpectedDisconnection)
        {
            IgnoreNextUnexpectedDisconnection = false;
        }
        else // unexpected disconnection
        {
            UnexpectedDisconnection?.Invoke(this, new DeviceUnexpectedDisconnectionEventArgs(e));
        }
    }

    #endregion
}
