namespace Bluetooth.Abstractions.Scanning.Exceptions;

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
    /// <param name="characteristicAccessService">The characteristic service associated with the conversion issue.</param>
    /// <param name="value">The value that failed to convert.</param>
    /// <param name="targetType">The target type to which the value was being converted.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public CharacteristicValueConversionException(IBluetoothCharacteristicAccessService characteristicAccessService, ReadOnlyMemory<byte> value, Type targetType, Exception? innerException = null) : base(characteristicAccessService,
        $"Failed to convert the characteristic value to the target type {targetType}",
        innerException)
    {
        Value = value;
        TargetType = targetType;
    }

}
