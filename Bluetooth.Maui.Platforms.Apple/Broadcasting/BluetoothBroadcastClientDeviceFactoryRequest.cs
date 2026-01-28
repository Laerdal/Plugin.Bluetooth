namespace Bluetooth.Maui.Platforms.Apple.Broadcasting;

/// <inheritdoc/>
public record BluetoothBroadcastClientDeviceFactoryRequest : IBluetoothBroadcastClientDeviceFactory.BluetoothBroadcastClientDeviceFactoryRequest
{
    /// <summary>
    /// Gets the native iOS central device.
    /// </summary>
    public CBCentral? NativeCentral { get; init; }
}
