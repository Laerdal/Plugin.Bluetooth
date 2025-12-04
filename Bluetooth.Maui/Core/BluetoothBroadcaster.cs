namespace Bluetooth.Maui;

/// <inheritdoc />
public partial class BluetoothBroadcaster : BaseBluetoothBroadcaster
{

    /// <inheritdoc/>
    /// <exception cref="PlatformNotSupportedException">This platform-agnostic implementation is not supported on non-native platforms.</exception>
    protected override ValueTask NativeInitializeAsync()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    /// <exception cref="PlatformNotSupportedException">This platform-agnostic implementation is not supported on non-native platforms.</exception>
    protected override void NativeRefreshIsBluetoothOn()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    /// <exception cref="PlatformNotSupportedException">This platform-agnostic implementation is not supported on non-native platforms.</exception>
    protected override void NativeRefreshIsRunning()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    /// <exception cref="PlatformNotSupportedException">This platform-agnostic implementation is not supported on non-native platforms.</exception>
    protected override ValueTask NativeStartAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    /// <exception cref="PlatformNotSupportedException">This platform-agnostic implementation is not supported on non-native platforms.</exception>
    protected override ValueTask NativeStopAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }
}
