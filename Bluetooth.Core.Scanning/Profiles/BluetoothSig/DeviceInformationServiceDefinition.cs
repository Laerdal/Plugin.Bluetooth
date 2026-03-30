namespace Bluetooth.Core.Scanning.Profiles.BluetoothSig;

/// <summary>
///     Bluetooth SIG Device Information Service profile definition.
///     <para>
///     Official Service Specification:
///     <see href="https://www.bluetooth.com/specifications/specs/device-information-service-1-1/">Device Information Service 1.1</see>
///     </para>
///     <para>
///     Assigned UUID:
///     <see href="https://bitbucket.org/bluetooth-SIG/public/src/main/assigned_numbers/uuids/service_uuids.yaml">Service UUID 0x180A</see>
///     </para>
/// </summary>
[BluetoothServiceDefinition]
public static class DeviceInformationServiceDefinition
{
    /// <summary>
    ///     Gets the Device Information Service display name.
    /// </summary>
    public static readonly string Name = "Device Information";

    /// <summary>
    ///     Gets the Device Information Service UUID (0x180A).
    ///     <para>Bluetooth SIG Assigned Number: 0x180A</para>
    ///     <para>Service ID: org.bluetooth.service.device_information</para>
    /// </summary>
    public static readonly Guid Id = BluetoothServiceDefinitions.FromShortId(0x180A);

    /// <summary>
    ///     Gets an accessor for the Manufacturer Name String characteristic (0x2A29).
    ///     <para>Represents the name of the manufacturer of the device.</para>
    /// </summary>
    public static readonly IBluetoothCharacteristicAccessor<string, string> ManufacturerName = BluetoothServiceDefinitions.StringCharacteristic(Id, 0x2A29, Name, "Manufacturer Name String");

    /// <summary>
    ///     Gets an accessor for the Model Number String characteristic (0x2A24).
    /// </summary>
    public static readonly IBluetoothCharacteristicAccessor<string, string> ModelNumber = BluetoothServiceDefinitions.StringCharacteristic(Id, 0x2A24, Name, "Model Number String");

    /// <summary>
    ///     Gets an accessor for the Serial Number String characteristic (0x2A25).
    /// </summary>
    public static readonly IBluetoothCharacteristicAccessor<string, string> SerialNumber = BluetoothServiceDefinitions.StringCharacteristic(Id, 0x2A25, Name, "Serial Number String");

    /// <summary>
    ///     Gets an accessor for the Firmware Revision String characteristic (0x2A26).
    /// </summary>
    public static readonly IBluetoothCharacteristicAccessor<Version, Version> FirmwareRevision = BluetoothServiceDefinitions.VersionCharacteristic(Id, 0x2A26, Name, "Firmware Revision String");

    /// <summary>
    ///     Gets an accessor for the Hardware Revision String characteristic (0x2A27).
    /// </summary>
    public static readonly IBluetoothCharacteristicAccessor<string, string> HardwareRevision = BluetoothServiceDefinitions.StringCharacteristic(Id, 0x2A27, Name, "Hardware Revision String");

    /// <summary>
    ///     Gets an accessor for the Software Revision String characteristic (0x2A28).
    /// </summary>
    public static readonly IBluetoothCharacteristicAccessor<Version, Version> SoftwareRevision = BluetoothServiceDefinitions.VersionCharacteristic(Id, 0x2A28, Name, "Software Revision String");

    /// <summary>
    ///     Gets an accessor for the IEEE 11073-20601 Regulatory Certification Data List characteristic (0x2A2A).
    ///     <para>Contains regulatory and certification information for the device.</para>
    /// </summary>
    public static readonly IBluetoothCharacteristicAccessor<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>> RegulatoryCertificationDataList = BluetoothServiceDefinitions.BytesCharacteristic(Id, 0x2A2A, Name, "IEEE 11073-20601 Regulatory Certification Data List");

    /// <summary>
    ///     Gets an accessor for the PnP ID characteristic (0x2A50).
    ///     <para>
    ///     Contains Vendor ID, Product ID, and Product Version information.
    ///     Format: Vendor ID Source (1 byte), Vendor ID (2 bytes), Product ID (2 bytes), Product Version (2 bytes).
    ///     </para>
    /// </summary>
    public static readonly IBluetoothCharacteristicAccessor<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>> PnpId = BluetoothServiceDefinitions.BytesCharacteristic(Id, 0x2A50, Name, "PnP ID");

    /// <summary>
    ///     Gets an accessor for the System ID characteristic (0x2A23).
    ///     <para>
    ///     Contains a unique identifier for the device.
    ///     Format: Manufacturer Identifier (5 bytes) + Organizationally Unique Identifier (3 bytes).
    ///     </para>
    /// </summary>
    public static readonly IBluetoothCharacteristicAccessor<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>> SystemId = BluetoothServiceDefinitions.BytesCharacteristic(Id, 0x2A23, Name, "System ID");
}
