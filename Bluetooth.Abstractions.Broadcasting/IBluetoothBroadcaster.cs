using Bluetooth.Abstractions.Broadcasting.Options;

namespace Bluetooth.Abstractions.Broadcasting;

/// <summary>
/// Interface for managing Bluetooth broadcasting operations.
/// Enables the device to act as a Bluetooth peripheral/server, advertising services and handling client connections.
/// </summary>
public partial interface IBluetoothBroadcaster : IAsyncDisposable
{
    /// <summary>
    /// Gets the Bluetooth adapter associated with this broadcaster.
    /// </summary>
    IBluetoothAdapter Adapter { get; }
}
