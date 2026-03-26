namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Base exception for all characteristic accessor and codec failures.
/// </summary>
public class CharacteristicAccessorException : BluetoothScanningException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicAccessorException" /> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused this exception, if any.</param>
    public CharacteristicAccessorException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
