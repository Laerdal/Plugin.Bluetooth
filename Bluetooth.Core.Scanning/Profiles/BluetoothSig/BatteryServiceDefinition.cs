namespace Bluetooth.Core.Scanning.Profiles.BluetoothSig;

/// <summary>
///     Bluetooth SIG Battery Service profile definition.
///     <para>
///     Official Service Specification:
///     <see href="https://www.bluetooth.com/specifications/specs/battery-service-1-0/">Battery Service 1.0</see>
///     </para>
///     <para>
///     Assigned UUID:
///     <see href="https://bitbucket.org/bluetooth-SIG/public/src/main/assigned_numbers/uuids/service_uuids.yaml">Service UUID 0x180F</see>
///     </para>
/// </summary>
[BluetoothServiceDefinition]
public static class BatteryServiceDefinition
{
    /// <summary>
    ///     Gets the Battery Service display name.
    /// </summary>
    public static readonly string Name = "Battery Service";

    /// <summary>
    ///     Gets the Battery Service UUID (0x180F).
    ///     <para>Bluetooth SIG Assigned Number: 0x180F</para>
    ///     <para>Service ID: org.bluetooth.service.battery_service</para>
    /// </summary>
    public static readonly Guid Id = BluetoothServiceDefinitions.FromShortId(0x180F);

    /// <summary>
    ///     Gets an accessor for the Battery Level characteristic (0x2A19).
    ///     <para>
    ///     Returns the current battery level as a percentage (0-100).
    ///     </para>
    ///     <para>
    ///     Characteristic Specification:
    ///     <see href="https://bitbucket.org/bluetooth-SIG/public/src/main/assigned_numbers/uuids/characteristic_uuids.yaml">Characteristic UUID 0x2A19</see>
    ///     </para>
    /// </summary>
    public static readonly IBluetoothCharacteristicAccessor<byte, byte> BatteryLevel = BluetoothServiceDefinitions.ByteCharacteristic(Id, 0x2A19, Name, "Battery Level");
}
