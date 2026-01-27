using Bluetooth.Maui.PlatformSpecific;

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
        OnAdvertisementReceived(new BluetoothAdvertisement(peripheral, advertisementData, rssi));
    }

    #region BluetoothDeviceEventProxy.IScanner Implementation

    /// <summary>
    /// Gets the device proxy for a given Core Bluetooth peripheral.
    /// </summary>
    /// <param name="peripheral">The peripheral to find.</param>
    /// <returns>The device proxy for the peripheral.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="peripheral"/> is <c>null</c>.</exception>
    /// <exception cref="DeviceNotFoundException">Thrown when the device is not found in the device list.</exception>
    /// <exception cref="MultipleDevicesFoundException">Thrown when multiple devices match the peripheral identifier.</exception>
    public CbCentralManagerWrapper.ICbPeripheralDelegate GetDevice(CBPeripheral peripheral)
    {
        ArgumentNullException.ThrowIfNull(peripheral);

        try
        {
            var match = GetDeviceOrDefault(device => AreRepresentingTheSameObject(peripheral, device));
            return match as CbCentralManagerWrapper.ICbPeripheralDelegate ?? throw new DeviceNotFoundException(this, peripheral.Identifier.ToString());
        }
        catch (InvalidOperationException e)
        {
            var matches = GetDevices(device => AreRepresentingTheSameObject(peripheral, device));
            throw new MultipleDevicesFoundException(this, matches, e);
        }
    }

    private static bool AreRepresentingTheSameObject(CBPeripheral peripheral, IBluetoothDevice device)
    {
        return device is BluetoothDevice sharedDevice
            && sharedDevice.CbPeripheralDelegateWrapper.CbPeripheral.Identifier.ToString() == peripheral.Identifier.ToString()
            && sharedDevice.CbPeripheralDelegateWrapper.CbPeripheral.Handle.Handle == peripheral.Handle.Handle;
    }

    #endregion

}
