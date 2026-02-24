namespace Bluetooth.Maui.Platforms.Apple.Logging;

/// <summary>
///     High-performance logging messages for Apple (iOS/macOS) Bluetooth operations using LoggerMessage source generation.
/// </summary>
/// <remarks>
///     LoggerMessage delegates provide:
///     - Zero-allocation logging
///     - Compile-time validation
///     - Better performance than string interpolation
///     - Strongly-typed parameters
/// </remarks>
internal static partial class AppleBluetoothLoggerMessages
{
    #region Scanner Logging

    [LoggerMessage(
        EventId = 1000,
        Level = LogLevel.Information,
        Message = "Starting BLE scan with mode: {ScanMode}, callback type: {CallbackType}")]
    public static partial void LogScanStarting(
        this ILogger logger,
        BluetoothScanMode scanMode,
        BluetoothScanCallbackType callbackType);

    [LoggerMessage(
        EventId = 1001,
        Level = LogLevel.Information,
        Message = "BLE scan started successfully")]
    public static partial void LogScanStarted(this ILogger logger);

    [LoggerMessage(
        EventId = 1002,
        Level = LogLevel.Warning,
        Message = "BLE scan start attempt {Attempt} of {MaxRetries} failed")]
    public static partial void LogScanStartRetry(
        this ILogger logger,
        int attempt,
        int maxRetries,
        Exception exception);

    [LoggerMessage(
        EventId = 1003,
        Level = LogLevel.Error,
        Message = "BLE scan failed to start after {Attempts} attempts")]
    public static partial void LogScanStartFailed(
        this ILogger logger,
        int attempts,
        Exception exception);

    [LoggerMessage(
        EventId = 1004,
        Level = LogLevel.Information,
        Message = "Stopping BLE scan")]
    public static partial void LogScanStopping(this ILogger logger);

    [LoggerMessage(
        EventId = 1005,
        Level = LogLevel.Information,
        Message = "BLE scan stopped successfully")]
    public static partial void LogScanStopped(this ILogger logger);

    [LoggerMessage(
        EventId = 1006,
        Level = LogLevel.Warning,
        Message = "BLE scan state error: {Error}")]
    public static partial void LogScanStateError(
        this ILogger logger,
        string error);

    [LoggerMessage(
        EventId = 1007,
        Level = LogLevel.Debug,
        Message = "Device discovered: {DeviceAddress}, RSSI: {Rssi}")]
    public static partial void LogDeviceDiscovered(
        this ILogger logger,
        string deviceAddress,
        int rssi);

    [LoggerMessage(
        EventId = 1008,
        Level = LogLevel.Information,
        Message = "CBCentralManager state changed to: {State}")]
    public static partial void LogCentralManagerStateChanged(
        this ILogger logger,
        CBManagerState state);

    #endregion

    #region Connection Logging

    [LoggerMessage(
        EventId = 2000,
        Level = LogLevel.Information,
        Message = "Connecting to device {DeviceAddress}")]
    public static partial void LogConnecting(
        this ILogger logger,
        string deviceAddress);

    [LoggerMessage(
        EventId = 2001,
        Level = LogLevel.Information,
        Message = "Successfully connected to device {DeviceAddress}")]
    public static partial void LogConnected(
        this ILogger logger,
        string deviceAddress);

    [LoggerMessage(
        EventId = 2002,
        Level = LogLevel.Warning,
        Message = "Connection attempt {Attempt} of {MaxRetries} to device {DeviceAddress} failed")]
    public static partial void LogConnectionRetry(
        this ILogger logger,
        int attempt,
        int maxRetries,
        string deviceAddress,
        Exception exception);

    [LoggerMessage(
        EventId = 2003,
        Level = LogLevel.Error,
        Message = "Failed to connect to device {DeviceAddress} after {Attempts} attempts")]
    public static partial void LogConnectionFailed(
        this ILogger logger,
        string deviceAddress,
        int attempts,
        Exception exception);

    [LoggerMessage(
        EventId = 2004,
        Level = LogLevel.Information,
        Message = "Disconnecting from device {DeviceAddress}")]
    public static partial void LogDisconnecting(
        this ILogger logger,
        string deviceAddress);

    [LoggerMessage(
        EventId = 2005,
        Level = LogLevel.Information,
        Message = "Successfully disconnected from device {DeviceAddress}")]
    public static partial void LogDisconnected(
        this ILogger logger,
        string deviceAddress);

    [LoggerMessage(
        EventId = 2006,
        Level = LogLevel.Error,
        Message = "Connection to device {DeviceAddress} failed with error: {ErrorMessage}")]
    public static partial void LogConnectionError(
        this ILogger logger,
        string deviceAddress,
        string errorMessage,
        Exception? exception);

    [LoggerMessage(
        EventId = 2007,
        Level = LogLevel.Information,
        Message = "Device {DeviceAddress} name updated to: {NewName}")]
    public static partial void LogDeviceNameUpdated(
        this ILogger logger,
        string deviceAddress,
        string newName);

    [LoggerMessage(
        EventId = 2008,
        Level = LogLevel.Debug,
        Message = "Device {DeviceAddress} ready to send write without response")]
    public static partial void LogReadyToSendWriteWithoutResponse(
        this ILogger logger,
        string deviceAddress);

    #endregion

    #region Service Discovery Logging

    [LoggerMessage(
        EventId = 3000,
        Level = LogLevel.Information,
        Message = "Starting service discovery for device {DeviceAddress}")]
    public static partial void LogServiceDiscoveryStarting(
        this ILogger logger,
        string deviceAddress);

    [LoggerMessage(
        EventId = 3001,
        Level = LogLevel.Information,
        Message = "Service discovery completed for device {DeviceAddress}, found {ServiceCount} services")]
    public static partial void LogServiceDiscoveryCompleted(
        this ILogger logger,
        string deviceAddress,
        int serviceCount);

    [LoggerMessage(
        EventId = 3002,
        Level = LogLevel.Warning,
        Message = "Service discovery attempt {Attempt} of {MaxRetries} failed for device {DeviceAddress}")]
    public static partial void LogServiceDiscoveryRetry(
        this ILogger logger,
        int attempt,
        int maxRetries,
        string deviceAddress,
        Exception exception);

    [LoggerMessage(
        EventId = 3003,
        Level = LogLevel.Error,
        Message = "Service discovery failed for device {DeviceAddress} after {Attempts} attempts")]
    public static partial void LogServiceDiscoveryFailed(
        this ILogger logger,
        string deviceAddress,
        int attempts,
        Exception exception);

    [LoggerMessage(
        EventId = 3004,
        Level = LogLevel.Error,
        Message = "Service discovery error for device {DeviceAddress}: {ErrorMessage}")]
    public static partial void LogServiceDiscoveryError(
        this ILogger logger,
        string deviceAddress,
        string errorMessage,
        Exception? exception);

    [LoggerMessage(
        EventId = 3005,
        Level = LogLevel.Information,
        Message = "Services modified on device {DeviceAddress}, invalidating {ServiceCount} services")]
    public static partial void LogServicesModified(
        this ILogger logger,
        string deviceAddress,
        int serviceCount);

    [LoggerMessage(
        EventId = 3006,
        Level = LogLevel.Information,
        Message = "Discovering characteristics for service {ServiceId} on device {DeviceAddress}")]
    public static partial void LogCharacteristicDiscoveryStarting(
        this ILogger logger,
        Guid serviceId,
        string deviceAddress);

    [LoggerMessage(
        EventId = 3007,
        Level = LogLevel.Information,
        Message = "Discovered {CharacteristicCount} characteristics for service {ServiceId} on device {DeviceAddress}")]
    public static partial void LogCharacteristicDiscoveryCompleted(
        this ILogger logger,
        Guid serviceId,
        string deviceAddress,
        int characteristicCount);

    [LoggerMessage(
        EventId = 3008,
        Level = LogLevel.Error,
        Message = "Characteristic discovery error for service {ServiceId} on device {DeviceAddress}: {ErrorMessage}")]
    public static partial void LogCharacteristicDiscoveryError(
        this ILogger logger,
        Guid serviceId,
        string deviceAddress,
        string errorMessage,
        Exception? exception);

    #endregion

    #region GATT Operation Logging

    [LoggerMessage(
        EventId = 4000,
        Level = LogLevel.Debug,
        Message = "Reading characteristic {CharacteristicId} from device {DeviceAddress}")]
    public static partial void LogCharacteristicRead(
        this ILogger logger,
        Guid characteristicId,
        string deviceAddress);

    [LoggerMessage(
        EventId = 4001,
        Level = LogLevel.Debug,
        Message = "Read {ByteCount} bytes from characteristic {CharacteristicId} on device {DeviceAddress}")]
    public static partial void LogCharacteristicReadCompleted(
        this ILogger logger,
        Guid characteristicId,
        string deviceAddress,
        int byteCount);

    [LoggerMessage(
        EventId = 4002,
        Level = LogLevel.Error,
        Message = "Read failed for characteristic {CharacteristicId} on device {DeviceAddress}: {ErrorMessage}")]
    public static partial void LogCharacteristicReadError(
        this ILogger logger,
        Guid characteristicId,
        string deviceAddress,
        string errorMessage,
        Exception? exception);

    [LoggerMessage(
        EventId = 4003,
        Level = LogLevel.Debug,
        Message = "Writing {ByteCount} bytes to characteristic {CharacteristicId} on device {DeviceAddress}")]
    public static partial void LogCharacteristicWrite(
        this ILogger logger,
        Guid characteristicId,
        string deviceAddress,
        int byteCount);

    [LoggerMessage(
        EventId = 4004,
        Level = LogLevel.Debug,
        Message = "Write completed for characteristic {CharacteristicId} on device {DeviceAddress}")]
    public static partial void LogCharacteristicWriteCompleted(
        this ILogger logger,
        Guid characteristicId,
        string deviceAddress);

    [LoggerMessage(
        EventId = 4005,
        Level = LogLevel.Error,
        Message = "Write failed for characteristic {CharacteristicId} on device {DeviceAddress}: {ErrorMessage}")]
    public static partial void LogCharacteristicWriteError(
        this ILogger logger,
        Guid characteristicId,
        string deviceAddress,
        string errorMessage,
        Exception? exception);

    [LoggerMessage(
        EventId = 4006,
        Level = LogLevel.Warning,
        Message = "Characteristic write attempt {Attempt} of {MaxRetries} failed for {CharacteristicId} on device {DeviceAddress}")]
    public static partial void LogCharacteristicWriteRetry(
        this ILogger logger,
        int attempt,
        int maxRetries,
        Guid characteristicId,
        string deviceAddress,
        Exception exception);

    [LoggerMessage(
        EventId = 4007,
        Level = LogLevel.Debug,
        Message = "Reading descriptor {DescriptorId} from device {DeviceAddress}")]
    public static partial void LogDescriptorRead(
        this ILogger logger,
        Guid descriptorId,
        string deviceAddress);

    [LoggerMessage(
        EventId = 4008,
        Level = LogLevel.Debug,
        Message = "Read {ByteCount} bytes from descriptor {DescriptorId} on device {DeviceAddress}")]
    public static partial void LogDescriptorReadCompleted(
        this ILogger logger,
        Guid descriptorId,
        string deviceAddress,
        int byteCount);

    [LoggerMessage(
        EventId = 4009,
        Level = LogLevel.Error,
        Message = "Read failed for descriptor {DescriptorId} on device {DeviceAddress}: {ErrorMessage}")]
    public static partial void LogDescriptorReadError(
        this ILogger logger,
        Guid descriptorId,
        string deviceAddress,
        string errorMessage,
        Exception? exception);

    [LoggerMessage(
        EventId = 4010,
        Level = LogLevel.Debug,
        Message = "Writing {ByteCount} bytes to descriptor {DescriptorId} on device {DeviceAddress}")]
    public static partial void LogDescriptorWrite(
        this ILogger logger,
        Guid descriptorId,
        string deviceAddress,
        int byteCount);

    [LoggerMessage(
        EventId = 4011,
        Level = LogLevel.Debug,
        Message = "Write completed for descriptor {DescriptorId} on device {DeviceAddress}")]
    public static partial void LogDescriptorWriteCompleted(
        this ILogger logger,
        Guid descriptorId,
        string deviceAddress);

    [LoggerMessage(
        EventId = 4012,
        Level = LogLevel.Error,
        Message = "Write failed for descriptor {DescriptorId} on device {DeviceAddress}: {ErrorMessage}")]
    public static partial void LogDescriptorWriteError(
        this ILogger logger,
        Guid descriptorId,
        string deviceAddress,
        string errorMessage,
        Exception? exception);

    [LoggerMessage(
        EventId = 4013,
        Level = LogLevel.Warning,
        Message = "Descriptor write attempt {Attempt} of {MaxRetries} failed for {DescriptorId} on device {DeviceAddress}")]
    public static partial void LogDescriptorWriteRetry(
        this ILogger logger,
        int attempt,
        int maxRetries,
        Guid descriptorId,
        string deviceAddress,
        Exception exception);

    #endregion

    #region Notification Logging

    [LoggerMessage(
        EventId = 5000,
        Level = LogLevel.Debug,
        Message = "{Action} notifications for characteristic {CharacteristicId} on device {DeviceAddress}")]
    public static partial void LogNotificationStateChange(
        this ILogger logger,
        string action,
        Guid characteristicId,
        string deviceAddress);

    [LoggerMessage(
        EventId = 5001,
        Level = LogLevel.Trace,
        Message = "Notification received for characteristic {CharacteristicId} on device {DeviceAddress}, {ByteCount} bytes")]
    public static partial void LogNotificationReceived(
        this ILogger logger,
        Guid characteristicId,
        string deviceAddress,
        int byteCount);

    [LoggerMessage(
        EventId = 5002,
        Level = LogLevel.Error,
        Message = "Notification state change failed for characteristic {CharacteristicId} on device {DeviceAddress}: {ErrorMessage}")]
    public static partial void LogNotificationStateChangeError(
        this ILogger logger,
        Guid characteristicId,
        string deviceAddress,
        string errorMessage,
        Exception? exception);

    #endregion

    #region MTU Logging

    [LoggerMessage(
        EventId = 6000,
        Level = LogLevel.Debug,
        Message = "MTU for device {DeviceAddress} changed to {Mtu}")]
    public static partial void LogMtuChanged(
        this ILogger logger,
        string deviceAddress,
        int mtu);

    #endregion

    #region Broadcaster Logging

    [LoggerMessage(
        EventId = 7000,
        Level = LogLevel.Error,
        Message = "Error stopping broadcaster")]
    public static partial void LogBroadcasterStopError(
        this ILogger logger,
        Exception exception);

    #endregion

    #region L2CAP Channel Logging

    [LoggerMessage(
        EventId = 8000,
        Level = LogLevel.Information,
        Message = "Opening L2CAP channel to PSM {Psm} for device {DeviceId}")]
    public static partial void LogL2CapChannelOpening(
        this ILogger logger,
        int psm,
        string deviceId);

    [LoggerMessage(
        EventId = 8001,
        Level = LogLevel.Information,
        Message = "L2CAP channel opened successfully to PSM {Psm}, MTU: {Mtu}")]
    public static partial void LogL2CapChannelOpened(
        this ILogger logger,
        int psm,
        int mtu);

    [LoggerMessage(
        EventId = 8002,
        Level = LogLevel.Error,
        Message = "Failed to open L2CAP channel to PSM {Psm}")]
    public static partial void LogL2CapChannelOpenFailed(
        this ILogger logger,
        int psm,
        Exception exception);

    [LoggerMessage(
        EventId = 8003,
        Level = LogLevel.Debug,
        Message = "Closing L2CAP channel for PSM {Psm}")]
    public static partial void LogL2CapChannelClosing(
        this ILogger logger,
        int psm);

    [LoggerMessage(
        EventId = 8004,
        Level = LogLevel.Information,
        Message = "L2CAP channel closed successfully for PSM {Psm}")]
    public static partial void LogL2CapChannelClosed(
        this ILogger logger,
        int psm);

    [LoggerMessage(
        EventId = 8005,
        Level = LogLevel.Debug,
        Message = "Reading from L2CAP channel PSM {Psm}, buffer size: {BufferSize}")]
    public static partial void LogL2CapChannelReading(
        this ILogger logger,
        int psm,
        int bufferSize);

    [LoggerMessage(
        EventId = 8006,
        Level = LogLevel.Debug,
        Message = "Read {BytesRead} bytes from L2CAP channel PSM {Psm}")]
    public static partial void LogL2CapChannelRead(
        this ILogger logger,
        int psm,
        int bytesRead);

    [LoggerMessage(
        EventId = 8007,
        Level = LogLevel.Debug,
        Message = "Writing {DataLength} bytes to L2CAP channel PSM {Psm}")]
    public static partial void LogL2CapChannelWriting(
        this ILogger logger,
        int psm,
        int dataLength);

    [LoggerMessage(
        EventId = 8008,
        Level = LogLevel.Debug,
        Message = "Wrote successfully to L2CAP channel PSM {Psm}")]
    public static partial void LogL2CapChannelWritten(
        this ILogger logger,
        int psm);

    [LoggerMessage(
        EventId = 8009,
        Level = LogLevel.Debug,
        Message = "Data received on L2CAP channel PSM {Psm}, {BytesReceived} bytes")]
    public static partial void LogL2CapDataReceived(
        this ILogger logger,
        int psm,
        int bytesReceived);

    [LoggerMessage(
        EventId = 8010,
        Level = LogLevel.Error,
        Message = "Error in L2CAP stream delegate for PSM {Psm}")]
    public static partial void LogL2CapStreamError(
        this ILogger logger,
        int psm,
        Exception exception);

    [LoggerMessage(
        EventId = 8011,
        Level = LogLevel.Error,
        Message = "Error closing L2CAP channel for PSM {Psm}")]
    public static partial void LogL2CapChannelCloseError(
        this ILogger logger,
        int psm,
        Exception exception);

    #endregion
}
