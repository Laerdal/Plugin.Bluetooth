namespace Bluetooth.Abstractions.Scanning;

public partial interface IBluetoothRemoteDevice
{
    /// <summary>
    ///     Gets a value indicating whether the device is stale (not seen for the configured inactivity timeout).
    /// </summary>
    bool IsStale { get; }
}
