namespace Bluetooth.Abstractions.Scanning.Profiles;

/// <summary>
///     Registry for Bluetooth service and characteristic service definitions.
///     Provides name resolution and definition lookup for known service definitions.
/// </summary>
public interface IBluetoothServiceDefinitionRegistry
{
    /// <summary>
    ///     Registers a Bluetooth service definition.
    /// </summary>
    /// <param name="definition">The service definition to register.</param>
    void Register(BluetoothServiceDefinition definition);

    /// <summary>
    ///     Registers a Bluetooth characteristic definition.
    /// </summary>
    /// <param name="definition">The characteristic definition to register.</param>
    void Register(BluetoothCharacteristicDefinition definition);

    /// <summary>
    ///     Gets the known name for a service by its UUID.
    /// </summary>
    /// <param name="serviceId">The UUID of the service.</param>
    /// <returns>The service name, or <c>null</c> if not registered.</returns>
    string? GetServiceName(Guid serviceId);

    /// <summary>
    ///     Gets the known name for a characteristic by service and characteristic UUID pair.
    /// </summary>
    /// <param name="serviceId">The UUID of the owning service.</param>
    /// <param name="characteristicId">The UUID of the characteristic.</param>
    /// <returns>The characteristic name, or <c>null</c> if not registered.</returns>
    string? GetCharacteristicName(Guid serviceId, Guid characteristicId);

    /// <summary>
    ///     Gets the known name for a characteristic by its UUID alone.
    ///     Returns <c>null</c> if the UUID is ambiguous (registered under multiple services with different names)
    ///     or not registered.
    /// </summary>
    /// <param name="characteristicId">The UUID of the characteristic.</param>
    /// <returns>The characteristic name if unambiguous, or <c>null</c>.</returns>
    string? GetCharacteristicName(Guid characteristicId);
}
