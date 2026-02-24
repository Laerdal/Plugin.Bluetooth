// ReSharper disable InconsistentNaming

#pragma warning disable CA1707 // Identifiers should not contain underscores
namespace Bluetooth.Maui.Platforms.Droid.Enums;

/// <summary>
///     Represents GATT operation status codes returned by Android Bluetooth operations.
/// </summary>
/// <remarks>
///     These status codes align with the Bluetooth specification and Android's BluetoothGatt status values.
/// </remarks>
public enum GattCallbackStatus
{
    /// <summary>
    ///     The operation completed successfully.
    /// </summary>
    GATT_SUCCESS = 0x00,

    /// <summary>
    ///     The attribute handle is invalid.
    /// </summary>
    GATT_INVALID_HANDLE = 0x01,

    /// <summary>
    ///     Reading is not permitted on this attribute.
    /// </summary>
    GATT_READ_NOT_PERMIT = 0x02,

    /// <summary>
    ///     Writing is not permitted on this attribute.
    /// </summary>
    GATT_WRITE_NOT_PERMIT = 0x03,

    /// <summary>
    ///     The PDU (Protocol Data Unit) is invalid.
    /// </summary>
    GATT_INVALID_PDU = 0x04,

    /// <summary>
    ///     Insufficient authentication for the operation.
    /// </summary>
    GATT_INSUF_AUTHENTICATION = 0x05,

    /// <summary>
    ///     The requested operation is not supported.
    /// </summary>
    GATT_REQ_NOT_SUPPORTED = 0x06,

    /// <summary>
    ///     The specified offset is invalid.
    /// </summary>
    GATT_INVALID_OFFSET = 0x07,

    /// <summary>
    ///     Insufficient authorization for the operation.
    /// </summary>
    GATT_INSUF_AUTHORIZATION = 0x08,

    /// <summary>
    ///     The prepare write queue is full.
    /// </summary>
    GATT_PREPARE_Q_FULL = 0x09,

    /// <summary>
    ///     The requested attribute was not found.
    /// </summary>
    GATT_NOT_FOUND = 0x0a,

    /// <summary>
    ///     The attribute is not long enough for the operation.
    /// </summary>
    GATT_NOT_LONG = 0x0b,

    /// <summary>
    ///     The encryption key size is insufficient.
    /// </summary>
    GATT_INSUF_KEY_SIZE = 0x0c,

    /// <summary>
    ///     The attribute value length is invalid.
    /// </summary>
    GATT_INVALID_ATTR_LEN = 0x0d,

    /// <summary>
    ///     The operation failed for an unlikely reason.
    /// </summary>
    GATT_ERR_UNLIKELY = 0x0e,

    /// <summary>
    ///     Insufficient encryption for the operation.
    /// </summary>
    GATT_INSUF_ENCRYPTION = 0x0f,

    /// <summary>
    ///     The attribute group type is not supported.
    /// </summary>
    GATT_UNSUPPORT_GRP_TYPE = 0x10,

    /// <summary>
    ///     Insufficient resources to complete the operation.
    /// </summary>
    GATT_INSUF_RESOURCE = 0x11,

    /// <summary>
    ///     Connection terminated due to LMP (Link Manager Protocol) timeout.
    /// </summary>
    GATT_CONN_LMP_TIMEOUT = 0x22,

    /// <summary>
    ///     The Bluetooth controller is busy.
    /// </summary>
    GATT_CONTROLLER_BUSY = 0x3A,

    /// <summary>
    ///     The connection interval is unacceptable.
    /// </summary>
    GATT_UNACCEPT_CONN_INTERVAL = 0x3B,

    /// <summary>
    ///     An illegal parameter was provided.
    /// </summary>
    GATT_ILLEGAL_PARAMETER = 0x87,

    /// <summary>
    ///     No resources are available for the operation.
    /// </summary>
    GATT_NO_RESOURCES = 0x80,

    /// <summary>
    ///     An internal error occurred.
    /// </summary>
    GATT_INTERNAL_ERROR = 0x81,

    /// <summary>
    ///     The operation cannot be performed in the current state.
    /// </summary>
    GATT_WRONG_STATE = 0x82,

    /// <summary>
    ///     The GATT database is full.
    /// </summary>
    GATT_DB_FULL = 0x83,

    /// <summary>
    ///     The GATT server is busy processing another spec.
    /// </summary>
    GATT_BUSY = 0x84,

    /// <summary>
    ///     A generic GATT error occurred.
    /// </summary>
    GATT_ERROR = 0x85,

    /// <summary>
    ///     The GATT command has been started.
    /// </summary>
    GATT_CMD_STARTED = 0x86,

    /// <summary>
    ///     The operation is pending.
    /// </summary>
    GATT_PENDING = 0x88,

    /// <summary>
    ///     Authentication failed.
    /// </summary>
    GATT_AUTH_FAIL = 0x89,

    /// <summary>
    ///     More data is available.
    /// </summary>
    GATT_MORE = 0x8a,

    /// <summary>
    ///     The configuration is invalid.
    /// </summary>
    GATT_INVALID_CFG = 0x8b,

    /// <summary>
    ///     The GATT service has been started.
    /// </summary>
    GATT_SERVICE_STARTED = 0x8c,

    /// <summary>
    ///     The connection is encrypted but without Man-in-the-Middle (MITM) protection.
    /// </summary>
    GATT_ENCRYPTED_NO_MITM = 0x8d,

    /// <summary>
    ///     The connection is not encrypted.
    /// </summary>
    GATT_NOT_ENCRYPTED = 0x8e,

    /// <summary>
    ///     The connection is congested.
    /// </summary>
    GATT_CONGESTED = 0x8f,

    /// <summary>
    ///     Client Characteristic Configuration Descriptor (CCCD) is improperly configured.
    /// </summary>
    GATT_CCCD_CFG_ERROR = 0xFD,

    /// <summary>
    ///     A GATT procedure is already in progress.
    /// </summary>
    GATT_PROCEDURE_IN_PROGRESS = 0xFE,

    /// <summary>
    ///     The attribute value is out of the acceptable range.
    /// </summary>
    GATT_VALUE_OUT_OF_RANGE = 0xFF
}

#pragma warning restore CA1707 // Identifiers should not contain underscores
