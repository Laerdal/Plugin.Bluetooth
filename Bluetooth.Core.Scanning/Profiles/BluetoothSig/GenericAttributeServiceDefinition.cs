namespace Bluetooth.Core.Scanning.Profiles.BluetoothSig;

/// <summary>
///     Bluetooth SIG Generic Attribute Service profile definition.
/// </summary>
[BluetoothServiceDefinition]
public static class GenericAttributeServiceDefinition
{
    /// <summary>
    ///     Gets the Generic Attribute Service display name.
    /// </summary>
    public static readonly string Name = "Generic Attribute Service";

    /// <summary>
    ///     Gets the Generic Attribute Service UUID (0x1801).
    /// </summary>
    public static readonly Guid Id = BluetoothServiceDefinitions.FromShortId(0x1801);

    /// <summary>
    ///     Gets an accessor for the Service Changed characteristic (0x2A05).
    /// </summary>
    public static readonly IBluetoothCharacteristicAccessor<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>> ServiceChanged = BluetoothServiceDefinitions.BytesCharacteristic(Id, 0x2A05, Name, "Service Changed");
}
