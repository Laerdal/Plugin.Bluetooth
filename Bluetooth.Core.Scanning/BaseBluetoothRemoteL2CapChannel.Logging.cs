namespace Bluetooth.Core.Scanning;

public abstract partial class BaseBluetoothRemoteL2CapChannel
{
    #region LoggerMessage Definitions (EventId 1000-1099)

    // Disposal operations (1000-1009)
    [LoggerMessage(EventId = 1000, Level = LogLevel.Error,
        Message = "L2CAP channel (PSM {Psm}) on device {DeviceId} error closing channel during disposal")]
    partial void LogL2CapChannelDisposalError(int psm, string deviceId, Exception exception);

    #endregion
}
