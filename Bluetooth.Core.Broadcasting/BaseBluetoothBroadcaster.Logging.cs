namespace Bluetooth.Core.Broadcasting;

public abstract partial class BaseBluetoothBroadcaster
{
    #region LoggerMessage Definitions (EventId 500-599)

    // Start/Stop operations (500-519)
    [LoggerMessage(EventId = 500, Level = LogLevel.Information,
        Message = "Broadcaster starting")]
    partial void LogBroadcasterStarting();

    [LoggerMessage(EventId = 501, Level = LogLevel.Information,
        Message = "Broadcaster started successfully")]
    partial void LogBroadcasterStarted();

    [LoggerMessage(EventId = 502, Level = LogLevel.Error,
        Message = "Broadcaster failed to start")]
    partial void LogBroadcasterStartFailed(Exception exception);

    [LoggerMessage(EventId = 503, Level = LogLevel.Information,
        Message = "Broadcaster stopping")]
    partial void LogBroadcasterStopping();

    [LoggerMessage(EventId = 504, Level = LogLevel.Information,
        Message = "Broadcaster stopped successfully")]
    partial void LogBroadcasterStopped();

    [LoggerMessage(EventId = 505, Level = LogLevel.Error,
        Message = "Broadcaster failed to stop")]
    partial void LogBroadcasterStopFailed(Exception exception);

    [LoggerMessage(EventId = 506, Level = LogLevel.Debug,
        Message = "Broadcaster merging concurrent start attempts")]
    partial void LogMergingStartAttempts();

    [LoggerMessage(EventId = 507, Level = LogLevel.Debug,
        Message = "Broadcaster merging concurrent stop attempts")]
    partial void LogMergingStopAttempts();

    [LoggerMessage(EventId = 508, Level = LogLevel.Warning,
        Message = "Broadcaster start - unexpected completion without pending operation")]
    partial void LogUnexpectedStart();

    [LoggerMessage(EventId = 509, Level = LogLevel.Warning,
        Message = "Broadcaster stop - unexpected completion without pending operation")]
    partial void LogUnexpectedStop();

    [LoggerMessage(EventId = 510, Level = LogLevel.Error,
        Message = "Broadcaster already started")]
    partial void LogBroadcasterAlreadyStarted();

    [LoggerMessage(EventId = 511, Level = LogLevel.Error,
        Message = "Broadcaster already stopped")]
    partial void LogBroadcasterAlreadyStopped();

    // Configuration operations (512-519)
    [LoggerMessage(EventId = 512, Level = LogLevel.Information,
        Message = "Broadcaster updating configuration")]
    partial void LogUpdatingConfiguration();

    [LoggerMessage(EventId = 513, Level = LogLevel.Information,
        Message = "Broadcaster configuration updated successfully")]
    partial void LogConfigurationUpdated();

    [LoggerMessage(EventId = 514, Level = LogLevel.Error,
        Message = "Broadcaster configuration update failed")]
    partial void LogConfigurationUpdateFailed(Exception exception);

    // Service list operations (520-539)
    [LoggerMessage(EventId = 520, Level = LogLevel.Information,
        Message = "Broadcaster adding service {ServiceId}")]
    partial void LogAddingService(Guid serviceId);

    [LoggerMessage(EventId = 521, Level = LogLevel.Information,
        Message = "Broadcaster service {ServiceId} added successfully")]
    partial void LogServiceAdded(Guid serviceId);

    [LoggerMessage(EventId = 522, Level = LogLevel.Error,
        Message = "Broadcaster failed to add service {ServiceId}")]
    partial void LogServiceAddFailed(Guid serviceId, Exception exception);

    [LoggerMessage(EventId = 523, Level = LogLevel.Information,
        Message = "Broadcaster removing service {ServiceId}")]
    partial void LogRemovingService(Guid serviceId);

    [LoggerMessage(EventId = 524, Level = LogLevel.Information,
        Message = "Broadcaster service {ServiceId} removed successfully")]
    partial void LogServiceRemoved(Guid serviceId);

    [LoggerMessage(EventId = 525, Level = LogLevel.Error,
        Message = "Broadcaster failed to remove service {ServiceId}")]
    partial void LogServiceRemoveFailed(Guid serviceId, Exception exception);

    [LoggerMessage(EventId = 526, Level = LogLevel.Error,
        Message = "Broadcaster service {ServiceId} not found")]
    partial void LogServiceNotFound(Guid serviceId);

    [LoggerMessage(EventId = 527, Level = LogLevel.Error,
        Message = "Broadcaster service {ServiceId} already exists")]
    partial void LogServiceAlreadyExists(Guid serviceId);

    [LoggerMessage(EventId = 528, Level = LogLevel.Information,
        Message = "Broadcaster clearing all services")]
    partial void LogClearingServices();

    [LoggerMessage(EventId = 529, Level = LogLevel.Information,
        Message = "Broadcaster services cleared - {ServiceCount} services removed")]
    partial void LogServicesCleared(int serviceCount);

    // Client device list operations (540-559)
    [LoggerMessage(EventId = 540, Level = LogLevel.Information,
        Message = "Broadcaster client device {DeviceId} connected")]
    partial void LogClientConnected(string deviceId);

    [LoggerMessage(EventId = 541, Level = LogLevel.Information,
        Message = "Broadcaster client device {DeviceId} disconnected")]
    partial void LogClientDisconnected(string deviceId);

    [LoggerMessage(EventId = 542, Level = LogLevel.Debug,
        Message = "Broadcaster client device list updated - {DeviceCount} devices")]
    partial void LogClientListUpdated(int deviceCount);

    [LoggerMessage(EventId = 543, Level = LogLevel.Error,
        Message = "Broadcaster client device {DeviceId} not found")]
    partial void LogClientDeviceNotFound(string deviceId);

    [LoggerMessage(EventId = 544, Level = LogLevel.Warning,
        Message = "Broadcaster multiple client devices found with ID {DeviceId}")]
    partial void LogMultipleClientsFound(string deviceId);

    #endregion
}