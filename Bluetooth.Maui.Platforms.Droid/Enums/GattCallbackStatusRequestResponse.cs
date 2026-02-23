// ReSharper disable InconsistentNaming

#pragma warning disable CA1707 // Identifiers should not contain underscores
namespace Bluetooth.Maui.Platforms.Droid.Enums;

/// <summary>
///     Represents GATT request and response status codes for GATT server operations on Android.
/// </summary>
/// <remarks>
///     These status codes are specifically used when responding to GATT requests in a GATT server or broadcaster role.
///     They correspond to ATT (Attribute Protocol) opcodes defined in the Bluetooth specification.
/// </remarks>
public enum GattCallbackStatusRequestResponse
{
    /// <summary>
    ///     The request completed successfully.
    /// </summary>
    GATT_SUCCESS = 0,

    /// <summary>
    ///     Error response indicating the request failed.
    /// </summary>
    GATT_RSP_ERROR = 1,

    /// <summary>
    ///     MTU (Maximum Transmission Unit) exchange request.
    /// </summary>
    GATT_REQ_MTU = 2,

    /// <summary>
    ///     MTU (Maximum Transmission Unit) exchange response.
    /// </summary>
    GATT_RSP_MTU = 3,

    /// <summary>
    ///     Find Information request to discover attribute handles and types.
    /// </summary>
    GATT_REQ_FIND_INFO = 4,

    /// <summary>
    ///     Find Information response containing attribute handles and types.
    /// </summary>
    GATT_RSP_FIND_INFO = 5,

    /// <summary>
    ///     Find By Type Value request to discover attributes by type and value.
    /// </summary>
    GATT_REQ_FIND_TYPE_VALUE = 6,

    /// <summary>
    ///     Find By Type Value response containing matching attributes.
    /// </summary>
    GATT_RSP_FIND_TYPE_VALUE = 7,

    /// <summary>
    ///     Read By Type request to read attribute values by their type.
    /// </summary>
    GATT_REQ_READ_BY_TYPE = 8,

    /// <summary>
    ///     Read By Type response containing attribute values.
    /// </summary>
    GATT_RSP_READ_BY_TYPE = 9,

    /// <summary>
    ///     Read request to retrieve an attribute value.
    /// </summary>
    GATT_REQ_READ = 10,

    /// <summary>
    ///     Read response containing the requested attribute value.
    /// </summary>
    GATT_RSP_READ = 11,

    /// <summary>
    ///     Read Blob request to read part of a long attribute value.
    /// </summary>
    GATT_REQ_READ_BLOB = 12,

    /// <summary>
    ///     Read Blob response containing part of a long attribute value.
    /// </summary>
    GATT_RSP_READ_BLOB = 13,

    /// <summary>
    ///     Read Multiple request to read multiple attribute values in one operation.
    /// </summary>
    GATT_REQ_READ_MULTI = 14,

    /// <summary>
    ///     Read Multiple response containing multiple attribute values.
    /// </summary>
    GATT_RSP_READ_MULTI = 15,

    /// <summary>
    ///     Read By Group Type request to read attributes grouped by type.
    /// </summary>
    GATT_REQ_READ_BY_GRP_TYPE = 16,

    /// <summary>
    ///     Read By Group Type response containing grouped attribute values.
    /// </summary>
    GATT_RSP_READ_BY_GRP_TYPE = 17,

    /// <summary>
    ///     Write request to modify an attribute value.
    /// </summary>
    GATT_REQ_WRITE = 18,

    /// <summary>
    ///     Write response confirming the write operation.
    /// </summary>
    GATT_RSP_WRITE = 19,

    /// <summary>
    ///     Write command (without response) to modify an attribute value.
    /// </summary>
    GATT_CMD_WRITE = 82,

    /// <summary>
    ///     Prepare Write request to queue a write operation.
    /// </summary>
    GATT_REQ_PREPARE_WRITE = 22,

    /// <summary>
    ///     Prepare Write response confirming the queued write.
    /// </summary>
    GATT_RSP_PREPARE_WRITE = 23,

    /// <summary>
    ///     Execute Write request to commit or cancel queued write operations.
    /// </summary>
    GATT_REQ_EXEC_WRITE = 24,

    /// <summary>
    ///     Execute Write response confirming the execution.
    /// </summary>
    GATT_RSP_EXEC_WRITE = 25,

    /// <summary>
    ///     Handle Value Notification sent from server to client without acknowledgment.
    /// </summary>
    GATT_HANDLE_VALUE_NOTIF = 27,

    /// <summary>
    ///     Handle Value Indication sent from server to client requiring acknowledgment.
    /// </summary>
    GATT_HANDLE_VALUE_IND = 29,

    /// <summary>
    ///     Handle Value Confirmation acknowledging a received indication.
    /// </summary>
    GATT_HANDLE_VALUE_CONF = 30,

    /// <summary>
    ///     Signed Write command for authenticated writes without response.
    /// </summary>
    GATT_SIGN_CMD_WRITE = 210
}

#pragma warning restore CA1707 // Identifiers should not contain underscores
