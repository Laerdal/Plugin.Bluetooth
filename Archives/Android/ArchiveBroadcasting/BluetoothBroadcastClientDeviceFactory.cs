using Bluetooth.Abstractions.Broadcasting.Factories;

namespace Bluetooth.Maui.Platforms.Droid.Broadcasting;

/// <inheritdoc/>
public class BluetoothBroadcastClientDeviceFactory : IBluetoothBroadcastClientDeviceFactory
{
    /// <inheritdoc/>
    public IBluetoothBroadcastClientDevice CreateClientDevice(IBluetoothBroadcaster bluetoothBroadcaster,
        IBluetoothBroadcastClientDeviceFactory.BluetoothBroadcastClientDeviceFactoryRequest request)
    {
        return new BluetoothBroadcastClientDevice(bluetoothBroadcaster, request);
    }
}
