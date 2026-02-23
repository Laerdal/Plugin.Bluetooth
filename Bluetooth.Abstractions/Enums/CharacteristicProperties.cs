namespace Bluetooth.Abstractions.Enums;

/// <summary>
///     Defines the properties of a GATT characteristic, specifying the operations that can be performed on it.
/// </summary>
[Flags]
public enum CharacteristicProperties
{
    /// <summary>
    ///     No properties are set.
    /// </summary>
    None = 0,

    /// <summary>
    ///     The characteristic supports broadcasting its value.
    /// </summary>
    Broadcast = 1 << 0,

    /// <summary>
    ///     The characteristic value can be read.
    /// </summary>
    Read = 1 << 1,

    /// <summary>
    ///     The characteristic value can be written without response.
    /// </summary>
    WriteWithoutResponse = 1 << 2,

    /// <summary>
    ///     The characteristic value can be written.
    /// </summary>
    Write = 1 << 3,

    /// <summary>
    ///     The characteristic supports notifications (without acknowledgment from the client).
    /// </summary>
    Notify = 1 << 4,

    /// <summary>
    ///     The characteristic supports indications (with acknowledgment from the client).
    /// </summary>
    Indicate = 1 << 5,

    /// <summary>
    ///     The characteristic supports signed writes.
    /// </summary>
    AuthenticatedSignedWrites = 1 << 6,

    /// <summary>
    ///     The characteristic has extended properties.
    /// </summary>
    ExtendedProperties = 1 << 7,

    /// <summary>
    ///     The characteristic supports reliable writes (queued writes with verification).
    /// </summary>
    ReliableWrite = 1 << 8,

    /// <summary>
    ///     The characteristic's User Description descriptor is writable.
    /// </summary>
    WritableAuxiliaries = 1 << 9
}
