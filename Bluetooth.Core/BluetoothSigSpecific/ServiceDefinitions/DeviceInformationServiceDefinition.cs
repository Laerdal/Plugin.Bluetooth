namespace Bluetooth.Core.BluetoothSigSpecific.ServiceDefinitions;

/// <summary>
///     Device Information | device_information | 180A
/// </summary>
[ServiceDefinition]
public static class DeviceInformationServiceDefinition
{
    // https://www.bluetooth.com/specifications/gatt/characteristics/
    /// <summary>
    /// Gets the human-readable name of the Device Information Service.
    /// </summary>
    public readonly static string Name = "Device Information";

    /// <summary>
    /// Gets the UUID of the Device Information Service (0x180A).
    /// </summary>
    public readonly static Guid Id = new Guid($"0000180A{BluetoothSigConstants.StandardGuidExtension}");

    /// <summary>
    ///     The System ID characteristic shall represent a structure containing an Organizationally
    ///     Unique Identifier (OUI) followed by a manufacturer-defined identifier and is unique for
    ///     each individual instance of the product.
    /// </summary>
    public readonly static CharacteristicAccessService<string> SystemId = CharacteristicAccessServiceFactory.CreateForUtf8String(new Guid($"00002a23{BluetoothSigConstants.StandardGuidExtension}"), "System ID");

    /// <summary>
    ///     The Model Number String characteristic shall represent the model number that is assigned
    ///     by the device vendor.
    /// </summary>
    public readonly static CharacteristicAccessService<string> ModelNumber = CharacteristicAccessServiceFactory.CreateForUtf8String(new Guid($"00002a24{BluetoothSigConstants.StandardGuidExtension}"), "Model Number String");

    /// <summary>
    ///     The Serial Number String characteristic shall represent the serial number for a particular
    ///     instance of the device.
    /// </summary>
    public readonly static CharacteristicAccessService<string> SerialNumber = CharacteristicAccessServiceFactory.CreateForUtf8String(new Guid($"00002a25{BluetoothSigConstants.StandardGuidExtension}"), "Serial Number String");

    /// <summary>
    ///     The Firmware Revision String characteristic shall represent the firmware revision of the
    ///     firmware within the device.
    ///     All devices have a "Main" firmware, with only an app version - this is what "FirmwareRevision" refers to.
    ///     Note that the resuscitation-manikin also has a BLE firmware (with only App Version) and three or more modules (CAN
    ///     Modules)
    ///     composed of an app (with an app version) and a lib (with a lib version).
    /// </summary>
    public readonly static CharacteristicAccessService<Version> FirmwareRevision = CharacteristicAccessServiceFactory.CreateForVersion(new Guid($"00002a26{BluetoothSigConstants.StandardGuidExtension}"), "Firmware Revision String");

    /// <summary>
    ///     The Hardware Revision String characteristic shall represent the hardware revision for the
    ///     hardware within the device.
    /// </summary>
    public readonly static CharacteristicAccessService<string> HardwareRevision = CharacteristicAccessServiceFactory.CreateForUtf8String(new Guid($"00002a27{BluetoothSigConstants.StandardGuidExtension}"), "Hardware Revision String");

    /// <summary>
    ///     The Software Revision String characteristic shall represent the software revision for the
    ///     software within the device.
    /// </summary>
    public readonly static CharacteristicAccessService<Version> SoftwareRevision = CharacteristicAccessServiceFactory.CreateForVersion(new Guid($"00002a28{BluetoothSigConstants.StandardGuidExtension}"), "Software Revision String");

    /// <summary>
    ///     The Manufacturer Name String characteristic shall represent the name of the manufacturer
    ///     of the device.
    /// </summary>
    public readonly static CharacteristicAccessService<string> ManufacturerName = CharacteristicAccessServiceFactory.CreateForUtf8String(new Guid($"00002a29{BluetoothSigConstants.StandardGuidExtension}"), "Manufacturer Name String");

    /// <summary>
    ///     The IEEE 11073-20601 Regulatory Certification Data List characteristic shall represent
    ///     regulatory and certification information for the product in a list defined in IEEE 11073-
    ///     20601
    /// </summary>
    public readonly static CharacteristicAccessService<string> RegulatoryCertificationDataList = CharacteristicAccessServiceFactory.CreateForUtf8String(new Guid($"00002a2a{BluetoothSigConstants.StandardGuidExtension}"), "IEEE 11073-20601 Regulatory Certification Data List");
}
