namespace Bluetooth.Abstractions.Broadcasting.Factories;

/// <summary>
///     Factory interface for creating local Bluetooth GATT descriptors exposed by a broadcaster (GATT server).
/// </summary>
public interface IBluetoothLocalDescriptorFactory
{
    /// <summary>
    ///     Creates a local Bluetooth GATT descriptor that will be exposed by the specified local characteristic.
    /// </summary>
    /// <param name="localCharacteristic">The local characteristic that will expose the descriptor.</param>
    /// <param name="spec">The specification describing the descriptor to create.</param>
    /// <returns>The created local Bluetooth descriptor instance.</returns>
    IBluetoothLocalDescriptor CreateDescriptor(
        IBluetoothLocalCharacteristic localCharacteristic,
        BluetoothLocalDescriptorSpec spec);

    /// <summary>
    ///     Specification describing a local Bluetooth GATT descriptor to expose from a characteristic.
    /// </summary>
    record BluetoothLocalDescriptorSpec
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BluetoothLocalDescriptorSpec" /> record.
        /// </summary>
        /// <param name="id">The unique identifier (UUID) of the descriptor.</param>
        /// <param name="permissions">The access permissions for the descriptor value.</param>
        /// <param name="initialValue">Optional initial value for the descriptor.</param>
        public BluetoothLocalDescriptorSpec(
            Guid id,
            BluetoothDescriptorPermissions permissions,
            ReadOnlyMemory<byte>? initialValue = null)
        {
            Id = id;
            Permissions = permissions;
            InitialValue = initialValue;
        }

        /// <summary>
        ///     The unique identifier (UUID) of the descriptor.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        ///     Access permissions for the descriptor value.
        /// </summary>
        public BluetoothDescriptorPermissions Permissions { get; init; }

        /// <summary>
        ///     Optional initial value for the descriptor.
        /// </summary>
        public ReadOnlyMemory<byte>? InitialValue { get; init; }
    }
}