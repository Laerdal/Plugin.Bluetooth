using Bluetooth.Abstractions.Scanning.Profiles;

namespace Bluetooth.Core.Scanning.Profiles;

/// <summary>
///     Thread-safe implementation of <see cref="IBluetoothServiceDefinitionRegistry" />.
///     Stores service and characteristic definitions in concurrent dictionaries keyed by UUID pair.
/// </summary>
public class BluetoothServiceDefinitionRegistry : IBluetoothServiceDefinitionRegistry
{
    private readonly ConcurrentDictionary<Guid, BluetoothServiceDefinition> _services = new();
    private readonly ConcurrentDictionary<(Guid ServiceId, Guid CharacteristicId), BluetoothCharacteristicDefinition> _characteristics = new();

    /// <inheritdoc />
    public void Register(BluetoothServiceDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);
        _services.TryAdd(definition.ServiceId, definition);
    }

    /// <inheritdoc />
    public void Register(BluetoothCharacteristicDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);
        _characteristics.TryAdd((definition.ServiceId, definition.CharacteristicId), definition);
    }

    /// <inheritdoc />
    public string? GetServiceName(Guid serviceId)
        => _services.TryGetValue(serviceId, out var def) ? def.ServiceName : null;

    /// <inheritdoc />
    public string? GetCharacteristicName(Guid serviceId, Guid characteristicId)
        => _characteristics.TryGetValue((serviceId, characteristicId), out var def) ? def.CharacteristicName : null;

    /// <inheritdoc />
    public string? GetCharacteristicName(Guid characteristicId)
    {
        var names = _characteristics
            .Where(kvp => kvp.Key.CharacteristicId == characteristicId)
            .Select(kvp => kvp.Value.CharacteristicName)
            .Distinct()
            .ToList();

        return names.Count == 1 ? names[0] : null;
    }
}
