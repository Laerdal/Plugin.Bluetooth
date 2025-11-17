
namespace Plugin.Bluetooth.BaseClasses;

/// <inheritdoc cref="IBluetoothBroadcaster" />
public abstract class BaseBluetoothBroadcaster : BaseBluetoothActivity, IBluetoothBroadcaster
{
    /// <summary>
    /// Platform-specific implementation to set the advertising data for the broadcaster.
    /// </summary>
    /// <returns>A task that completes when the advertising data has been set.</returns>
    public abstract Task NativeSetAdvertisingDataAsync();
}
