using Bluetooth.Avalonia.Platforms.Linux.Scanning.Factories;

namespace Bluetooth.Avalonia.Platforms.Linux.Scanning;

/// <summary>
///     Linux BLE remote GATT characteristic backed by a BlueZ <c>org.bluez.GattCharacteristic1</c> D-Bus object.
/// </summary>
public class LinuxBluetoothRemoteCharacteristic : BaseBluetoothRemoteCharacteristic, IAsyncDisposable
{
    private readonly LinuxBluetoothAdapter _adapter;
    private readonly string[] _flags;
    private IDisposable? _notifySubscription;

    /// <inheritdoc />
    public LinuxBluetoothRemoteCharacteristic(
        IBluetoothRemoteService parentService,
        IBluetoothRemoteCharacteristicFactory.BluetoothRemoteCharacteristicFactorySpec spec,
        IBluetoothRemoteDescriptorFactory descriptorFactory,
        IBluetoothNameProvider? nameProvider = null,
        ILogger<IBluetoothRemoteCharacteristic>? logger = null)
        : base(parentService, spec, descriptorFactory, nameProvider, logger)
    {
        ArgumentNullException.ThrowIfNull(spec);

        if (spec is not LinuxBluetoothRemoteCharacteristicFactorySpec linuxSpec)
        {
            throw new ArgumentException(
                $"Expected {nameof(LinuxBluetoothRemoteCharacteristicFactorySpec)} but got {spec.GetType()}.",
                nameof(spec));
        }

        ObjectPath = linuxSpec.ObjectPath;
        _flags = linuxSpec.Flags;

        if (parentService is not LinuxBluetoothRemoteService linuxService)
        {
            throw new ArgumentException(
                "Parent service must be a LinuxBluetoothRemoteService.", nameof(parentService));
        }

        _adapter = linuxService.Device is LinuxBluetoothRemoteDevice linuxDevice
            ? linuxDevice.Adapter
            : throw new ArgumentException("Device must be a LinuxBluetoothRemoteDevice.", nameof(parentService));
    }

    /// <summary>
    ///     Gets the D-Bus object path of this GATT characteristic.
    /// </summary>
    internal string ObjectPath { get; }

    // ==================== Capability detection ====================

    /// <inheritdoc />
    protected override bool NativeCanRead()
        => Array.IndexOf(_flags, BlueZConstants.FlagRead) >= 0;

    /// <inheritdoc />
    protected override bool NativeCanWrite()
        => Array.IndexOf(_flags, BlueZConstants.FlagWrite) >= 0
           || Array.IndexOf(_flags, BlueZConstants.FlagWriteWithoutResponse) >= 0;

    /// <inheritdoc />
    protected override bool NativeCanListen()
        => Array.IndexOf(_flags, BlueZConstants.FlagNotify) >= 0
           || Array.IndexOf(_flags, BlueZConstants.FlagIndicate) >= 0;

    // ==================== Read ====================

    /// <inheritdoc />
    protected override async ValueTask NativeReadValueAsync()
    {
        try
        {
            var connection = _adapter.Connection;

            // ReadValue(a{sv} options) -> ay
            var message = CreateReadValueMessage(connection, ObjectPath);

            var bytes = await connection
                .CallMethodAsync(message, static (msg, _) => msg.GetBodyReader().ReadArrayOfByte())
                .ConfigureAwait(false);

            OnReadValueSucceeded(bytes);
        }
        catch (Exception ex)
        {
            OnReadValueFailed(ex);
        }
    }

    // ==================== Write ====================

    /// <inheritdoc />
    protected override async ValueTask NativeWriteValueAsync(ReadOnlyMemory<byte> value)
    {
        try
        {
            var connection = _adapter.Connection;
            var withoutResponse = Array.IndexOf(_flags, BlueZConstants.FlagWriteWithoutResponse) >= 0
                                  && Array.IndexOf(_flags, BlueZConstants.FlagWrite) < 0;

            // WriteValue(ay value, a{sv} options)
            var message = CreateWriteValueMessage(connection, ObjectPath, value, withoutResponse);

            await connection.CallMethodAsync(message).ConfigureAwait(false);
            OnWriteValueSucceeded();
        }
        catch (Exception ex)
        {
            OnWriteValueFailed(ex);
        }
    }

    // ==================== Reliable Write (not supported on Linux via BlueZ D-Bus) ====================

    /// <inheritdoc />
    protected override ValueTask NativeBeginReliableWriteAsync()
    {
        // BlueZ does not expose reliable write (ATT Prepared Write) via D-Bus.
        // Treat as a no-op so the base-class state machine continues.
        OnBeginReliableWriteSucceeded();
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    protected override ValueTask NativeExecuteReliableWriteAsync()
    {
        OnExecuteReliableWriteSucceeded();
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    protected override ValueTask NativeAbortReliableWriteAsync()
    {
        OnAbortReliableWriteSucceeded();
        return ValueTask.CompletedTask;
    }

    // ==================== Listen (Notify / Indicate) ====================

    /// <inheritdoc />
    protected override async ValueTask NativeReadIsListeningAsync()
    {
        try
        {
            var connection = _adapter.Connection;
            var props = await BlueZObjectManager
                .GetAllPropertiesAsync(connection, ObjectPath, BlueZConstants.GattCharacteristic1Interface)
                .ConfigureAwait(false);

            var notifying = props.TryGetValue(BlueZConstants.PropNotifying, out var v)
                            && v.Type == VariantValueType.Bool
                            && v.GetBool();

            OnReadIsListeningSucceeded(notifying);
        }
        catch (Exception ex)
        {
            OnReadIsListeningFailed(ex);
        }
    }

    /// <inheritdoc />
    protected override async ValueTask NativeWriteIsListeningAsync(bool shouldBeListening)
    {
        try
        {
            var connection = _adapter.Connection;

            if (shouldBeListening)
            {
                // Subscribe to PropertiesChanged to receive value notifications.
                _notifySubscription?.Dispose();
                _notifySubscription = await BlueZObjectManager
                    .WatchPropertiesChangedAsync(connection, ObjectPath, OnNotifyPropertiesChanged)
                    .ConfigureAwait(false);

                // StartNotify()
                var message = CreateStartNotifyMessage(connection, ObjectPath);
                await connection.CallMethodAsync(message).ConfigureAwait(false);
            }
            else
            {
                // StopNotify()
                var message = CreateStopNotifyMessage(connection, ObjectPath);
                await connection.CallMethodAsync(message).ConfigureAwait(false);

                _notifySubscription?.Dispose();
                _notifySubscription = null;
            }

            OnWriteIsListeningSucceeded();
        }
        catch (Exception ex)
        {
            OnWriteIsListeningFailed(ex);
        }
    }

    // ==================== Descriptor exploration ====================

    /// <inheritdoc />
    protected override async ValueTask NativeDescriptorsExplorationAsync(
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var connection = _adapter.Connection;

            var objects = await BlueZObjectManager
                .GetManagedObjectsAsync(connection, cancellationToken)
                .ConfigureAwait(false);

            var descriptorObjects = objects
                .Where(o => o.HasInterface(BlueZConstants.GattDescriptor1Interface)
                            && o.Path.StartsWith(ObjectPath + "/", StringComparison.Ordinal))
                .ToList();

            OnDescriptorsExplorationSucceeded(
                descriptorObjects,
                (native, existing) => existing is LinuxBluetoothRemoteDescriptor d && d.ObjectPath == native.Path,
                native =>
                {
                    var spec = new LinuxBluetoothRemoteDescriptorFactorySpec(native);
                    return (DescriptorFactory
                            ?? throw new InvalidOperationException(
                                "DescriptorFactory must be set via the spec-based constructor."))
                        .Create(this, spec);
                });
        }
        catch (Exception ex)
        {
            OnDescriptorsExplorationFailed(ex);
        }
    }

    // ==================== Disposal ====================

    /// <inheritdoc />
    public new async ValueTask DisposeAsync()
    {
        _notifySubscription?.Dispose();
        _notifySubscription = null;
        await base.DisposeAsync().ConfigureAwait(false);
    }

    // ==================== Private helpers ====================

    private static MessageBuffer CreateReadValueMessage(DBusConnection connection, string objectPath)
    {
        using var writer = connection.GetMessageWriter();
        writer.WriteMethodCallHeader(
            destination: BlueZConstants.ServiceName,
            path: objectPath,
            @interface: BlueZConstants.GattCharacteristic1Interface,
            signature: "a{sv}",
            member: BlueZConstants.MethodReadValue);
        writer.WriteDictionary(new Dictionary<string, VariantValue>());
        return writer.CreateMessage();
    }

    private static MessageBuffer CreateWriteValueMessage(
        DBusConnection connection,
        string objectPath,
        ReadOnlyMemory<byte> value,
        bool withoutResponse)
    {
        using var writer = connection.GetMessageWriter();
        writer.WriteMethodCallHeader(
            destination: BlueZConstants.ServiceName,
            path: objectPath,
            @interface: BlueZConstants.GattCharacteristic1Interface,
            signature: "aya{sv}",
            member: BlueZConstants.MethodWriteValue);
        writer.WriteArray(value.ToArray());
        // Set "type" = "command" for write-without-response.
        writer.WriteDictionary(withoutResponse
            ? new Dictionary<string, VariantValue> { { "type", "command" } }
            : new Dictionary<string, VariantValue>());
        return writer.CreateMessage();
    }

    private static MessageBuffer CreateStartNotifyMessage(DBusConnection connection, string objectPath)
    {
        using var writer = connection.GetMessageWriter();
        writer.WriteMethodCallHeader(
            destination: BlueZConstants.ServiceName,
            path: objectPath,
            @interface: BlueZConstants.GattCharacteristic1Interface,
            member: BlueZConstants.MethodStartNotify);
        return writer.CreateMessage();
    }

    private static MessageBuffer CreateStopNotifyMessage(DBusConnection connection, string objectPath)
    {
        using var writer = connection.GetMessageWriter();
        writer.WriteMethodCallHeader(
            destination: BlueZConstants.ServiceName,
            path: objectPath,
            @interface: BlueZConstants.GattCharacteristic1Interface,
            member: BlueZConstants.MethodStopNotify);
        return writer.CreateMessage();
    }

    private void OnNotifyPropertiesChanged(
        Exception? error,
        (string InterfaceName, IReadOnlyDictionary<string, VariantValue> Changed) args)
    {
        if (error != null || args.InterfaceName != BlueZConstants.GattCharacteristic1Interface)
        {
            return;
        }

        if (!args.Changed.TryGetValue(BlueZConstants.PropValue, out var valueV))
        {
            return;
        }

        var bytes = valueV.Type == VariantValueType.Array
            ? valueV.GetArray<byte>()
            : [];

        OnReadValueSucceeded(bytes);
    }
}
