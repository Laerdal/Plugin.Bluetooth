using Bluetooth.Maui.PlatformSpecific.Exceptions;

namespace Bluetooth.Maui;

public partial class BluetoothDevice
{
    #region Connection - GattSessionStatus

    public GattSessionStatus GattSessionStatus
    {
        get => GetValue(GattSessionStatus.Closed);
        private set => SetValue(value);
    }

    public ValueTask WaitForGattSessionStatusAsync(GattSessionStatus state, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        return WaitForPropertyToBeOfValue(nameof(GattSessionStatus), state, timeout, cancellationToken);
    }

    public void OnGattSessionStatusChanged(GattSessionStatus argsStatus)
    {
        GattSessionStatus = argsStatus;
    }

    #endregion

    #region Connection - BluetoothConnectionStatus

    public BluetoothConnectionStatus BluetoothConnectionStatus
    {
        get => GetValue(BluetoothConnectionStatus.Disconnected);
        private set => SetValue(value);
    }

    public ValueTask WaitForBluetoothConnectionStatusAsync(BluetoothConnectionStatus state, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        return WaitForPropertyToBeOfValue(nameof(BluetoothConnectionStatus), state, timeout, cancellationToken);
    }

    public void OnConnectionStatusChanged(BluetoothConnectionStatus newConnectionStatus)
    {
        BluetoothConnectionStatus = newConnectionStatus;
        switch (BluetoothConnectionStatus)
        {
            case BluetoothConnectionStatus.Connected:
                OnConnectSucceeded();
                break;
            case BluetoothConnectionStatus.Disconnected:
                OnDisconnect();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newConnectionStatus), newConnectionStatus, null);
        }
    }

    #endregion

    protected override void NativeRefreshIsConnected()
    {
        IsConnected = BluetoothLeDeviceProxy?.BluetoothLeDevice is { ConnectionStatus: BluetoothConnectionStatus.Connected };
    }

    protected async override ValueTask NativeConnectAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        NativeRefreshIsConnected();

        try
        {
            ArgumentNullException.ThrowIfNull(BluetoothLeDeviceProxy);
            BluetoothLeDeviceProxy = await PlatformSpecific.BluetoothLeDeviceProxy.GetInstanceAsync(BluetoothLeDeviceProxy.BluetoothLeDevice.BluetoothAddress, this, cancellationToken).ConfigureAwait(false);
            CachedName = BluetoothLeDeviceProxy.BluetoothLeDevice.Name;
            GattSessionProxy = await PlatformSpecific.GattSessionProxy.GetInstanceAsync(BluetoothLeDeviceProxy.BluetoothLeDevice, this, cancellationToken).ConfigureAwait(false);
            if (GattSessionProxy.GattSession.CanMaintainConnection)
            {
                GattSessionProxy.GattSession.MaintainConnection = true;
            }

            await WaitForGattSessionStatusAsync(GattSessionStatus.Active, timeout, cancellationToken).ConfigureAwait(false);

            var accessStatus = await BluetoothLeDeviceProxy.BluetoothLeDevice.RequestAccessAsync().AsTask(cancellationToken).ConfigureAwait(false);
            if (accessStatus != DeviceAccessStatus.Allowed)
            {
                throw new WindowsNativeBluetoothException(accessStatus);
            }

            await BluetoothLeDeviceProxy.ReadGattServicesAsync(BluetoothCacheMode.Uncached, cancellationToken: cancellationToken).ConfigureAwait(false); // HACK: kick the connection on
        }
        catch (Exception e)
        {
            OnConnectFailed(e);
        }
    }

    protected async override ValueTask NativeDisconnectAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        NativeRefreshIsConnected();
        try
        {
            await ClearServicesAsync().ConfigureAwait(false);

            BluetoothLeDeviceProxy?.Dispose();
            BluetoothLeDeviceProxy = null;

            if (GattSessionProxy != null)
            {
                if (GattSessionProxy.GattSession.CanMaintainConnection)
                {
                    GattSessionProxy.GattSession.MaintainConnection = false;
                }

                await WaitForGattSessionStatusAsync(GattSessionStatus.Closed, timeout, cancellationToken).ConfigureAwait(false);
                GattSessionProxy.Dispose();
                GattSessionProxy = null;
            }

            OnDisconnect();
        }
        catch (Exception e)
        {
            OnDisconnect(e);
        }
    }

}
