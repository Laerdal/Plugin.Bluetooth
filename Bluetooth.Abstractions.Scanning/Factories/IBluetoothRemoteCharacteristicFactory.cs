namespace Bluetooth.Abstractions.Scanning.Factories;

/// <summary>
///     Factory interface for creating Bluetooth remote characteristic instances.
/// </summary>
public interface IBluetoothRemoteCharacteristicFactory
{
    /// <summary>
    ///     Creates a new instance of a Bluetooth characteristic.
    /// </summary>
    /// <param name="remoteService">The Bluetooth service to which the characteristic belongs.</param>
    /// <param name="spec">The spec containing information needed to create the characteristic.</param>
    /// <returns>A new instance of <see cref="IBluetoothRemoteCharacteristic" />.</returns>
    IBluetoothRemoteCharacteristic Create(IBluetoothRemoteService remoteService, BluetoothRemoteCharacteristicFactorySpec spec);

    /// <summary>
    ///     Request object for creating Bluetooth remote characteristic instances.
    /// </summary>
    record BluetoothRemoteCharacteristicFactorySpec
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BluetoothRemoteCharacteristicFactorySpec" /> class with the specified characteristic identifier.
        /// </summary>
        /// <param name="characteristicId">The unique identifier (UUID) of the Bluetooth characteristic to be created.</param>
        protected BluetoothRemoteCharacteristicFactorySpec(Guid characteristicId)
        {
            CharacteristicId = characteristicId;
        }

        /// <summary>
        ///     The unique identifier (UUID) of the Bluetooth characteristic to be created.
        /// </summary>
        public Guid CharacteristicId { get; }
    }
}
