namespace Bluetooth.Core.Scanning.Profiles.BluetoothSig;

/// <summary>
///     Bluetooth SIG Environmental Sensing Service profile definition.
///     <para>
///     Official Service Specification:
///     <see href="https://www.bluetooth.com/specifications/specs/environmental-sensing-service-1-0/">Environmental Sensing Service 1.0</see>
///     </para>
///     <para>
///     Assigned UUID:
///     <see href="https://bitbucket.org/bluetooth-SIG/public/src/main/assigned_numbers/uuids/service_uuids.yaml">Service UUID 0x181A</see>
///     </para>
/// </summary>
[BluetoothServiceDefinition]
public static class EnvironmentalSensingServiceDefinition
{
    /// <summary>
    ///     Gets the Environmental Sensing Service display name.
    /// </summary>
    public static readonly string Name = "Environmental Sensing";

    /// <summary>
    ///     Gets the Environmental Sensing Service UUID (0x181A).
    ///     <para>Bluetooth SIG Assigned Number: 0x181A</para>
    ///     <para>Service ID: org.bluetooth.service.environmental_sensing</para>
    /// </summary>
    public static readonly Guid Id = BluetoothServiceDefinitions.FromShortId(0x181A);

    /// <summary>
    ///     Gets an accessor for the Temperature characteristic (0x2A6E).
    ///     <para>
    ///     Temperature measurement in degrees Celsius with a resolution of 0.01 degrees (sint16).
    ///     </para>
    ///     <para>
    ///     Characteristic Specification:
    ///     <see href="https://bitbucket.org/bluetooth-SIG/public/src/main/assigned_numbers/uuids/characteristic_uuids.yaml">Characteristic UUID 0x2A6E</see>
    ///     </para>
    /// </summary>
    public static readonly IBluetoothCharacteristicAccessor<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>> Temperature = BluetoothServiceDefinitions.BytesCharacteristic(Id, 0x2A6E, Name, "Temperature");

    /// <summary>
    ///     Gets an accessor for the Humidity characteristic (0x2A6F).
    ///     <para>
    ///     Relative humidity measurement in percent with a resolution of 0.01% (uint16).
    ///     </para>
    ///     <para>
    ///     Characteristic Specification:
    ///     <see href="https://bitbucket.org/bluetooth-SIG/public/src/main/assigned_numbers/uuids/characteristic_uuids.yaml">Characteristic UUID 0x2A6F</see>
    ///     </para>
    /// </summary>
    public static readonly IBluetoothCharacteristicAccessor<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>> Humidity = BluetoothServiceDefinitions.BytesCharacteristic(Id, 0x2A6F, Name, "Humidity");

    /// <summary>
    ///     Gets an accessor for the Pressure characteristic (0x2A6D).
    ///     <para>
    ///     Pressure measurement in pascals (Pa) with resolution of 0.1 Pa (uint32).
    ///     </para>
    ///     <para>
    ///     Characteristic Specification:
    ///     <see href="https://bitbucket.org/bluetooth-SIG/public/src/main/assigned_numbers/uuids/characteristic_uuids.yaml">Characteristic UUID 0x2A6D</see>
    ///     </para>
    /// </summary>
    public static readonly IBluetoothCharacteristicAccessor<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>> Pressure = BluetoothServiceDefinitions.BytesCharacteristic(Id, 0x2A6D, Name, "Pressure");

    /// <summary>
    ///     Gets an accessor for the UV Index characteristic (0x2A76).
    ///     <para>
    ///     UV Index measurement (uint8). Range: 0-255.
    ///     </para>
    ///     <para>
    ///     Characteristic Specification:
    ///     <see href="https://bitbucket.org/bluetooth-SIG/public/src/main/assigned_numbers/uuids/characteristic_uuids.yaml">Characteristic UUID 0x2A76</see>
    ///     </para>
    /// </summary>
    public static readonly IBluetoothCharacteristicAccessor<byte, byte> UvIndex = BluetoothServiceDefinitions.ByteCharacteristic(Id, 0x2A76, Name, "UV Index");
}
