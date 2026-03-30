namespace Bluetooth.Core.Scanning.Profiles.BluetoothSig;

/// <summary>
///     Bluetooth SIG Generic Access Profile (GAP) Service definition.
///     <para>
///     Official Service Specification:
///     <see href="https://www.bluetooth.com/specifications/specs/core-specification/">Bluetooth Core Specification</see>
///     </para>
///     <para>
///     Assigned UUID:
///     <see href="https://bitbucket.org/bluetooth-SIG/public/src/main/assigned_numbers/uuids/service_uuids.yaml">Service UUID 0x1800</see>
///     </para>
/// </summary>
[BluetoothServiceDefinition]
public static class GenericAccessServiceDefinition
{
    /// <summary>
    ///     Gets the Generic Access Service display name.
    /// </summary>
    public static readonly string Name = "Generic Access";

    /// <summary>
    ///     Gets the Generic Access Service UUID (0x1800).
    ///     <para>Bluetooth SIG Assigned Number: 0x1800</para>
    ///     <para>Service ID: org.bluetooth.service.gap</para>
    /// </summary>
    public static readonly Guid Id = BluetoothServiceDefinitions.FromShortId(0x1800);

    /// <summary>
    ///     Gets an accessor for the Device Name characteristic (0x2A00).
    ///     <para>The name of the device as a UTF-8 string.</para>
    /// </summary>
    public static readonly IBluetoothCharacteristicAccessor<string, string> DeviceName = BluetoothServiceDefinitions.StringCharacteristic(Id, 0x2A00, Name, "Device Name");

    /// <summary>
    ///     Gets an accessor for the Appearance characteristic (0x2A01).
    ///     <para>
    ///     Describes the external appearance of the device (2 bytes).
    ///     See Bluetooth Assigned Numbers for appearance values.
    ///     </para>
    /// </summary>
    public static readonly IBluetoothCharacteristicAccessor<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>> Appearance = BluetoothServiceDefinitions.BytesCharacteristic(Id, 0x2A01, Name, "Appearance");

    /// <summary>
    ///     Gets an accessor for the Peripheral Privacy Flag characteristic (0x2A02).
    ///     <para>Indicates if privacy is enabled on the device (deprecated in Bluetooth 5.0+).</para>
    /// </summary>
    public static readonly IBluetoothCharacteristicAccessor<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>> PeripheralPrivacyFlag = BluetoothServiceDefinitions.BytesCharacteristic(Id, 0x2A02, Name, "Peripheral Privacy Flag");

    /// <summary>
    ///     Gets an accessor for the Reconnection Address characteristic (0x2A03).
    ///     <para>Used for reconnection purposes (deprecated in Bluetooth 5.0+).</para>
    /// </summary>
    public static readonly IBluetoothCharacteristicAccessor<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>> ReconnectionAddress = BluetoothServiceDefinitions.BytesCharacteristic(Id, 0x2A03, Name, "Reconnection Address");

    /// <summary>
    ///     Gets an accessor for the Peripheral Preferred Connection Parameters characteristic (0x2A04).
    ///     <para>
    ///     Contains the preferred connection parameters: Min Interval, Max Interval,
    ///     Slave Latency, and Connection Supervision Timeout Multiplier.
    ///     </para>
    /// </summary>
    public static readonly IBluetoothCharacteristicAccessor<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>> PeripheralPreferredConnectionParameters = BluetoothServiceDefinitions.BytesCharacteristic(Id, 0x2A04, Name, "Peripheral Preferred Connection Parameters");
}
