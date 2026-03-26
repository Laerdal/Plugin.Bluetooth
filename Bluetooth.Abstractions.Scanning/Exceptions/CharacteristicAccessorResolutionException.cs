namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Exception thrown when a characteristic accessor cannot resolve the target service or characteristic on a device.
/// </summary>
public class CharacteristicAccessorResolutionException : CharacteristicAccessorException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicAccessorResolutionException" /> class.
    /// </summary>
    /// <param name="serviceId">The target service UUID involved in the resolution failure.</param>
    /// <param name="characteristicId">The target characteristic UUID involved in the resolution failure.</param>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused this exception, if any.</param>
    public CharacteristicAccessorResolutionException(Guid serviceId, Guid characteristicId, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        ServiceId = serviceId;
        CharacteristicId = characteristicId;
    }

    /// <summary>
    ///     Gets the target service UUID involved in the resolution failure.
    /// </summary>
    public Guid ServiceId { get; }

    /// <summary>
    ///     Gets the target characteristic UUID involved in the resolution failure.
    /// </summary>
    public Guid CharacteristicId { get; }
}
