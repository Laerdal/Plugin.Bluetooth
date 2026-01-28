namespace Bluetooth.Maui.Platforms.Apple.Broadcasting;

/// <summary>
/// Options for Apple Bluetooth functionality.
/// </summary>
public sealed record BluetoothApplePeripheralOptions
{
    /// <summary>
    /// The label for the peripheral dispatch queue.
    /// </summary>
    public string QueueLabel { get; init; } = "ble.peripheral";

    /// <summary>
    /// Indicates whether to show power alert when Bluetooth is powered off.
    /// </summary>
    public bool ShowPowerAlert { get; init; } = true;

    /// <summary>
    /// The restore identifier key for the peripheral manager.
    /// </summary>
    public string? RestoreIdentifierKey { get; init; }
}
