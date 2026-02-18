namespace Bluetooth.Abstractions.Broadcasting.Enums;

/// <summary>
/// Defines properties that describe how a Bluetooth GATT characteristic can be accessed.
/// </summary>
[Flags]
public enum BluetoothCharacteristicProperties
{
    /// <summary>
    /// No properties defined.
    /// </summary>
    None = 0,

    /// <summary>
    /// The characteristic supports read operations.
    /// </summary>
    Read = 1 << 0,
    
    /// <summary>
    /// The characteristic supports write operations with response.
    /// </summary>
    Write = 1 << 1,
    
    /// <summary>
    /// The characteristic supports write operations without response.
    /// </summary>
    WriteWithoutResponse = 1 << 2,
    
    /// <summary>
    /// The characteristic supports notifications without acknowledgment.
    /// </summary>
    Notify = 1 << 3,
    
    /// <summary>
    /// The characteristic supports indications with acknowledgment.
    /// </summary>
    Indicate = 1 << 4,

    /// <summary>
    /// The characteristic supports broadcast operations.
    /// </summary>
    Broadcast = 1 << 5,
    
    /// <summary>
    /// The characteristic supports signed write operations.
    /// </summary>
    SignedWrite = 1 << 6,
    
    /// <summary>
    /// The characteristic has extended properties defined in a descriptor.
    /// </summary>
    ExtendedProperties = 1 << 7,
}