namespace Bluetooth.Maui.Platforms.Apple.Tools;

/// <summary>
///     Extension methods for converting between Bluetooth characteristic properties and permissions to their iOS CoreBluetooth equivalents.
/// </summary>
public static class GattMapping
{
    /// <summary>
    ///     Converts the specified <see cref="BluetoothCharacteristicProperties" /> to the corresponding iOS <see cref="CBCharacteristicProperties" /> flags.
    /// </summary>
    /// <param name="props">The Bluetooth characteristic properties to convert.</param>
    /// <returns>A <see cref="CBCharacteristicProperties" /> value representing the specified properties.</returns>
    public static CBCharacteristicProperties ToNative(this BluetoothCharacteristicProperties props)
    {
        CBCharacteristicProperties result = 0;

        if (props.HasFlag(BluetoothCharacteristicProperties.Broadcast))
        {
            result |= CBCharacteristicProperties.Broadcast;
        }

        if (props.HasFlag(BluetoothCharacteristicProperties.Read))
        {
            result |= CBCharacteristicProperties.Read;
        }

        if (props.HasFlag(BluetoothCharacteristicProperties.WriteWithoutResponse))
        {
            result |= CBCharacteristicProperties.WriteWithoutResponse;
        }

        if (props.HasFlag(BluetoothCharacteristicProperties.Write))
        {
            result |= CBCharacteristicProperties.Write;
        }

        if (props.HasFlag(BluetoothCharacteristicProperties.Notify))
        {
            result |= CBCharacteristicProperties.Notify;
        }

        if (props.HasFlag(BluetoothCharacteristicProperties.Indicate))
        {
            result |= CBCharacteristicProperties.Indicate;
        }

        if (props.HasFlag(BluetoothCharacteristicProperties.ExtendedProperties))
        {
            result |= CBCharacteristicProperties.ExtendedProperties;
        }

        // CoreBluetooth does not expose "SignedWrite" as a first-class local/server capability in a way
        // that matches Android's signed write permissions. Ignore or treat as requiring authentication.
        // (You can choose to throw if you want strict behavior.)
        return result;
    }

    /// <summary>
    ///     Converts the specified <see cref="BluetoothCharacteristicPermissions" /> to the corresponding iOS <see cref="CBAttributePermissions" /> flags.
    /// </summary>
    /// <param name="perms">The Bluetooth characteristic permissions to convert.</param>
    /// <returns>A <see cref="CBAttributePermissions" /> value representing the specified permissions.</returns>
    public static CBAttributePermissions ToNative(this BluetoothCharacteristicPermissions perms)
    {
        CBAttributePermissions result = 0;

        // Read
        if (perms.HasFlag(BluetoothCharacteristicPermissions.ReadAuthenticated))
        {
            result |= CBAttributePermissions.ReadEncryptionRequired; // closest available
        }
        else if (perms.HasFlag(BluetoothCharacteristicPermissions.ReadEncrypted))
        {
            result |= CBAttributePermissions.ReadEncryptionRequired;
        }
        else if (perms.HasFlag(BluetoothCharacteristicPermissions.Read))
        {
            result |= CBAttributePermissions.Readable;
        }

        // Write
        if (perms.HasFlag(BluetoothCharacteristicPermissions.WriteAuthenticated))
        {
            result |= CBAttributePermissions.WriteEncryptionRequired; // closest available
        }
        else if (perms.HasFlag(BluetoothCharacteristicPermissions.WriteEncrypted))
        {
            result |= CBAttributePermissions.WriteEncryptionRequired;
        }
        else if (perms.HasFlag(BluetoothCharacteristicPermissions.Write))
        {
            result |= CBAttributePermissions.Writeable;
        }

        // Signed writes are not modeled in CBAttributePermissions like Android.
        // Best-effort: treat as encryption required (or ignore).
        if (perms.HasFlag(BluetoothCharacteristicPermissions.WriteSigned) ||
            perms.HasFlag(BluetoothCharacteristicPermissions.WriteSignedAuthenticated))
        {
            // Choose best-effort:
            result |= CBAttributePermissions.WriteEncryptionRequired;
        }

        return result;
    }

    /// <summary>
    ///     Converts the specified <see cref="BluetoothDescriptorPermissions" /> to the corresponding iOS <see cref="CBAttributePermissions" /> flags.
    /// </summary>
    /// <param name="perms">The Bluetooth descriptor permissions to convert.</param>
    /// <returns>A <see cref="CBAttributePermissions" /> value representing the specified permissions.</returns>
    public static CBAttributePermissions ToNative(this BluetoothDescriptorPermissions perms)
    {
        CBAttributePermissions result = 0;

        // Read
        if (perms.HasFlag(BluetoothDescriptorPermissions.ReadAuthenticated) ||
            perms.HasFlag(BluetoothDescriptorPermissions.ReadEncrypted))
        {
            result |= CBAttributePermissions.ReadEncryptionRequired;
        }
        else if (perms.HasFlag(BluetoothDescriptorPermissions.Read))
        {
            result |= CBAttributePermissions.Readable;
        }

        // Write
        if (perms.HasFlag(BluetoothDescriptorPermissions.WriteAuthenticated) ||
            perms.HasFlag(BluetoothDescriptorPermissions.WriteEncrypted))
        {
            result |= CBAttributePermissions.WriteEncryptionRequired;
        }
        else if (perms.HasFlag(BluetoothDescriptorPermissions.Write))
        {
            result |= CBAttributePermissions.Writeable;
        }

        return result;
    }
}
