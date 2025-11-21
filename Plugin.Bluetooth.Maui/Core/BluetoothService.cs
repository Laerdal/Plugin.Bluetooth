namespace Plugin.Bluetooth.Maui;

/// <inheritdoc/>
public class BluetoothService : BaseBluetoothService
{

    /// <inheritdoc/>
    /// <exception cref="NotImplementedException">This platform-agnostic implementation throws NotImplementedException.</exception>
    public BluetoothService(IBluetoothDevice device, Guid serviceUuid) : base(device, serviceUuid)
    {
    }

    /// <inheritdoc/>
    /// <exception cref="NotImplementedException">This platform-agnostic implementation throws NotImplementedException.</exception>
    protected override ValueTask NativeCharacteristicsExplorationAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }
}
