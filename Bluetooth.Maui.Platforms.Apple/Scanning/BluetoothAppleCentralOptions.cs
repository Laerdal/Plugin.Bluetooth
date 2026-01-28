namespace Bluetooth.Maui.Platforms.Apple.Scanning;

/// <summary>
/// Options for Apple Bluetooth functionality.
/// </summary>
public sealed record BluetoothAppleCentralOptions
{
    /// <summary>
    /// The label for the central dispatch queue.
    /// </summary>
    public string QueueLabel { get; init; } = "ble.central";

    /// <summary>
    /// Indicates whether to show power alert when Bluetooth is powered off.
    /// </summary>
    public bool ShowPowerAlert { get; init; } = true;

    /// <summary>
    /// The restore identifier key for the central manager.
    /// </summary>
    public string? RestoreIdentifierKey { get; init; }
}
