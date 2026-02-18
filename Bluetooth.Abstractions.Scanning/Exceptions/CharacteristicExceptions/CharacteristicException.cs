namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs in Bluetooth characteristic operations.
/// </summary>
/// <remarks>
///     This exception provides information about the Bluetooth characteristic associated with the error,
///     allowing for easier debugging and tracking of characteristic-related issues.
/// </remarks>
/// <seealso cref="IBluetoothRemoteCharacteristic" />
public abstract class CharacteristicException : BluetoothScanningException
{

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicException"/> class.
    /// </summary>
    /// <param name="remoteCharacteristic">The Bluetooth characteristic associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    protected CharacteristicException(
        IBluetoothRemoteCharacteristic remoteCharacteristic,
        string message = "Unknown Bluetooth characteristic exception",
        Exception? innerException = null)
        : base(message, innerException)
    {
        ArgumentNullException.ThrowIfNull(remoteCharacteristic);
        RemoteCharacteristic = remoteCharacteristic;
    }

    /// <summary>
    ///     Gets the Bluetooth characteristic associated with the exception.
    /// </summary>
    public IBluetoothRemoteCharacteristic RemoteCharacteristic { get; }
}
