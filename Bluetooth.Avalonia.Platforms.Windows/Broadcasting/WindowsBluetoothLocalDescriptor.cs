namespace Bluetooth.Avalonia.Platforms.Windows.Broadcasting;

/// <inheritdoc />
public class WindowsBluetoothLocalDescriptor : BaseBluetoothLocalDescriptor
{
    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    public WindowsBluetoothLocalDescriptor(IBluetoothLocalCharacteristic characteristic,
        Guid id,
        ReadOnlyMemory<byte>? initialValue = null,
        string? name = null,
        ILogger<IBluetoothLocalDescriptor>? logger = null) : base(characteristic,
                                                                  id,
                                                                  initialValue,
                                                                  name,
                                                                  logger)
    {
        throw new PlatformNotSupportedException("Windows BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Win.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask NativeUpdateValueAsync(ReadOnlyMemory<byte> value, TimeSpan? timeout, CancellationToken cancellationToken)
    {
        throw new PlatformNotSupportedException("Windows BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Win.");
    }
}
