namespace Bluetooth.Abstractions.Scanning.Options;

/// <summary>
///     Defines how a scanner should handle devices that have not been seen for a configured timeout period.
/// </summary>
public enum BluetoothDeviceDisappearanceBehavior
{
    /// <summary>
    ///     Keep the device in the list and mark it as stale.
    /// </summary>
    MarkAsStale = 0,

    /// <summary>
    ///     Remove the device from the scanner list.
    /// </summary>
    RemoveFromList = 1
}
