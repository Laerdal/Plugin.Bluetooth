namespace Bluetooth.Maui;

public partial class BluetoothScanner
{
    /// <inheritdoc/>
    /// <remarks>
    /// Creates an iOS-specific <see cref="BluetoothDevice"/> from the advertisement.
    /// </remarks>
    /// <exception cref="InvalidOperationException">Thrown when the advertisement is not of type <see cref="BluetoothAdvertisement"/>.</exception>
    protected override IBluetoothDevice NativeCreateDevice(IBluetoothAdvertisement advertisement)
    {
        if (advertisement is BluetoothAdvertisement bluetoothAdvertisement)
        {
            return new BluetoothDevice(this, bluetoothAdvertisement);
        }
        throw new InvalidOperationException("Advertisement is not of type BluetoothAdvertisement");
    }

    /// <summary>
    /// Called when the central manager discovers a peripheral.
    /// </summary>
    /// <param name="peripheral">The discovered peripheral.</param>
    /// <param name="advertisementData">The advertisement data from the peripheral.</param>
    /// <param name="rssi">The received signal strength indicator (RSSI) in dBm.</param>
    /// <remarks>
    /// This method creates a <see cref="BluetoothAdvertisement"/> from the discovery data and processes it.
    /// </remarks>
    public void DiscoveredPeripheral(CBPeripheral peripheral, NSDictionary advertisementData, NSNumber rssi)
    {
        NativeRefreshIsRunning();
        OnStartSucceeded(); // If we received a peripheral, we can assume that the start scan was successful
        OnAdvertisementReceived( new BluetoothAdvertisement(peripheral, advertisementData, rssi));
    }
}
