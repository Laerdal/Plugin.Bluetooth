namespace Bluetooth.Abstractions.Enums;

/// <summary>
///     Defines the transport type for Bluetooth connections.
/// </summary>
public enum TransportType
{
    /// <summary>
    ///     Automatically select transport based on device capabilities (default).
    /// </summary>
    /// <remarks>
    ///     Android: TRANSPORT_AUTO
    /// </remarks>
    Auto = 0,

    /// <summary>
    ///     Use Bluetooth Low Energy (LE) transport.
    /// </summary>
    /// <remarks>
    ///     Android: TRANSPORT_LE
    ///     iOS/macOS: Always used (only option available)
    /// </remarks>
    Le = 2,

    /// <summary>
    ///     Use classic Bluetooth (BR/EDR) transport.
    /// </summary>
    /// <remarks>
    ///     Android: TRANSPORT_BREDR
    ///     Not supported for BLE operations
    /// </remarks>
    BrEdr = 1
}
