namespace Plugin.Bluetooth.Maui;

public partial class BluetoothScanner
{
    public void OnAdvertisementReceived(BluetoothLEAdvertisementReceivedEventArgs argsAdvertisement)
    {
        NativeRefreshIsRunning();
        OnStartSucceeded(); // If we received a peripheral, we can assume that the start scan was successful
        OnAdvertisementReceived(new BluetoothAdvertisement(argsAdvertisement));
    }

    protected override IBluetoothDevice NativeCreateDevice(IBluetoothAdvertisement advertisement)
    {
        if (advertisement is BluetoothAdvertisement bluetoothAdvertisement)
        {
            return new BluetoothDevice(this, bluetoothAdvertisement);
        }
        throw new InvalidOperationException("Advertisement is not of type BluetoothAdvertisement");
    }
}
