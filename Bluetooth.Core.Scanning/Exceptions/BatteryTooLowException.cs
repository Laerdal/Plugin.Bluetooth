using Bluetooth.Abstractions.Scanning;

namespace Bluetooth.Core.Scanning.Exceptions;

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