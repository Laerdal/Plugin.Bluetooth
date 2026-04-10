namespace Bluetooth.Avalonia.Platforms.Apple.Exceptions;

/// <summary>
///     Exception thrown when a CoreBluetooth operation fails.
/// </summary>
public class BluetoothAppleException : BluetoothException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BluetoothAppleException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public BluetoothAppleException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BluetoothAppleException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public BluetoothAppleException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
