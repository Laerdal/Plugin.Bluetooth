using Bluetooth.Maui.PlatformSpecific;
using Bluetooth.Maui.PlatformSpecific.Exceptions;
using Bluetooth.Maui.PlatformSpecific.NativeOptions;

namespace Bluetooth.Maui;

public partial class BluetoothDevice
{
    /// <summary>
    /// Gets or sets the current Android GATT profile connection state.
    /// </summary>
    public ProfileState ProfileState
    {
        get => GetValue(ProfileState.Disconnected);
        protected set => SetValue(value);
    }

    protected override void NativeRefreshIsConnected()
    {
        ProfileState = BluetoothManagerProxy.BluetoothManager.GetConnectionState(NativeDevice, ProfileType.Gatt);
        IsConnected = ProfileState == ProfileState.Connected;
    }

    /// <summary>
    /// Gets or sets the Android-specific connection options used when connecting to the device.
    /// </summary>
    public ConnectionOptions? ConnectionOptions { get; set; }

    protected override ValueTask NativeConnectAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        if (BluetoothGattProxy is null)
        {
            BluetoothGattProxy = new BluetoothGattProxy(this, ConnectionOptions, NativeDevice);
        }
        else
        {
            BluetoothGattProxy.Reconnect();
        }

        return ValueTask.CompletedTask;
    }

    private void OnConnectionStateChangedToConnected(GattStatus status)
    {
        try
        {
            AndroidNativeGattCallbackStatusConnectionException.ThrowIfNotSuccess((GattCallbackStatusConnection) status);
            OnConnectSucceeded();
        }
        catch (Exception e)
        {
            OnConnectFailed(e);
        }
    }

    protected override ValueTask NativeDisconnectAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        // Ensure BluetoothGatt exists and is available
        ArgumentNullException.ThrowIfNull(BluetoothGattProxy);
        BluetoothGattProxy.BluetoothGatt.Disconnect();
        return ValueTask.CompletedTask;
    }

    private void OnConnectionStateChangedToDisconnected(GattStatus status)
    {
        try
        {
            AndroidNativeGattCallbackStatusConnectionException.ThrowIfNotSuccess(status);
            OnDisconnect();
        }
        catch (Exception e)
        {
            OnDisconnect(e);
        }
        finally
        {
            BluetoothGattProxy?.Dispose();
            BluetoothGattProxy = null;
        }
    }

    /// <summary>
    /// Called when the GATT connection state changes on the Android platform.
    /// </summary>
    /// <param name="status">The status of the connection state change.</param>
    /// <param name="newState">The new connection state.</param>
    public void OnConnectionStateChange(GattStatus status, ProfileState newState)
    {
        ProfileState = newState;
        switch (newState)
        {
            case ProfileState.Connected:
                OnConnectionStateChangedToConnected(status);
                break;
            case ProfileState.Disconnected:
                OnConnectionStateChangedToDisconnected(status);
                break;
        }
    }

}
