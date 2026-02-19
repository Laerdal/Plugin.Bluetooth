namespace Bluetooth.Abstractions.Exceptions;

/// <summary>
/// Provides a mechanism for listening to and handling unhandled Bluetooth exceptions.
/// </summary>
public class BluetoothUnhandledExceptionListener : ExceptionListener
{
    /// <summary>
    /// List of listeners for unhandled Bluetooth GATT exceptions.
    /// </summary>
    private readonly static List<BluetoothUnhandledExceptionListener> _bluetoothUnhandledExceptionListeners = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="BluetoothUnhandledExceptionListener"/> class.
    /// </summary>
    /// <param name="received">The event handler to call when an unhandled exception occurs.</param>
    public BluetoothUnhandledExceptionListener(EventHandler<ExceptionEventArgs> received) : base(received)
    {
        _bluetoothUnhandledExceptionListeners.Add(this);
    }

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="BluetoothUnhandledExceptionListener"/> and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _bluetoothUnhandledExceptionListeners.Remove(this);
        }
        base.Dispose(disposing);
    }

    /// <summary>
    /// Dispatches an unhandled Bluetooth exception to all registered listeners.
    /// </summary>
    /// <param name="sender">The object that raised the exception.</param>
    /// <param name="e">The exception that was not handled.</param>
    public static void OnBluetoothUnhandledException(object? sender, Exception e)
    {
        if (_bluetoothUnhandledExceptionListeners.Count == 0)
        {
            // No listeners registered; rethrow the exception.
            throw e;
        }
        foreach (var listener in _bluetoothUnhandledExceptionListeners.ToList())
        {
            listener.OnReceived(sender, e);
        }
    }
}
