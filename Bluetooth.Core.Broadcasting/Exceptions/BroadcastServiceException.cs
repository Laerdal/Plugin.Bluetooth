using Bluetooth.Abstractions.Broadcasting;
using Bluetooth.Core.Exceptions;

namespace Bluetooth.Core.Broadcasting.Exceptions;

/// <summary>
///     Represents an exception that occurs in Bluetooth broadcaster operations.
/// </summary>
/// <remarks>
///     This exception provides information about the Bluetooth broadcaster associated with the error,
///     allowing for easier debugging and tracking of broadcaster-related issues.
/// </remarks>
/// <seealso cref="IBluetoothBroadcastService" />
/// <seealso cref="BluetoothException" />
public abstract class BroadcastServiceException : BluetoothException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BroadcastServiceException"/> class.
    /// </summary>
    protected BroadcastServiceException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BroadcastServiceException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    protected BroadcastServiceException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BroadcasterException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    protected BroadcastServiceException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BroadcastServiceException"/> class.
    /// </summary>
    /// <param name="broadcastService">The Bluetooth broadcaster associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    protected BroadcastServiceException(IBluetoothBroadcastService broadcastService, string message = "Unknown broadcastService exception", Exception? innerException = null) : base(message, innerException)
    {
        BroadcastService = broadcastService;
    }

    /// <summary>
    ///     Gets the Bluetooth activity associated with the exception.
    /// </summary>
    public IBluetoothBroadcastService? BroadcastService { get; }
}
