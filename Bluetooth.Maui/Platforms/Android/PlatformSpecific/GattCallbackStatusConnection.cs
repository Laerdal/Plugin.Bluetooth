// ReSharper disable InconsistentNaming
#pragma warning disable CA1707 // Identifiers should not contain underscores
namespace Bluetooth.Maui.PlatformSpecific;

// ONLY USED WHEN RESPONDING TO GATT CONNECTION STATUS
public enum GattCallbackStatusConnection
{
    GATT_SUCCESS = 0,

    GATT_CONN_L2C_FAILURE = 0x01,

    GATT_CONN_TIMEOUT = 0x08,

    GATT_CONN_TERMINATE_PEER_USER = 0x13,

    GATT_CONN_TERMINATE_LOCAL_HOST = 0x16,

    GATT_CONN_FAIL_ESTABLISH = 0x3E,

    GATT_CONN_LMP_TIMEOUT = 0x22,

    GATT_TOO_MANY_OPEN_CONNECTIONS = 0x0101,

    GATT_CONN_CANCEL = 0x0100,

    GATT_ERROR = 0x85
}

#pragma warning restore CA1707 // Identifiers should not contain underscores
