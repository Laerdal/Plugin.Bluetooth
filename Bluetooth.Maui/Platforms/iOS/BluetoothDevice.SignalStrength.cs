using Bluetooth.Maui.PlatformSpecific.Exceptions;

namespace Bluetooth.Maui;

public partial class BluetoothDevice
{
    protected override void NativeReadSignalStrength()
    {
        CbPeripheralDelegateProxy.CbPeripheral.ReadRSSI();
    }

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

    public void RssiUpdated(NSError? error)
    {
        // CbPeripheralDelegateProxy.Peripheral.RSSI is Obsolete in iOS 8
        if (!OperatingSystem.IsIOSVersionAtLeast(8) && CbPeripheralDelegateProxy.CbPeripheral.RSSI != null)
        {
            RssiRead(error, CbPeripheralDelegateProxy.CbPeripheral.RSSI);
        }
    }
}
