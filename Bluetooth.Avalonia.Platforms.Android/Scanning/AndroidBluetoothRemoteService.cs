namespace Bluetooth.Avalonia.Platforms.Android.Scanning;

/// <inheritdoc />
public class AndroidBluetoothRemoteService : BaseBluetoothRemoteService
{

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    public AndroidBluetoothRemoteService(IBluetoothRemoteDevice parentDevice,
        Guid id,
        IBluetoothNameProvider? nameProvider = null,
        ILogger<IBluetoothRemoteService>? logger = null) : base(parentDevice, id, nameProvider, logger)
    {
        throw new PlatformNotSupportedException("Android BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Droid.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask NativeCharacteristicsExplorationAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new PlatformNotSupportedException("Android BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Droid.");
    }
}
