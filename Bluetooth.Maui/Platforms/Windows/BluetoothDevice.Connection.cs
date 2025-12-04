using Bluetooth.Maui.PlatformSpecific.Exceptions;

namespace Bluetooth.Maui;

public partial class BluetoothDevice
{
    #region Connection - GattSessionStatus

    /// <summary>
    /// Gets or sets the current Windows GATT session status.
    /// </summary>
    public GattSessionStatus GattSessionStatus
    {
        get => GetValue(GattSessionStatus.Closed);
        private set => SetValue(value);
    }

    /// <summary>
    /// Waits for the GATT session status to reach a specific state.
    /// </summary>
    /// <param name="state">The desired GATT session status.</param>
    /// <param name="timeout">Optional timeout for the operation.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that completes when the desired state is reached.</returns>
    public ValueTask WaitForGattSessionStatusAsync(GattSessionStatus state, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        return WaitForPropertyToBeOfValue(nameof(GattSessionStatus), state, timeout, cancellationToken);
    }

    /// <summary>
    /// Called when the GATT session status changes on the Windows platform.
    /// </summary>
    /// <param name="argsStatus">The new GATT session status.</param>
    public void OnGattSessionStatusChanged(GattSessionStatus argsStatus)
    {
        GattSessionStatus = argsStatus;
    }

    #endregion

    #region Connection - BluetoothConnectionStatus

    /// <summary>
    /// Gets or sets the current Windows Bluetooth connection status.
    /// </summary>
    public BluetoothConnectionStatus BluetoothConnectionStatus
    {
        get => GetValue(BluetoothConnectionStatus.Disconnected);
        private set => SetValue(value);
    }

    /// <summary>
    /// Waits for the Bluetooth connection status to reach a specific state.
    /// </summary>
    /// <param name="state">The desired Bluetooth connection status.</param>
    /// <param name="timeout">Optional timeout for the operation.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that completes when the desired state is reached.</returns>
    public ValueTask WaitForBluetoothConnectionStatusAsync(BluetoothConnectionStatus state, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        return WaitForPropertyToBeOfValue(nameof(BluetoothConnectionStatus), state, timeout, cancellationToken);
    }

    /// <summary>
    /// Called when the Bluetooth connection status changes on the Windows platform.
    /// </summary>
    /// <param name="newConnectionStatus">The new connection status.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the connection status is not a recognized value.</exception>
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

    /// <inheritdoc/>
    /// <remarks>
    /// On Windows, this checks the <c>BluetoothLeDevice.ConnectionStatus</c> property to determine if the device is connected.
    /// </remarks>
    protected override void NativeRefreshIsConnected()
    {
        IsConnected = BluetoothLeDeviceProxy?.BluetoothLeDevice is { ConnectionStatus: BluetoothConnectionStatus.Connected };
    }

    /// <inheritdoc/>
    /// <remarks>
    /// On Windows, this establishes a connection by creating a <c>BluetoothLeDeviceProxy</c> and <c>GattSessionProxy</c>,
    /// requesting device access, and initiating GATT service discovery. The GATT session is configured to maintain the connection.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when the BluetoothLeDeviceProxy is <c>null</c>.</exception>
    /// <exception cref="WindowsNativeBluetoothException">Thrown when device access is denied.</exception>
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

    /// <inheritdoc/>
    /// <remarks>
    /// On Windows, this disconnects by clearing all services, disposing the <c>BluetoothLeDeviceProxy</c>,
    /// and releasing the GATT session. The GATT session's <c>MaintainConnection</c> property is set to <c>false</c> before disposal.
    /// </remarks>
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
