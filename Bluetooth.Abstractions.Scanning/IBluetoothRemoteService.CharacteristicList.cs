namespace Bluetooth.Abstractions.Scanning;

public partial interface IBluetoothRemoteService
{
    // ## LIST OF CHARACTERISTICS ##

    #region Characteristics - Events

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

    #endregion

    #region Characteristics - Exploration

    /// <summary>
    /// Gets a value indicating whether the service is exploring characteristics.
    /// </summary>
    bool IsExploringCharacteristics { get; }

    /// <summary>
    /// Explores the characteristics of the service asynchronously.
    /// </summary>
    /// <param name="options">
    /// Optional exploration configuration. If null, uses default options (characteristics only, with caching enabled).
    /// Use <see cref="Options.CharacteristicExplorationOptions.CharacteristicsOnly"/> for basic exploration,
    /// or <see cref="Options.CharacteristicExplorationOptions.Full"/> to include descriptors.
    /// Set <c>UseCache = false</c> to force re-exploration even if characteristics were previously discovered.
    /// </param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <remarks>
    /// <b>Common Usage Patterns:</b>
    /// <example>
    /// <code>
    /// // Simple exploration (uses defaults: characteristics only, with caching):
    /// await service.ExploreCharacteristicsAsync();
    ///
    /// // Force re-exploration (ignore cache):
    /// await service.ExploreCharacteristicsAsync(new() { UseCache = false });
    ///
    /// // Explore characteristics and descriptors:
    /// await service.ExploreCharacteristicsAsync(CharacteristicExplorationOptions.Full);
    ///
    /// // Custom options with UUID filtering:
    /// await service.ExploreCharacteristicsAsync(new CharacteristicExplorationOptions
    /// {
    ///     ExploreDescriptors = true,
    ///     CharacteristicUuidFilter = uuid => uuid == myCharacteristicUuid,
    ///     UseCache = false
    /// });
    /// </code>
    /// </example>
    ///
    /// <b>Caching Behavior:</b>
    /// By default (<c>options = null</c>), caching is enabled (<c>UseCache = true</c>).
    /// This means if characteristics have already been explored, the method returns immediately
    /// without re-querying the device. To force re-exploration, explicitly set <c>UseCache = false</c>.
    /// </remarks>
    /// <exception cref="InvalidOperationException">Thrown when exploration is already in progress.</exception>
    /// <exception cref="DeviceNotConnectedException">Thrown when the device is not connected.</exception>
    /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
    ValueTask ExploreCharacteristicsAsync(Options.CharacteristicExplorationOptions? options = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    #endregion

    #region Characteristics - Clear

    /// <summary>
    /// Resets the list of characteristics and descriptors, and stops all subscriptions and notifications.
    /// </summary>
    ValueTask ClearCharacteristicsAsync();

    #endregion

    #region Characteristics - Get

    /// <summary>
    /// Gets the characteristic that matches the specified filter.
    /// </summary>
    /// <param name="filter">The filter to apply to the characteristics.</param>
    /// <returns>The characteristic that matches the filter.</returns>
    /// <exception cref="CharacteristicNotFoundException">Thrown if no characteristic matches the specified filter.</exception>
    /// <exception cref="MultipleCharacteristicsFoundException">Thrown if multiple characteristics match the specified filter.</exception>
    IBluetoothRemoteCharacteristic GetCharacteristic(Func<IBluetoothRemoteCharacteristic, bool> filter);

    /// <summary>
    /// Gets the characteristic with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the characteristic to get.</param>
    /// <returns>The characteristic with the specified ID.</returns>
    /// <exception cref="CharacteristicNotFoundException">Thrown if no characteristic with the specified ID is found.</exception>
    /// <exception cref="MultipleCharacteristicsFoundException">Thrown if multiple characteristics match the specified filter.</exception>
    IBluetoothRemoteCharacteristic GetCharacteristic(Guid id);

    /// <summary>
    /// Gets the characteristic that matches the specified filter.
    /// </summary>
    /// <param name="filter">The filter to apply to the characteristics.</param>
    /// <returns>The characteristic that matches the filter, or null if no such characteristic exists.</returns>
    /// <exception cref="MultipleCharacteristicsFoundException">Thrown if multiple characteristics match the specified filter.</exception>
    IBluetoothRemoteCharacteristic? GetCharacteristicOrDefault(Func<IBluetoothRemoteCharacteristic, bool> filter);

    /// <summary>
    /// Gets the characteristic with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the characteristic to get.</param>
    /// <returns>The characteristic with the specified ID, or null if no such characteristic exists.</returns>
    /// <exception cref="MultipleCharacteristicsFoundException">Thrown if multiple characteristics match the specified ID.</exception>
    IBluetoothRemoteCharacteristic? GetCharacteristicOrDefault(Guid id);

    /// <summary>
    /// Gets the characteristics that match the specified filter.
    /// </summary>
    /// <param name="filter">The filter to apply to the characteristics.</param>
    /// <returns>The characteristics that match the filter, or all characteristics if the filter is null.</returns>
    IEnumerable<IBluetoothRemoteCharacteristic> GetCharacteristics(Func<IBluetoothRemoteCharacteristic, bool>? filter = null);

    #endregion

    #region Characteristics - Has

    /// <summary>
    /// Checks if a characteristic that matches the specified filter exists.
    /// </summary>
    /// <param name="filter">The filter to apply to the characteristics.</param>
    /// <returns>True if a characteristic that matches the filter exists, false otherwise.</returns>
    bool HasCharacteristic(Func<IBluetoothRemoteCharacteristic, bool> filter);

    /// <summary>
    /// Checks if a characteristic with the specified ID exists.
    /// </summary>
    /// <param name="id">The ID of the characteristic to check for.</param>
    /// <returns>True if a characteristic with the specified ID exists, false otherwise.</returns>
    bool HasCharacteristic(Guid id);

    #endregion

}
