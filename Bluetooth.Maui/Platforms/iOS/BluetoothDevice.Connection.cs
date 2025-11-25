using Bluetooth.Maui.PlatformSpecific.Exceptions;

namespace Bluetooth.Maui;

public partial class BluetoothDevice
{
    /// <summary>
    /// Gets or sets the current iOS Core Bluetooth peripheral state.
    /// </summary>
    public CBPeripheralState CbPeripheralState
    {
        get => GetValue(CBPeripheralState.Disconnected);
        protected set => SetValue(value);
    }

    protected override void NativeRefreshIsConnected()
    {
        CbPeripheralState = CbPeripheralDelegateProxy.CbPeripheral.State;
        IsConnected = CbPeripheralState == CBPeripheralState.Connected;
    }

    /// <summary>
    /// Gets or sets the iOS-specific peripheral connection options used when connecting to the device.
    /// </summary>
    public PeripheralConnectionOptions? PeripheralConnectionOptions { get; set; }

    protected override ValueTask NativeConnectAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        NativeRefreshIsConnected();
        var centralManager = ((BluetoothScanner) Scanner).CbCentralManagerProxy;
        ArgumentNullException.ThrowIfNull(centralManager);
        centralManager.CbCentralManager.ConnectPeripheral(CbPeripheralDelegateProxy.CbPeripheral, PeripheralConnectionOptions);
        return ValueTask.CompletedTask;
    }

    protected override ValueTask NativeDisconnectAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        NativeRefreshIsConnected();
        var centralManager = ((BluetoothScanner) Scanner).CbCentralManagerProxy;
        ArgumentNullException.ThrowIfNull(centralManager);
        centralManager.CbCentralManager.CancelPeripheralConnection(CbPeripheralDelegateProxy.CbPeripheral);
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Called when a connection attempt to the peripheral fails on the iOS platform.
    /// </summary>
    /// <param name="error">The error that occurred during the connection attempt.</param>
    /// <exception cref="AppleNativeBluetoothException">Thrown when the error parameter indicates a Bluetooth error.</exception>
    public void FailedToConnectPeripheral(NSError? error)
    {
        NativeRefreshIsConnected();
        try
        {
            AppleNativeBluetoothException.ThrowIfError(error);
            throw new DeviceFailedToConnectException(this, "Failed to connect to peripheral, Unknown error");
        }
        catch (Exception e)
        {
            OnConnectFailed(e);
        }
    }

    /// <summary>
    /// Called when the peripheral disconnects on the iOS platform.
    /// </summary>
    /// <param name="error">The error that occurred during disconnection, or <c>null</c> if disconnection was intentional.</param>
    public void DisconnectedPeripheral(NSError? error)
    {
        NativeRefreshIsConnected();
        try
        {
            AppleNativeBluetoothException.ThrowIfError(error);

            OnDisconnect();
        }
        catch (Exception e)
        {
            OnDisconnect(e);
        }
    }

    /// <summary>
    /// Called when the peripheral successfully connects on the iOS platform.
    /// </summary>
    public void ConnectedPeripheral()
    {
        NativeRefreshIsConnected();
        OnConnectSucceeded();
    }

    /// <summary>
    /// Called when a connection event occurs on the iOS platform.
    /// </summary>
    /// <param name="connectionEvent">The type of connection event that occurred.</param>
    /// <remarks>
    /// Placeholder for future implementation.
    /// </remarks>
    public void ConnectionEventDidOccur(CBConnectionEvent connectionEvent)
    {
        NativeRefreshIsConnected();
    }

    /// <summary>
    /// Called when the ANCS (Apple Notification Center Service) authorization status changes on the iOS platform.
    /// </summary>
    /// <remarks>
    /// Placeholder for future implementation.
    /// </remarks>
    public void DidUpdateAncsAuthorization()
    {
        // Placeholder for future implementation
    }

    /// <summary>
    /// Called when the peripheral disconnects on the iOS platform with additional information.
    /// </summary>
    /// <param name="timestamp">The timestamp of the disconnection event.</param>
    /// <param name="isReconnecting">Indicates whether the system is attempting to reconnect.</param>
    /// <param name="error">The error that occurred during disconnection, or <c>null</c> if disconnection was intentional.</param>
    /// <remarks>
    /// Available on iOS 13.0 and later. Placeholder for future implementation.
    /// </remarks>
    public void DidDisconnectPeripheral(double timestamp, bool isReconnecting, NSError? error)
    {
        // Unclear how we are supposed to use this method ... Apple documentation is not clear
        DisconnectedPeripheral(error);
    }
}
