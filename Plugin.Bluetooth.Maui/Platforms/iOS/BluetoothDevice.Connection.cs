using Plugin.Bluetooth.Maui.PlatformSpecific.Exceptions;

namespace Plugin.Bluetooth.Maui;

public partial class BluetoothDevice
{
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

    public PeripheralConnectionOptions? PeripheralConnectionOptions { get; set; }

    protected override void NativeConnect()
    {
        NativeRefreshIsConnected();
        var centralManager = ((BluetoothScanner) Scanner).CbCentralManagerProxy;
        ArgumentNullException.ThrowIfNull(centralManager);
        centralManager.CbCentralManager.ConnectPeripheral(CbPeripheralDelegateProxy.CbPeripheral, PeripheralConnectionOptions);
    }

    protected override void NativeDisconnect()
    {
        NativeRefreshIsConnected();
        var centralManager = ((BluetoothScanner) Scanner).CbCentralManagerProxy;
        ArgumentNullException.ThrowIfNull(centralManager);
        centralManager.CbCentralManager.CancelPeripheralConnection(CbPeripheralDelegateProxy.CbPeripheral);
    }

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

    public void ConnectedPeripheral()
    {
        NativeRefreshIsConnected();
        OnConnectSucceeded();
    }

    public void ConnectionEventDidOccur(CBConnectionEvent connectionEvent)
    {
        NativeRefreshIsConnected();
    }

    public void DidUpdateAncsAuthorization()
    {
        // Placeholder for future implementation
    }

    public void DidDisconnectPeripheral(double timestamp, bool isReconnecting, NSError? error)
    {
        // Unclear how we are supposed to use this method ... Apple documentation is not clear
        DisconnectedPeripheral(error);
    }
}
