namespace Bluetooth.Abstractions.Broadcasting.Enums;

/// <summary>
/// Defines permissions required for accessing Bluetooth GATT characteristic operations.
/// </summary>
[Flags]
public enum BluetoothCharacteristicPermissions
{
    /// <summary>
    /// No permissions required.
    /// </summary>
    None = 0,

    /// <summary>
    /// Permission to read the characteristic value.
    /// </summary>
    Read = 1 << 0,
    
    /// <summary>
    /// Permission to write the characteristic value.
    /// </summary>
    Write = 1 << 1,

    /// <summary>
    /// Permission to read the characteristic value with encryption required.
    /// </summary>
    ReadEncrypted = 1 << 2,
    
    /// <summary>
    /// Permission to read the characteristic value with authenticated encryption required (MITM protection).
    /// </summary>
    ReadAuthenticated = 1 << 3,

    /// <summary>
    /// Permission to write the characteristic value with encryption required.
    /// </summary>
    WriteEncrypted = 1 << 4,
    
    /// <summary>
    /// Permission to write the characteristic value with authenticated encryption required (MITM protection).
    /// </summary>
    WriteAuthenticated = 1 << 5,

    /// <summary>
    /// Permission to write the characteristic value with signature required.
    /// </summary>
    WriteSigned = 1 << 6,
    
    /// <summary>
    /// Permission to write the characteristic value with authenticated signature required.
    /// </summary>
    WriteSignedAuthenticated = 1 << 7,
}