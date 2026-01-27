using Bluetooth.Maui.PlatformSpecific;

namespace Bluetooth.Maui;

/// <summary>
/// Represents a Windows-specific Bluetooth Low Energy GATT characteristic.
/// This class wraps Windows's GattCharacteristic via a proxy and provides platform-specific
/// implementations for reading, writing, and listening to characteristic values.
/// </summary>
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
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="service"/> is <c>null</c>.</exception>
    /// <exception cref="CharacteristicFoundInWrongServiceException">Thrown when the characteristic is defined for a different service than the one provided (inherited from base).</exception>
    public BluetoothCharacteristic(IBluetoothService service, Guid id, GattCharacteristic nativeCharacteristic) : base(service, id)
    {
        GattCharacteristicProxy = new GattCharacteristicProxy(nativeCharacteristic, bluetoothCharacteristicProxyDelegate: this);
    }

    /// <summary>
    /// Called when the characteristic value changes on the Windows platform.
    /// This method is invoked by the GattCharacteristicProxy when a value changed notification is received.
    /// </summary>
    /// <param name="value">The new value of the characteristic.</param>
    /// <param name="argsTimestamp">The timestamp when the value changed.</param>
    public void OnValueChanged(byte[] value, DateTimeOffset argsTimestamp)
    {
        Value = value;
    }
}
