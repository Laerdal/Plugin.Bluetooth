using Bluetooth.Abstractions.Scanning;

namespace Bluetooth.Core.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when a characteristic is not found.
/// </summary>
/// <seealso cref="ServiceException" />
public class CharacteristicNotFoundException : ServiceException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicNotFoundException"/> class.
    /// </summary>
    public CharacteristicNotFoundException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicNotFoundException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public CharacteristicNotFoundException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicNotFoundException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public CharacteristicNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicNotFoundException"/> class.
    /// </summary>
    /// <param name="service">The Bluetooth service associated with the exception.</param>
    /// <param name="characteristicAddress">The characteristic address that was not found.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public CharacteristicNotFoundException(
        IBluetoothService service,
        Guid? characteristicAddress,
        Exception? innerException = null)
        : base(service, FormatCharacteristicMessage(characteristicAddress), innerException)
    {
        CharacteristicAddress = characteristicAddress;
    }

    /// <summary>
    ///     Gets the characteristic address that was not found.
    /// </summary>
    public Guid? CharacteristicAddress { get; }

    private static string FormatCharacteristicMessage(Guid? characteristicAddress)
    {
        return $"Failed to find the Characteristic '{characteristicAddress?.ToString() ?? "NULL"}'";
    }
}