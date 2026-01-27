using System.Collections.ObjectModel;
using System.ComponentModel;

using Bluetooth.Abstractions.Enums;

namespace Bluetooth.Abstractions.Broadcasting;

/// <summary>
/// Represents a service in the context of bluetooth broadcasting.
/// </summary>
public interface IBluetoothBroadcastService : INotifyPropertyChanged, IAsyncDisposable
{
    /// <summary>
    /// Gets the Bluetooth broadcaster hosting this service.
    /// </summary>
    IBluetoothBroadcaster Broadcaster { get; }

    /// <summary>
    /// Gets the factory used to create Bluetooth broadcast characteristics.
    /// </summary>
    IBluetoothBroadcastCharacteristicFactory CharacteristicFactory { get; }

    /// <summary>
    /// Gets the unique identifier of the service.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Gets the name of the service.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets if the service is primary or not (default = true)
    /// </summary>
    bool IsPrimary { get; }

    /// <summary>
    /// Gets the collection of Broadcasted Characteristics within that service
    /// </summary>
    ReadOnlyDictionary<Guid, IBluetoothBroadcastCharacteristic> Characteristics { get; }

    /// <summary>
    /// Creates a GATT characteristic under this service
    /// </summary>
    /// <param name="id">Unique identifier of the characteristic</param>
    /// <param name="name">Name of the characteristic</param>
    /// <param name="properties">The properties of the characteristic (read, write, notify, etc.).</param>
    /// <param name="permissions">The permissions required to access the characteristic.</param>
    /// <param name="initialValue">Value of the characteristic</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>The added characteristic.</returns>
    Task<IBluetoothBroadcastCharacteristic> AddCharacteristicAsync(Guid id,
        string name,
        CharacteristicProperties properties,
        CharacteristicPermissions permissions,
        ReadOnlyMemory<byte>? initialValue = null,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default);


    /// <summary>
    /// Removes a GATT characteristic from the service
    /// </summary>
    /// <param name="id">The UUID of the characteristic to remove.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task RemoveCharacteristicAsync(Guid id, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a GATT characteristic from the service
    /// </summary>
    /// <param name="characteristic">The characteristic to remove.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task RemoveCharacteristicAsync(IBluetoothBroadcastCharacteristic characteristic, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes all GATT characteristics from the service
    /// </summary>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task RemoveAllCharacteristicsAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);


}
