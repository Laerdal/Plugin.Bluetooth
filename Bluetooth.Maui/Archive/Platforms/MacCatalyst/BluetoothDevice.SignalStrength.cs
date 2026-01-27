namespace Bluetooth.Maui;

public partial class BluetoothDevice
{
    /// <inheritdoc/>
    protected override void NativeReadSignalStrength()
    {
        CbPeripheralDelegateWrapper.CbPeripheral.ReadRSSI();
    }

    /// <summary>
    /// Called when an RSSI read operation completes on the iOS platform.
    /// </summary>
    /// <param name="error">The error that occurred during the read operation, or <c>null</c> if successful.</param>
    /// <param name="rssi">The RSSI value in dBm.</param>
    /// <exception cref="AppleNativeBluetoothException">Thrown when the error parameter indicates a Bluetooth error.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="rssi"/> is <c>null</c>.</exception>
    public void RssiRead(NSError? error, NSNumber rssi)
    {
        try
        {
            AppleNativeBluetoothException.ThrowIfError(error);
            ArgumentNullException.ThrowIfNull(rssi);

            OnSignalStrengthRead(rssi.Int32Value);
        }
        catch (Exception e)
        {
            OnSignalStrengthReadFailed(e);
        }
    }

    /// <summary>
    /// Called when the RSSI value is updated on the iOS platform (deprecated callback).
    /// </summary>
    /// <param name="error">The error that occurred during the update, or <c>null</c> if successful.</param>
    /// <remarks>
    /// This method is for compatibility with iOS versions prior to iOS 8.
    /// The peripheral's RSSI property is obsolete in iOS 8 and later.
    /// </remarks>
    public void RssiUpdated(NSError? error)
    {
        // CbPeripheralDelegateManager.Peripheral.RSSI is Obsolete in iOS 8
        if (!OperatingSystem.IsIOSVersionAtLeast(8) && CbPeripheralDelegateWrapper.CbPeripheral.RSSI != null)
        {
            RssiRead(error, CbPeripheralDelegateWrapper.CbPeripheral.RSSI);
        }
    }
}
