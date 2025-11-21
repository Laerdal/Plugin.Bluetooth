namespace Plugin.Bluetooth.Maui;

public partial class BluetoothScanner
{
    protected override IBluetoothDevice NativeCreateDevice(IBluetoothAdvertisement advertisement)
    {
        if (advertisement is BluetoothAdvertisement bluetoothAdvertisement)
        {
            return new BluetoothDevice(this, bluetoothAdvertisement);
        }
        throw new InvalidOperationException("Advertisement is not of type BluetoothAdvertisement");
    }

    public virtual void OnScanResult(ScanCallbackType callbackType, IEnumerable<ScanResult> results)
    {
        IsRunning = true;
        OnStartSucceeded(); // Since there is no success callback for start scanning, we assume that we started when we receive an advertisement
        OnAdvertisementsReceived(results.Where(result => NativeAdvertisementFilter.Invoke(callbackType, result)).Select(result => new BluetoothAdvertisement(result)));
    }

    public virtual void OnScanResult(ScanCallbackType callbackType, ScanResult result)
    {
        IsRunning = true;
        OnStartSucceeded(); // Since there is no success callback for start scanning, we assume that we started when we receive an advertisement
        if (NativeAdvertisementFilter.Invoke(callbackType, result))
        {
            OnAdvertisementReceived(new BluetoothAdvertisement(result));
        }
    }

}
