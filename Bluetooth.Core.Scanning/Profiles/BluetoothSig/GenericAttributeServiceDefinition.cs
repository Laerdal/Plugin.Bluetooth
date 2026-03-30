namespace Bluetooth.Core.Scanning.Profiles.BluetoothSig;

/// <summary>
///     Bluetooth SIG Generic Attribute Profile (GATT) Service definition.
///     <para>
///     Official Service Specification:
///     <see href="https://www.bluetooth.com/specifications/specs/core-specification/">Bluetooth Core Specification</see>
///     </para>
///     <para>
///     Assigned UUID:
///     <see href="https://bitbucket.org/bluetooth-SIG/public/src/main/assigned_numbers/uuids/service_uuids.yaml">Service UUID 0x1801</see>
///     </para>
/// </summary>
[BluetoothServiceDefinition]
public static class GenericAttributeServiceDefinition
{
    /// <summary>
    ///     Gets the Generic Attribute Service display name.
    /// </summary>
    public static readonly string Name = "Generic Attribute";

    /// <summary>
    ///     Gets the Generic Attribute Service UUID (0x1801).
    ///     <para>Bluetooth SIG Assigned Number: 0x1801</para>
    ///     <para>Service ID: org.bluetooth.service.gatt</para>
    /// </summary>
    public static readonly Guid Id = BluetoothServiceDefinitions.FromShortId(0x1801);

    /// <summary>
    ///     Gets an accessor for the Service Changed characteristic (0x2A05).
    ///     <para>
    ///     Indicates when services have been added, removed, or modified.
    ///     Format: Start Handle (2 bytes) + End Handle (2 bytes) of affected attribute range.
    ///     </para>
    /// </summary>
    public static readonly IBluetoothCharacteristicAccessor<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>> ServiceChanged = BluetoothServiceDefinitions.BytesCharacteristic(Id, 0x2A05, Name, "Service Changed");
}
