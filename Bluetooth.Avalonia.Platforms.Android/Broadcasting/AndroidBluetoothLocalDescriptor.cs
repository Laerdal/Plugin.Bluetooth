namespace Bluetooth.Avalonia.Platforms.Android.Broadcasting;

/// <inheritdoc />
public class AndroidBluetoothLocalDescriptor : BaseBluetoothLocalDescriptor
{
    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    public AndroidBluetoothLocalDescriptor(IBluetoothLocalCharacteristic characteristic,
        Guid id,
        ReadOnlyMemory<byte>? initialValue = null,
        string? name = null,
        ILogger<IBluetoothLocalDescriptor>? logger = null) : base(characteristic,
                                                                  id,
                                                                  initialValue,
                                                                  name,
                                                                  logger)
    {
        throw new PlatformNotSupportedException("Android BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Droid.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask NativeUpdateValueAsync(ReadOnlyMemory<byte> value, TimeSpan? timeout, CancellationToken cancellationToken)
    {
        throw new PlatformNotSupportedException("Android BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Droid.");
    }
}
