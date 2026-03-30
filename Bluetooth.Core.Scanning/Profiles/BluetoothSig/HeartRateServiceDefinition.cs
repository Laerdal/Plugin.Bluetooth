namespace Bluetooth.Core.Scanning.Profiles.BluetoothSig;

/// <summary>
///     Bluetooth SIG Heart Rate Service profile definition.
///     <para>
///     Official Service Specification:
///     <see href="https://www.bluetooth.com/specifications/specs/heart-rate-service-1-0/">Heart Rate Service 1.0</see>
///     </para>
///     <para>
///     Assigned UUID:
///     <see href="https://bitbucket.org/bluetooth-SIG/public/src/main/assigned_numbers/uuids/service_uuids.yaml">Service UUID 0x180D</see>
///     </para>
/// </summary>
[BluetoothServiceDefinition]
public static class HeartRateServiceDefinition
{
    /// <summary>
    ///     Gets the Heart Rate Service display name.
    /// </summary>
    public static readonly string Name = "Heart Rate";

    /// <summary>
    ///     Gets the Heart Rate Service UUID (0x180D).
    ///     <para>Bluetooth SIG Assigned Number: 0x180D</para>
    ///     <para>Service ID: org.bluetooth.service.heart_rate</para>
    /// </summary>
    public static readonly Guid Id = BluetoothServiceDefinitions.FromShortId(0x180D);

    /// <summary>
    ///     Gets an accessor for the Heart Rate Measurement characteristic (0x2A37).
    ///     <para>
    ///     Contains the current heart rate measurement in beats per minute (BPM).
    ///     Format: Flags (1 byte) + Heart Rate Value (1-2 bytes) + optional Energy Expended + optional RR-Interval values.
    ///     </para>
    ///     <para>
    ///     Characteristic Specification:
    ///     <see href="https://bitbucket.org/bluetooth-SIG/public/src/main/assigned_numbers/uuids/characteristic_uuids.yaml">Characteristic UUID 0x2A37</see>
    ///     </para>
    /// </summary>
    public static readonly IBluetoothCharacteristicAccessor<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>> HeartRateMeasurement = BluetoothServiceDefinitions.BytesCharacteristic(Id, 0x2A37, Name, "Heart Rate Measurement");

    /// <summary>
    ///     Gets an accessor for the Body Sensor Location characteristic (0x2A38).
    ///     <para>
    ///     Describes the intended location of the heart rate measurement sensor.
    ///     Values: 0=Other, 1=Chest, 2=Wrist, 3=Finger, 4=Hand, 5=Ear Lobe, 6=Foot.
    ///     </para>
    ///     <para>
    ///     Characteristic Specification:
    ///     <see href="https://bitbucket.org/bluetooth-SIG/public/src/main/assigned_numbers/uuids/characteristic_uuids.yaml">Characteristic UUID 0x2A38</see>
    ///     </para>
    /// </summary>
    public static readonly IBluetoothCharacteristicAccessor<byte, byte> BodySensorLocation = BluetoothServiceDefinitions.ByteCharacteristic(Id, 0x2A38, Name, "Body Sensor Location");

    /// <summary>
    ///     Gets an accessor for the Heart Rate Control Point characteristic (0x2A39).
    ///     <para>
    ///     Used to control certain behaviors of the heart rate sensor. Write-only characteristic.
    ///     Value: 0x01 = Reset Energy Expended.
    ///     </para>
    ///     <para>
    ///     Characteristic Specification:
    ///     <see href="https://bitbucket.org/bluetooth-SIG/public/src/main/assigned_numbers/uuids/characteristic_uuids.yaml">Characteristic UUID 0x2A39</see>
    ///     </para>
    /// </summary>
    public static readonly IBluetoothCharacteristicAccessor<byte, byte> HeartRateControlPoint = BluetoothServiceDefinitions.ByteCharacteristic(Id, 0x2A39, Name, "Heart Rate Control Point");
}
