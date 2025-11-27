namespace Bluetooth.Maui;

public partial class BluetoothScanner
{
    /// <summary>
    /// Called when a Bluetooth LE advertisement is received.
    /// </summary>
    /// <param name="argsAdvertisement">The advertisement event arguments.</param>
    /// <remarks>
    /// This method creates a <see cref="BluetoothAdvertisement"/> from the received data and processes it.
    /// </remarks>
    public void OnAdvertisementReceived(BluetoothLEAdvertisementReceivedEventArgs argsAdvertisement)
    {
        NativeRefreshIsRunning();
        OnStartSucceeded(); // If we received a peripheral, we can assume that the start scan was successful
        OnAdvertisementReceived(new BluetoothAdvertisement(argsAdvertisement));
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Creates a Windows-specific <see cref="BluetoothDevice"/> from the advertisement.
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
}
