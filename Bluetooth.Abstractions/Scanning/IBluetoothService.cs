
using System.ComponentModel;

using Bluetooth.Abstractions.Scanning.EventArgs;

namespace Bluetooth.Abstractions.Scanning;

/// <summary>
/// Interface representing a Bluetooth service, providing properties and methods for interacting with it.
/// </summary>
public interface IBluetoothService : INotifyPropertyChanged, IAsyncDisposable
{
    /// <summary>
    /// Gets the Bluetooth device associated with this service.
    /// </summary>
    IBluetoothDevice Device { get; }

    /// <summary>
    /// Gets the factory used to create Bluetooth characteristics.
    /// </summary>
    IBluetoothCharacteristicFactory CharacteristicFactory { get; }

    /// <summary>
    /// Gets the unique identifier of the service.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Gets the name of the service.
    /// </summary>
    string Name { get; }

    #region Characteristics - Exploration

    /// <summary>
    /// Gets a value indicating whether the service is exploring characteristics.
    /// </summary>
    bool IsExploringCharacteristics { get; }

    /// <summary>
    /// Occurs when the characteristic list changes.
    /// </summary>
    event EventHandler<CharacteristicListChangedEventArgs>? CharacteristicListChanged;

    /// <summary>
    /// Event triggered when characteristics are added.
    /// </summary>
    event EventHandler<CharacteristicsAddedEventArgs>? CharacteristicsAdded;

    /// <summary>
    /// Event triggered when characteristics are removed.
    /// </summary>
    event EventHandler<CharacteristicsRemovedEventArgs>? CharacteristicsRemoved;

    /// <summary>
    /// Explores the characteristics of the service asynchronously.
    /// </summary>
    /// <param name="clearBeforeExploring">True to clear the characteristics before exploring.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ExploreCharacteristicsAsync(bool clearBeforeExploring = false, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    #endregion

    #region Characteristics - Clear

    /// <summary>
    /// Resets the list of characteristics and characteristics, and stops all subscriptions and notifications.
    /// </summary>
    ValueTask ClearCharacteristicsAsync();

    #endregion

    #region Characteristics - Has

    /// <summary>
    /// Checks if the service has a characteristic with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the characteristic to check for.</param>
    /// <returns>True if the service has the characteristic, false otherwise.</returns>
    bool HasCharacteristic(Guid id);

    /// <summary>
    /// Checks if the service has a characteristic with the specified ID.
    /// </summary>
    /// <param name="filter">The filter to apply to the characteristics.</param>
    /// <returns>True if the service has the characteristic, false otherwise.</returns>
    bool HasCharacteristic(Func<IBluetoothCharacteristic, bool> filter);

    /// <summary>
    /// Checks if the service has a characteristic with the specified ID asynchronously.
    /// Explore then bool
    /// </summary>
    /// <param name="id">The ID of the characteristic to check for.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the service has the characteristic.</returns>
    ValueTask<bool> HasCharacteristicAsync(Guid id, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the service has a characteristic with the specified ID asynchronously.
    /// Explore then bool
    /// </summary>
    /// <param name="filter">The filter to apply to the characteristics.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the service has the characteristic.</returns>
    ValueTask<bool> HasCharacteristicAsync(Func<IBluetoothCharacteristic, bool> filter, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    #endregion

    #region Characteristics - Get

    /// <summary>
    /// Gets the characteristic that matches the specified filter.
    /// 0-1
    /// </summary>
    /// <param name="filter">The filter to apply to the characteristics.</param>
    /// <returns>The characteristic that matches the filter, or null if no such characteristic exists.</returns>
    IBluetoothCharacteristic? GetCharacteristicOrDefault(Func<IBluetoothCharacteristic, bool> filter);

    /// <summary>
    /// Gets the characteristic with the specified ID.
    /// 0-1
    /// </summary>
    /// <param name="id">The ID of the characteristic to get.</param>
    /// <returns>The characteristic with the specified ID, or null if no such characteristic exists.</returns>
    IBluetoothCharacteristic? GetCharacteristicOrDefault(Guid id);

    /// <summary>
    /// Gets the characteristics that match the specified filter.
    /// 0-N
    /// </summary>
    /// <param name="filter">The filter to apply to the characteristics.</param>
    /// <returns>The characteristics that match the filter, or all characteristics if the filter is null.</returns>
    IEnumerable<IBluetoothCharacteristic> GetCharacteristics(Func<IBluetoothCharacteristic, bool>? filter = null);

    /// <summary>
    /// Gets the characteristics with the specified ID.
    /// 0-N
    /// </summary>
    /// <param name="id">The ID of the characteristics to get.</param>
    /// <returns>The characteristics with the specified ID.</returns>
    IEnumerable<IBluetoothCharacteristic> GetCharacteristics(Guid id);

    /// <summary>
    /// Gets the characteristic that matches the specified filter asynchronously.
    /// Explore then 0-1
    /// </summary>
    /// <param name="filter">The filter to apply to the characteristics.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the characteristic that matches the filter, or null if no such characteristic exists.</returns>
    ValueTask<IBluetoothCharacteristic?> GetCharacteristicOrDefaultAsync(Func<IBluetoothCharacteristic, bool> filter, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the characteristic with the specified ID asynchronously.
    /// Explore then 0-1
    /// </summary>
    /// <param name="id">The ID of the characteristic to get.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the characteristic with the specified ID, or null if no such characteristic exists.</returns>
    ValueTask<IBluetoothCharacteristic?> GetCharacteristicOrDefaultAsync(Guid id, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the characteristics that match the specified filter asynchronously.
    /// Explore then 0-N
    /// </summary>
    /// <param name="filter">The filter to apply to the characteristics.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the characteristics that match the filter, or all characteristics if the filter is null.</returns>
    ValueTask<IEnumerable<IBluetoothCharacteristic>> GetCharacteristicsAsync(Func<IBluetoothCharacteristic, bool>? filter = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the characteristics with the specified ID asynchronously.
    /// Explore then 0-N
    /// </summary>
    /// <param name="id">The ID of the characteristics to get.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the characteristics with the specified ID.</returns>
    ValueTask<IEnumerable<IBluetoothCharacteristic>> GetCharacteristicsAsync(Guid id, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    #endregion
}
