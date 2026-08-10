namespace Bluetooth.Maui.Platforms.Win.Broadcasting;

public partial class WindowsBluetoothBroadcaster
{
    [LoggerMessage(EventId = 6100, Level = LogLevel.Information,
        Message = "Starting Windows broadcaster with {AdvertisedServiceCount} advertised services and IncludeDeviceName={IncludeDeviceName}")]
    partial void LogNativeStartRequested(int advertisedServiceCount, bool includeDeviceName);

    [LoggerMessage(EventId = 6101, Level = LogLevel.Information,
        Message = "Windows broadcaster native start completed (AdvertisedLocalServiceCount={ServiceCount})")]
    partial void LogNativeStartCompleted(int serviceCount);

    [LoggerMessage(EventId = 6102, Level = LogLevel.Information,
        Message = "Stopping Windows broadcaster")]
    partial void LogNativeStopRequested();

    [LoggerMessage(EventId = 6103, Level = LogLevel.Information,
        Message = "Windows broadcaster native stop completed")]
    partial void LogNativeStopCompleted();

    [LoggerMessage(EventId = 6104, Level = LogLevel.Information,
        Message = "Windows advertisement publisher status changed to {Status} (ErrorCode={ErrorCode}, TxPowerDbm={TxPowerDbm})")]
    partial void LogAdvertisementPublisherStatusChanged(BluetoothLEAdvertisementPublisherStatus status,
        BluetoothError errorCode,
        short? txPowerDbm);

    [LoggerMessage(EventId = 6105, Level = LogLevel.Error,
        Message = "Windows advertisement publisher reported a non-success error: {ErrorCode}")]
    partial void LogAdvertisementPublisherStatusError(BluetoothError errorCode, Exception exception);

    [LoggerMessage(EventId = 6106, Level = LogLevel.Information,
        Message = "Started advertising for Windows local service {ServiceId}")]
    partial void LogServiceAdvertisingStarted(Guid serviceId);

    [LoggerMessage(EventId = 6107, Level = LogLevel.Information,
        Message = "Stopped advertising for Windows local service {ServiceId}")]
    partial void LogServiceAdvertisingStopped(Guid serviceId);

    [LoggerMessage(EventId = 6108, Level = LogLevel.Information,
        Message = "Created Windows local service {ServiceId} (IsPrimary={IsPrimary})")]
    partial void LogNativeServiceCreated(Guid serviceId, bool isPrimary);

    [LoggerMessage(EventId = 6109, Level = LogLevel.Information,
        Message = "Created Windows client device {DeviceId}")]
    partial void LogClientDeviceCreated(string deviceId);

    [LoggerMessage(EventId = 6110, Level = LogLevel.Debug,
        Message = "Updated Windows client device {DeviceId} native client reference")]
    partial void LogClientDeviceUpdated(string deviceId);

    [LoggerMessage(EventId = 6111, Level = LogLevel.Information,
        Message = "Removed Windows client device {DeviceId} because it no longer had active subscriptions")]
    partial void LogClientDeviceRemovedNoSubscriptions(string deviceId);

    [LoggerMessage(EventId = 6112, Level = LogLevel.Information,
        Message = "Removed Windows client device {DeviceId} during broadcaster stop")]
    partial void LogClientDeviceRemovedOnStop(string deviceId);
}
