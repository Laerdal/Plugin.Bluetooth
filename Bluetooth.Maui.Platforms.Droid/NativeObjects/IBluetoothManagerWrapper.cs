namespace Bluetooth.Maui.Platforms.Droid.NativeObjects;

/// <summary>
///     Interface for accessing the BluetoothManager on Android devices.
/// </summary>
public interface IBluetoothManagerWrapper
{
    /// <summary>
    ///     Gets the BluetoothManager instance.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when BluetoothManager is not available on the device.</exception>
    BluetoothManager BluetoothManager { get; }
}
