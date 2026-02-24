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
    IBluetoothLocalDescriptor Create(
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
        /// <param name="initialValue">Optional initial value for the descriptor.</param>
        public BluetoothLocalDescriptorSpec(
            Guid id,
            ReadOnlyMemory<byte>? initialValue = null)
        {
            Id = id;
            InitialValue = initialValue;
        }

        /// <summary>
        ///     The unique identifier (UUID) of the descriptor.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        ///     Optional initial value for the descriptor.
        /// </summary>
        public ReadOnlyMemory<byte>? InitialValue { get; init; }
    }
}
