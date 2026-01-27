using Bluetooth.Abstractions.AccessService;
using Bluetooth.Abstractions.Scanning;
using Bluetooth.Core.Exceptions;

namespace Bluetooth.Core.Scanning.Exceptions;

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
public abstract class CharacteristicAccessServiceException : BluetoothException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicAccessServiceException"/> class.
    /// </summary>
    protected CharacteristicAccessServiceException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicAccessServiceException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    protected CharacteristicAccessServiceException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicAccessServiceException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    protected CharacteristicAccessServiceException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicAccessServiceException"/> class.
    /// </summary>
    /// <param name="characteristicAccessService">The characteristic access service that encountered the exception.</param>
    /// <param name="device">The Bluetooth device associated with the characteristic.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    protected CharacteristicAccessServiceException(
        IBluetoothCharacteristicAccessService characteristicAccessService,
        IBluetoothDevice device,
        string message = "Unknown Bluetooth characteristic access exception",
        Exception? innerException = null)
        : base(FormatMessage(characteristicAccessService, device, message), innerException)
    {
        ArgumentNullException.ThrowIfNull(characteristicAccessService);
        ArgumentNullException.ThrowIfNull(device);

        Device = device;
        CharacteristicAccessService = characteristicAccessService;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicAccessServiceException"/> class.
    /// </summary>
    /// <param name="characteristicAccessService">The characteristic access service that encountered the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    protected CharacteristicAccessServiceException(
        IBluetoothCharacteristicAccessService characteristicAccessService,
        string message = "Unknown Bluetooth characteristic access exception",
        Exception? innerException = null)
        : base(FormatMessage(characteristicAccessService, message), innerException)
    {
        ArgumentNullException.ThrowIfNull(characteristicAccessService);

        CharacteristicAccessService = characteristicAccessService;
    }

    /// <summary>
    ///     Gets the Bluetooth device associated with the characteristic, if available.
    /// </summary>
    public IBluetoothDevice? Device { get; }

    /// <summary>
    ///     Gets the characteristic access service that encountered the exception.
    /// </summary>
    public IBluetoothCharacteristicAccessService? CharacteristicAccessService { get; }

    private static string FormatMessage(
        IBluetoothCharacteristicAccessService characteristicAccessService,
        IBluetoothDevice device,
        string message)
    {
        ArgumentNullException.ThrowIfNull(characteristicAccessService);
        ArgumentNullException.ThrowIfNull(device);

        return $"{device} > {characteristicAccessService.ServiceName} ({characteristicAccessService.ServiceId}) > " +
               $"{characteristicAccessService.CharacteristicName} ({characteristicAccessService.CharacteristicId}) : {message}";
    }

    private static string FormatMessage(
        IBluetoothCharacteristicAccessService characteristicAccessService,
        string message)
    {
        ArgumentNullException.ThrowIfNull(characteristicAccessService);

        return $"{characteristicAccessService.ServiceName} ({characteristicAccessService.ServiceId}) > " +
               $"{characteristicAccessService.CharacteristicName} ({characteristicAccessService.CharacteristicId}) : {message}";
    }
}