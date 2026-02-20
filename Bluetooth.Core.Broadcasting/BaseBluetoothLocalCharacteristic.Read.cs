namespace Bluetooth.Core.Broadcasting;

/// <summary>
///     Base class for Bluetooth broadcast characteristics.
/// </summary>
public abstract partial class BaseBluetoothLocalCharacteristic
{
    /// <summary>
    ///     Called when a read request is received from a client device.
    /// </summary>
    /// <param name="device">The client device that sent the read request.</param>
    /// <returns>The data to be sent in response to the read request.</returns>
    protected ReadOnlySpan<byte> OnReadRequestReceived(IBluetoothConnectedDevice device)
    {
        ArgumentNullException.ThrowIfNull(device);
        LogReadRequest(Id, LocalService.Id, device.Id);
        // TODO : EVENT
        var response = ValueSpan;
        LogReadResponse(Id, LocalService.Id, response.Length);
        return response;
    }
}