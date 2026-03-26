namespace Bluetooth.Abstractions.Broadcasting;

public partial interface IBluetoothLocalService
{
    // ## LIST OF CHARACTERISTICS ##

    #region Characteristics - Add

    /// <summary>
    ///     Adds a GATT characteristic to be hosted by the broadcaster.
    /// </summary>
    /// <param name="id">The UUID of the characteristic to add.</param>
    /// <param name="properties">The GATT properties supported by this characteristic (Read/Write/Notify/Indicate, etc.).</param>
    /// <param name="permissions">The permissions of the characteristic.</param>
    /// <param name="name">An optional name for the characteristic. If not provided, a default name may be assigned based on the UUID or other heuristics.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>The added characteristic.</returns>
    ValueTask<IBluetoothLocalCharacteristic> CreateCharacteristicAsync(Guid id,
        BluetoothCharacteristicProperties properties,
        BluetoothCharacteristicPermissions permissions,
        string? name = null,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default);

    #endregion

    #region Characteristics - Get

    /// <summary>
    ///     Gets a hosted GATT characteristic that matches the specified filter.
    /// </summary>
    /// <param name="filter">A function to filter characteristics. Should return true for the desired characteristic.</param>
    /// <returns>The matching characteristic.</returns>
    /// <exception cref="CharacteristicNotFoundException">Thrown if no characteristic matches the specified filter.</exception>
    /// <exception cref="MultipleCharacteristicsFoundException">Thrown if multiple characteristics match the specified filter.</exception>
    IBluetoothLocalCharacteristic GetCharacteristic(Func<IBluetoothLocalCharacteristic, bool> filter);

    /// <summary>
    ///     Gets a hosted GATT characteristic by its UUID.
    /// </summary>
    /// <param name="id">The UUID of the characteristic to retrieve.</param>
    /// <returns>The matching characteristic.</returns>
    /// <exception cref="CharacteristicNotFoundException">Thrown if no characteristic matches the specified filter.</exception>
    /// <exception cref="MultipleCharacteristicsFoundException">Thrown if multiple characteristics match the specified filter.</exception>
    IBluetoothLocalCharacteristic GetCharacteristic(Guid id);

    /// <summary>
    ///     Gets a hosted GATT characteristic that matches the specified filter.
    /// </summary>
    /// <param name="filter">A function to filter characteristics. Should return true for the desired characteristic.</param>
    /// <returns>The matching characteristic, or null if not found.</returns>
    IBluetoothLocalCharacteristic? GetCharacteristicOrDefault(Func<IBluetoothLocalCharacteristic, bool> filter);

    /// <summary>
    ///     Gets a hosted GATT characteristic by its UUID.
    /// </summary>
    /// <param name="id">The UUID of the characteristic to retrieve.</param>
    /// <returns>The matching characteristic, or null if not found.</returns>
    IBluetoothLocalCharacteristic? GetCharacteristicOrDefault(Guid id);

    /// <summary>
    ///     Gets all hosted GATT characteristics.
    /// </summary>
    /// <param name="filter">An optional filter to apply to the characteristics.</param>
    /// <returns>A collection of all hosted characteristics.</returns>
    IEnumerable<IBluetoothLocalCharacteristic> GetCharacteristics(Func<IBluetoothLocalCharacteristic, bool>? filter = null);

    #endregion

    #region Characteristics - Remove

    /// <summary>
    ///     Removes a hosted GATT characteristic from the broadcaster.
    /// </summary>
    /// <param name="id">The UUID of the characteristic to remove.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RemoveCharacteristicAsync(Guid id, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Removes a hosted GATT characteristic from the broadcaster.
    /// </summary>
    /// <param name="localCharacteristic">The characteristic to remove.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RemoveCharacteristicAsync(IBluetoothLocalCharacteristic localCharacteristic, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Removes all hosted characteristics from the broadcaster.
    /// </summary>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RemoveAllCharacteristicsAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    #endregion

    #region Characteristics - Has

    /// <summary>
    ///     Checks if a hosted GATT characteristic that matches the specified filter exists.
    /// </summary>
    /// <param name="filter">A function to filter characteristics. Should return true for the desired characteristic.</param>
    /// <returns>True if a matching characteristic exists, false otherwise.</returns>
    bool HasCharacteristic(Func<IBluetoothLocalCharacteristic, bool> filter);

    /// <summary>
    ///     Checks if a hosted GATT characteristic with the specified UUID exists.
    /// </summary>
    /// <param name="id">The UUID of the characteristic to check for.</param>
    /// <returns>True if a matching characteristic exists, false otherwise.</returns>
    bool HasCharacteristic(Guid id);

    #endregion

}
