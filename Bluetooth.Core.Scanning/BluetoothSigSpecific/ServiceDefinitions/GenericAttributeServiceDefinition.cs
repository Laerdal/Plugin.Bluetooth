using Bluetooth.Core.Scanning.CharacteristicAccess;

namespace Bluetooth.Core.Scanning.BluetoothSigSpecific.ServiceDefinitions;

/// <summary>
///     Generic Attribute Service | generic_attribute_service | 1801
/// </summary>
[ServiceDefinition]
public static class GenericAttributeServiceDefinition
{
    // https:// www.bluetooth.com/specifications/gatt/characteristics/
    /// <summary>
    /// Gets the human-readable name of the Generic Attribute Service.
    /// </summary>
    public readonly static string Name = "Generic Attribute Service";

    /// <summary>
    /// Gets the UUID of the Generic Attribute Service (0x1801).
    /// </summary>
    public readonly static Guid Id = new Guid($"00001801{BluetoothSigConstants.StandardGuidExtension}");

    /// <summary>
    /// Gets the Service Changed characteristic access service for reading service change notifications.
    /// </summary>
    public readonly static CharacteristicAccessService<byte[]> ServiceChanged = CharacteristicAccessServiceFactory.CreateForByteArray(new Guid($"00002a05{BluetoothSigConstants.StandardGuidExtension}"), "Service Changed");

}
