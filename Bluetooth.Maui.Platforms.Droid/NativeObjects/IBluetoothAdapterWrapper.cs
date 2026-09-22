namespace Bluetooth.Maui.Platforms.Droid.NativeObjects;

/// <summary>
///     Interface for the BluetoothManager wrapper to allow for abstraction and easier testing.
/// </summary>
public interface IBluetoothAdapterWrapper : INotifyPropertyChanged
{
    /// <summary>
    ///     Gets the BluetoothAdapter instance.
    /// </summary>
    Android.Bluetooth.BluetoothAdapter BluetoothAdapter { get; }

    /// <summary>
    ///     Gets a value indicating whether the Bluetooth adapter is enabled. Refreshed periodically
    ///     (see <see cref="BluetoothAdapterWrapper" />'s ticker) and raises
    ///     <see cref="INotifyPropertyChanged.PropertyChanged" /> on change.
    /// </summary>
    bool BluetoothAdapterIsEnabled { get; }

    /// <summary>
    ///     Attempts to enable the Bluetooth adapter.
    /// </summary>
    /// <returns>True if the adapter is enabled; otherwise, false.</returns>
    bool TryEnableAdapter();

    /// <summary>
    ///     Attempts to disable the Bluetooth adapter.
    /// </summary>
    /// <returns>True if the adapter is disabled; otherwise, false.</returns>
    bool TryDisableAdapter();
}
