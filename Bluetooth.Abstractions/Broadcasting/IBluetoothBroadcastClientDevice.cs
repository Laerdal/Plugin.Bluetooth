using System.ComponentModel;

namespace Bluetooth.Abstractions.Broadcasting;


/// <summary>
/// Represents a Bluetooth device that has connected to a broadcast (GATT server).
/// </summary>
public interface IBluetoothBroadcastClientDevice : INotifyPropertyChanged, IAsyncDisposable
{
    /// <summary>
    /// Gets the Bluetooth broadcaster hosting this service.
    /// </summary>
    IBluetoothBroadcaster Broadcaster { get; }

    /// <summary>
    /// Gets the unique identifier of the connected client device.
    /// </summary>
    string ClientId { get; }
}
