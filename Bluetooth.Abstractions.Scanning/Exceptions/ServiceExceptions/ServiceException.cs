namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs in Bluetooth service operations.
/// </summary>
/// <remarks>
///     This exception provides information about the Bluetooth service associated with the error,
///     allowing for easier debugging and tracking of service-related issues.
/// </remarks>
/// <seealso cref="IBluetoothRemoteService" />
public abstract class ServiceException : BluetoothScanningException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ServiceException" /> class.
    /// </summary>
    /// <param name="remoteService">The Bluetooth service associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    protected ServiceException(
        IBluetoothRemoteService remoteService,
        string message = "Unknown Bluetooth service exception",
        Exception? innerException = null)
        : base(message, innerException)
    {
        ArgumentNullException.ThrowIfNull(remoteService);
        RemoteService = remoteService;
    }

    /// <summary>
    ///     Gets the Bluetooth service associated with the exception.
    /// </summary>
    public IBluetoothRemoteService RemoteService { get; }
}