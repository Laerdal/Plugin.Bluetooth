namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when there is an issue accessing a Bluetooth characteristic
///     within a specified Bluetooth device and service.
/// </summary>
/// <remarks>
///     This exception provides detailed information about the specific Bluetooth device, service,
///     and characteristic associated with the error, allowing for easier debugging and tracking.
/// </remarks>
/// <example>
///     <code>
/// try
/// {
///     // Attempt to access a characteristic
/// }
/// catch (CharacteristicAccessServiceException ex)
/// {
///     Console.WriteLine(ex.Message);
/// }
/// </code>
/// </example>
/// <seealso cref="IBluetoothCharacteristicAccessService" />
/// <seealso cref="IBluetoothDevice" />
public abstract class CharacteristicAccessServiceException : BluetoothScanningException
{

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicAccessServiceException"/> class.
    /// </summary>
    /// <param name="characteristicAccessService">The characteristic access service that encountered the exception.</param>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    protected CharacteristicAccessServiceException(
        IBluetoothCharacteristicAccessService characteristicAccessService,
        string? message = null,
        Exception? innerException = null)
        : base(message ?? $"Unknown Characteristic access error on {characteristicAccessService}", innerException)
    {
        ArgumentNullException.ThrowIfNull(characteristicAccessService);
        CharacteristicAccessService = characteristicAccessService;
    }

    /// <summary>
    ///     Gets the characteristic access service that encountered the exception.
    /// </summary>
    public IBluetoothCharacteristicAccessService? CharacteristicAccessService { get; }

}
