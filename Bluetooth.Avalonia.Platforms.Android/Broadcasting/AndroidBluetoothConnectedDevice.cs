namespace Bluetooth.Avalonia.Platforms.Android.Broadcasting;

/// <inheritdoc />
public class AndroidBluetoothConnectedDevice : BaseBluetoothConnectedDevice
{
    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    public AndroidBluetoothConnectedDevice(IBluetoothBroadcaster broadcaster, string id, ILogger<IBluetoothConnectedDevice>? logger = null) : base(broadcaster, id, logger)
    {
        throw new PlatformNotSupportedException("Android BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Droid.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask NativeDisconnectAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new PlatformNotSupportedException("Android BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Droid.");
    }
}
