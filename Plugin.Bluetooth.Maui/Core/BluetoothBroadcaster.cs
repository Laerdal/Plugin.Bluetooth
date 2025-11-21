namespace Plugin.Bluetooth.Maui;

/// <inheritdoc />
public class BluetoothBroadcaster : BaseBluetoothBroadcaster
{

    /// <inheritdoc/>
    protected override void NativeRefreshIsBluetoothOn()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    protected override void NativeRefreshIsRunning()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    protected override void NativeStart()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    protected override void NativeStop()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    protected override ValueTask NativeInitializeAsync()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }
}
