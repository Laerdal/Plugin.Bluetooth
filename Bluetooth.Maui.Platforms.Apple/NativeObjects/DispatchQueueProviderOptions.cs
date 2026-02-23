namespace Bluetooth.Maui.Platforms.Apple.NativeObjects;

/// <summary>
///     Options for configuring the DispatchQueueProvider.
/// </summary>
public record DispatchQueueProviderOptions
{
    /// <summary>
    ///     The label for the central dispatch queue.
    /// </summary>
    public string CentralQueueLabel { get; set; } = "com.bluetooth.maui.central";

    /// <summary>
    ///     The label for the peripheral dispatch queue.
    /// </summary>
    public string PeripheralQueueLabel { get; set; } = "com.bluetooth.maui.peripheral";
}
