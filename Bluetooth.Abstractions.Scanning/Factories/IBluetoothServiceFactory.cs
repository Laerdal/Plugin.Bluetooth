namespace Bluetooth.Abstractions.Scanning.Factories;

/// <summary>
///     Interface representing a factory for creating Bluetooth services.
/// </summary>
public interface IBluetoothServiceFactory
{
    /// <summary>
    ///     Creates a Bluetooth service based on the provided factory request.
    /// </summary>
    /// <param name="device">The Bluetooth device to which the service will be associated.</param>
    /// <param name="request">The factory request containing the necessary information to create the service.</param>
    /// <returns>The created Bluetooth service.</returns>
    IBluetoothRemoteService CreateService(IBluetoothRemoteDevice device, BluetoothServiceFactoryRequest request);

    /// <summary>
    ///     Record representing a request to create a Bluetooth service.
    /// </summary>
    record BluetoothServiceFactoryRequest
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BluetoothServiceFactoryRequest" /> class with the specified service identifier.
        /// </summary>
        /// <param name="serviceId">The unique identifier (UUID) of the Bluetooth service to be created.</param>
        protected BluetoothServiceFactoryRequest(Guid serviceId)
        {
            ServiceId = serviceId;
        }

        /// <summary>
        ///     Gets the unique identifier of the service to be created.
        /// </summary>
        public Guid ServiceId { get; }
    }
}