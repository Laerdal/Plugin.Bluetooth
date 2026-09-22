using Bluetooth.Maui.Platforms.Droid.NativeObjects;

namespace Bluetooth.Maui.Platforms.Droid;

/// <summary>
///     Android implementation of the Bluetooth adapter.
/// </summary>
public class AndroidBluetoothAdapter : BaseBluetoothAdapter
{
    /// <inheritdoc />
    public AndroidBluetoothAdapter(IBluetoothManagerWrapper bluetoothManagerWrapper, IBluetoothAdapterWrapper bluetoothAdapterWrapper, ILogger<IBluetoothAdapter>? logger = null) : base(logger)
    {
        BluetoothManagerWrapper = bluetoothManagerWrapper;
        BluetoothAdapterWrapper = bluetoothAdapterWrapper;

        BluetoothAdapterWrapper.PropertyChanged += OnBluetoothAdapterWrapperPropertyChanged;

        // Touching BluetoothAdapter (rather than waiting for a scan attempt) starts the wrapper's
        // 1s refresh ticker immediately, so IsEnabled is accurate as soon as the app starts, not
        // only once something tries to scan.
        IsEnabled = BluetoothAdapterWrapper.BluetoothAdapter.IsEnabled;
    }

    private void OnBluetoothAdapterWrapperPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IBluetoothAdapterWrapper.BluetoothAdapterIsEnabled))
        {
            IsEnabled = BluetoothAdapterWrapper.BluetoothAdapterIsEnabled;
        }
    }

    /// <summary>
    ///     Gets the BluetoothManager wrapper for accessing Bluetooth system services.
    /// </summary>
    public IBluetoothManagerWrapper BluetoothManagerWrapper { get; }

    /// <summary>
    ///     Gets the BluetoothAdapter wrapper for accessing Bluetooth adapter functionalities.
    /// </summary>
    public IBluetoothAdapterWrapper BluetoothAdapterWrapper { get; }

    /// <summary>
    ///     Gets the native Android Bluetooth adapter.
    /// </summary>
    public Android.Bluetooth.BluetoothAdapter NativeBluetoothAdapter => BluetoothAdapterWrapper.BluetoothAdapter;

    /// <summary>
    ///     Gets the native Android Bluetooth manager.
    /// </summary>
    public BluetoothManager NativeBluetoothManager => BluetoothManagerWrapper.BluetoothManager;
}
