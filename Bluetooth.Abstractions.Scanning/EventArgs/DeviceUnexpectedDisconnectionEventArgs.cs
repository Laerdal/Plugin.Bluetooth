namespace Bluetooth.Abstractions.Scanning.EventArgs;

/// <summary>
///     Event arguments for when a device unexpectedly disconnects.
/// </summary>
public class DeviceUnexpectedDisconnectionEventArgs : System.EventArgs
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceUnexpectedDisconnectionEventArgs" /> class.
    /// </summary>
    /// <param name="exception">The exception associated with the unexpected disconnection, if any.</param>
    public DeviceUnexpectedDisconnectionEventArgs(Exception? exception)
    {
        Exception = exception;
    }

    /// <summary>
    ///     Gets the exception associated with the unexpected disconnection, if any.
    /// </summary>
    public Exception? Exception { get; }
}