using Application = Android.App.Application;

namespace Bluetooth.Maui.Platforms.Droid.NativeObjects;

/// <summary>
///     Wrapper class for Android's BluetoothManager to provide a consistent interface for Bluetooth operations in the application.
///     This class lazily initializes the BluetoothManager instance and ensures it is available when accessed.
/// </summary>
public class BluetoothManagerWrapper : IBluetoothManagerWrapper, IDisposable
{
    private readonly Lock _lock = new Lock();
    private BluetoothManager? _bluetoothManager;

    /// <summary>
    ///     Gets the BluetoothManager instance.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when BluetoothManager is not available on the device.</exception>
    public BluetoothManager BluetoothManager
    {
        get
        {
            if (_bluetoothManager == null)
            {
                lock (_lock)
                {
                    _bluetoothManager = Application.Context.GetSystemService(Context.BluetoothService) as BluetoothManager;
                    if (_bluetoothManager == null)
                    {
                        throw new InvalidOperationException("BluetoothManager is null - ensure Bluetooth is available on this device");
                    }
                }
            }

            return _bluetoothManager;
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
    }

    /// <summary>
    ///     Disposes of the BluetoothManager instance if it has been initialized.
    /// </summary>
    /// <param name="disposing">Indicates whether the method is being called from the Dispose method (true) or from a finalizer (false).</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _bluetoothManager?.Dispose();
        }
    }
}
