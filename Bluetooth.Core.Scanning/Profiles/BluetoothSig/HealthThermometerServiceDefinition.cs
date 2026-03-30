namespace Bluetooth.Core.Scanning.Profiles.BluetoothSig;

/// <summary>
///     Bluetooth SIG Health Thermometer Service profile definition.
///     <para>
///     Official Service Specification:
///     <see href="https://www.bluetooth.com/specifications/specs/health-thermometer-service-1-0/">Health Thermometer Service 1.0</see>
///     </para>
///     <para>
///     Assigned UUID:
///     <see href="https://bitbucket.org/bluetooth-SIG/public/src/main/assigned_numbers/uuids/service_uuids.yaml">Service UUID 0x1809</see>
///     </para>
/// </summary>
[BluetoothServiceDefinition]
public static class HealthThermometerServiceDefinition
{
    /// <summary>
    ///     Gets the Health Thermometer Service display name.
    /// </summary>
    public static readonly string Name = "Health Thermometer";

    /// <summary>
    ///     Gets the Health Thermometer Service UUID (0x1809).
    ///     <para>Bluetooth SIG Assigned Number: 0x1809</para>
    ///     <para>Service ID: org.bluetooth.service.health_thermometer</para>
    /// </summary>
    public static readonly Guid Id = BluetoothServiceDefinitions.FromShortId(0x1809);

    /// <summary>
    ///     Gets an accessor for the Temperature Measurement characteristic (0x2A1C).
    ///     <para>
    ///     Contains the temperature measurement value and associated metadata.
    ///     Format: Flags (1 byte) + Temperature Value (4 bytes IEEE-11073 FLOAT) + optional Time Stamp + optional Temperature Type.
    ///     </para>
    ///     <para>
    ///     Characteristic Specification:
    ///     <see href="https://bitbucket.org/bluetooth-SIG/public/src/main/assigned_numbers/uuids/characteristic_uuids.yaml">Characteristic UUID 0x2A1C</see>
    ///     </para>
    /// </summary>
    public static readonly IBluetoothCharacteristicAccessor<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>> TemperatureMeasurement = BluetoothServiceDefinitions.BytesCharacteristic(Id, 0x2A1C, Name, "Temperature Measurement");

    /// <summary>
    ///     Gets an accessor for the Temperature Type characteristic (0x2A1D).
    ///     <para>
    ///     Indicates where the temperature was measured.
    ///     Values: 1=Armpit, 2=Body (general), 3=Ear, 4=Finger, 5=Gastro-intestinal Tract, 6=Mouth, 7=Rectum, 8=Toe, 9=Tympanum (ear drum).
    ///     </para>
    ///     <para>
    ///     Characteristic Specification:
    ///     <see href="https://bitbucket.org/bluetooth-SIG/public/src/main/assigned_numbers/uuids/characteristic_uuids.yaml">Characteristic UUID 0x2A1D</see>
    ///     </para>
    /// </summary>
    public static readonly IBluetoothCharacteristicAccessor<byte, byte> TemperatureType = BluetoothServiceDefinitions.ByteCharacteristic(Id, 0x2A1D, Name, "Temperature Type");

    /// <summary>
    ///     Gets an accessor for the Intermediate Temperature characteristic (0x2A1E).
    ///     <para>
    ///     Contains intermediate temperature values sent while a measurement is in progress.
    ///     Format: Same as Temperature Measurement characteristic.
    ///     </para>
    ///     <para>
    ///     Characteristic Specification:
    ///     <see href="https://bitbucket.org/bluetooth-SIG/public/src/main/assigned_numbers/uuids/characteristic_uuids.yaml">Characteristic UUID 0x2A1E</see>
    ///     </para>
    /// </summary>
    public static readonly IBluetoothCharacteristicAccessor<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>> IntermediateTemperature = BluetoothServiceDefinitions.BytesCharacteristic(Id, 0x2A1E, Name, "Intermediate Temperature");

    /// <summary>
    ///     Gets an accessor for the Measurement Interval characteristic (0x2A21).
    ///     <para>
    ///     Defines the interval between measurements in seconds (uint16).
    ///     Value of 0x0000 means no periodic measurement.
    ///     </para>
    ///     <para>
    ///     Characteristic Specification:
    ///     <see href="https://bitbucket.org/bluetooth-SIG/public/src/main/assigned_numbers/uuids/characteristic_uuids.yaml">Characteristic UUID 0x2A21</see>
    ///     </para>
    /// </summary>
    public static readonly IBluetoothCharacteristicAccessor<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>> MeasurementInterval = BluetoothServiceDefinitions.BytesCharacteristic(Id, 0x2A21, Name, "Measurement Interval");
}
