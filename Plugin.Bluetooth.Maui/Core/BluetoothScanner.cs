namespace Plugin.Bluetooth.Maui;

/// <inheritdoc  />
public class BluetoothScanner : BaseBluetoothScanner
{
    /// <inheritdoc/>
    /// <exception cref="NotImplementedException">This platform-agnostic implementation throws NotImplementedException.</exception>
    protected override ValueTask NativeInitializeAsync()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    /// <exception cref="NotImplementedException">This platform-agnostic implementation throws NotImplementedException.</exception>
    protected override void NativeRefreshIsBluetoothOn()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    /// <exception cref="NotImplementedException">This platform-agnostic implementation throws NotImplementedException.</exception>
    protected override void NativeRefreshIsRunning()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    /// <exception cref="NotImplementedException">This platform-agnostic implementation throws NotImplementedException.</exception>
    protected override void NativeStart()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    /// <exception cref="NotImplementedException">This platform-agnostic implementation throws NotImplementedException.</exception>
    protected override void NativeStop()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    /// <exception cref="NotImplementedException">This platform-agnostic implementation throws NotImplementedException.</exception>
    protected override IBluetoothDevice NativeCreateDevice(IBluetoothAdvertisement advertisement)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }
}
