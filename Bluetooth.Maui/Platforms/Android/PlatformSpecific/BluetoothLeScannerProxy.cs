namespace Bluetooth.Maui.PlatformSpecific;

/// <summary>
/// Provides a thread-safe proxy for accessing the Android Bluetooth LE scanner.
/// Uses lazy initialization to ensure the scanner is only created when needed.
/// </summary>
public static class BluetoothLeScannerProxy
{
    private readonly static Lazy<BluetoothLeScanner> _lazyBluetoothLeScanner =
        new Lazy<BluetoothLeScanner>(() => BluetoothAdapterProxy.BluetoothAdapter.BluetoothLeScanner ?? throw new InvalidOperationException("BluetoothAdapter.BluetoothLeScanner is not available"));

    /// <summary>
    /// Gets the Bluetooth LE scanner instance.
    /// </summary>
    /// <value>The BluetoothLeScanner instance.</value>
    /// <exception cref="InvalidOperationException">Thrown when the Bluetooth LE scanner is not available.</exception>
    public static BluetoothLeScanner BluetoothLeScanner => _lazyBluetoothLeScanner.Value;

    /// <summary>
    /// Gets a value indicating whether the Bluetooth LE scanner has been initialized.
    /// </summary>
    /// <value>True if the scanner has been created; otherwise, false.</value>
    public static bool IsInitialized => _lazyBluetoothLeScanner.IsValueCreated;
}
