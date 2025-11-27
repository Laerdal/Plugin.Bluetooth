using Bluetooth.Maui.PlatformSpecific;

namespace Bluetooth.Maui;

/// <summary>
/// iOS implementation of the Bluetooth scanner using Core Bluetooth framework.
/// </summary>
/// <remarks>
/// This implementation uses <see cref="CBCentralManager"/> to scan for BLE peripherals.
/// </remarks>
public partial class BluetoothScanner : BaseBluetoothScanner, CbCentralManagerProxy.ICbCentralManagerProxyDelegate
{
    /// <summary>
    /// Gets the Core Bluetooth central manager proxy.
    /// </summary>
    public CbCentralManagerProxy? CbCentralManagerProxy { get; protected set; }


    #region CbCentralManagerProxy

    /// <summary>
    /// Called when the central manager is about to restore its state.
    /// </summary>
    /// <param name="dict">A dictionary containing state restoration information.</param>
    /// <remarks>
    /// This is called during state restoration when the app is relaunched in the background.
    /// </remarks>
    public void WillRestoreState(NSDictionary dict)
    {
        // Placeholder for future implementation if needed
    }

    /// <summary>
    /// Gets the device proxy for a given Core Bluetooth peripheral.
    /// </summary>
    /// <param name="peripheral">The peripheral to find.</param>
    /// <returns>The device proxy for the peripheral.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="peripheral"/> is <c>null</c>.</exception>
    /// <exception cref="DeviceNotFoundException">Thrown when the device is not found in the device list.</exception>
    /// <exception cref="MultipleDevicesFoundException">Thrown when multiple devices match the peripheral identifier.</exception>
    public CbCentralManagerProxy.ICbPeripheralDelegate GetDevice(CBPeripheral peripheral)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(peripheral);

            var match = Devices.OfType<BluetoothDevice>().SingleOrDefault(device => device.CbPeripheralDelegateProxy.CbPeripheral.Identifier.ToString() == peripheral.Identifier.ToString() && device.CbPeripheralDelegateProxy.CbPeripheral.Handle.Handle == peripheral.Handle.Handle);

            if (match == null)
            {
                throw new DeviceNotFoundException(this, peripheral.Identifier.ToString());
            }

            return match;
        }
        catch (InvalidOperationException e)
        {
            var matches = Devices.OfType<BluetoothDevice>().Where(device => device.CbPeripheralDelegateProxy.CbPeripheral.Identifier.ToString() == peripheral.Identifier.ToString() && device.CbPeripheralDelegateProxy.CbPeripheral.Handle.Handle == peripheral.Handle.Handle).ToArray();
            throw new MultipleDevicesFoundException(this, matches, e);
        }
    }

    #endregion

}
