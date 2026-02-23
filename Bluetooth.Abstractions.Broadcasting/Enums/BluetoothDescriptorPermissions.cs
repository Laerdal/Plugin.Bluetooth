namespace Bluetooth.Abstractions.Broadcasting.Enums;

/// <summary>
///     Defines permissions required for accessing Bluetooth GATT descriptor operations.
/// </summary>
[Flags]
public enum BluetoothDescriptorPermissions
{
    /// <summary>
    ///     No permissions required.
    /// </summary>
    None = 0,

    /// <summary>
    ///     Permission to read the descriptor value.
    /// </summary>
    Read = 1 << 0,

    /// <summary>
    ///     Permission to write the descriptor value.
    /// </summary>
    Write = 1 << 1,

    /// <summary>
    ///     Permission to read the descriptor value with encryption required.
    /// </summary>
    ReadEncrypted = 1 << 2,

    /// <summary>
    ///     Permission to read the descriptor value with authenticated encryption required (MITM protection).
    /// </summary>
    ReadAuthenticated = 1 << 3,

    /// <summary>
    ///     Permission to write the descriptor value with encryption required.
    /// </summary>
    WriteEncrypted = 1 << 4,

    /// <summary>
    ///     Permission to write the descriptor value with authenticated encryption required (MITM protection).
    /// </summary>
    WriteAuthenticated = 1 << 5
}
