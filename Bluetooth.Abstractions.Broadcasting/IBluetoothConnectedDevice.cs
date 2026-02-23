namespace Bluetooth.Abstractions.Broadcasting;

/// <summary>
///     Represents a Bluetooth device that has connected to a broadcast (GATT server).
/// </summary>
public partial interface IBluetoothConnectedDevice : INotifyPropertyChanged, IAsyncDisposable
{
    /// <summary>
    ///     Gets the Bluetooth broadcaster hosting this service.
    /// </summary>
    IBluetoothBroadcaster Broadcaster { get; }

    /// <summary>
    ///     Gets the name of the connected client device.
    /// </summary>
    string? Name { get; }

    /// <summary>
    ///     Gets the unique identifier of the connected client device. This is typically a string that distinguishes this client device from others in the Bluetooth broadcasting context, such as a MAC address or a custom identifier assigned by the
    ///     platform.
    /// </summary>
    string Id { get; }
}
