namespace Bluetooth.Maui.PlatformSpecific;

/// <summary>
/// Provides a thread-safe proxy for accessing the Android Bluetooth LE advertiser.
/// Uses lazy initialization to ensure the advertiser is only created when needed.
/// </summary>
public static class BluetoothLeAdvertiserProxy
{
    private readonly static Lazy<BluetoothLeAdvertiser> _lazyBluetoothLeAdvertiser =
        new Lazy<BluetoothLeAdvertiser>(() => BluetoothAdapterProxy.BluetoothAdapter.BluetoothLeAdvertiser ?? throw new InvalidOperationException("BluetoothAdapter.BluetoothLeAdvertiser is not available"));

    /// <summary>
    /// Gets the Bluetooth LE advertiser instance.
    /// </summary>
    /// <value>The BluetoothLeAdvertiser instance.</value>
    /// <exception cref="InvalidOperationException">Thrown when the Bluetooth LE advertiser is not available.</exception>
    public static BluetoothLeAdvertiser BluetoothLeAdvertiser => _lazyBluetoothLeAdvertiser.Value;

    /// <summary>
    /// Gets a value indicating whether the Bluetooth LE advertiser has been initialized.
    /// </summary>
    /// <value>True if the advertiser has been created; otherwise, false.</value>
    public static bool IsInitialized => _lazyBluetoothLeAdvertiser.IsValueCreated;
}
