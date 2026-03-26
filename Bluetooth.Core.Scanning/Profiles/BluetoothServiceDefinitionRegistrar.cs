namespace Bluetooth.Core.Scanning.Profiles;

/// <summary>
///     Registers Bluetooth service definitions declared on static types.
/// </summary>
public static class BluetoothServiceDefinitionRegistrar
{
    /// <summary>
    ///     Registers a static Bluetooth service definition type into the supplied registry.
    /// </summary>
    public static void Register(IBluetoothServiceDefinitionRegistry registry, Type definitionType)
    {
        ArgumentNullException.ThrowIfNull(registry);
        ArgumentNullException.ThrowIfNull(definitionType);

        if (!definitionType.IsAbstract || !definitionType.IsSealed)
        {
            throw new ArgumentException($"Type '{definitionType.FullName}' must be a static class.", nameof(definitionType));
        }

        if (definitionType.GetCustomAttribute<BluetoothServiceDefinitionAttribute>() == null)
        {
            throw new ArgumentException($"Type '{definitionType.FullName}' is not marked with {nameof(BluetoothServiceDefinitionAttribute)}.", nameof(definitionType));
        }

        var serviceId = ReadStaticMember<Guid>(definitionType, "Id");
        var serviceName = ReadStaticMember<string>(definitionType, "Name");

        registry.Register(new BluetoothServiceDefinition(serviceId, serviceName));

        foreach (var accessor in GetCharacteristicAccessors(definitionType))
        {
            registry.Register(new BluetoothCharacteristicDefinition(accessor.ServiceId, accessor.CharacteristicId, accessor.CharacteristicName));
        }
    }

    private static IEnumerable<IBluetoothCharacteristicAccessor> GetCharacteristicAccessors(Type definitionType)
    {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy;

        foreach (var field in definitionType.GetFields(flags))
        {
            if (field.GetValue(null) is IBluetoothCharacteristicAccessor accessor)
            {
                yield return accessor;
            }
        }

        foreach (var property in definitionType.GetProperties(flags))
        {
            if (property.GetIndexParameters().Length == 0 && property.GetValue(null) is IBluetoothCharacteristicAccessor accessor)
            {
                yield return accessor;
            }
        }
    }

    private static T ReadStaticMember<T>(Type definitionType, string memberName)
    {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy;

        var fieldValue = definitionType.GetField(memberName, flags)?.GetValue(null);
        if (fieldValue is T typedFieldValue)
        {
            return typedFieldValue;
        }

        var propertyValue = definitionType.GetProperty(memberName, flags)?.GetValue(null);
        if (propertyValue is T typedPropertyValue)
        {
            return typedPropertyValue;
        }

        throw new InvalidOperationException($"Type '{definitionType.FullName}' must expose a public static member named '{memberName}' of type '{typeof(T).Name}'.");
    }
}
