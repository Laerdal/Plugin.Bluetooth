namespace Bluetooth.Maui;

public partial class BluetoothScanner
{
    /// <inheritdoc/>
    /// <remarks>
    /// Creates an Android-specific <see cref="BluetoothDevice"/> from the advertisement.
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
    /// Processes multiple scan results received from Android's Bluetooth LE scanner.
    /// </summary>
    /// <param name="callbackType">The type of scan callback.</param>
    /// <param name="results">The collection of scan results.</param>
    /// <remarks>
    /// This method filters results using <see cref="NativeAdvertisementFilter"/>, marks the scanner as running,
    /// and processes the filtered advertisements in batch.
    /// </remarks>
    public virtual void OnScanResult(ScanCallbackType callbackType, IEnumerable<ScanResult> results)
    {
        IsRunning = true;
        OnStartSucceeded(); // Since there is no success callback for start scanning, we assume that we started when we receive an advertisement
        OnAdvertisementsReceived(results.Where(result => NativeAdvertisementFilter.Invoke(callbackType, result)).Select(result => new BluetoothAdvertisement(result)));
    }

    /// <summary>
    /// Processes a single scan result received from Android's Bluetooth LE scanner.
    /// </summary>
    /// <param name="callbackType">The type of scan callback.</param>
    /// <param name="result">The scan result.</param>
    /// <remarks>
    /// This method filters the result using <see cref="NativeAdvertisementFilter"/>, marks the scanner as running,
    /// and processes the advertisement if it passes the filter.
    /// </remarks>
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
