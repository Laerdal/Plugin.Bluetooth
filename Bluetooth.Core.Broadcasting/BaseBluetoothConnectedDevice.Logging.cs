namespace Bluetooth.Core.Broadcasting;

public abstract partial class BaseBluetoothConnectedDevice
{
    #region LoggerMessage Definitions (EventId 800-899)

    // Connection operations (800-819)
    [LoggerMessage(EventId = 800, Level = LogLevel.Information,
        Message = "Connected device {DeviceId} connected")]
    partial void LogDeviceConnected(string deviceId);

    [LoggerMessage(EventId = 801, Level = LogLevel.Information,
        Message = "Connected device {DeviceId} disconnected")]
    partial void LogDeviceDisconnected(string deviceId);

    [LoggerMessage(EventId = 802, Level = LogLevel.Information,
        Message = "Connected device {DeviceId} disconnecting")]
    partial void LogDeviceDisconnecting(string deviceId);

    [LoggerMessage(EventId = 803, Level = LogLevel.Debug,
        Message = "Connected device {DeviceId} connection status changed to {IsConnected}")]
    partial void LogConnectionStatusChanged(string deviceId, bool isConnected);

    [LoggerMessage(EventId = 804, Level = LogLevel.Error,
        Message = "Connected device {DeviceId} failed to disconnect")]
    partial void LogDisconnectFailed(string deviceId, Exception exception);

    // MTU operations (820-829)
    [LoggerMessage(EventId = 820, Level = LogLevel.Information,
        Message = "Connected device {DeviceId} MTU changed from {OldMtu} to {NewMtu}")]
    partial void LogMtuChanged(string deviceId, int oldMtu, int newMtu);

    [LoggerMessage(EventId = 821, Level = LogLevel.Debug,
        Message = "Connected device {DeviceId} current MTU: {Mtu}")]
    partial void LogCurrentMtu(string deviceId, int mtu);

    // Subscription operations (830-849)
    [LoggerMessage(EventId = 830, Level = LogLevel.Information,
        Message = "Connected device {DeviceId} subscribed to characteristic {CharacteristicId} in service {ServiceId}")]
    partial void LogCharacteristicSubscriptionAdded(string deviceId, Guid characteristicId, Guid serviceId);

    [LoggerMessage(EventId = 831, Level = LogLevel.Information,
        Message = "Connected device {DeviceId} unsubscribed from characteristic {CharacteristicId} in service {ServiceId}")]
    partial void LogCharacteristicSubscriptionRemoved(string deviceId, Guid characteristicId, Guid serviceId);

    [LoggerMessage(EventId = 832, Level = LogLevel.Information,
        Message = "Connected device {DeviceId} cleared all characteristic subscriptions - {SubscriptionCount} subscriptions removed")]
    partial void LogAllSubscriptionsCleared(string deviceId, int subscriptionCount);

    [LoggerMessage(EventId = 833, Level = LogLevel.Debug,
        Message = "Connected device {DeviceId} has {SubscriptionCount} active subscriptions")]
    partial void LogSubscriptionCount(string deviceId, int subscriptionCount);

    #endregion
}