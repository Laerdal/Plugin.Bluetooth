namespace Bluetooth.Abstractions.Broadcasting.Factories;

/// <summary>
///     Factory interface for creating local Bluetooth GATT characteristics exposed by a broadcaster (GATT server).
/// </summary>
public interface IBluetoothLocalCharacteristicFactory
{
    /// <summary>
    ///     Creates a local Bluetooth GATT characteristic that will be exposed by the specified local service.
    /// </summary>
    /// <param name="localService">The local Bluetooth service that will expose the characteristic.</param>
    /// <param name="spec">The specification describing the characteristic to create.</param>
    /// <returns>The created local Bluetooth characteristic instance.</returns>
    IBluetoothLocalCharacteristic CreateCharacteristic(
        IBluetoothLocalService localService,
        BluetoothLocalCharacteristicSpec spec);

    /// <summary>
    ///     Specification describing a local Bluetooth GATT characteristic to expose from a service.
    /// </summary>
    record BluetoothLocalCharacteristicSpec
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BluetoothLocalCharacteristicSpec" /> record.
        /// </summary>
        /// <param name="id">The unique identifier (UUID) of the characteristic.</param>
        /// <param name="properties">The properties supported by the characteristic.</param>
        /// <param name="permissions">The access permissions for the characteristic value.</param>
        /// <param name="initialValue">Optional initial value for the characteristic.</param>
        public BluetoothLocalCharacteristicSpec(
            Guid id,
            BluetoothCharacteristicProperties properties,
            BluetoothCharacteristicPermissions permissions,
            ReadOnlyMemory<byte>? initialValue = null)
        {
            Id = id;
            Properties = properties;
            Permissions = permissions;
            InitialValue = initialValue;
        }

        /// <summary>
        ///     The unique identifier (UUID) of the characteristic.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        ///     The properties supported by the characteristic (read/write/notify/indicate, etc.).
        /// </summary>
        public BluetoothCharacteristicProperties Properties { get; init; }

        /// <summary>
        ///     The access permissions for the characteristic value.
        /// </summary>
        public BluetoothCharacteristicPermissions Permissions { get; init; }

        /// <summary>
        ///     Optional initial value for the characteristic.
        /// </summary>
        public ReadOnlyMemory<byte>? InitialValue { get; init; }
    }
}