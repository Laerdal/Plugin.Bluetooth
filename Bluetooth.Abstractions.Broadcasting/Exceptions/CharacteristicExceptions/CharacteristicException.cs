namespace Bluetooth.Abstractions.Broadcasting.Exceptions;

/// <summary>
///     Represents an exception that occurs in Bluetooth broadcast characteristic operations.
/// </summary>
/// <remarks>
///     This exception provides information about the Bluetooth broadcast characteristic associated with the error,
///     allowing for easier debugging and tracking of characteristic-related issues.
/// </remarks>
/// <seealso cref="IBluetoothLocalCharacteristic" />
/// <seealso cref="BluetoothBroadcastingException" />
public abstract class CharacteristicException : BluetoothBroadcastingException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicException" /> class.
    /// </summary>
    /// <param name="broadcastLocalCharacteristic">The Bluetooth broadcast characteristic associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    protected CharacteristicException(IBluetoothLocalCharacteristic broadcastLocalCharacteristic, string? message = null, Exception? innerException = null) : base(message ?? "Unknown broadcast characteristic exception", innerException)
    {
        ArgumentNullException.ThrowIfNull(broadcastLocalCharacteristic);
        BroadcastLocalCharacteristic = broadcastLocalCharacteristic;
    }

    /// <summary>
    ///     Gets the Bluetooth broadcast characteristic associated with the exception.
    /// </summary>
    public IBluetoothLocalCharacteristic BroadcastLocalCharacteristic { get; }
}
