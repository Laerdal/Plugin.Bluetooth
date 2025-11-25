using Bluetooth.Maui.PlatformSpecific;

namespace Bluetooth.Maui;

/// <summary>
/// Represents an Android-specific Bluetooth Low Energy GATT characteristic.
/// This class wraps Android's BluetoothGattCharacteristic and provides platform-specific
/// implementations for reading, writing, and listening to characteristic values.
/// </summary>
public partial class BluetoothCharacteristic : BaseBluetoothCharacteristic, BluetoothGattProxy.ICharacteristic
{
    /// <summary>
    /// The UUID of the Client Characteristic Configuration Descriptor (CCCD) used for enabling/disabling notifications and indications.
    /// This is a standard Bluetooth SIG descriptor UUID defined in the GATT specification.
    /// </summary>
    /// <remarks>
    /// See org.bluetooth.descriptor.gatt.client_characteristic_configuration at
    /// <see href="https://www.bluetooth.com/specifications/gatt/descriptors/"/>.
    /// </remarks>
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
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="service"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when the service's device is not a valid Android BluetoothDevice.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the BluetoothGattProxy is not initialized.</exception>
    /// <exception cref="CharacteristicFoundInWrongServiceException">Thrown when the characteristic is defined for a different service than the one provided (inherited from base).</exception>
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
