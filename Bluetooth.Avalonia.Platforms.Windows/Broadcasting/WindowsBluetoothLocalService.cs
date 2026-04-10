namespace Bluetooth.Avalonia.Platforms.Windows.Broadcasting;

/// <inheritdoc />
public class WindowsBluetoothLocalService : BaseBluetoothLocalService
{
    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    public WindowsBluetoothLocalService(IBluetoothBroadcaster broadcaster,
        Guid id,
        string? name = null,
        bool isPrimary = true,
        ILogger<IBluetoothLocalService>? logger = null) : base(broadcaster,
                                                               id,
                                                               name,
                                                               isPrimary,
                                                               logger)
    {
        throw new PlatformNotSupportedException("Windows BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Win.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask<IBluetoothLocalCharacteristic> NativeCreateCharacteristicAsync(Guid id,
        BluetoothCharacteristicProperties properties,
        BluetoothCharacteristicPermissions permissions,
        string? name = null,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        throw new PlatformNotSupportedException("Windows BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Win.");
    }
}
