namespace Bluetooth.Maui.Platforms.Apple.Tools;

/// <summary>
///     Extension methods for converting between Bluetooth characteristic properties and permissions to their iOS CoreBluetooth equivalents.
///     Supports bidirectional conversion between abstract types and native Apple types.
/// </summary>
public static class GattMapping
{
    #region Properties - Shared to Native

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

    #endregion

    #region Properties - Native to Shared

    /// <summary>
    ///     Converts iOS <see cref="CBCharacteristicProperties" /> flags to the corresponding <see cref="BluetoothCharacteristicProperties" />.
    /// </summary>
    /// <param name="nativeProps">The iOS CoreBluetooth characteristic property flags to convert.</param>
    /// <returns>A <see cref="BluetoothCharacteristicProperties" /> value representing the specified native properties.</returns>
    public static BluetoothCharacteristicProperties ToSharedCharacteristicProperties(this CBCharacteristicProperties nativeProps)
    {
        BluetoothCharacteristicProperties result = 0;

        if (nativeProps.HasFlag(CBCharacteristicProperties.Broadcast))
        {
            result |= BluetoothCharacteristicProperties.Broadcast;
        }

        if (nativeProps.HasFlag(CBCharacteristicProperties.Read))
        {
            result |= BluetoothCharacteristicProperties.Read;
        }

        if (nativeProps.HasFlag(CBCharacteristicProperties.WriteWithoutResponse))
        {
            result |= BluetoothCharacteristicProperties.WriteWithoutResponse;
        }

        if (nativeProps.HasFlag(CBCharacteristicProperties.Write))
        {
            result |= BluetoothCharacteristicProperties.Write;
        }

        if (nativeProps.HasFlag(CBCharacteristicProperties.Notify))
        {
            result |= BluetoothCharacteristicProperties.Notify;
        }

        if (nativeProps.HasFlag(CBCharacteristicProperties.Indicate))
        {
            result |= BluetoothCharacteristicProperties.Indicate;
        }

        if (nativeProps.HasFlag(CBCharacteristicProperties.ExtendedProperties))
        {
            result |= BluetoothCharacteristicProperties.ExtendedProperties;
        }

        // Note: CoreBluetooth AuthenticatedSignedWrites exist but aren't directly exposed in CBCharacteristicProperties
        // SignedWrite would need special handling if present in CBCharacteristicProperties

        return result;
    }

    #endregion

    #region Characteristic Permissions - Shared to Native

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

    #endregion

    #region Characteristic Permissions - Native to Shared

    /// <summary>
    ///     Converts iOS <see cref="CBAttributePermissions" /> flags to the corresponding <see cref="BluetoothCharacteristicPermissions" />.
    /// </summary>
    /// <param name="nativePerms">The iOS CoreBluetooth attribute permission flags to convert.</param>
    /// <returns>A <see cref="BluetoothCharacteristicPermissions" /> value representing the specified native permissions.</returns>
    /// <remarks>
    ///     CoreBluetooth's permission model is simpler than Android's, so some granularity is lost in reverse conversion.
    ///     Encryption-required permissions are mapped to authenticated permissions for consistency.
    /// </remarks>
    public static BluetoothCharacteristicPermissions ToSharedCharacteristicPermissions(this CBAttributePermissions nativePerms)
    {
        BluetoothCharacteristicPermissions result = 0;

        // Read permissions
        if (nativePerms.HasFlag(CBAttributePermissions.ReadEncryptionRequired))
        {
            result |= BluetoothCharacteristicPermissions.ReadAuthenticated;
        }
        else if (nativePerms.HasFlag(CBAttributePermissions.Readable))
        {
            result |= BluetoothCharacteristicPermissions.Read;
        }

        // Write permissions
        if (nativePerms.HasFlag(CBAttributePermissions.WriteEncryptionRequired))
        {
            result |= BluetoothCharacteristicPermissions.WriteAuthenticated;
        }
        else if (nativePerms.HasFlag(CBAttributePermissions.Writeable))
        {
            result |= BluetoothCharacteristicPermissions.Write;
        }

        return result;
    }

    #endregion

    #region Descriptor Permissions - Shared to Native

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

    #endregion

    #region Descriptor Permissions - Native to Shared

    /// <summary>
    ///     Converts iOS <see cref="CBAttributePermissions" /> flags to the corresponding <see cref="BluetoothDescriptorPermissions" />.
    /// </summary>
    /// <param name="nativePerms">The iOS CoreBluetooth attribute permission flags to convert.</param>
    /// <returns>A <see cref="BluetoothDescriptorPermissions" /> value representing the specified native permissions.</returns>
    /// <remarks>
    ///     CoreBluetooth's permission model is simpler than Android's, so some granularity is lost in reverse conversion.
    ///     Encryption-required permissions are mapped to authenticated permissions for consistency.
    /// </remarks>
    public static BluetoothDescriptorPermissions ToSharedDescriptorPermissions(this CBAttributePermissions nativePerms)
    {
        BluetoothDescriptorPermissions result = 0;

        // Read permissions
        if (nativePerms.HasFlag(CBAttributePermissions.ReadEncryptionRequired))
        {
            result |= BluetoothDescriptorPermissions.ReadAuthenticated;
        }
        else if (nativePerms.HasFlag(CBAttributePermissions.Readable))
        {
            result |= BluetoothDescriptorPermissions.Read;
        }

        // Write permissions
        if (nativePerms.HasFlag(CBAttributePermissions.WriteEncryptionRequired))
        {
            result |= BluetoothDescriptorPermissions.WriteAuthenticated;
        }
        else if (nativePerms.HasFlag(CBAttributePermissions.Writeable))
        {
            result |= BluetoothDescriptorPermissions.Write;
        }

        return result;
    }

    #endregion
}
