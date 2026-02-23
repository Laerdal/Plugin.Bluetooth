namespace Bluetooth.Core.Scanning;

public abstract partial class BaseBluetoothRemoteDevice
{
    #region LoggerMessage Definitions (EventId 200-299)

    // Connection operations (200-209)
    [LoggerMessage(EventId = 200, Level = LogLevel.Information,
        Message = "Device {DeviceId} connecting")]
    partial void LogDeviceConnecting(string deviceId);

    [LoggerMessage(EventId = 201, Level = LogLevel.Information,
        Message = "Device {DeviceId} connected successfully")]
    partial void LogDeviceConnected(string deviceId);

    [LoggerMessage(EventId = 202, Level = LogLevel.Error,
        Message = "Device {DeviceId} failed to connect")]
    partial void LogDeviceConnectionFailed(string deviceId, Exception exception);

    [LoggerMessage(EventId = 203, Level = LogLevel.Warning,
        Message = "Device {DeviceId} is already connected")]
    partial void LogDeviceAlreadyConnected(string deviceId);

    [LoggerMessage(EventId = 204, Level = LogLevel.Debug,
        Message = "Device {DeviceId} merging concurrent connection attempts")]
    partial void LogMergingConnectionAttempts(string deviceId);

    [LoggerMessage(EventId = 205, Level = LogLevel.Debug,
        Message = "Device {DeviceId} waiting for advertisement before connecting")]
    partial void LogWaitingForAdvertisement(string deviceId);

    // Disconnection operations (210-219)
    [LoggerMessage(EventId = 210, Level = LogLevel.Information,
        Message = "Device {DeviceId} disconnecting")]
    partial void LogDeviceDisconnecting(string deviceId);

    [LoggerMessage(EventId = 211, Level = LogLevel.Information,
        Message = "Device {DeviceId} disconnected successfully")]
    partial void LogDeviceDisconnected(string deviceId);

    [LoggerMessage(EventId = 212, Level = LogLevel.Error,
        Message = "Device {DeviceId} failed to disconnect")]
    partial void LogDeviceDisconnectionFailed(string deviceId, Exception exception);

    [LoggerMessage(EventId = 213, Level = LogLevel.Warning,
        Message = "Device {DeviceId} is already disconnected")]
    partial void LogDeviceAlreadyDisconnected(string deviceId);

    [LoggerMessage(EventId = 214, Level = LogLevel.Debug,
        Message = "Device {DeviceId} merging concurrent disconnection attempts")]
    partial void LogMergingDisconnectionAttempts(string deviceId);

    // Unexpected disconnection (220-229)
    [LoggerMessage(EventId = 220, Level = LogLevel.Warning,
        Message = "Device {DeviceId} unexpectedly disconnected")]
    partial void LogUnexpectedDisconnection(string deviceId, Exception? exception);

    [LoggerMessage(EventId = 221, Level = LogLevel.Debug,
        Message = "Device {DeviceId} unexpected disconnection ignored (IgnoreNextUnexpectedDisconnection was true)")]
    partial void LogUnexpectedDisconnectionIgnored(string deviceId);

    // Connection priority (230-239)
    [LoggerMessage(EventId = 230, Level = LogLevel.Information,
        Message = "Device {DeviceId} requesting connection priority: {Priority}")]
    partial void LogRequestingConnectionPriority(string deviceId, BluetoothConnectionPriority priority);

    [LoggerMessage(EventId = 231, Level = LogLevel.Error,
        Message = "Device {DeviceId} cannot request connection priority - not connected")]
    partial void LogConnectionPriorityNotConnected(string deviceId);

    // Service exploration (240-259)
    [LoggerMessage(EventId = 240, Level = LogLevel.Information,
        Message = "Device {DeviceId} exploring services")]
    partial void LogExploringServices(string deviceId);

    [LoggerMessage(EventId = 241, Level = LogLevel.Information,
        Message = "Device {DeviceId} service exploration succeeded - {ServiceCount} services found")]
    partial void LogServiceExplorationSucceeded(string deviceId, int serviceCount);

    [LoggerMessage(EventId = 242, Level = LogLevel.Error,
        Message = "Device {DeviceId} service exploration failed")]
    partial void LogServiceExplorationFailed(string deviceId, Exception exception);

    [LoggerMessage(EventId = 243, Level = LogLevel.Warning,
        Message = "Device {DeviceId} service exploration - unexpected completion without pending operation")]
    partial void LogUnexpectedServiceExploration(string deviceId);

    [LoggerMessage(EventId = 244, Level = LogLevel.Debug,
        Message = "Device {DeviceId} merging concurrent service exploration attempts")]
    partial void LogMergingServiceExploration(string deviceId);

    [LoggerMessage(EventId = 245, Level = LogLevel.Error,
        Message = "Device {DeviceId} cannot explore services - not connected")]
    partial void LogServiceExplorationNotConnected(string deviceId);

    [LoggerMessage(EventId = 246, Level = LogLevel.Debug,
        Message = "Device {DeviceId} using cached services - {ServiceCount} services available")]
    partial void LogUsingCachedServices(string deviceId, int serviceCount);

    [LoggerMessage(EventId = 247, Level = LogLevel.Debug,
        Message = "Device {DeviceId} cascading exploration to {ServiceCount} services")]
    partial void LogCascadingToCharacteristics(string deviceId, int serviceCount);

    [LoggerMessage(EventId = 248, Level = LogLevel.Debug,
        Message = "Device {DeviceId} cleared {ServiceCount} services")]
    partial void LogServicesCleared(string deviceId, int serviceCount);

    #endregion
}
