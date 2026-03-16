namespace Bluetooth.Abstractions.Scanning;

/// <summary>
///     Interface for providing names for Bluetooth scanning objects such as remote characteristics, descriptors, and services.
/// </summary>
public interface IBluetoothNameProvider
{
    /// <summary>
    ///     Gets the known name for a Bluetooth service based on its unique identifier (UUID).
    /// </summary>
    string? GetKnownServiceName(Guid service);
    
    /// <summary>
    ///     Gets the known name for a Bluetooth characteristic based on its unique identifier (UUID).
    /// </summary>
    string? GetKnownCharacteristicName(Guid characteristic);
    
    /// <summary>
    ///     Gets the known name for a Bluetooth descriptor based on its unique identifier (UUID).
    /// </summary>
    string? GetKnownDescriptorName(Guid descriptor);
}
