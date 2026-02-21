namespace Bluetooth.Maui.Platforms.Droid.Scanning.NativeObjects;

/// <summary>
///     Represents a part of a Bluetooth LE advertisement scan record.
///     Contains the size, type, and payload data for a specific advertisement data element.
/// </summary>
public partial class ScanRecordPart
{
    /// <summary>
    ///     Defines the types of advertisement records that can be present in a Bluetooth LE advertisement.
    /// </summary>
    /// <remarks>
    ///     These values correspond to the Bluetooth SIG assigned numbers for Generic Access Profile (GAP)
    ///     advertisement data types as defined in the Bluetooth Core Specification.
    /// </remarks>
    public enum AdvertisementRecordType
    {
        /// <summary>
        ///     No advertisement record type specified or unknown type.
        /// </summary>
        None = 0x00,

        /// <summary>
        ///     Flags data type as defined in Bluetooth Core Specification.
        /// </summary>
        Flags = 0x01,

        /// <summary>
        ///     Incomplete list of 16-bit Service Class UUIDs as defined in Bluetooth Core Specification.
        /// </summary>
        UuidsIncomplete16Bit = 0x02,

        /// <summary>
        ///     Complete list of 16-bit Service Class UUIDs as defined in Bluetooth Core Specification.
        /// </summary>
        UuidsComplete16Bit = 0x03,

        /// <summary>
        ///     Incomplete list of 32-bit Service Class UUIDs as defined in Bluetooth Core Specification.
        /// </summary>
        UuidsIncomplete32Bit = 0x04,

        /// <summary>
        ///     Complete list of 32-bit Service Class UUIDs as defined in Bluetooth Core Specification.
        /// </summary>
        UuidCom32Bit = 0x05,

        /// <summary>
        ///     Incomplete list of 128-bit Service Class UUIDs as defined in Bluetooth Core Specification.
        /// </summary>
        UuidsIncomplete128Bit = 0x06,

        /// <summary>
        ///     Complete list of 128-bit Service Class UUIDs as defined in Bluetooth Core Specification.
        /// </summary>
        UuidsComplete128Bit = 0x07,

        /// <summary>
        ///     Shortened local name as defined in Bluetooth Core Specification.
        /// </summary>
        ShortLocalName = 0x08,

        /// <summary>
        ///     Complete local name as defined in Bluetooth Core Specification.
        /// </summary>
        CompleteLocalName = 0x09,

        /// <summary>
        ///     TX Power Level as defined in Bluetooth Core Specification.
        /// </summary>
        TxPowerLevel = 0x0A,

        /// <summary>
        ///     Class of Device as defined in Bluetooth Core Specification.
        /// </summary>
        DeviceClass = 0x0D,

        /// <summary>
        ///     Simple Pairing Hash C / Simple Pairing Hash C-192 as defined in Bluetooth Core Specification.
        /// </summary>
        SimplePairingHash = 0x0E,

        /// <summary>
        ///     Simple Pairing Randomizer R / Simple Pairing Randomizer R-192 as defined in Bluetooth Core Specification.
        /// </summary>
        SimplePairingRandomizer = 0x0F,

        /// <summary>
        ///     Device ID or Security Manager TK Value as defined in Device ID Profile v1.3 or Bluetooth Core Specification.
        /// </summary>
        DeviceId = 0x10,

        /// <summary>
        ///     Security Manager Out of Band Flags as defined in Bluetooth Core Specification.
        /// </summary>
        SecurityManager = 0x11,

        /// <summary>
        ///     Slave Connection Interval Range as defined in Bluetooth Core Specification.
        /// </summary>
        SlaveConnectionInterval = 0x12,

        /// <summary>
        ///     List of 16-bit Service Solicitation UUIDs as defined in Bluetooth Core Specification.
        /// </summary>
        SsUuids16Bit = 0x14,

        /// <summary>
        ///     List of 128-bit Service Solicitation UUIDs as defined in Bluetooth Core Specification.
        /// </summary>
        SsUuids128Bit = 0x15,

        /// <summary>
        ///     Service Data with 16-bit UUID as defined in Bluetooth Core Specification.
        /// </summary>
        ServiceData = 0x16,

        /// <summary>
        ///     Public Target Address as defined in Bluetooth Core Specification.
        /// </summary>
        PublicTargetAddress = 0x17,

        /// <summary>
        ///     Random Target Address as defined in Bluetooth Core Specification.
        /// </summary>
        RandomTargetAddress = 0x18,

        /// <summary>
        ///     Appearance as defined in Bluetooth Core Specification.
        /// </summary>
        Appearance = 0x19,

        /// <summary>
        ///     LE Bluetooth Device Address as defined in Core Specification Supplement.
        /// </summary>
        DeviceAddress = 0x1B,

        /// <summary>
        ///     LE Role as defined in Core Specification Supplement.
        /// </summary>
        LeRole = 0x1C,

        /// <summary>
        ///     Simple Pairing Hash C-256 as defined in Core Specification Supplement.
        /// </summary>
        PairingHash = 0x1D,

        /// <summary>
        ///     Simple Pairing Randomizer R-256 as defined in Core Specification Supplement.
        /// </summary>
        PairingRandomizer = 0x1E,

        /// <summary>
        ///     List of 32-bit Service Solicitation UUIDs as defined in Core Specification Supplement.
        /// </summary>
        SsUuids32Bit = 0x1F,

        /// <summary>
        ///     Service Data with 32-bit UUID as defined in Core Specification Supplement.
        /// </summary>
        ServiceDataUuid32Bit = 0x20,

        /// <summary>
        ///     Service Data with 128-bit UUID as defined in Core Specification Supplement.
        /// </summary>
        ServiceData128Bit = 0x21,

        /// <summary>
        ///     LE Secure Connections Confirmation Value as defined in Core Specification Supplement.
        /// </summary>
        SecureConnectionsConfirmationValue = 0x22,

        /// <summary>
        ///     LE Secure Connections Random Value as defined in Core Specification Supplement.
        /// </summary>
        SecureConnectionsRandomValue = 0x23,

        /// <summary>
        ///     3D Information Data as defined in 3D Synchronization Profile, v1.0 or later.
        /// </summary>
        Information3DData = 0x3D,

        /// <summary>
        ///     Manufacturer Specific Data as defined in Bluetooth Core Specification.
        /// </summary>
        ManufacturerSpecificData = 0xFF,

        /// <summary>
        ///     Indicates whether the device is connectable. This is only reliable for iOS implementation.
        ///     The Android stack does not expose this information to clients.
        /// </summary>
        IsConnectable = 0xAA
    }
}