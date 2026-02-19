namespace Bluetooth.Core.Scanning;

public abstract partial class BaseBluetoothScanner
{
    #region LoggerMessage Definitions (EventId 100-199)

    [LoggerMessage(EventId = 100, Level = LogLevel.Information,
        Message = "Scanner starting with {ServiceUuidCount} service UUIDs")]
    partial void LogScannerStarting(int serviceUuidCount);

    [LoggerMessage(EventId = 101, Level = LogLevel.Information,
        Message = "Scanner started successfully")]
    partial void LogScannerStarted();

    [LoggerMessage(EventId = 102, Level = LogLevel.Error,
        Message = "Scanner failed to start")]
    partial void LogScannerStartFailed(Exception exception);

    [LoggerMessage(EventId = 103, Level = LogLevel.Information,
        Message = "Scanner stopping")]
    partial void LogScannerStopping();

    [LoggerMessage(EventId = 104, Level = LogLevel.Information,
        Message = "Scanner stopped successfully")]
    partial void LogScannerStopped();

    [LoggerMessage(EventId = 105, Level = LogLevel.Error,
        Message = "Scanner failed to stop")]
    partial void LogScannerStopFailed(Exception exception);

    [LoggerMessage(EventId = 106, Level = LogLevel.Warning,
        Message = "Scanner started unexpectedly without pending start operation")]
    partial void LogScannerUnexpectedStart();

    [LoggerMessage(EventId = 107, Level = LogLevel.Warning,
        Message = "Scanner stopped unexpectedly without pending stop operation")]
    partial void LogScannerUnexpectedStop();

    [LoggerMessage(EventId = 108, Level = LogLevel.Information,
        Message = "Updating scanner configuration")]
    partial void LogUpdatingScannerConfiguration();

    [LoggerMessage(EventId = 109, Level = LogLevel.Error,
        Message = "Scanner configuration update failed")]
    partial void LogScannerConfigurationUpdateFailed(Exception exception);

    [LoggerMessage(EventId = 110, Level = LogLevel.Debug,
        Message = "Scanner attempting to merge concurrent start operation")]
    partial void LogMergingStartOperation();

    [LoggerMessage(EventId = 111, Level = LogLevel.Debug,
        Message = "Scanner attempting to merge concurrent stop operation")]
    partial void LogMergingStopOperation();

    [LoggerMessage(EventId = 112, Level = LogLevel.Warning,
        Message = "Scanner already started, throwing ScannerIsAlreadyStartedException")]
    partial void LogScannerAlreadyStarted();

    [LoggerMessage(EventId = 113, Level = LogLevel.Warning,
        Message = "Scanner already stopped, throwing ScannerIsAlreadyStoppedException")]
    partial void LogScannerAlreadyStopped();

    #endregion
}
