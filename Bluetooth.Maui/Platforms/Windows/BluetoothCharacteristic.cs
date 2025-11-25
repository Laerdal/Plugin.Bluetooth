using Bluetooth.Maui.PlatformSpecific;

namespace Bluetooth.Maui;

/// <inheritdoc/>
public partial class BluetoothCharacteristic : BaseBluetoothCharacteristic, GattCharacteristicProxy.IBluetoothCharacteristicProxyDelegate
{
    /// <summary>
    /// Gets the Windows-specific GATT characteristic proxy used for Windows Bluetooth operations.
    /// </summary>
    public GattCharacteristicProxy GattCharacteristicProxy { get; }

    /// <summary>
    /// Initializes a new instance of the Windows <see cref="BluetoothCharacteristic"/> class.
    /// </summary>
    /// <param name="service">The Bluetooth service associated with this characteristic.</param>
    /// <param name="id">The unique identifier of the characteristic.</param>
    /// <param name="nativeCharacteristic">The native Windows GATT characteristic.</param>
    public BluetoothCharacteristic(IBluetoothService service, Guid id, GattCharacteristic nativeCharacteristic) : base(service, id)
    {
        GattCharacteristicProxy = new GattCharacteristicProxy(nativeCharacteristic, bluetoothCharacteristicProxyDelegate: this);
    }

    /// <summary>
    /// Called when the characteristic value changes on the Windows platform.
    /// </summary>
    /// <param name="value">The new value of the characteristic.</param>
    /// <param name="argsTimestamp">The timestamp when the value changed.</param>
    /// <exception cref="NotImplementedException">This method is not yet implemented for the Windows platform.</exception>
    public void OnValueChanged(byte[] value, DateTimeOffset argsTimestamp)
    {
        Value = value;
    }
}
