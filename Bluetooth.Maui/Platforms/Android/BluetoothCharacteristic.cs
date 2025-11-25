using Bluetooth.Maui.PlatformSpecific;

namespace Bluetooth.Maui;

/// <inheritdoc cref="BaseBluetoothCharacteristic"/>
public partial class BluetoothCharacteristic : BaseBluetoothCharacteristic, BluetoothGattProxy.ICharacteristic
{
    /// <summary>
    ///     The UUID of the Client Characteristic Configuration descriptor used for notifications and indications.
    ///     See org.bluetooth.descriptor.gatt.client_characteristic_configuration in https://
    ///     www.bluetooth.com/specifications/gatt/descriptors/
    /// </summary>
    public const string NotificationDescriptorId = "00002902-0000-1000-8000-00805f9b34fb";

    /// <summary>
    /// Gets the native Android GATT characteristic used for Android Bluetooth operations.
    /// </summary>
    public BluetoothGattCharacteristic NativeCharacteristic { get; }

    /// <summary>
    /// Gets the Android-specific GATT proxy used for Bluetooth operations.
    /// </summary>
    public BluetoothGattProxy BluetoothGattProxy { get; }

    /// <summary>
    /// Initializes a new instance of the Android <see cref="BluetoothCharacteristic"/> class.
    /// </summary>
    /// <param name="service">The Bluetooth service associated with this characteristic.</param>
    /// <param name="id">The unique identifier of the characteristic.</param>
    /// <param name="bluetoothGattCharacteristic">The native Android GATT characteristic.</param>
    /// <exception cref="ArgumentException">Thrown when the service's device is not a valid Android BluetoothDevice.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the BluetoothGattProxy is not initialized.</exception>
    public BluetoothCharacteristic(IBluetoothService service, Guid id, BluetoothGattCharacteristic bluetoothGattCharacteristic) : base(service, id)
    {
        if (Service.Device is not BluetoothDevice androidDevice)
        {
            throw new ArgumentException("The provided service's device is not a valid Android BluetoothDevice.", nameof(service));
        }
        BluetoothGattProxy = androidDevice.BluetoothGattProxy ?? throw new InvalidOperationException("The BluetoothGattProxy is not initialized.");
        NativeCharacteristic = bluetoothGattCharacteristic;
    }
}
