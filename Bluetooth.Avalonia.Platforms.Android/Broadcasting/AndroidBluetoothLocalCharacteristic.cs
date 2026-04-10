namespace Bluetooth.Avalonia.Platforms.Android.Broadcasting;

/// <inheritdoc/>
public class AndroidBluetoothLocalCharacteristic : Core.Broadcasting.BaseBluetoothLocalCharacteristic
{

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    public AndroidBluetoothLocalCharacteristic(IBluetoothLocalService service,
        Guid id,
        BluetoothCharacteristicProperties properties,
        BluetoothCharacteristicPermissions permissions,
        ReadOnlyMemory<byte>? initialValue = null,
        string? name = null,
        ILogger<IBluetoothLocalCharacteristic>? logger = null) : base(service,
                                                                      id,
                                                                      properties,
                                                                      permissions,
                                                                      initialValue,
                                                                      name,
                                                                      logger)
    {
        throw new PlatformNotSupportedException("Android BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Droid.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask<IBluetoothLocalDescriptor> NativeCreateDescriptorAsync(Guid id, string? name = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new PlatformNotSupportedException("Android BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Droid.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask NativeUpdateValueAsync(ReadOnlyMemory<byte> value, bool notifyClients, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new PlatformNotSupportedException("Android BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Droid.");
    }
}
