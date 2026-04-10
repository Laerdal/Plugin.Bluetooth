namespace Bluetooth.Avalonia.Platforms.Linux.BlueZ;

/// <summary>
///     Helper for calling <c>org.freedesktop.DBus.ObjectManager.GetManagedObjects</c> on BlueZ
///     and for subscribing to <c>InterfacesAdded</c> / <c>InterfacesRemoved</c> signals.
/// </summary>
internal static class BlueZObjectManager
{
    // ==================== GetManagedObjects ====================

    /// <summary>
    ///     Calls <c>GetManagedObjects</c> on the BlueZ service and returns all managed objects.
    /// </summary>
    public static Task<IList<BlueZObjectInfo>> GetManagedObjectsAsync(
        DBusConnection connection,
        CancellationToken cancellationToken = default)
    {
        var message = CreateGetManagedObjectsMessage(connection);
        return connection.CallMethodAsync(message, ParseManagedObjectsReply);
    }

    private static MessageBuffer CreateGetManagedObjectsMessage(DBusConnection connection)
    {
        using var writer = connection.GetMessageWriter();
        writer.WriteMethodCallHeader(
            destination: BlueZConstants.ServiceName,
            path: BlueZConstants.RootPath,
            @interface: BlueZConstants.ObjectManagerInterface,
            member: BlueZConstants.MethodGetManagedObjects);
        return writer.CreateMessage();
    }

    private static IList<BlueZObjectInfo> ParseManagedObjectsReply(Message message, object? state)
    {
        var result = new List<BlueZObjectInfo>();
        var reader = message.GetBodyReader();

        // a{oa{sa{sv}}}
        var outerDictEnd = reader.ReadDictionaryStart();
        while (reader.HasNext(outerDictEnd))
        {
            var path = reader.ReadObjectPathAsString();
            var interfaces = ReadInterfacesDictionary(ref reader);
            result.Add(new BlueZObjectInfo(path, interfaces));
        }

        return result;
    }

    // ==================== Properties.GetAll ====================

    /// <summary>
    ///     Calls <c>org.freedesktop.DBus.Properties.GetAll</c> on the given object path and interface.
    /// </summary>
    public static Task<IReadOnlyDictionary<string, VariantValue>> GetAllPropertiesAsync(
        DBusConnection connection,
        string objectPath,
        string interfaceName,
        CancellationToken cancellationToken = default)
    {
        var message = CreateGetAllMessage(connection, objectPath, interfaceName);
        return connection.CallMethodAsync(message, ParsePropertiesGetAllReply);
    }

    private static MessageBuffer CreateGetAllMessage(
        DBusConnection connection,
        string objectPath,
        string interfaceName)
    {
        using var writer = connection.GetMessageWriter();
        writer.WriteMethodCallHeader(
            destination: BlueZConstants.ServiceName,
            path: objectPath,
            @interface: BlueZConstants.PropertiesInterface,
            signature: "s",
            member: BlueZConstants.MethodPropertiesGetAll);
        writer.WriteString(interfaceName);
        return writer.CreateMessage();
    }

    private static IReadOnlyDictionary<string, VariantValue> ParsePropertiesGetAllReply(Message message, object? state)
    {
        var reader = message.GetBodyReader();
        return reader.ReadDictionaryOfStringToVariantValue();
    }

    // ==================== Signal subscription — InterfacesAdded ====================

    /// <summary>
    ///     Subscribes to <c>org.bluez InterfacesAdded</c> signals.
    /// </summary>
    /// <param name="connection">The D-Bus connection.</param>
    /// <param name="handler">
    ///     Invoked for each signal. First arg is the object path, second is the interface+property map.
    /// </param>
    public static ValueTask<IDisposable> WatchInterfacesAddedAsync(
        DBusConnection connection,
        Action<Exception?, (string Path, IReadOnlyDictionary<string, IReadOnlyDictionary<string, VariantValue>> Interfaces)> handler)
    {
        var rule = new MatchRule
        {
            Type = MessageType.Signal,
            Sender = BlueZConstants.ServiceName,
            Interface = BlueZConstants.ObjectManagerInterface,
            Member = BlueZConstants.SignalInterfacesAdded,
        };

#pragma warning disable CS0618
        return connection.AddMatchAsync(
            rule,
            ParseInterfacesAddedSignal,
            (Exception? ex, (string, IReadOnlyDictionary<string, IReadOnlyDictionary<string, VariantValue>>) value, object? _, object? __) => handler(ex, value),
            ObserverFlags.None);
#pragma warning restore CS0618
    }

    private static (string Path, IReadOnlyDictionary<string, IReadOnlyDictionary<string, VariantValue>> Interfaces)
        ParseInterfacesAddedSignal(Message message, object? state)
    {
        var reader = message.GetBodyReader();
        var path = reader.ReadObjectPathAsString();
        var interfaces = ReadInterfacesDictionary(ref reader);
        return (path, interfaces);
    }

    // ==================== Signal subscription — InterfacesRemoved ====================

    /// <summary>
    ///     Subscribes to <c>org.bluez InterfacesRemoved</c> signals.
    /// </summary>
    public static ValueTask<IDisposable> WatchInterfacesRemovedAsync(
        DBusConnection connection,
        Action<Exception?, (string Path, string[] Interfaces)> handler)
    {
        var rule = new MatchRule
        {
            Type = MessageType.Signal,
            Sender = BlueZConstants.ServiceName,
            Interface = BlueZConstants.ObjectManagerInterface,
            Member = BlueZConstants.SignalInterfacesRemoved,
        };

#pragma warning disable CS0618
        return connection.AddMatchAsync(
            rule,
            ParseInterfacesRemovedSignal,
            (Exception? ex, (string, string[]) value, object? _, object? __) => handler(ex, value),
            ObserverFlags.None);
#pragma warning restore CS0618
    }

    private static (string Path, string[] Interfaces) ParseInterfacesRemovedSignal(Message message, object? state)
    {
        var reader = message.GetBodyReader();
        var path = reader.ReadObjectPathAsString();
        var interfaces = reader.ReadArrayOfString();
        return (path, interfaces);
    }

    // ==================== Signal subscription — PropertiesChanged ====================

    /// <summary>
    ///     Subscribes to <c>org.freedesktop.DBus.Properties.PropertiesChanged</c> on a specific path.
    /// </summary>
    public static ValueTask<IDisposable> WatchPropertiesChangedAsync(
        DBusConnection connection,
        string objectPath,
        Action<Exception?, (string InterfaceName, IReadOnlyDictionary<string, VariantValue> Changed)> handler)
    {
        var rule = new MatchRule
        {
            Type = MessageType.Signal,
            Sender = BlueZConstants.ServiceName,
            Interface = BlueZConstants.PropertiesInterface,
            Member = BlueZConstants.SignalPropertiesChanged,
            Path = objectPath,
        };

#pragma warning disable CS0618
        return connection.AddMatchAsync(
            rule,
            ParsePropertiesChangedSignal,
            (Exception? ex, (string, IReadOnlyDictionary<string, VariantValue>) value, object? _, object? __) => handler(ex, value),
            ObserverFlags.None);
#pragma warning restore CS0618
    }

    private static (string InterfaceName, IReadOnlyDictionary<string, VariantValue> Changed)
        ParsePropertiesChangedSignal(Message message, object? state)
    {
        var reader = message.GetBodyReader();
        var interfaceName = reader.ReadString();
        var changed = reader.ReadDictionaryOfStringToVariantValue();
        // Skip invalidated array (third argument) — not needed for current use-cases
        return (interfaceName, changed);
    }

    // ==================== Parsing helpers ====================

    /// <summary>
    ///     Reads an <c>a{sa{sv}}</c> D-Bus value (interface-name → property-name → variant).
    /// </summary>
    internal static IReadOnlyDictionary<string, IReadOnlyDictionary<string, VariantValue>> ReadInterfacesDictionary(
        ref Reader reader)
    {
        var result = new Dictionary<string, IReadOnlyDictionary<string, VariantValue>>();

        var ifacesDictEnd = reader.ReadDictionaryStart();
        while (reader.HasNext(ifacesDictEnd))
        {
            var interfaceName = reader.ReadString();
            var props = ReadPropertyDictionary(ref reader);
            result[interfaceName] = props;
        }

        return result;
    }

    /// <summary>
    ///     Reads an <c>a{sv}</c> D-Bus value (property-name → variant).
    /// </summary>
    internal static IReadOnlyDictionary<string, VariantValue> ReadPropertyDictionary(ref Reader reader)
    {
        return reader.ReadDictionaryOfStringToVariantValue();
    }
}
