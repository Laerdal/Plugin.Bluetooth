namespace Bluetooth.Maui.Platforms.Win.Broadcasting;

public partial class WindowsBluetoothLocalCharacteristic
{
    [LoggerMessage(EventId = 6200, Level = LogLevel.Debug,
        Message = "Windows read request received for characteristic {CharacteristicId} in service {ServiceId} from device {DeviceId} with offset {Offset}")]
    partial void LogReadRequestReceived(Guid characteristicId, Guid serviceId, string deviceId, int offset);

    [LoggerMessage(EventId = 6201, Level = LogLevel.Warning,
        Message = "Windows read request rejected due to invalid offset for characteristic {CharacteristicId} in service {ServiceId} (Offset={Offset}, ValueLength={ValueLength})")]
    partial void LogReadRequestInvalidOffset(Guid characteristicId, Guid serviceId, int offset, int valueLength);

    [LoggerMessage(EventId = 6202, Level = LogLevel.Debug,
        Message = "Windows read request succeeded for characteristic {CharacteristicId} in service {ServiceId} with {ByteCount} response bytes")]
    partial void LogReadRequestSucceeded(Guid characteristicId, Guid serviceId, int byteCount);

    [LoggerMessage(EventId = 6203, Level = LogLevel.Warning,
        Message = "Windows read request mapped to RequestNotSupported for characteristic {CharacteristicId} in service {ServiceId}")]
    partial void LogReadRequestMappedToRequestNotSupported(Guid characteristicId, Guid serviceId, Exception exception);

    [LoggerMessage(EventId = 6204, Level = LogLevel.Warning,
        Message = "Windows read request mapped to InvalidAttributeValueLength for characteristic {CharacteristicId} in service {ServiceId}")]
    partial void LogReadRequestMappedToInvalidValueLength(Guid characteristicId, Guid serviceId, Exception exception);

    [LoggerMessage(EventId = 6205, Level = LogLevel.Error,
        Message = "Windows read request failed for characteristic {CharacteristicId} in service {ServiceId}")]
    partial void LogReadRequestFailed(Guid characteristicId, Guid serviceId, Exception exception);

    [LoggerMessage(EventId = 6210, Level = LogLevel.Debug,
        Message = "Windows write request received for characteristic {CharacteristicId} in service {ServiceId} from device {DeviceId} with option {WriteOption}, offset {Offset}, and {ByteCount} bytes")]
    partial void LogWriteRequestReceived(Guid characteristicId,
        Guid serviceId,
        string deviceId,
        GattWriteOption writeOption,
        int offset,
        int byteCount);

    [LoggerMessage(EventId = 6211, Level = LogLevel.Warning,
        Message = "Windows write request rejected due to unsupported offset for characteristic {CharacteristicId} in service {ServiceId} (Offset={Offset})")]
    partial void LogWriteRequestInvalidOffset(Guid characteristicId, Guid serviceId, int offset);

    [LoggerMessage(EventId = 6212, Level = LogLevel.Warning,
        Message = "Windows write request rejected because characteristic {CharacteristicId} in service {ServiceId} does not support option {WriteOption}")]
    partial void LogWriteRequestNotPermitted(Guid characteristicId, Guid serviceId, GattWriteOption writeOption);

    [LoggerMessage(EventId = 6213, Level = LogLevel.Debug,
        Message = "Windows write request succeeded for characteristic {CharacteristicId} in service {ServiceId}")]
    partial void LogWriteRequestSucceeded(Guid characteristicId, Guid serviceId);

    [LoggerMessage(EventId = 6214, Level = LogLevel.Warning,
        Message = "Windows write request mapped to RequestNotSupported for characteristic {CharacteristicId} in service {ServiceId}")]
    partial void LogWriteRequestMappedToRequestNotSupported(Guid characteristicId, Guid serviceId, Exception exception);

    [LoggerMessage(EventId = 6215, Level = LogLevel.Warning,
        Message = "Windows write request mapped to InvalidAttributeValueLength for characteristic {CharacteristicId} in service {ServiceId}")]
    partial void LogWriteRequestMappedToInvalidValueLength(Guid characteristicId, Guid serviceId, Exception exception);

    [LoggerMessage(EventId = 6216, Level = LogLevel.Error,
        Message = "Windows write request failed for characteristic {CharacteristicId} in service {ServiceId}")]
    partial void LogWriteRequestFailed(Guid characteristicId, Guid serviceId, Exception exception);

    [LoggerMessage(EventId = 6220, Level = LogLevel.Information,
        Message = "Windows client {DeviceId} subscribed to characteristic {CharacteristicId} in service {ServiceId}")]
    partial void LogSubscriptionAdded(string deviceId, Guid characteristicId, Guid serviceId);

    [LoggerMessage(EventId = 6221, Level = LogLevel.Information,
        Message = "Windows client {DeviceId} unsubscribed from characteristic {CharacteristicId} in service {ServiceId}")]
    partial void LogSubscriptionRemoved(string deviceId, Guid characteristicId, Guid serviceId);

    [LoggerMessage(EventId = 6222, Level = LogLevel.Information,
        Message = "Removed tracked Windows subscription during characteristic dispose for device {DeviceId}, characteristic {CharacteristicId}, service {ServiceId}")]
    partial void LogSubscriptionDisposeCleanup(string deviceId, Guid characteristicId, Guid serviceId);

    [LoggerMessage(EventId = 6230, Level = LogLevel.Debug,
        Message = "Dispatching Windows notify for characteristic {CharacteristicId} in service {ServiceId} with {ByteCount} bytes to {SubscribedClientCount} subscribed clients")]
    partial void LogNotifyDispatch(Guid characteristicId, Guid serviceId, int byteCount, int subscribedClientCount);

    [LoggerMessage(EventId = 6231, Level = LogLevel.Debug,
        Message = "Skipping Windows notify for characteristic {CharacteristicId} in service {ServiceId} because notifyClients is false")]
    partial void LogNotifySkipped(Guid characteristicId, Guid serviceId);
}
