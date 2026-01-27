using Bluetooth.Maui.PlatformSpecific;

namespace Bluetooth.Maui;

/// <summary>
/// iOS implementation of a mutable Bluetooth characteristic for the broadcaster/peripheral role.
/// </summary>
/// <remarks>
/// This implementation wraps iOS's <see cref="CBMutableCharacteristic"/> for hosting GATT characteristics.
/// Unlike <see cref="BluetoothCharacteristic"/>, this is used when the device acts as a peripheral.
/// </remarks>
public partial class BluetoothBroadcasterCharacteristic : BaseBluetoothBroadcastCharacteristic, CbPeripheralManagerWrapper.ICbCharacteristicDelegate
{
    /// <inheritdoc />
    public BluetoothBroadcasterCharacteristic(IBluetoothBroadcastService service, Guid id, string name, bool isPrimary,
        CharacteristicPermissions permissions,
        CharacteristicProperties properties,
        ReadOnlyMemory<byte>? initialValue = null) : base(service, id, name, isPrimary,
                                                          permissions,
                                                          properties,
                                                          initialValue)
    {
    }

    /// <inheritdoc />
    protected override Task NativeUpdateValueAsync(ReadOnlyMemory<byte> value, bool notifyClients = true, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void CharacteristicSubscribed(CBCentral central, CBCharacteristic characteristic)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void CharacteristicUnsubscribed(CBCentral central, CBCharacteristic characteristic)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void ReadRequestReceived(CBATTRequest request)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void WriteRequestsReceived(CBATTRequest request)
    {
        throw new NotImplementedException();
    }

    private static CBCharacteristicProperties ConvertProperties(CharacteristicProperties properties)
    {
        CBCharacteristicProperties result = 0;

        if (properties.HasFlag(CharacteristicProperties.Broadcast))
        {
            result |= CBCharacteristicProperties.Broadcast;
        }
        if (properties.HasFlag(CharacteristicProperties.Read))
        {
            result |= CBCharacteristicProperties.Read;
        }
        if (properties.HasFlag(CharacteristicProperties.WriteWithoutResponse))
        {
            result |= CBCharacteristicProperties.WriteWithoutResponse;
        }
        if (properties.HasFlag(CharacteristicProperties.Write))
        {
            result |= CBCharacteristicProperties.Write;
        }
        if (properties.HasFlag(CharacteristicProperties.Notify))
        {
            result |= CBCharacteristicProperties.Notify;
        }
        if (properties.HasFlag(CharacteristicProperties.Indicate))
        {
            result |= CBCharacteristicProperties.Indicate;
        }
        if (properties.HasFlag(CharacteristicProperties.AuthenticatedSignedWrites))
        {
            result |= CBCharacteristicProperties.AuthenticatedSignedWrites;
        }
        if (properties.HasFlag(CharacteristicProperties.ExtendedProperties))
        {
            result |= CBCharacteristicProperties.ExtendedProperties;
        }

        return result;
    }

    private static CBAttributePermissions ConvertPermissions(CharacteristicPermissions permissions)
    {
        CBAttributePermissions result = 0;

        if (permissions.HasFlag(CharacteristicPermissions.Read))
        {
            result |= CBAttributePermissions.Readable;
        }
        if (permissions.HasFlag(CharacteristicPermissions.ReadEncrypted))
        {
            result |= CBAttributePermissions.ReadEncryptionRequired;
        }
        if (permissions.HasFlag(CharacteristicPermissions.Write))
        {
            result |= CBAttributePermissions.Writeable;
        }
        if (permissions.HasFlag(CharacteristicPermissions.WriteEncrypted))
        {
            result |= CBAttributePermissions.WriteEncryptionRequired;
        }

        return result;
    }
}
