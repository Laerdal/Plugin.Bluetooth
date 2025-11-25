// ReSharper disable InconsistentNaming
#pragma warning disable CA1707 // Identifiers should not contain underscores
namespace Bluetooth.Maui.PlatformSpecific;

// ONLY USED WHEN RESPONDING TO GATT REQUESTS IN GATT SERVER / BROADCASTER
public enum GattCallbackStatusRequestResponse
{
    GATT_SUCCESS = 0,

    GATT_RSP_ERROR = 1,

    GATT_REQ_MTU = 2,

    GATT_RSP_MTU = 3,

    GATT_REQ_FIND_INFO = 4,

    GATT_RSP_FIND_INFO = 5,

    GATT_REQ_FIND_TYPE_VALUE = 6,

    GATT_RSP_FIND_TYPE_VALUE = 7,

    GATT_REQ_READ_BY_TYPE = 8,

    GATT_RSP_READ_BY_TYPE = 9,

    GATT_REQ_READ = 10,

    GATT_RSP_READ = 11,

    GATT_REQ_READ_BLOB = 12,

    GATT_RSP_READ_BLOB = 13,

    GATT_REQ_READ_MULTI = 14,

    GATT_RSP_READ_MULTI = 15,

    GATT_REQ_READ_BY_GRP_TYPE = 16,

    GATT_RSP_READ_BY_GRP_TYPE = 17,

    GATT_REQ_WRITE = 18,

    GATT_RSP_WRITE = 19,

    GATT_CMD_WRITE = 82,

    GATT_REQ_PREPARE_WRITE = 22,

    GATT_RSP_PREPARE_WRITE = 23,

    GATT_REQ_EXEC_WRITE = 24,

    GATT_RSP_EXEC_WRITE = 25,

    GATT_HANDLE_VALUE_NOTIF = 27,

    GATT_HANDLE_VALUE_IND = 29,

    GATT_HANDLE_VALUE_CONF = 30,

    GATT_SIGN_CMD_WRITE = 210
}

#pragma warning restore CA1707 // Identifiers should not contain underscores
