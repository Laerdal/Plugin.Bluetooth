namespace Bluetooth.Maui.Platforms.Droid.NativeObjects;

public interface IBluetoothManagerWrapper
{
    /// <summary>
    /// Gets the BluetoothManager instance.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when BluetoothManager is not available on the device.</exception>
    Android.Bluetooth.BluetoothManager BluetoothManager { get; }
}