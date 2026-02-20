namespace Bluetooth.Abstractions.Scanning.Exceptions;

/// <summary>
///     Represents an exception that occurs when the battery level is too low.
/// </summary>
/// <seealso cref="DeviceException" />
public class BatteryTooLowException : DeviceException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BatteryTooLowException" /> class.
    /// </summary>
    /// <param name="device">The Bluetooth device associated with the exception.</param>
    /// <param name="batteryLevelPrc">The current battery level percentage.</param>
    /// <param name="minBatteryLevelPrc">The minimum required battery level percentage.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public BatteryTooLowException(
        IBluetoothRemoteDevice device,
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