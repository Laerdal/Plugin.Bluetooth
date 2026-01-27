namespace Bluetooth.Maui.Platforms.Droid.Broadcasting;

/// <inheritdoc/>
public class BluetoothBroadcastClientDeviceFactory : IBluetoothBroadcastClientDeviceFactory
{
    /// <inheritdoc/>
    public ValueTask<IBluetoothBroadcastClientDevice> CreateBroadcastClientDeviceAsync(IBluetoothBroadcaster bluetoothBroadcaster,
        IBluetoothBroadcastClientDeviceFactory.BluetoothBroadcastClientDeviceFactoryRequest request,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
#pragma warning disable CA2000 // Dispose objects before losing scope
        return ValueTask.FromResult<IBluetoothBroadcastClientDevice>(new BluetoothBroadcastClientDevice(bluetoothBroadcaster, request));
#pragma warning restore CA2000 // Dispose objects before losing scope
    }
}
