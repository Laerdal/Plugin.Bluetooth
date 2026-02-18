namespace Bluetooth.Abstractions.Scanning;

public partial interface IBluetoothRemoteCharacteristic
{
    // ## LIST OF DESCRIPTORS ##

    #region Descriptors - Events

    /// <summary>
    /// Event triggered when the list of available descriptors changes.
    /// </summary>
    event EventHandler<DescriptorListChangedEventArgs>? DescriptorListChanged;

    /// <summary>
    /// Event triggered when descriptors are added.
    /// </summary>
    event EventHandler<DescriptorsAddedEventArgs>? DescriptorsAdded;

    /// <summary>
    /// Event triggered when descriptors are removed.
    /// </summary>
    event EventHandler<DescriptorsRemovedEventArgs>? DescriptorsRemoved;

    #endregion

    #region Descriptors - Exploration

    /// <summary>
    /// Gets a value indicating whether the service is exploring descriptors.
    /// </summary>
    bool IsExploringDescriptors { get; }

    /// <summary>
    /// Explores (discovers) the descriptors of this characteristic asynchronously.
    /// </summary>
    /// <param name="timeout">The timeout for this operation.</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ExploreDescriptorsAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Explores (discovers) the descriptors of this characteristic asynchronously only if they have not been explored yet.
    /// </summary>
    /// <param name="timeout">The timeout for this operation.</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ExploreDescriptorsIfNeededAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Explores (discovers) the descriptors of this characteristic asynchronously with configurable exploration options.
    /// </summary>
    /// <param name="options">The exploration options to use. If null, uses default options (with caching).</param>
    /// <param name="timeout">The timeout for this operation.</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <remarks>
    /// This method provides flexible control over descriptor exploration.
    /// Use <see cref="Options.DescriptorExplorationOptions"/> to configure:
    /// <list type="bullet">
    /// <item>Caching behavior</item>
    /// <item>Descriptor UUID filtering</item>
    /// </list>
    /// <example>
    /// <code>
    /// // Discover all descriptors:
    /// await characteristic.ExploreDescriptorsAsync(DescriptorExplorationOptions.Default);
    ///
    /// // Discover with custom filter (only specific descriptor UUIDs):
    /// var options = new DescriptorExplorationOptions
    /// {
    ///     DescriptorUuidFilter = uuid => uuid == BluetoothUuids.ClientCharacteristicConfiguration
    /// };
    /// await characteristic.ExploreDescriptorsAsync(options);
    /// </code>
    /// </example>
    /// </remarks>
    Task ExploreDescriptorsAsync(Options.DescriptorExplorationOptions? options, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    #endregion

    #region Descriptors - Get

    /// <summary>
    /// Gets the descriptor that matches the specified filter.
    /// </summary>
    /// <param name="filter">The filter to apply to the descriptors.</param>
    /// <returns>The descriptor that matches the filter.</returns>
    /// <exception cref="DescriptorNotFoundException">Thrown when no descriptor matches the specified filter.</exception>
    /// <exception cref="MultipleDescriptorsFoundException">Thrown when multiple descriptors match the specified filter.</exception>
    IBluetoothRemoteDescriptor GetDescriptor(Func<IBluetoothRemoteDescriptor, bool> filter);

    /// <summary>
    /// Gets a descriptor by its ID.
    /// </summary>
    /// <param name="id">The ID of the descriptor to get.</param>
    /// <returns>The descriptor with the specified ID.</returns>
    /// <exception cref="DescriptorNotFoundException">Thrown when no descriptor with the specified ID is found.</exception>
    /// <exception cref="MultipleDescriptorsFoundException">Thrown when multiple descriptors with the specified ID are found.</exception>
    IBluetoothRemoteDescriptor GetDescriptor(Guid id);

    /// <summary>
    /// Gets the descriptor that matches the specified filter.
    /// </summary>
    /// <param name="filter">The filter to apply to the descriptors.</param>
    /// <returns>The descriptor that matches the filter, or null if not found.</returns>
    /// <exception cref="MultipleDescriptorsFoundException">Thrown if multiple descriptors match the specified filter.</exception>
    IBluetoothRemoteDescriptor? GetDescriptorOrDefault(Func<IBluetoothRemoteDescriptor, bool> filter);

    /// <summary>
    /// Gets a descriptor by its ID.
    /// </summary>
    /// <param name="id">The ID of the descriptor to get.</param>
    /// <returns>The descriptor with the specified ID, or null if not found.</returns>
    /// <exception cref="MultipleDescriptorsFoundException">Thrown if multiple descriptors match the specified ID.</exception>
    IBluetoothRemoteDescriptor? GetDescriptorOrDefault(Guid id);

    /// <summary>
    /// Gets the descriptors that match the specified filter.
    /// 0-N
    /// </summary>
    /// <param name="filter">The filter to apply to the descriptors.</param>
    /// <returns>The descriptors that match the filter, or all descriptors if the filter is null.</returns>
    IEnumerable<IBluetoothRemoteDescriptor> GetDescriptors(Func<IBluetoothRemoteDescriptor, bool>? filter = null);

    #endregion

    #region Descriptors - Clear

    /// <summary>
    /// Resets the list of descriptors, and stops all subscriptions and notifications.
    /// </summary>
    ValueTask ClearDescriptorsAsync();

    #endregion

    #region Descriptors - Has

    /// <summary>
    /// Gets a value indicating whether this characteristic has a descriptor that matches the specified filter.
    /// </summary>
    /// <param name="filter">The filter to apply to the descriptors.</param>
    /// <returns>True if a matching descriptor is found; otherwise, false.</returns>
    bool HasDescriptor(Func<IBluetoothRemoteDescriptor, bool> filter);

    /// <summary>
    /// Gets a value indicating whether this characteristic has a descriptor with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the descriptor to check for.</param>
    /// <returns>True if a descriptor with the specified ID is found; otherwise, false.</returns>
    bool HasDescriptor(Guid id);

    #endregion
}
