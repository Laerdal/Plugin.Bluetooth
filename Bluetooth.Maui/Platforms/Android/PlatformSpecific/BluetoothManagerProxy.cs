namespace Bluetooth.Maui.PlatformSpecific;

/// <summary>
/// Provides a proxy for Android BluetoothManager operations with lazy initialization.
/// </summary>
public static class BluetoothManagerProxy
{
    private readonly static Lazy<Android.Bluetooth.BluetoothManager> _lazyBluetoothManager =
        new Lazy<BluetoothManager>(() => Android.App.Application.Context.GetSystemService(Android.Content.Context.BluetoothService) as Android.Bluetooth.BluetoothManager
                                      ?? throw new InvalidOperationException("BluetoothManager is null - ensure Bluetooth is available on this device"));

    /// <summary>
    /// Gets the BluetoothManager instance.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when BluetoothManager is not available on the device.</exception>
    public static Android.Bluetooth.BluetoothManager BluetoothManager => _lazyBluetoothManager.Value;

    /// <summary>
    /// Gets a value indicating whether the BluetoothManager has been initialized.
    /// </summary>
    public static bool IsInitialized => _lazyBluetoothManager.IsValueCreated;
}

