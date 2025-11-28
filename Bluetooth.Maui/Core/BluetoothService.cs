namespace Bluetooth.Maui;

/// <inheritdoc/>
public class BluetoothService : BaseBluetoothService
{

    /// <summary>
    /// Initializes a new instance of the <see cref="BluetoothService"/> class.
    /// </summary>
    /// <param name="device">The Bluetooth device associated with this service.</param>
    /// <param name="serviceUuid">The unique identifier of the service.</param>
    public BluetoothService(IBluetoothDevice device, Guid serviceUuid) : base(device, serviceUuid)
    {
    }

    /// <inheritdoc/>
    /// <exception cref="PlatformNotSupportedException">This platform-agnostic implementation is not supported on non-native platforms.</exception>
    protected override ValueTask NativeCharacteristicsExplorationAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }
}
