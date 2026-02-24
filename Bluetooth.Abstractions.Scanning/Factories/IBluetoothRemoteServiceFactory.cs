namespace Bluetooth.Abstractions.Scanning.Factories;

/// <summary>
///     Interface representing a factory for creating Bluetooth remote services.
/// </summary>
public interface IBluetoothRemoteServiceFactory
{
    /// <summary>
    ///     Creates a Bluetooth service based on the provided factory spec.
    /// </summary>
    /// <param name="device">The Bluetooth device to which the service will be associated.</param>
    /// <param name="spec">The factory spec containing the necessary information to create the service.</param>
    /// <returns>The created Bluetooth service.</returns>
    IBluetoothRemoteService Create(IBluetoothRemoteDevice device, BluetoothRemoteServiceFactorySpec spec);

    /// <summary>
    ///     Record representing a request to create a Bluetooth remote service.
    /// </summary>
    record BluetoothRemoteServiceFactorySpec
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BluetoothRemoteServiceFactorySpec" /> class with the specified service identifier.
        /// </summary>
        /// <param name="serviceId">The unique identifier (UUID) of the Bluetooth service to be created.</param>
        protected BluetoothRemoteServiceFactorySpec(Guid serviceId)
        {
            ServiceId = serviceId;
        }

        /// <summary>
        ///     Gets the unique identifier of the service to be created.
        /// </summary>
        public Guid ServiceId { get; }
    }
}
