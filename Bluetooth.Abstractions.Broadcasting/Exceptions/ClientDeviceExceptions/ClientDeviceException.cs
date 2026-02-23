namespace Bluetooth.Abstractions.Broadcasting.Exceptions;

/// <summary>
///     Represents an exception that occurs in Bluetooth broadcaster operations.
/// </summary>
/// <seealso cref="IBluetoothConnectedDevice" />
/// <seealso cref="BluetoothBroadcastingException" />
public class ClientDeviceException : BluetoothBroadcastingException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ClientDeviceException" /> class.
    /// </summary>
    /// <param name="broadcastConnectedDevice">The Bluetooth broadcaster associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    protected ClientDeviceException(IBluetoothConnectedDevice broadcastConnectedDevice, string message = "Unknown broadcastClientDevice exception", Exception? innerException = null) : base(message, innerException)
    {
        ArgumentNullException.ThrowIfNull(broadcastConnectedDevice);
        BroadcastConnectedDevice = broadcastConnectedDevice;
    }

    /// <summary>
    ///     Gets the Bluetooth broadcast client device associated with the exception.
    /// </summary>
    public IBluetoothConnectedDevice BroadcastConnectedDevice { get; }
}
