namespace Bluetooth.Avalonia.Platforms.Linux.BlueZ;

/// <summary>
///     Represents a single D-Bus object as returned by
///     <c>org.freedesktop.DBus.ObjectManager.GetManagedObjects</c>.
/// </summary>
/// <param name="Path">The absolute D-Bus object path (e.g. <c>/org/bluez/hci0/dev_AA_BB_CC_DD_EE_FF</c>).</param>
/// <param name="Interfaces">
///     Dictionary keyed by interface name (e.g. <c>org.bluez.Device1</c>), with each value being a
///     property-name-to-<see cref="VariantValue" /> dictionary.
/// </param>
internal sealed record BlueZObjectInfo(
    string Path,
    IReadOnlyDictionary<string, IReadOnlyDictionary<string, VariantValue>> Interfaces)
{
    /// <summary>Returns <see langword="true" /> if the object exposes the specified D-Bus interface.</summary>
    public bool HasInterface(string interfaceName) => Interfaces.ContainsKey(interfaceName);

    /// <summary>
    ///     Tries to get a string property from the specified interface.
    /// </summary>
    public string? GetStringProp(string interfaceName, string propName)
    {
        if (!Interfaces.TryGetValue(interfaceName, out var props)) return null;
        if (!props.TryGetValue(propName, out var v)) return null;
        return v.Type == VariantValueType.String ? v.GetString() : null;
    }

    /// <summary>
    ///     Tries to get a boolean property from the specified interface.
    /// </summary>
    public bool? GetBoolProp(string interfaceName, string propName)
    {
        if (!Interfaces.TryGetValue(interfaceName, out var props)) return null;
        if (!props.TryGetValue(propName, out var v)) return null;
        return v.Type == VariantValueType.Bool ? v.GetBool() : null;
    }

    /// <summary>
    ///     Tries to get an int16 property from the specified interface.
    /// </summary>
    public short? GetInt16Prop(string interfaceName, string propName)
    {
        if (!Interfaces.TryGetValue(interfaceName, out var props)) return null;
        if (!props.TryGetValue(propName, out var v)) return null;
        return v.Type == VariantValueType.Int16 ? v.GetInt16() : null;
    }

    /// <summary>
    ///     Tries to get a string-array property from the specified interface.
    /// </summary>
    public string[]? GetStringArrayProp(string interfaceName, string propName)
    {
        if (!Interfaces.TryGetValue(interfaceName, out var props)) return null;
        if (!props.TryGetValue(propName, out var v)) return null;
        return v.Type == VariantValueType.Array ? v.GetArray<string>() : null;
    }

    /// <summary>
    ///     Tries to get a byte-array property from the specified interface.
    /// </summary>
    public byte[]? GetByteArrayProp(string interfaceName, string propName)
    {
        if (!Interfaces.TryGetValue(interfaceName, out var props)) return null;
        if (!props.TryGetValue(propName, out var v)) return null;
        return v.Type == VariantValueType.Array ? v.GetArray<byte>() : null;
    }
}
