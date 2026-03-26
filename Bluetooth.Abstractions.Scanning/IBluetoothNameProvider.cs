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
    ///     Returns <c>null</c> if the UUID maps to multiple services with different names (ambiguous).
    ///     Prefer <see cref="GetKnownCharacteristicName(Guid, Guid)" /> when the service context is available.
    /// </summary>
    string? GetKnownCharacteristicName(Guid characteristic);

    /// <summary>
    ///     Gets the known name for a Bluetooth characteristic given its owning service UUID and characteristic UUID.
    ///     Use this overload when the service context is known to avoid ambiguity when the same characteristic UUID
    ///     appears in multiple services.
    /// </summary>
    string? GetKnownCharacteristicName(Guid serviceId, Guid characteristicId);

    /// <summary>
    ///     Gets the known name for a Bluetooth descriptor based on its unique identifier (UUID).
    /// </summary>
    string? GetKnownDescriptorName(Guid descriptor);
}
