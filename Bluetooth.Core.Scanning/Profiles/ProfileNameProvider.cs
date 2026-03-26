using Bluetooth.Abstractions.Scanning.Profiles;

namespace Bluetooth.Core.Scanning.Profiles;

/// <summary>
///     Implementation of <see cref="IBluetoothNameProvider" /> backed by an <see cref="IBluetoothServiceDefinitionRegistry" />.
///     Resolves service, characteristic, and descriptor names from registered service definitions.
/// </summary>
public class ProfileNameProvider : IBluetoothNameProvider
{
    private readonly IBluetoothServiceDefinitionRegistry _registry;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ProfileNameProvider" /> class.
    /// </summary>
    /// <param name="registry">The service definition registry used to resolve names.</param>
    public ProfileNameProvider(IBluetoothServiceDefinitionRegistry registry)
    {
        ArgumentNullException.ThrowIfNull(registry);
        _registry = registry;
    }

    /// <inheritdoc />
    public string? GetKnownServiceName(Guid service)
        => _registry.GetServiceName(service);

    /// <inheritdoc />
    public string? GetKnownCharacteristicName(Guid characteristic)
        => _registry.GetCharacteristicName(characteristic);

    /// <inheritdoc />
    public string? GetKnownCharacteristicName(Guid serviceId, Guid characteristicId)
        => _registry.GetCharacteristicName(serviceId, characteristicId);

    /// <inheritdoc />
    /// <remarks>
    ///     Descriptor names are not yet tracked by the service definition registry. Always returns <c>null</c>.
    /// </remarks>
    public string? GetKnownDescriptorName(Guid descriptor)
        => null;
}
