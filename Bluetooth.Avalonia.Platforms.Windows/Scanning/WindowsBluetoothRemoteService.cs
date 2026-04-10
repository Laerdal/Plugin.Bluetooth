namespace Bluetooth.Avalonia.Platforms.Windows.Scanning;

/// <inheritdoc />
public class WindowsBluetoothRemoteService : BaseBluetoothRemoteService
{

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    public WindowsBluetoothRemoteService(IBluetoothRemoteDevice parentDevice,
        Guid id,
        IBluetoothNameProvider? nameProvider = null,
        ILogger<IBluetoothRemoteService>? logger = null) : base(parentDevice, id, nameProvider, logger)
    {
        throw new PlatformNotSupportedException("Windows BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Win.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask NativeCharacteristicsExplorationAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new PlatformNotSupportedException("Windows BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Win.");
    }
}
