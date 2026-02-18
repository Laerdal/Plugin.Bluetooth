using Microsoft.Extensions.Logging;

namespace Bluetooth.Core.Scanning;

public abstract partial class BaseBluetoothRemoteCharacteristic
{
    #region LoggerMessage Definitions (EventId 400-499)

    // Read operations (400-419)
    [LoggerMessage(EventId = 400, Level = LogLevel.Debug,
        Message = "Characteristic {CharacteristicId} on device {DeviceId} reading value")]
    partial void LogReadingValue(Guid characteristicId, string deviceId);

    [LoggerMessage(EventId = 401, Level = LogLevel.Debug,
        Message = "Characteristic {CharacteristicId} on device {DeviceId} read succeeded - {ByteCount} bytes")]
    partial void LogReadSucceeded(Guid characteristicId, string deviceId, int byteCount);

    [LoggerMessage(EventId = 402, Level = LogLevel.Error,
        Message = "Characteristic {CharacteristicId} on device {DeviceId} read failed")]
    partial void LogReadFailed(Guid characteristicId, string deviceId, Exception exception);

    [LoggerMessage(EventId = 403, Level = LogLevel.Warning,
        Message = "Characteristic {CharacteristicId} on device {DeviceId} read - unexpected completion without pending operation")]
    partial void LogUnexpectedRead(Guid characteristicId, string deviceId);

    [LoggerMessage(EventId = 404, Level = LogLevel.Debug,
        Message = "Characteristic {CharacteristicId} on device {DeviceId} merging concurrent read attempts")]
    partial void LogMergingReadAttempts(Guid characteristicId, string deviceId);

    [LoggerMessage(EventId = 405, Level = LogLevel.Error,
        Message = "Characteristic {CharacteristicId} on device {DeviceId} cannot read - not connected")]
    partial void LogReadNotConnected(Guid characteristicId, string deviceId);

    [LoggerMessage(EventId = 406, Level = LogLevel.Error,
        Message = "Characteristic {CharacteristicId} on device {DeviceId} cannot read - characteristic not readable")]
    partial void LogCharacteristicNotReadable(Guid characteristicId, string deviceId);

    // Write operations (420-439)
    [LoggerMessage(EventId = 420, Level = LogLevel.Debug,
        Message = "Characteristic {CharacteristicId} on device {DeviceId} writing value - {ByteCount} bytes")]
    partial void LogWritingValue(Guid characteristicId, string deviceId, int byteCount);

    [LoggerMessage(EventId = 421, Level = LogLevel.Debug,
        Message = "Characteristic {CharacteristicId} on device {DeviceId} write succeeded")]
    partial void LogWriteSucceeded(Guid characteristicId, string deviceId);

    [LoggerMessage(EventId = 422, Level = LogLevel.Error,
        Message = "Characteristic {CharacteristicId} on device {DeviceId} write failed")]
    partial void LogWriteFailed(Guid characteristicId, string deviceId, Exception exception);

    [LoggerMessage(EventId = 423, Level = LogLevel.Warning,
        Message = "Characteristic {CharacteristicId} on device {DeviceId} write - unexpected completion without pending operation")]
    partial void LogUnexpectedWrite(Guid characteristicId, string deviceId);

    [LoggerMessage(EventId = 424, Level = LogLevel.Debug,
        Message = "Characteristic {CharacteristicId} on device {DeviceId} queuing write operation")]
    partial void LogQueuingWrite(Guid characteristicId, string deviceId);

    [LoggerMessage(EventId = 425, Level = LogLevel.Error,
        Message = "Characteristic {CharacteristicId} on device {DeviceId} cannot write - not connected")]
    partial void LogWriteNotConnected(Guid characteristicId, string deviceId);

    [LoggerMessage(EventId = 426, Level = LogLevel.Error,
        Message = "Characteristic {CharacteristicId} on device {DeviceId} cannot write - characteristic not writable")]
    partial void LogCharacteristicNotWritable(Guid characteristicId, string deviceId);

    // Listen / Notify operations (440-459)
    [LoggerMessage(EventId = 440, Level = LogLevel.Information,
        Message = "Characteristic {CharacteristicId} on device {DeviceId} starting notifications")]
    partial void LogStartingNotifications(Guid characteristicId, string deviceId);

    [LoggerMessage(EventId = 441, Level = LogLevel.Information,
        Message = "Characteristic {CharacteristicId} on device {DeviceId} notifications started successfully")]
    partial void LogNotificationsStarted(Guid characteristicId, string deviceId);

    [LoggerMessage(EventId = 442, Level = LogLevel.Error,
        Message = "Characteristic {CharacteristicId} on device {DeviceId} failed to start notifications")]
    partial void LogNotificationsStartFailed(Guid characteristicId, string deviceId, Exception exception);

    [LoggerMessage(EventId = 443, Level = LogLevel.Information,
        Message = "Characteristic {CharacteristicId} on device {DeviceId} stopping notifications")]
    partial void LogStoppingNotifications(Guid characteristicId, string deviceId);

    [LoggerMessage(EventId = 444, Level = LogLevel.Information,
        Message = "Characteristic {CharacteristicId} on device {DeviceId} notifications stopped successfully")]
    partial void LogNotificationsStopped(Guid characteristicId, string deviceId);

    [LoggerMessage(EventId = 445, Level = LogLevel.Error,
        Message = "Characteristic {CharacteristicId} on device {DeviceId} failed to stop notifications")]
    partial void LogNotificationsStopFailed(Guid characteristicId, string deviceId, Exception exception);

    [LoggerMessage(EventId = 446, Level = LogLevel.Error,
        Message = "Characteristic {CharacteristicId} on device {DeviceId} cannot start notifications - not connected")]
    partial void LogNotificationsNotConnected(Guid characteristicId, string deviceId);

    [LoggerMessage(EventId = 447, Level = LogLevel.Error,
        Message = "Characteristic {CharacteristicId} on device {DeviceId} cannot start notifications - characteristic does not support notifications")]
    partial void LogCharacteristicNotNotifiable(Guid characteristicId, string deviceId);

    [LoggerMessage(EventId = 448, Level = LogLevel.Debug,
        Message = "Characteristic {CharacteristicId} on device {DeviceId} value updated - {ByteCount} bytes")]
    partial void LogValueUpdated(Guid characteristicId, string deviceId, int byteCount);

    [LoggerMessage(EventId = 449, Level = LogLevel.Debug,
        Message = "Characteristic {CharacteristicId} on device {DeviceId} merging concurrent notification start attempts")]
    partial void LogMergingNotificationAttempts(Guid characteristicId, string deviceId);

    #endregion
}
