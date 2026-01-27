namespace Bluetooth.Maui.Platforms.DotNetCore.Broadcasting;

/// <inheritdoc/>
public class BluetoothBroadcastClientDevice : BaseBluetoothBroadcastClientDevice
{
    /// <inheritdoc/>
    public BluetoothBroadcastClientDevice(IBluetoothBroadcaster broadcaster, IBluetoothBroadcastClientDeviceFactory.BluetoothBroadcastClientDeviceFactoryRequest request) : base(broadcaster, request)
    {
    }

    /// <inheritdoc/>
    public override ValueTask DisposeAsync()
    {
        throw new NotImplementedException();
    }
}
