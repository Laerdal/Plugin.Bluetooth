using Bluetooth.Abstractions.Broadcasting.Factories;
using Bluetooth.Maui.Platforms.Apple.PlatformSpecific;

namespace Bluetooth.Maui.Platforms.Apple.Broadcasting;

/// <inheritdoc/>
public record BluetoothBroadcastCharacteristicFactoryRequest : IBluetoothBroadcastCharacteristicFactory.BluetoothBroadcastCharacteristicFactoryRequest
{
    /// <summary>
    /// Gets the UUID of the characteristic.
    /// </summary>
    public BluetoothBroadcastCharacteristicFactoryRequest()
    {
        _lazyNativeCharacteristic = new Lazy<CBMutableCharacteristic>(CreateCbMutableCharacteristic);
    }

    #region NativeCharacteristic

    /// <summary>
    /// Gets the native iOS mutable characteristic to use for this broadcast characteristic.
    /// </summary>
    public CBMutableCharacteristic NativeCharacteristic => _lazyNativeCharacteristic.Value;

    private readonly Lazy<CBMutableCharacteristic> _lazyNativeCharacteristic;

    private CBMutableCharacteristic CreateCbMutableCharacteristic()
    {
        using var nsUuid = Id.ToNsUuid();
        return new CBMutableCharacteristic(CBUUID.FromNSUuid(nsUuid), ConvertProperties(Properties), InitialValue.HasValue ? NSData.FromArray(InitialValue.Value.ToArray()) : null, ConvertPermissions(Permissions));
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

    #endregion NativeCharacteristic

}
