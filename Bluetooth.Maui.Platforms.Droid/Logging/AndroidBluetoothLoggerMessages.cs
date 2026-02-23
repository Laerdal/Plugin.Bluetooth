namespace Bluetooth.Maui.Platforms.Droid.Logging;

/// <summary>
///     High-performance logging messages for Android Bluetooth operations using LoggerMessage source generation.
/// </summary>
/// <remarks>
///     LoggerMessage delegates provide:
///     - Zero-allocation logging
///     - Compile-time validation
///     - Better performance than string interpolation
///     - Strongly-typed parameters
/// </remarks>
internal static partial class AndroidBluetoothLoggerMessages
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
        Message = "BLE scan failure received with error code: {ErrorCode}")]
    public static partial void LogScanFailure(
        this ILogger logger,
        string errorCode);

    [LoggerMessage(
        EventId = 1007,
        Level = LogLevel.Debug,
        Message = "Device discovered: {DeviceAddress}, RSSI: {Rssi}")]
    public static partial void LogDeviceDiscovered(
        this ILogger logger,
        string deviceAddress,
        int rssi);

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
        Level = LogLevel.Warning,
        Message = "Failed to auto-apply ConnectionPriority {Priority} for device {DeviceAddress}. Connection remains active.")]
    public static partial void LogConnectionPriorityFailed(
        this ILogger logger,
        BluetoothConnectionPriority priority,
        string deviceAddress,
        Exception exception);

    [LoggerMessage(
        EventId = 2007,
        Level = LogLevel.Debug,
        Message = "Applied ConnectionPriority {Priority} to device {DeviceAddress}")]
    public static partial void LogConnectionPriorityApplied(
        this ILogger logger,
        BluetoothConnectionPriority priority,
        string deviceAddress);

    [LoggerMessage(
        EventId = 2008,
        Level = LogLevel.Error,
        Message = "Error during disconnect for device {DeviceAddress}")]
    public static partial void LogDisconnectError(
        this ILogger logger,
        string deviceAddress,
        Exception exception);

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
        Message = "Writing {ByteCount} bytes to characteristic {CharacteristicId} on device {DeviceAddress}")]
    public static partial void LogCharacteristicWrite(
        this ILogger logger,
        Guid characteristicId,
        string deviceAddress,
        int byteCount);

    [LoggerMessage(
        EventId = 4002,
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
        EventId = 4003,
        Level = LogLevel.Debug,
        Message = "Reading descriptor {DescriptorId} from device {DeviceAddress}")]
    public static partial void LogDescriptorRead(
        this ILogger logger,
        Guid descriptorId,
        string deviceAddress);

    [LoggerMessage(
        EventId = 4004,
        Level = LogLevel.Debug,
        Message = "Writing {ByteCount} bytes to descriptor {DescriptorId} on device {DeviceAddress}")]
    public static partial void LogDescriptorWrite(
        this ILogger logger,
        Guid descriptorId,
        string deviceAddress,
        int byteCount);

    [LoggerMessage(
        EventId = 4005,
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

    #endregion

    #region Broadcaster Logging

    [LoggerMessage(
        EventId = 6000,
        Level = LogLevel.Error,
        Message = "Error stopping broadcaster")]
    public static partial void LogBroadcasterStopError(
        this ILogger logger,
        Exception exception);

    #endregion

    #region L2CAP Channel Logging

    [LoggerMessage(
        EventId = 7000,
        Level = LogLevel.Information,
        Message = "Opening L2CAP channel to PSM {Psm} for device {DeviceAddress}")]
    public static partial void LogL2CapChannelOpening(
        this ILogger logger,
        int psm,
        string deviceAddress);

    [LoggerMessage(
        EventId = 7001,
        Level = LogLevel.Information,
        Message = "L2CAP channel opened successfully to PSM {Psm}, MTU: {Mtu}")]
    public static partial void LogL2CapChannelOpened(
        this ILogger logger,
        int psm,
        int mtu);

    [LoggerMessage(
        EventId = 7002,
        Level = LogLevel.Error,
        Message = "Failed to open L2CAP channel to PSM {Psm}")]
    public static partial void LogL2CapChannelOpenFailed(
        this ILogger logger,
        int psm,
        Exception exception);

    [LoggerMessage(
        EventId = 7003,
        Level = LogLevel.Debug,
        Message = "Closing L2CAP channel for PSM {Psm}")]
    public static partial void LogL2CapChannelClosing(
        this ILogger logger,
        int psm);

    [LoggerMessage(
        EventId = 7004,
        Level = LogLevel.Information,
        Message = "L2CAP channel closed successfully for PSM {Psm}")]
    public static partial void LogL2CapChannelClosed(
        this ILogger logger,
        int psm);

    [LoggerMessage(
        EventId = 7005,
        Level = LogLevel.Debug,
        Message = "Reading from L2CAP channel PSM {Psm}, buffer size: {BufferSize}")]
    public static partial void LogL2CapChannelReading(
        this ILogger logger,
        int psm,
        int bufferSize);

    [LoggerMessage(
        EventId = 7006,
        Level = LogLevel.Debug,
        Message = "Read {BytesRead} bytes from L2CAP channel PSM {Psm}")]
    public static partial void LogL2CapChannelRead(
        this ILogger logger,
        int psm,
        int bytesRead);

    [LoggerMessage(
        EventId = 7007,
        Level = LogLevel.Debug,
        Message = "Writing {DataLength} bytes to L2CAP channel PSM {Psm}")]
    public static partial void LogL2CapChannelWriting(
        this ILogger logger,
        int psm,
        int dataLength);

    [LoggerMessage(
        EventId = 7008,
        Level = LogLevel.Debug,
        Message = "Wrote successfully to L2CAP channel PSM {Psm}")]
    public static partial void LogL2CapChannelWritten(
        this ILogger logger,
        int psm);

    [LoggerMessage(
        EventId = 7009,
        Level = LogLevel.Debug,
        Message = "Data received on L2CAP channel PSM {Psm}, {BytesReceived} bytes")]
    public static partial void LogL2CapDataReceived(
        this ILogger logger,
        int psm,
        int bytesReceived);

    [LoggerMessage(
        EventId = 7010,
        Level = LogLevel.Error,
        Message = "Error in L2CAP read loop for PSM {Psm}")]
    public static partial void LogL2CapReadLoopError(
        this ILogger logger,
        int psm,
        Exception exception);

    #endregion
}
