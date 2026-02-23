namespace Bluetooth.Core.Broadcasting;

/// <summary>
///     Base class for Bluetooth broadcaster devices.
/// </summary>
public abstract partial class BaseBluetoothConnectedDevice
{
    /// <inheritdoc />
    public event EventHandler? Connected;

    /// <inheritdoc />
    public event EventHandler? Disconnected;

    /// <inheritdoc />
    public bool IsConnected { get; protected set; }

    /// <inheritdoc />
    public ValueTask DisconnectAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        LogDeviceDisconnecting(Id);
        return NativeDisconnectAsync(timeout, cancellationToken);
    }


    /// <summary>
    ///     Performs the native disconnection logic for the client device.
    /// </summary>
    protected abstract ValueTask NativeDisconnectAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Called when the connection status of the client device changes.
    /// </summary>
    protected void OnConnectionStatusChanged(bool isConnected)
    {
        if (IsConnected != isConnected)
        {
            LogConnectionStatusChanged(Id, isConnected);
            IsConnected = isConnected;
            OnPropertyChanged(nameof(IsConnected));
            if (IsConnected)
            {
                LogDeviceConnected(Id);
                Connected?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                LogDeviceDisconnected(Id);
                Disconnected?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
