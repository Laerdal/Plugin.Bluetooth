using Bluetooth.Avalonia.Platforms.Linux.Scanning.Factories;

namespace Bluetooth.Avalonia.Platforms.Linux.Scanning;

/// <summary>
///     Linux BLE remote GATT descriptor backed by a BlueZ <c>org.bluez.GattDescriptor1</c> D-Bus object.
/// </summary>
public class LinuxBluetoothRemoteDescriptor : BaseBluetoothRemoteDescriptor
{
    private readonly LinuxBluetoothAdapter _adapter;

    /// <inheritdoc />
    public LinuxBluetoothRemoteDescriptor(
        IBluetoothRemoteCharacteristic parentCharacteristic,
        IBluetoothRemoteDescriptorFactory.BluetoothRemoteDescriptorFactorySpec spec,
        IBluetoothNameProvider? nameProvider = null,
        ILogger<IBluetoothRemoteDescriptor>? logger = null)
        : base(parentCharacteristic, spec, logger)
    {
        ArgumentNullException.ThrowIfNull(spec);

        if (spec is not LinuxBluetoothRemoteDescriptorFactorySpec linuxSpec)
        {
            throw new ArgumentException(
                $"Expected {nameof(LinuxBluetoothRemoteDescriptorFactorySpec)} but got {spec.GetType()}.",
                nameof(spec));
        }

        ObjectPath = linuxSpec.ObjectPath;

        if (parentCharacteristic is not LinuxBluetoothRemoteCharacteristic linuxChar)
        {
            throw new ArgumentException(
                "Parent characteristic must be a LinuxBluetoothRemoteCharacteristic.", nameof(parentCharacteristic));
        }

        _adapter = linuxChar.Service.Device is LinuxBluetoothRemoteDevice linuxDevice
            ? linuxDevice.Adapter
            : throw new ArgumentException("Device must be a LinuxBluetoothRemoteDevice.", nameof(parentCharacteristic));
    }

    /// <summary>
    ///     Gets the D-Bus object path of this GATT descriptor.
    /// </summary>
    internal string ObjectPath { get; }

    // ==================== Capability detection ====================

    /// <inheritdoc />
    protected override bool NativeCanRead() => true; // BlueZ descriptors are always readable

    /// <inheritdoc />
    protected override bool NativeCanWrite() => true; // BlueZ descriptors are writable (e.g. CCCD)

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

            // WriteValue(ay value, a{sv} options)
            var message = CreateWriteValueMessage(connection, ObjectPath, value);

            await connection.CallMethodAsync(message).ConfigureAwait(false);
            OnWriteValueSucceeded();
        }
        catch (Exception ex)
        {
            OnWriteValueFailed(ex);
        }
    }

    // ==================== Private helpers ====================

    private static MessageBuffer CreateReadValueMessage(DBusConnection connection, string objectPath)
    {
        using var writer = connection.GetMessageWriter();
        writer.WriteMethodCallHeader(
            destination: BlueZConstants.ServiceName,
            path: objectPath,
            @interface: BlueZConstants.GattDescriptor1Interface,
            signature: "a{sv}",
            member: BlueZConstants.MethodReadValue);
        writer.WriteDictionary(new Dictionary<string, VariantValue>());
        return writer.CreateMessage();
    }

    private static MessageBuffer CreateWriteValueMessage(
        DBusConnection connection,
        string objectPath,
        ReadOnlyMemory<byte> value)
    {
        using var writer = connection.GetMessageWriter();
        writer.WriteMethodCallHeader(
            destination: BlueZConstants.ServiceName,
            path: objectPath,
            @interface: BlueZConstants.GattDescriptor1Interface,
            signature: "aya{sv}",
            member: BlueZConstants.MethodWriteValue);
        writer.WriteArray(value.ToArray());
        writer.WriteDictionary(new Dictionary<string, VariantValue>());
        return writer.CreateMessage();
    }
}
