using Bluetooth.Maui.Platforms.Win.NativeObjects;

namespace Bluetooth.Maui.Platforms.Win;

/// <summary>
///     Windows implementation of the Bluetooth adapter using Windows.Devices.Bluetooth namespace.
/// </summary>
public partial class BluetoothAdapter : BaseBluetoothAdapter
{
    /// <summary>
    ///     Gets the Bluetooth adapter wrapper that provides access to the Bluetooth adapter properties and state.
    /// </summary>
    public IBluetoothAdapterWrapper BluetoothAdapterWrapper { get; }

    /// <summary>
    ///    Gets the Bluetooth radio wrapper that provides access to the Bluetooth radio properties and state.
    /// </summary>
    public IRadioWrapper RadioWrapper { get; }

    /// <summary>
    ///    Initializes a new instance of the <see cref="BluetoothAdapter" /> class with the specified Bluetooth adapter and radio wrappers.
    /// </summary>
    public BluetoothAdapter(IBluetoothAdapterWrapper bluetoothAdapterWrapper, IRadioWrapper radioWrapper, ILogger<IBluetoothAdapter>? logger = null) : base(logger)
    {
        BluetoothAdapterWrapper = bluetoothAdapterWrapper;
        RadioWrapper = radioWrapper;
    }
}
