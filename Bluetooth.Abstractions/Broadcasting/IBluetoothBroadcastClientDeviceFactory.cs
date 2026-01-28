using System.Diagnostics.CodeAnalysis;

namespace Bluetooth.Abstractions.Broadcasting;

/// <summary>
/// Factory interface for creating Bluetooth broadcast device instances.
/// </summary>
[SuppressMessage("Design", "CA1034:Nested types should not be visible")]
public interface IBluetoothBroadcastClientDeviceFactory
{
    /// <summary>
    /// Creates a new instance of a Bluetooth broadcast device.
    /// </summary>
    /// <param name="bluetoothBroadcaster">The Bluetooth broadcaster associated with the device.</param>
    /// <param name="request">The request containing information needed to create the broadcast device.</param>
    /// <returns>A new instance of <see cref="IBluetoothBroadcastClientDevice"/>.</returns>
    IBluetoothBroadcastClientDevice CreateBroadcastClientDevice(IBluetoothBroadcaster bluetoothBroadcaster, BluetoothBroadcastClientDeviceFactoryRequest request);

    /// <summary>
    /// Record representing a request to create a Bluetooth broadcast device.
    /// </summary>
    public record BluetoothBroadcastClientDeviceFactoryRequest
    {
        /// <summary>
        /// Gets the unique identifier of the client device connecting to the broadcast.
        /// </summary>
        public string ClientId { get; init; } = null!;
    }

}
