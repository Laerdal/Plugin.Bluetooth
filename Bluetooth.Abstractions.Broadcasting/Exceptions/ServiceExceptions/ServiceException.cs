namespace Bluetooth.Abstractions.Broadcasting.Exceptions;

/// <summary>
///     Represents an exception that occurs in Bluetooth broadcaster operations.
/// </summary>
/// <seealso cref="IBluetoothLocalService" />
/// <seealso cref="BluetoothBroadcastingException" />
public abstract class ServiceException : BluetoothBroadcastingException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ServiceException"/> class.
    /// </summary>
    /// <param name="localService">The Bluetooth broadcast service associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    protected ServiceException(IBluetoothLocalService localService, string message = "Unknown broadcast service exception", Exception? innerException = null) : base(message, innerException)
    {
        ArgumentNullException.ThrowIfNull(localService);
        LocalService = localService;
    }

    /// <summary>
    ///     Gets the Bluetooth activity associated with the exception.
    /// </summary>
    public IBluetoothLocalService LocalService { get; }
}
