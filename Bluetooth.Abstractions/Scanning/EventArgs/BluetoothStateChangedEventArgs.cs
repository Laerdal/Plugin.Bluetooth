namespace Bluetooth.Abstractions.Scanning.EventArgs;

/// <summary>
///     Event arguments for when the Bluetooth state changes.
/// </summary>
public class BluetoothStateChangedEventArgs : System.EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BluetoothStateChangedEventArgs"/> class.
    /// </summary>
    /// <param name="isBluetoothOn">A value indicating whether Bluetooth is currently enabled.</param>
    public BluetoothStateChangedEventArgs(bool isBluetoothOn)
    {
        IsBluetoothOn = isBluetoothOn;
    }

    /// <summary>
    ///     Gets a value indicating whether Bluetooth is currently enabled.
    /// </summary>
    public bool IsBluetoothOn { get; }
}
