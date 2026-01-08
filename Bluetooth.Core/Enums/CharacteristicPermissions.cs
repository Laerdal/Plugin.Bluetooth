namespace Bluetooth.Core.Enums;

/// <summary>
/// Defines the permissions required to access a GATT characteristic.
/// </summary>
[Flags]
public enum CharacteristicPermissions
{
    /// <summary>
    /// No permissions are required.
    /// </summary>
    None = 0,

    /// <summary>
    /// Reading the characteristic is allowed.
    /// </summary>
    Read = 1 << 0,

    /// <summary>
    /// Reading the characteristic requires an encrypted connection.
    /// </summary>
    ReadEncrypted = 1 << 1,

    /// <summary>
    /// Reading the characteristic requires an encrypted connection with MITM protection.
    /// </summary>
    ReadEncryptedMitm = 1 << 2,

    /// <summary>
    /// Writing to the characteristic is allowed.
    /// </summary>
    Write = 1 << 3,

    /// <summary>
    /// Writing to the characteristic requires an encrypted connection.
    /// </summary>
    WriteEncrypted = 1 << 4,

    /// <summary>
    /// Writing to the characteristic requires an encrypted connection with MITM protection.
    /// </summary>
    WriteEncryptedMitm = 1 << 5,

    /// <summary>
    /// Writing to the characteristic requires a signed write.
    /// </summary>
    WriteSigned = 1 << 6,

    /// <summary>
    /// Writing to the characteristic requires a signed write with MITM protection.
    /// </summary>
    WriteSignedMitm = 1 << 7
}
