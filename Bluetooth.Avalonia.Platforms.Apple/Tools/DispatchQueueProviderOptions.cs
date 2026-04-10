namespace Bluetooth.Avalonia.Platforms.Apple.Tools;

/// <summary>
///     Options for configuring Dispatch Queue labels used by Core Bluetooth managers.
/// </summary>
public record DispatchQueueProviderOptions
{
    /// <summary>
    ///     Gets or sets the label for the CBCentralManager dispatch queue.
    /// </summary>
    public string CentralQueueLabel { get; init; } = "com.bluetooth.avalonia.central";

    /// <summary>
    ///     Gets or sets the label for the CBPeripheralManager dispatch queue.
    /// </summary>
    public string PeripheralQueueLabel { get; init; } = "com.bluetooth.avalonia.peripheral";
}
