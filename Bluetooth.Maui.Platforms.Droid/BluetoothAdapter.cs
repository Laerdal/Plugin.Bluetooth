using Bluetooth.Maui.Platforms.Droid.NativeObjects;

namespace Bluetooth.Maui.Platforms.Droid;

/// <summary>
/// Android implementation of the Bluetooth adapter.
/// </summary>
public class BluetoothAdapter : BaseBluetoothAdapter
{
    /// <summary>
    /// Gets the BluetoothManager wrapper for accessing Bluetooth system services.
    /// </summary>
    public IBluetoothManagerWrapper BluetoothManagerWrapper { get; }

    /// <summary>
    /// Gets the BluetoothAdapter wrapper for accessing Bluetooth adapter functionalities.
    /// </summary>
    public IBluetoothAdapterWrapper BluetoothAdapterWrapper { get; }

    /// <summary>
    /// Gets the native Android Bluetooth adapter.
    /// </summary>
    public Android.Bluetooth.BluetoothAdapter NativeBluetoothAdapter => BluetoothAdapterWrapper.BluetoothAdapter;

    /// <summary>
    /// Gets the native Android Bluetooth manager.
    /// </summary>
    public Android.Bluetooth.BluetoothManager NativeBluetoothManager => BluetoothManagerWrapper.BluetoothManager;

    /// <inheritdoc/>
    protected BluetoothAdapter(IBluetoothManagerWrapper bluetoothManagerWrapper, IBluetoothAdapterWrapper bluetoothAdapterWrapper, ILogger<IBluetoothAdapter>? logger = null) : base(logger)
    {
        BluetoothManagerWrapper = bluetoothManagerWrapper;
        BluetoothAdapterWrapper = bluetoothAdapterWrapper;
    }

}
