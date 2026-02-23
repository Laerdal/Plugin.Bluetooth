namespace Bluetooth.Core.Broadcasting;

public abstract partial class BaseBluetoothLocalCharacteristic
{
    #region LoggerMessage Definitions (EventId 700-799)

    // Read operations (700-709)
    [LoggerMessage(EventId = 700, Level = LogLevel.Debug,
        Message = "Local characteristic {CharacteristicId} in service {ServiceId} - read request received from device {DeviceId}")]
    partial void LogReadRequest(Guid characteristicId, Guid serviceId, string deviceId);

    [LoggerMessage(EventId = 701, Level = LogLevel.Debug,
        Message = "Local characteristic {CharacteristicId} in service {ServiceId} - read request responded with {ByteCount} bytes")]
    partial void LogReadResponse(Guid characteristicId, Guid serviceId, int byteCount);

    [LoggerMessage(EventId = 702, Level = LogLevel.Error,
        Message = "Local characteristic {CharacteristicId} in service {ServiceId} - read request failed")]
    partial void LogReadRequestFailed(Guid characteristicId, Guid serviceId, Exception exception);

    // Write operations (710-719)
    [LoggerMessage(EventId = 710, Level = LogLevel.Debug,
        Message = "Local characteristic {CharacteristicId} in service {ServiceId} - write request received from device {DeviceId} with {ByteCount} bytes")]
    partial void LogWriteRequest(Guid characteristicId, Guid serviceId, string deviceId, int byteCount);

    [LoggerMessage(EventId = 711, Level = LogLevel.Debug,
        Message = "Local characteristic {CharacteristicId} in service {ServiceId} - write request completed")]
    partial void LogWriteRequestCompleted(Guid characteristicId, Guid serviceId);

    [LoggerMessage(EventId = 712, Level = LogLevel.Error,
        Message = "Local characteristic {CharacteristicId} in service {ServiceId} - write request failed")]
    partial void LogWriteRequestFailed(Guid characteristicId, Guid serviceId, Exception exception);

    // Notify/Indicate operations (720-729)
    [LoggerMessage(EventId = 720, Level = LogLevel.Information,
        Message = "Local characteristic {CharacteristicId} in service {ServiceId} - sending notification to device {DeviceId} with {ByteCount} bytes")]
    partial void LogSendingNotification(Guid characteristicId, Guid serviceId, string deviceId, int byteCount);

    [LoggerMessage(EventId = 721, Level = LogLevel.Information,
        Message = "Local characteristic {CharacteristicId} in service {ServiceId} - notification sent successfully to device {DeviceId}")]
    partial void LogNotificationSent(Guid characteristicId, Guid serviceId, string deviceId);

    [LoggerMessage(EventId = 722, Level = LogLevel.Error,
        Message = "Local characteristic {CharacteristicId} in service {ServiceId} - failed to send notification to device {DeviceId}")]
    partial void LogNotificationFailed(Guid characteristicId, Guid serviceId, string deviceId, Exception exception);

    [LoggerMessage(EventId = 723, Level = LogLevel.Information,
        Message = "Local characteristic {CharacteristicId} in service {ServiceId} - device {DeviceId} subscribed to notifications")]
    partial void LogClientSubscribed(Guid characteristicId, Guid serviceId, string deviceId);

    [LoggerMessage(EventId = 724, Level = LogLevel.Information,
        Message = "Local characteristic {CharacteristicId} in service {ServiceId} - device {DeviceId} unsubscribed from notifications")]
    partial void LogClientUnsubscribed(Guid characteristicId, Guid serviceId, string deviceId);

    // Value operations (730-739)
    [LoggerMessage(EventId = 730, Level = LogLevel.Debug,
        Message = "Local characteristic {CharacteristicId} in service {ServiceId} - value updated with {ByteCount} bytes")]
    partial void LogValueUpdated(Guid characteristicId, Guid serviceId, int byteCount);

    #endregion
}
