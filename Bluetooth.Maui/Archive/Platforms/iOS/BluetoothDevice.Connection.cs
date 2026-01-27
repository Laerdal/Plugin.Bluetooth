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

    /// <inheritdoc/>
    /// <remarks>
    /// On iOS, this checks the <see cref="CBPeripheral.State"/> property of the Core Bluetooth peripheral.
    /// </remarks>
    protected override void NativeRefreshIsConnected()
    {
        CbPeripheralState = CbPeripheralDelegateWrapper.CbPeripheral.State;
        IsConnected = CbPeripheralState == CBPeripheralState.Connected;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// On iOS, this initiates a connection by calling <c>CBCentralManager.ConnectPeripheral</c> with the configured <see cref="PeripheralConnectionOptions"/>.
    /// The connection result is delivered asynchronously via the <see cref="ConnectedPeripheral"/> or <see cref="FailedToConnectPeripheral"/> callbacks.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when the central manager is <c>null</c>.</exception>
    protected override ValueTask NativeConnectAsync(IBluetoothDeviceConnectionOptions connectionOptions, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        NativeRefreshIsConnected();
        if(Scanner is not BluetoothScanner scanner)
        {
            throw new InvalidOperationException("Adapter is not a BluetoothAdapter");
        }

        if(connectionOptions is not BluetoothDeviceConnectionOptions iosConnectionOptions)
        {
            throw new ArgumentException($"Connection options must be of type {nameof(BluetoothDeviceConnectionOptions)} on iOS platform", nameof(connectionOptions));
        }

        ArgumentNullException.ThrowIfNull(scanner.CbCentralManager);
        scanner.CbCentralManager.ConnectPeripheral(CbPeripheralDelegateWrapper.CbPeripheral, iosConnectionOptions.PeripheralConnectionOptions);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// On iOS, this initiates a disconnection by calling <c>CBCentralManager.CancelPeripheralConnection</c>.
    /// The disconnection result is delivered asynchronously via the <see cref="DisconnectedPeripheral"/> callback.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when the central manager is <c>null</c>.</exception>
    protected override ValueTask NativeDisconnectAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        NativeRefreshIsConnected();
        if(Scanner.Adapter is not BluetoothAdapter bluetoothAdapter)
        {
            throw new InvalidOperationException("Adapter is not a BluetoothAdapter");
        }
        ArgumentNullException.ThrowIfNull(bluetoothAdapter.CbCentralManagerWrapper);
        bluetoothAdapter.CbCentralManagerWrapper.CbCentralManager.CancelPeripheralConnection(CbPeripheralDelegateWrapper.CbPeripheral);
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
