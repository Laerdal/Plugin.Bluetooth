namespace Bluetooth.Abstractions.Broadcasting;

public partial interface IBluetoothLocalCharacteristic
{
    // ## LIST OF DESCRIPTORS ##

    #region Descriptors - Add

    /// <summary>
    ///     Adds a descriptor to this characteristic.
    /// </summary>
    /// <param name="spec">The spec containing the descriptor details.</param>
    /// <param name="timeout">The timeout for this operation.</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the added descriptor.</returns>
    ValueTask<IBluetoothLocalDescriptor> AddDescriptorAsync(IBluetoothLocalDescriptorFactory.BluetoothLocalDescriptorSpec spec,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default);

    #endregion

    #region Descriptors - Get

    /// <summary>
    ///     Gets a descriptor by its ID.
    /// </summary>
    /// <param name="id">The ID of the descriptor to get.</param>
    /// <returns>The descriptor with the specified ID.</returns>
    IBluetoothLocalDescriptor GetDescriptor(Guid id);

    /// <summary>
    ///     Gets a descriptor that matches the specified filter.
    /// </summary>
    /// <param name="filter">The filter to apply to the descriptors.</param>
    /// <returns>The descriptor that matches the filter.</returns>
    IBluetoothLocalDescriptor GetDescriptor(Func<IBluetoothLocalDescriptor, bool> filter);

    /// <summary>
    ///     Gets a descriptor by its ID.
    /// </summary>
    /// <param name="id">The ID of the descriptor to get.</param>
    /// <returns>The descriptor with the specified ID, or null if not found.</returns>
    IBluetoothLocalDescriptor? GetDescriptorOrDefault(Guid id);

    /// <summary>
    ///     Gets a descriptor that matches the specified filter.
    /// </summary>
    /// <param name="filter">The filter to apply to the descriptors.</param>
    /// <returns>The descriptor that matches the filter, or null if not found.</returns>
    IBluetoothLocalDescriptor? GetDescriptorOrDefault(Func<IBluetoothLocalDescriptor, bool> filter);

    /// <summary>
    ///     Gets all descriptors for this characteristic.
    /// </summary>
    /// <param name="filter">An optional filter to apply to the descriptors.</param>
    /// <returns>A collection of descriptors.</returns>
    IEnumerable<IBluetoothLocalDescriptor> GetDescriptors(Func<IBluetoothLocalDescriptor, bool>? filter = null);

    #endregion

    #region Descriptors - Remove

    /// <summary>
    ///     Removes a descriptor from this characteristic.
    /// </summary>
    /// <param name="id">The ID of the descriptor to remove.</param>
    /// <param name="timeout">The timeout for this operation.</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RemoveDescriptorAsync(Guid id, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Removes a descriptor from this characteristic.
    /// </summary>
    /// <param name="localDescriptor">The descriptor to remove.</param>
    /// <param name="timeout">The timeout for this operation.</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RemoveDescriptorAsync(IBluetoothLocalDescriptor localDescriptor, TimeSpan? timeout = null, CancellationToken cancellationToken = default);


    /// <summary>
    ///     Removes all descriptors from this characteristic.
    /// </summary>
    /// <param name="timeout">The timeout for this operation.</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RemoveAllDescriptorsAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    #endregion

    #region Descriptors - Has

    /// <summary>
    ///     Checks if a descriptor that matches the specified filter exists.
    /// </summary>
    /// <param name="filter">The filter to apply to the descriptors.</param>
    /// <returns>True if a descriptor that matches the specified filter exists; otherwise, false.</returns>
    bool HasDescriptor(Func<IBluetoothLocalDescriptor, bool> filter);

    /// <summary>
    ///     Checks if a descriptor with the specified ID exists.
    /// </summary>
    /// <param name="id">The ID of the descriptor to check for.</param>
    /// <returns>True if a descriptor with the specified ID exists; otherwise, false.</returns>
    bool HasDescriptor(Guid id);

    #endregion
}
