namespace Bluetooth.Abstractions.Scanning.Factories;

/// <summary>
/// Factory interface for creating Bluetooth characteristic instances.
/// </summary>
public interface IBluetoothCharacteristicFactory
{
    /// <summary>
    /// Creates a new instance of a Bluetooth characteristic.
    /// </summary>
    /// <param name="remoteService">The Bluetooth service to which the characteristic belongs.</param>
    /// <param name="request">The request containing information needed to create the characteristic.</param>
    /// <returns>A new instance of <see cref="IBluetoothRemoteCharacteristic"/>.</returns>
    IBluetoothRemoteCharacteristic CreateCharacteristic(IBluetoothRemoteService remoteService, BluetoothCharacteristicFactoryRequest request);

    /// <summary>
    /// Request object for creating Bluetooth characteristic instances.
    /// </summary>
    public record BluetoothCharacteristicFactoryRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BluetoothCharacteristicFactoryRequest"/> class with the specified characteristic identifier.
        /// </summary>
        /// <param name="characteristicId">The unique identifier (UUID) of the Bluetooth characteristic to be created.</param>
        protected BluetoothCharacteristicFactoryRequest(Guid characteristicId)
        {
            CharacteristicId = characteristicId;
        }

        /// <summary>
        /// The unique identifier (UUID) of the Bluetooth characteristic to be created.
        /// </summary>
        public Guid CharacteristicId { get; }
    }

}