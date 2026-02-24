namespace Bluetooth.Abstractions.Scanning.Factories;

/// <summary>
///     Factory interface for creating instances of <see cref="IBluetoothRemoteDescriptor" />.
/// </summary>
public interface IBluetoothRemoteDescriptorFactory
{
    /// <summary>
    ///     Creates a new instance of <see cref="IBluetoothRemoteDescriptor" /> with the specified properties.
    /// </summary>
    /// <param name="remoteCharacteristic">The characteristic that owns this descriptor.</param>
    /// <param name="spec">The spec containing information needed to create the descriptor.</param>
    /// <returns>A new instance of <see cref="IBluetoothRemoteDescriptor" />.</returns>
    IBluetoothRemoteDescriptor Create(IBluetoothRemoteCharacteristic remoteCharacteristic, BluetoothRemoteDescriptorFactorySpec spec);

    /// <summary>
    ///     Request object for creating Bluetooth remote descriptor instances.
    /// </summary>
    record BluetoothRemoteDescriptorFactorySpec
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BluetoothRemoteDescriptorFactorySpec" /> class with the specified descriptor identifier.
        /// </summary>
        /// <param name="descriptorId">The unique identifier (UUID) of the Bluetooth descriptor to be created.</param>
        protected BluetoothRemoteDescriptorFactorySpec(Guid descriptorId)
        {
            DescriptorId = descriptorId;
        }

        /// <summary>
        ///     Gets the unique identifier of the descriptor.
        /// </summary>
        public Guid DescriptorId { get; }
    }
}
