// ReSharper disable InconsistentNaming

#pragma warning disable CA1707 // Identifiers should not contain underscores
namespace Bluetooth.Maui.Platforms.Droid.Enums;

/// <summary>
///     Represents GATT connection status codes returned by Android Bluetooth connection operations.
/// </summary>
/// <remarks>
///     These status codes are specifically used when responding to GATT connection status events
///     and differ from general GATT operation status codes.
/// </remarks>
public enum GattCallbackStatusConnection
{
    /// <summary>
    ///     The connection operation completed successfully.
    /// </summary>
    GATT_SUCCESS = 0,

    /// <summary>
    ///     L2CAP (Logical Link Control and Adaptation Protocol) connection failure.
    /// </summary>
    GATT_CONN_L2C_FAILURE = 0x01,

    /// <summary>
    ///     The connection attempt timed out.
    /// </summary>
    GATT_CONN_TIMEOUT = 0x08,

    /// <summary>
    ///     The connection was terminated by the remote peer user.
    /// </summary>
    GATT_CONN_TERMINATE_PEER_USER = 0x13,

    /// <summary>
    ///     The connection was terminated by the local host.
    /// </summary>
    GATT_CONN_TERMINATE_LOCAL_HOST = 0x16,

    /// <summary>
    ///     Failed to establish the connection.
    /// </summary>
    GATT_CONN_FAIL_ESTABLISH = 0x3E,

    /// <summary>
    ///     Connection terminated due to LMP (Link Manager Protocol) timeout.
    /// </summary>
    GATT_CONN_LMP_TIMEOUT = 0x22,

    /// <summary>
    ///     Too many open connections exist.
    /// </summary>
    GATT_TOO_MANY_OPEN_CONNECTIONS = 0x0101,

    /// <summary>
    ///     The connection attempt was cancelled.
    /// </summary>
    GATT_CONN_CANCEL = 0x0100,

    /// <summary>
    ///     A generic GATT error occurred during the connection operation.
    /// </summary>
    GATT_ERROR = 0x85
}

#pragma warning restore CA1707 // Identifiers should not contain underscores
