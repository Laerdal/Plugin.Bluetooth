namespace Bluetooth.Core.Broadcasting;

/// <summary>
/// Base class for Bluetooth broadcast characteristics.
/// </summary>
public abstract partial class BaseBluetoothLocalCharacteristic
{
    /// <summary>
    /// Called when a write request is received from a client device.
    /// </summary>
    /// <param name="device">The client device that sent the write request.</param>
    /// <param name="data">The data sent in the write request.</param>
    /// <param name="timeout">An optional timeout for the operation.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    protected async ValueTask OnWriteRequestReceivedAsync(IBluetoothConnectedDevice device, byte[] data, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(data);
        LogWriteRequest(Id, LocalService.Id, device.Id, data.Length);
        // TODO : EVENT
        await UpdateValueAsync(data, notifyClients: true, timeout: timeout, cancellationToken: cancellationToken).ConfigureAwait(false);
        LogWriteRequestCompleted(Id, LocalService.Id);
    }
}
