using Bluetooth.Abstractions.AccessService;

namespace Bluetooth.Core.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs during the conversion of a characteristic value within
///     a specified Bluetooth characteristic service.
/// </summary>
/// <remarks>
///     This exception provides contextual information about the characteristic service and a custom error message,
///     enabling easier debugging of characteristic value conversion issues.
/// </remarks>
/// <example>
///     <code>
/// try
/// {
///     // Code attempting to convert a characteristic value
/// }
/// catch (CharacteristicValueConversionException ex)
/// {
///     Console.WriteLine(ex.Message);
/// }
/// </code>
/// </example>
/// <seealso cref="IBluetoothCharacteristicAccessService" />
/// <seealso cref="CharacteristicAccessServiceException" />
public class CharacteristicValueConversionException : CharacteristicAccessServiceException
{

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicValueConversionException"/> class.
    /// </summary>
    protected CharacteristicValueConversionException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicValueConversionException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    protected CharacteristicValueConversionException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicValueConversionException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    protected CharacteristicValueConversionException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Gets the value that failed to convert.
    /// </summary>
    public ReadOnlyMemory<byte> Value { get; }

    /// <summary>
    ///     Gets the target type to which the value was being converted.
    /// </summary>
    public Type? TargetType { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicValueConversionException"/> class.
    /// </summary>
    /// <param name="value">The value that failed to convert.</param>
    /// <param name="targetType">The target type to which the value was being converted.</param>
    public CharacteristicValueConversionException(ReadOnlyMemory<byte> value, Type targetType)
    {
        Value = value;
        TargetType = targetType;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicValueConversionException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="value">The value that failed to convert.</param>
    /// <param name="targetType">The target type to which the value was being converted.</param>
    public CharacteristicValueConversionException(string message, ReadOnlyMemory<byte> value, Type targetType) : base(message)
    {
        Value = value;
        TargetType = targetType;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicValueConversionException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    /// <param name="value">The value that failed to convert.</param>
    /// <param name="targetType">The target type to which the value was being converted.</param>
    public CharacteristicValueConversionException(string message, ReadOnlyMemory<byte> value, Type targetType, Exception innerException) : base(message, innerException)
    {
        Value = value;
        TargetType = targetType;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicValueConversionException"/> class.
    /// </summary>
    /// <param name="characteristicAccessService">The characteristic service associated with the conversion issue.</param>
    /// <param name="message">A message that describes the error encountered during the conversion process.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    /// <param name="value">The value that failed to convert.</param>
    /// <param name="targetType">The target type to which the value was being converted.</param>
    public CharacteristicValueConversionException(
        IBluetoothCharacteristicAccessService characteristicAccessService,
        string message,
        ReadOnlyMemory<byte> value,
        Type targetType,
        Exception? innerException = null)
        : base(characteristicAccessService, message, innerException)
    {
        Value = value;
        TargetType = targetType;
    }
}