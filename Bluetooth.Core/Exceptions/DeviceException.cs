namespace Bluetooth.Core.Exceptions;

/// <summary>
///     Represents an exception that occurs in Bluetooth device operations.
/// </summary>
/// <remarks>
///     This exception provides information about the Bluetooth device associated with the error,
///     allowing for easier debugging and tracking of device-related issues.
/// </remarks>
/// <seealso cref="IBluetoothDevice" />
public abstract class DeviceException : BluetoothException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceException"/> class.
    /// </summary>
    protected DeviceException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    protected DeviceException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    protected DeviceException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceException"/> class.
    /// </summary>
    /// <param name="device">The Bluetooth device associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    protected DeviceException(
        IBluetoothDevice device,
        string message = "Unknown Bluetooth device exception",
        Exception? innerException = null)
        : base(message, innerException)
    {
        ArgumentNullException.ThrowIfNull(device);
        Device = device;
    }

    /// <summary>
    ///     Gets the Bluetooth device associated with the exception.
    /// </summary>
    public IBluetoothDevice? Device { get; }
}

/// <summary>
///     Represents an exception that occurs when parsing a Bluetooth advertisement fails.
/// </summary>
/// <seealso cref="DeviceException" />
public class DeviceAdvertisementParsingException : DeviceException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceAdvertisementParsingException"/> class.
    /// </summary>
    public DeviceAdvertisementParsingException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceAdvertisementParsingException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public DeviceAdvertisementParsingException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceAdvertisementParsingException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public DeviceAdvertisementParsingException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceAdvertisementParsingException"/> class.
    /// </summary>
    /// <param name="device">The Bluetooth device associated with the exception.</param>
    /// <param name="advertisement">The advertisement that failed to parse.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public DeviceAdvertisementParsingException(
        IBluetoothDevice device,
        IBluetoothAdvertisement advertisement,
        string message = "Failed to parse advertisement",
        Exception? innerException = null)
        : base(device, message, innerException)
    {
        ArgumentNullException.ThrowIfNull(advertisement);
        Advertisement = advertisement;
    }

    /// <summary>
    ///     Gets the advertisement that failed to parse.
    /// </summary>
    public IBluetoothAdvertisement? Advertisement { get; }
}

/// <summary>
///     Represents an exception that occurs when a Bluetooth device fails to connect.
/// </summary>
/// <seealso cref="DeviceException" />
public class DeviceFailedToConnectException : DeviceException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceFailedToConnectException"/> class.
    /// </summary>
    public DeviceFailedToConnectException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceFailedToConnectException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public DeviceFailedToConnectException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceFailedToConnectException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public DeviceFailedToConnectException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceFailedToConnectException"/> class.
    /// </summary>
    /// <param name="device">The Bluetooth device associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public DeviceFailedToConnectException(
        IBluetoothDevice device,
        string message = "Failed to connect",
        Exception? innerException = null)
        : base(device, message, innerException)
    {
    }
}

/// <summary>
///     Represents an exception that occurs when a Bluetooth device fails to disconnect.
/// </summary>
/// <seealso cref="DeviceException" />
public class DeviceFailedToDisconnectException : DeviceException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceFailedToDisconnectException"/> class.
    /// </summary>
    public DeviceFailedToDisconnectException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceFailedToDisconnectException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public DeviceFailedToDisconnectException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceFailedToDisconnectException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public DeviceFailedToDisconnectException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceFailedToDisconnectException"/> class.
    /// </summary>
    /// <param name="device">The Bluetooth device associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public DeviceFailedToDisconnectException(
        IBluetoothDevice device,
        string message = "Failed to disconnect",
        Exception? innerException = null)
        : base(device, message, innerException)
    {
    }
}

/// <summary>
///     Represents an exception that occurs when a Bluetooth device is already connected.
/// </summary>
/// <seealso cref="DeviceException" />
public class DeviceIsAlreadyConnectedException : DeviceException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceIsAlreadyConnectedException"/> class.
    /// </summary>
    public DeviceIsAlreadyConnectedException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceIsAlreadyConnectedException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public DeviceIsAlreadyConnectedException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceIsAlreadyConnectedException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public DeviceIsAlreadyConnectedException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceIsAlreadyConnectedException"/> class.
    /// </summary>
    /// <param name="device">The Bluetooth device associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public DeviceIsAlreadyConnectedException(
        IBluetoothDevice device,
        string message = "Device is already connected",
        Exception? innerException = null)
        : base(device, message, innerException)
    {
    }

    /// <summary>
    ///     Throws a <see cref="DeviceIsAlreadyConnectedException"/> if the device is already connected.
    /// </summary>
    /// <param name="device">The Bluetooth device to check.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="device"/> is null.</exception>
    /// <exception cref="DeviceIsAlreadyConnectedException">Thrown when the device is already connected.</exception>
    public static void ThrowIfAlreadyConnected(IBluetoothDevice device)
    {
        ArgumentNullException.ThrowIfNull(device);
        if (device.IsConnected)
        {
            throw new DeviceIsAlreadyConnectedException(device);
        }
    }
}

/// <summary>
///     Represents an exception that occurs when a Bluetooth device is already disconnected.
/// </summary>
/// <seealso cref="DeviceException" />
public class DeviceIsAlreadyDisconnectedException : DeviceException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceIsAlreadyDisconnectedException"/> class.
    /// </summary>
    public DeviceIsAlreadyDisconnectedException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceIsAlreadyDisconnectedException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public DeviceIsAlreadyDisconnectedException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceIsAlreadyDisconnectedException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public DeviceIsAlreadyDisconnectedException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceIsAlreadyDisconnectedException"/> class.
    /// </summary>
    /// <param name="device">The Bluetooth device associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public DeviceIsAlreadyDisconnectedException(
        IBluetoothDevice device,
        string message = "Device is already disconnected",
        Exception? innerException = null)
        : base(device, message, innerException)
    {
    }

    /// <summary>
    ///     Throws a <see cref="DeviceIsAlreadyDisconnectedException"/> if the device is already disconnected.
    /// </summary>
    /// <param name="device">The Bluetooth device to check.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="device"/> is null.</exception>
    /// <exception cref="DeviceIsAlreadyDisconnectedException">Thrown when the device is already disconnected.</exception>
    public static void ThrowIfAlreadyDisconnected(IBluetoothDevice device)
    {
        ArgumentNullException.ThrowIfNull(device);
        if (!device.IsConnected)
        {
            throw new DeviceIsAlreadyDisconnectedException(device);
        }
    }
}

/// <summary>
///     Represents an exception that occurs when a Bluetooth device is not connected.
/// </summary>
/// <seealso cref="DeviceException" />
public class DeviceNotConnectedException : DeviceException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceNotConnectedException"/> class.
    /// </summary>
    public DeviceNotConnectedException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceNotConnectedException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public DeviceNotConnectedException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceNotConnectedException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public DeviceNotConnectedException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceNotConnectedException"/> class.
    /// </summary>
    /// <param name="device">The Bluetooth device associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public DeviceNotConnectedException(
        IBluetoothDevice device,
        string message = "Device needs to be connected to execute this operation",
        Exception? innerException = null)
        : base(device, message, innerException)
    {
    }

    /// <summary>
    ///     Throws a <see cref="DeviceNotConnectedException"/> if the device is not connected.
    /// </summary>
    /// <param name="device">The Bluetooth device to check.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="device"/> is null.</exception>
    /// <exception cref="DeviceNotConnectedException">Thrown when the device is not connected.</exception>
    public static void ThrowIfNotConnected(IBluetoothDevice device)
    {
        ArgumentNullException.ThrowIfNull(device);
        if (!device.IsConnected)
        {
            throw new DeviceNotConnectedException(device);
        }
    }
}

/// <summary>
///     Represents an exception that occurs when multiple services are found matching criteria.
/// </summary>
/// <seealso cref="DeviceException" />
public class MultipleServicesFoundException : DeviceException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="MultipleServicesFoundException"/> class.
    /// </summary>
    public MultipleServicesFoundException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="MultipleServicesFoundException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public MultipleServicesFoundException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="MultipleServicesFoundException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public MultipleServicesFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="MultipleServicesFoundException"/> class.
    /// </summary>
    /// <param name="device">The Bluetooth device associated with the exception.</param>
    /// <param name="services">The services that were found matching the criteria.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public MultipleServicesFoundException(
        IBluetoothDevice device,
        IEnumerable<IBluetoothService> services,
        Exception? innerException = null)
        : base(device, "Multiple services have been found matching criteria", innerException)
    {
        ArgumentNullException.ThrowIfNull(services);
        Services = services;
    }

    /// <summary>
    ///     Gets the services that were found matching the criteria.
    /// </summary>
    public IEnumerable<IBluetoothService>? Services { get; }
}

/// <summary>
///     Represents an exception that occurs when service exploration fails.
/// </summary>
/// <seealso cref="DeviceException" />
public class ServiceExplorationException : DeviceException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ServiceExplorationException"/> class.
    /// </summary>
    public ServiceExplorationException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ServiceExplorationException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ServiceExplorationException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ServiceExplorationException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ServiceExplorationException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ServiceExplorationException"/> class.
    /// </summary>
    /// <param name="device">The Bluetooth device associated with the exception.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public ServiceExplorationException(
        IBluetoothDevice device,
        Exception? innerException = null)
        : base(device, "Failed to explore services", innerException)
    {
    }
}

/// <summary>
///     Represents an exception that occurs when an unexpected service exploration happens.
/// </summary>
/// <seealso cref="DeviceException" />
public class UnexpectedServiceExplorationException : DeviceException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="UnexpectedServiceExplorationException"/> class.
    /// </summary>
    public UnexpectedServiceExplorationException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="UnexpectedServiceExplorationException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public UnexpectedServiceExplorationException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="UnexpectedServiceExplorationException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public UnexpectedServiceExplorationException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="UnexpectedServiceExplorationException"/> class.
    /// </summary>
    /// <param name="device">The Bluetooth device associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public UnexpectedServiceExplorationException(
        IBluetoothDevice device,
        string message = "Unexpected service exploration",
        Exception? innerException = null)
        : base(device, message, innerException)
    {
    }
}

/// <summary>
///     Represents an exception that occurs when a service is not found.
/// </summary>
/// <seealso cref="DeviceException" />
public class ServiceNotFoundException : DeviceException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ServiceNotFoundException"/> class.
    /// </summary>
    public ServiceNotFoundException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ServiceNotFoundException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ServiceNotFoundException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ServiceNotFoundException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ServiceNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ServiceNotFoundException"/> class.
    /// </summary>
    /// <param name="device">The Bluetooth device associated with the exception.</param>
    /// <param name="serviceAddress">The service address that was not found.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public ServiceNotFoundException(
        IBluetoothDevice device,
        Guid? serviceAddress,
        Exception? innerException = null)
        : base(device, FormatServiceMessage(serviceAddress), innerException)
    {
        ServiceAddress = serviceAddress;
    }

    /// <summary>
    ///     Gets the service address that was not found.
    /// </summary>
    public Guid? ServiceAddress { get; }

    private static string FormatServiceMessage(Guid? serviceAddress)
    {
        return $"Failed to find the Service '{serviceAddress?.ToString() ?? "NULL"}'";
    }
}

/// <summary>
///     Represents an exception that occurs when a device disconnects unexpectedly.
/// </summary>
/// <seealso cref="DeviceException" />
public class DeviceUnexpectedDisconnectionException : DeviceException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceUnexpectedDisconnectionException"/> class.
    /// </summary>
    public DeviceUnexpectedDisconnectionException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceUnexpectedDisconnectionException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public DeviceUnexpectedDisconnectionException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceUnexpectedDisconnectionException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public DeviceUnexpectedDisconnectionException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeviceUnexpectedDisconnectionException"/> class.
    /// </summary>
    /// <param name="device">The Bluetooth device associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public DeviceUnexpectedDisconnectionException(
        IBluetoothDevice device,
        string message = "Device has disconnected unexpectedly",
        Exception? innerException = null)
        : base(device, message, innerException)
    {
    }
}

/// <summary>
///     Represents an exception that occurs when the battery level is too low.
/// </summary>
/// <seealso cref="DeviceException" />
public class BatteryTooLowException : DeviceException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BatteryTooLowException"/> class.
    /// </summary>
    public BatteryTooLowException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BatteryTooLowException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public BatteryTooLowException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BatteryTooLowException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public BatteryTooLowException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BatteryTooLowException"/> class.
    /// </summary>
    /// <param name="device">The Bluetooth device associated with the exception.</param>
    /// <param name="batteryLevelPrc">The current battery level percentage.</param>
    /// <param name="minBatteryLevelPrc">The minimum required battery level percentage.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public BatteryTooLowException(
        IBluetoothDevice device,
        double batteryLevelPrc,
        double minBatteryLevelPrc,
        string message = "Battery is too low to execute this operation",
        Exception? innerException = null)
        : base(device, message, innerException)
    {
        BatteryLevelPrc = batteryLevelPrc;
        MinBatteryLevelPrc = minBatteryLevelPrc;
    }

    /// <summary>
    ///     Gets the current battery level percentage.
    /// </summary>
    public double BatteryLevelPrc { get; }

    /// <summary>
    ///     Gets the minimum required battery level percentage.
    /// </summary>
    public double MinBatteryLevelPrc { get; }
}

/// <summary>
///     Represents an exception that occurs when bonding fails.
/// </summary>
/// <seealso cref="DeviceException" />
public class BondingFailedException : DeviceException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BondingFailedException"/> class.
    /// </summary>
    public BondingFailedException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BondingFailedException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public BondingFailedException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BondingFailedException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public BondingFailedException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BondingFailedException"/> class.
    /// </summary>
    /// <param name="device">The Bluetooth device associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public BondingFailedException(
        IBluetoothDevice device,
        string message = "Bonding failed",
        Exception? innerException = null)
        : base(device, message, innerException)
    {
    }
}

/// <summary>
///     Represents an exception that occurs when pairing fails.
/// </summary>
/// <seealso cref="DeviceException" />
public class PairingFailedException : DeviceException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="PairingFailedException"/> class.
    /// </summary>
    public PairingFailedException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="PairingFailedException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public PairingFailedException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="PairingFailedException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public PairingFailedException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="PairingFailedException"/> class.
    /// </summary>
    /// <param name="device">The Bluetooth device associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public PairingFailedException(
        IBluetoothDevice device,
        string message = "Failed to pair with device",
        Exception? innerException = null)
        : base(device, message, innerException)
    {
    }
}
