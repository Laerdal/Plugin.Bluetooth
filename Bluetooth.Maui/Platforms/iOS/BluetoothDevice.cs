using Bluetooth.Maui.PlatformSpecific;
using Bluetooth.Maui.PlatformSpecific.Exceptions;

namespace Bluetooth.Maui;

/// <summary>
/// Represents an iOS-specific Bluetooth Low Energy device.
/// This class wraps iOS Core Bluetooth's CBPeripheral, providing platform-specific
/// implementations for device connection, service discovery, and device property management.
/// </summary>
public partial class BluetoothDevice : BaseBluetoothDevice, CbPeripheralProxy.ICbPeripheralProxyDelegate, CbCentralManagerProxy.ICbPeripheralDelegate
{
    /// <summary>
    /// Gets the iOS Core Bluetooth peripheral delegate proxy used for peripheral operations.
    /// </summary>
    public CbPeripheralProxy CbPeripheralDelegateProxy { get; }

    /// <summary>
    /// Initializes a new instance of the iOS <see cref="BluetoothDevice"/> class.
    /// </summary>
    /// <param name="scanner">The Bluetooth scanner associated with this device.</param>
    /// <param name="id">The unique identifier of the device.</param>
    /// <param name="manufacturer">The manufacturer of the device.</param>
    /// <param name="cbPeripheralDelegateProxy">The iOS Core Bluetooth peripheral delegate proxy.</param>
    public BluetoothDevice(IBluetoothScanner scanner, string id, Manufacturer manufacturer, CbPeripheralProxy cbPeripheralDelegateProxy) : base(scanner, id, manufacturer)
    {
        CbPeripheralDelegateProxy = cbPeripheralDelegateProxy;
    }

    /// <summary>
    /// Initializes a new instance of the iOS <see cref="BluetoothDevice"/> class from an advertisement.
    /// </summary>
    /// <param name="scanner">The Bluetooth scanner associated with this device.</param>
    /// <param name="advertisement">The advertisement information used to initialize the device.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="advertisement"/> is <c>null</c>.</exception>
    public BluetoothDevice(IBluetoothScanner scanner, BluetoothAdvertisement advertisement) : base(scanner, advertisement)
    {
        ArgumentNullException.ThrowIfNull(advertisement);
        CbPeripheralDelegateProxy = new CbPeripheralProxy(this, advertisement.Peripheral);
    }

    #region CbPeripheralProxy.ICbPeripheralProxyDelegate

    /// <inheritdoc/>
    /// <remarks>
    /// Updates the cached device name when the peripheral's name changes on iOS.
    /// </remarks>
    public void UpdatedName()
    {
        if (CbPeripheralDelegateProxy.CbPeripheral.Name != null)
        {
            CachedName = CbPeripheralDelegateProxy.CbPeripheral.Name;
        }
    }

    /// <inheritdoc/>
    /// <exception cref="AppleNativeBluetoothException">Thrown when the error parameter indicates a Bluetooth error.</exception>
    /// <remarks>
    /// Placeholder for future L2CAP channel implementation.
    /// </remarks>
    public void DidOpenL2CapChannel(NSError? error, CBL2CapChannel? channel)
    {
        AppleNativeBluetoothException.ThrowIfError(error);
        // Placeholder for future implementation
    }

    #endregion
}
