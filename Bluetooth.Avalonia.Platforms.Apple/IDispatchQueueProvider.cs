namespace Bluetooth.Avalonia.Platforms.Apple;

/// <summary>
///     Provides Core Bluetooth dispatch queues for the central and peripheral managers.
/// </summary>
public interface IDispatchQueueProvider
{
    /// <summary>
    ///     Gets the dispatch queue used by <c>CBCentralManager</c>.
    /// </summary>
    DispatchQueue CentralQueue { get; }

    /// <summary>
    ///     Gets the dispatch queue used by <c>CBPeripheralManager</c>.
    /// </summary>
    DispatchQueue PeripheralQueue { get; }
}
