namespace Bluetooth.Abstractions.Scanning;

/// <summary>
/// Interface representing a Bluetooth descriptor, providing properties and methods for interacting with it.
/// Descriptors provide additional information about a characteristic (e.g., Client Characteristic Configuration Descriptor).
/// </summary>
public partial interface IBluetoothRemoteDescriptor : INotifyPropertyChanged, IAsyncDisposable
{
    /// <summary>
    /// Gets the Bluetooth characteristic associated with this descriptor.
    /// </summary>
    IBluetoothRemoteCharacteristic RemoteCharacteristic { get; }

    /// <summary>
    /// Gets the unique identifier of the descriptor. This is typically a UUID that identifies the type of descriptor (e.g., Client Characteristic Configuration Descriptor has a well-known UUID).
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// The name of the Bluetooth descriptor. This is typically used for debugging and logging purposes, and may not be available for all descriptor. If the descriptor is not recognized, this will default to "Unknown Descriptor".
    /// </summary>
    string Name { get; }

}

