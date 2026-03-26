namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Exception thrown when characteristic value encoding or decoding fails.
/// </summary>
public class CharacteristicCodecException : CharacteristicAccessorException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicCodecException" /> class.
    /// </summary>
    /// <param name="targetType">The target CLR type involved in the conversion failure.</param>
    /// <param name="rawBytes">The raw bytes involved in the conversion failure, if available.</param>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused this exception, if any.</param>
    public CharacteristicCodecException(Type targetType, ReadOnlyMemory<byte> rawBytes, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        ArgumentNullException.ThrowIfNull(targetType);
        TargetType = targetType;
        RawBytes = rawBytes;
    }

    /// <summary>
    ///     Gets the target CLR type involved in the conversion failure.
    /// </summary>
    public Type TargetType { get; }

    /// <summary>
    ///     Gets the raw bytes involved in the conversion failure.
    /// </summary>
    public ReadOnlyMemory<byte> RawBytes { get; }
}
