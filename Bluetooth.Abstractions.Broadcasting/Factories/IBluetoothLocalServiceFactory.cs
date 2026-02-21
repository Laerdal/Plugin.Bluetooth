namespace Bluetooth.Abstractions.Broadcasting.Factories;

/// <summary>
///     Factory interface for creating local Bluetooth GATT services exposed by a broadcaster (GATT server).
/// </summary>
public interface IBluetoothLocalServiceFactory
{
    /// <summary>
    ///     Creates a local Bluetooth GATT service that will be exposed by the specified broadcaster.
    /// </summary>
    /// <param name="broadcaster">The local GATT server that will expose the service.</param>
    /// <param name="spec">The specification describing the service to create.</param>
    /// <returns>The created local Bluetooth service instance.</returns>
    IBluetoothLocalService CreateService(
        IBluetoothBroadcaster broadcaster,
        BluetoothLocalServiceSpec spec);

    /// <summary>
    ///     Specification describing a local Bluetooth GATT service to expose from a broadcaster (GATT server).
    /// </summary>
    record BluetoothLocalServiceSpec
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BluetoothLocalServiceSpec" /> record.
        /// </summary>
        /// <param name="id">The unique identifier (UUID) of the service.</param>
        /// <param name="isPrimary">Whether the service is a primary service. Default is <c>true</c>.</param>
        public BluetoothLocalServiceSpec(Guid id, bool isPrimary = true)
        {
            Id = id;
            IsPrimary = isPrimary;
        }

        /// <summary>
        ///     The unique identifier (UUID) of the service.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        ///     Whether the service is a primary service.
        /// </summary>
        public bool IsPrimary { get; init; } = true;
    }
}