namespace Bluetooth.Core.Scanning.Profiles.BluetoothSig;

/// <summary>
///     Bluetooth SIG Device Information Service profile definition.
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
    /// </summary>
    public static readonly Guid Id = BluetoothServiceDefinitions.FromShortId(0x180A);

    /// <summary>
    ///     Gets an accessor for the System ID characteristic (0x2A23).
    /// </summary>
    public static readonly IBluetoothCharacteristicAccessor<string, string> SystemId = BluetoothServiceDefinitions.StringCharacteristic(Id, 0x2A23, Name, "System ID");

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
    ///     Gets an accessor for the Manufacturer Name String characteristic (0x2A29).
    /// </summary>
    public static readonly IBluetoothCharacteristicAccessor<string, string> ManufacturerName = BluetoothServiceDefinitions.StringCharacteristic(Id, 0x2A29, Name, "Manufacturer Name String");

    /// <summary>
    ///     Gets an accessor for the IEEE 11073-20601 Regulatory Certification Data List characteristic (0x2A2A).
    /// </summary>
    public static readonly IBluetoothCharacteristicAccessor<string, string> RegulatoryCertificationDataList = BluetoothServiceDefinitions.StringCharacteristic(Id, 0x2A2A, Name, "IEEE 11073-20601 Regulatory Certification Data List");
}
