namespace Bluetooth.Core.Scanning;

public abstract partial class BaseBluetoothRemoteService
{
    #region LoggerMessage Definitions (EventId 300-399)

    // Characteristic exploration (300-319)
    [LoggerMessage(EventId = 300, Level = LogLevel.Information,
        Message = "Service {ServiceId} on device {DeviceId} exploring characteristics")]
    partial void LogExploringCharacteristics(Guid serviceId, string deviceId);

    [LoggerMessage(EventId = 301, Level = LogLevel.Information,
        Message = "Service {ServiceId} on device {DeviceId} characteristic exploration succeeded - {CharacteristicCount} characteristics found")]
    partial void LogCharacteristicExplorationSucceeded(Guid serviceId, string deviceId, int characteristicCount);

    [LoggerMessage(EventId = 302, Level = LogLevel.Error,
        Message = "Service {ServiceId} on device {DeviceId} characteristic exploration failed")]
    partial void LogCharacteristicExplorationFailed(Guid serviceId, string deviceId, Exception exception);

    [LoggerMessage(EventId = 303, Level = LogLevel.Warning,
        Message = "Service {ServiceId} on device {DeviceId} characteristic exploration - unexpected completion without pending operation")]
    partial void LogUnexpectedCharacteristicExploration(Guid serviceId, string deviceId);

    [LoggerMessage(EventId = 304, Level = LogLevel.Debug,
        Message = "Service {ServiceId} on device {DeviceId} merging concurrent characteristic exploration attempts")]
    partial void LogMergingCharacteristicExploration(Guid serviceId, string deviceId);

    [LoggerMessage(EventId = 305, Level = LogLevel.Debug,
        Message = "Service {ServiceId} on device {DeviceId} using cached characteristics - {CharacteristicCount} characteristics available")]
    partial void LogUsingCachedCharacteristics(Guid serviceId, string deviceId, int characteristicCount);

    [LoggerMessage(EventId = 306, Level = LogLevel.Debug,
        Message = "Service {ServiceId} on device {DeviceId} cascading exploration to {CharacteristicCount} characteristics")]
    partial void LogCascadingToDescriptors(Guid serviceId, string deviceId, int characteristicCount);

    [LoggerMessage(EventId = 307, Level = LogLevel.Debug,
        Message = "Service {ServiceId} on device {DeviceId} cleared {CharacteristicCount} characteristics")]
    partial void LogCharacteristicsCleared(Guid serviceId, string deviceId, int characteristicCount);

    #endregion
}