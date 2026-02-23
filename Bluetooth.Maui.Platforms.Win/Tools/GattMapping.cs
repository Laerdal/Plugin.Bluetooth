namespace Bluetooth.Maui.Platforms.Win.Tools;

/// <summary>
///     Provides extension methods for converting Bluetooth characteristic properties and permissions to their corresponding Windows GATT flags.
/// </summary>
public static class GattMapping
{
    /// <summary>
    ///     Converts the specified <see cref="BluetoothCharacteristicProperties" /> to the corresponding Windows <see cref="GattCharacteristicProperties" /> flags.
    /// </summary>
    /// <param name="props">The Bluetooth characteristic properties to convert.</param>
    /// <returns>A <see cref="GattCharacteristicProperties" /> value representing the specified properties.</returns>
    public static GattCharacteristicProperties ToNative(this BluetoothCharacteristicProperties props)
    {
        var result = GattCharacteristicProperties.None;

        if (props.HasFlag(BluetoothCharacteristicProperties.Broadcast))
        {
            result |= GattCharacteristicProperties.Broadcast;
        }

        if (props.HasFlag(BluetoothCharacteristicProperties.Read))
        {
            result |= GattCharacteristicProperties.Read;
        }

        if (props.HasFlag(BluetoothCharacteristicProperties.WriteWithoutResponse))
        {
            result |= GattCharacteristicProperties.WriteWithoutResponse;
        }

        if (props.HasFlag(BluetoothCharacteristicProperties.Write))
        {
            result |= GattCharacteristicProperties.Write;
        }

        if (props.HasFlag(BluetoothCharacteristicProperties.Notify))
        {
            result |= GattCharacteristicProperties.Notify;
        }

        if (props.HasFlag(BluetoothCharacteristicProperties.Indicate))
        {
            result |= GattCharacteristicProperties.Indicate;
        }

        if (props.HasFlag(BluetoothCharacteristicProperties.ExtendedProperties))
        {
            result |= GattCharacteristicProperties.ExtendedProperties;
        }

        // SignedWrite doesn’t really exist as a first-class property in Windows GATT server.
        return result;
    }

    /// <summary>
    ///     Applies the specified <see cref="BluetoothCharacteristicPermissions" /> to the given <see cref="GattLocalCharacteristicParameters" /> by setting the appropriate read and write protection levels.
    /// </summary>
    /// <param name="perms">The Bluetooth characteristic permissions to apply.</param>
    /// <param name="parameters">The GATT local characteristic parameters to modify based on the permissions.</param>
    public static void ApplyTo(this BluetoothCharacteristicPermissions perms, GattLocalCharacteristicParameters parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);
        parameters.ReadProtectionLevel = MapReadProtection(perms);
        parameters.WriteProtectionLevel = MapWriteProtection(perms);
    }

    private static GattProtectionLevel MapReadProtection(BluetoothCharacteristicPermissions perms)
    {
        if (perms.HasFlag(BluetoothCharacteristicPermissions.ReadAuthenticated) &&
            perms.HasFlag(BluetoothCharacteristicPermissions.ReadEncrypted))
        {
            return GattProtectionLevel.EncryptionAndAuthenticationRequired;
        }

        if (perms.HasFlag(BluetoothCharacteristicPermissions.ReadAuthenticated))
        {
            return GattProtectionLevel.AuthenticationRequired;
        }

        if (perms.HasFlag(BluetoothCharacteristicPermissions.ReadEncrypted))
        {
            return GattProtectionLevel.EncryptionRequired;
        }

        return GattProtectionLevel.Plain;
    }

    private static GattProtectionLevel MapWriteProtection(BluetoothCharacteristicPermissions perms)
    {
        // Signed write -> best-effort treat as authentication required (or encryption+auth if requested)
        var wantsSigned = perms.HasFlag(BluetoothCharacteristicPermissions.WriteSigned) ||
                          perms.HasFlag(BluetoothCharacteristicPermissions.WriteSignedAuthenticated);

        if ((perms.HasFlag(BluetoothCharacteristicPermissions.WriteAuthenticated) || wantsSigned) &&
            perms.HasFlag(BluetoothCharacteristicPermissions.WriteEncrypted))
        {
            return GattProtectionLevel.EncryptionAndAuthenticationRequired;
        }

        if (perms.HasFlag(BluetoothCharacteristicPermissions.WriteAuthenticated) || wantsSigned)
        {
            return GattProtectionLevel.AuthenticationRequired;
        }

        if (perms.HasFlag(BluetoothCharacteristicPermissions.WriteEncrypted))
        {
            return GattProtectionLevel.EncryptionRequired;
        }

        return GattProtectionLevel.Plain;
    }

    /// <summary>
    ///     Applies the specified <see cref="BluetoothDescriptorPermissions" /> to the given <see cref="GattLocalDescriptorParameters" /> by setting the appropriate read and write protection levels.
    /// </summary>
    /// <param name="perms">The Bluetooth descriptor permissions to apply.</param>
    /// <param name="parameters">The GATT local descriptor parameters to modify based on the permissions.</param>
    public static void ApplyTo(
        this BluetoothDescriptorPermissions perms,
        GattLocalDescriptorParameters parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);
        parameters.ReadProtectionLevel = MapRead(perms);
        parameters.WriteProtectionLevel = MapWrite(perms);
    }

    private static GattProtectionLevel MapRead(BluetoothDescriptorPermissions perms)
    {
        var encrypted = perms.HasFlag(BluetoothDescriptorPermissions.ReadEncrypted);
        var auth = perms.HasFlag(BluetoothDescriptorPermissions.ReadAuthenticated);

        if (encrypted && auth)
        {
            return GattProtectionLevel.EncryptionAndAuthenticationRequired;
        }

        if (auth)
        {
            return GattProtectionLevel.AuthenticationRequired;
        }

        if (encrypted)
        {
            return GattProtectionLevel.EncryptionRequired;
        }

        return GattProtectionLevel.Plain;
    }

    private static GattProtectionLevel MapWrite(BluetoothDescriptorPermissions perms)
    {
        var encrypted = perms.HasFlag(BluetoothDescriptorPermissions.WriteEncrypted);
        var auth = perms.HasFlag(BluetoothDescriptorPermissions.WriteAuthenticated);

        if (encrypted && auth)
        {
            return GattProtectionLevel.EncryptionAndAuthenticationRequired;
        }

        if (auth)
        {
            return GattProtectionLevel.AuthenticationRequired;
        }

        if (encrypted)
        {
            return GattProtectionLevel.EncryptionRequired;
        }

        return GattProtectionLevel.Plain;
    }
}
