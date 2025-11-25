namespace Bluetooth.Core.Exceptions;

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

/// <summary>
///     Represents an exception that occurs when a Bluetooth characteristic is found in an unexpected service,
///     rather than the intended one.
/// </summary>
/// <remarks>
///     This exception provides detailed information about the characteristic, the unexpected service ID in which it was
///     found,
///     and the expected service ID. This information helps in identifying configuration or connection issues between
///     Bluetooth
///     services and their characteristics.
/// </remarks>
/// <example>
///     <code>
/// try
/// {
///     // Code attempting to locate a Bluetooth characteristic in a specific service
/// }
/// catch (CharacteristicFoundInWrongServiceException ex)
/// {
///     Console.WriteLine(ex.Message);
/// }
/// </code>
/// </example>
/// <seealso cref="IBluetoothCharacteristicAccessService" />
/// <seealso cref="IBluetoothCharacteristic" />
/// <seealso cref="CharacteristicAccessServiceException" />
public class CharacteristicFoundInWrongServiceException : CharacteristicAccessServiceException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicFoundInWrongServiceException"/> class.
    /// </summary>
    public CharacteristicFoundInWrongServiceException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicFoundInWrongServiceException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public CharacteristicFoundInWrongServiceException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicFoundInWrongServiceException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public CharacteristicFoundInWrongServiceException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicFoundInWrongServiceException"/> class.
    /// </summary>
    /// <param name="characteristicAccessService">The intended characteristic service.</param>
    /// <param name="characteristic">The characteristic found in the wrong service.</param>
    /// <param name="serviceId">The unique identifier of the service in which the characteristic was incorrectly found.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public CharacteristicFoundInWrongServiceException(
        IBluetoothCharacteristicAccessService characteristicAccessService,
        IBluetoothCharacteristic characteristic,
        Guid serviceId,
        Exception? innerException = null)
        : base(characteristicAccessService, FormatCharacteristicMessage(characteristic, serviceId, characteristicAccessService), innerException)
    {
        ArgumentNullException.ThrowIfNull(characteristicAccessService);
        ArgumentNullException.ThrowIfNull(characteristic);

        Characteristic = characteristic;
        ServiceId = serviceId;
    }

    /// <summary>
    ///     Gets the characteristic found in the wrong service.
    /// </summary>
    public IBluetoothCharacteristic? Characteristic { get; }

    /// <summary>
    ///     Gets the unique identifier of the service in which the characteristic was incorrectly found.
    /// </summary>
    public Guid ServiceId { get; }

    private static string FormatCharacteristicMessage(IBluetoothCharacteristic characteristic, Guid actualServiceId, IBluetoothCharacteristicAccessService characteristicAccessService)
    {
        ArgumentNullException.ThrowIfNull(characteristic);

        return $"{characteristic.Id} : found in {actualServiceId} instead of {characteristicAccessService.ServiceId}";
    }
}

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
