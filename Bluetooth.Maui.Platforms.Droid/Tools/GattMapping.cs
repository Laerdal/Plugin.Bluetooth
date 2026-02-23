namespace Bluetooth.Maui.Platforms.Droid.Tools;

/// <summary>
///     Provides extension methods for converting Bluetooth characteristic properties and permissions to their corresponding Android GATT flags.
///     Supports bidirectional conversion between abstract types and native Android types.
/// </summary>
public static class GattMapping
{
    #region Properties - Shared to Native

    /// <summary>
    ///     Converts the specified <see cref="BluetoothCharacteristicProperties" /> to the corresponding Android <see cref="GattProperty" /> flags.
    /// </summary>
    /// <param name="props">The Bluetooth characteristic properties to convert.</param>
    /// <returns>A <see cref="GattProperty" /> value representing the specified properties.</returns>
    public static GattProperty ToNative(this BluetoothCharacteristicProperties props)
    {
        GattProperty result = 0;

        if (props.HasFlag(BluetoothCharacteristicProperties.Broadcast))
        {
            result |= GattProperty.Broadcast;
        }

        if (props.HasFlag(BluetoothCharacteristicProperties.Read))
        {
            result |= GattProperty.Read;
        }

        if (props.HasFlag(BluetoothCharacteristicProperties.WriteWithoutResponse))
        {
            result |= GattProperty.WriteNoResponse;
        }

        if (props.HasFlag(BluetoothCharacteristicProperties.Write))
        {
            result |= GattProperty.Write;
        }

        if (props.HasFlag(BluetoothCharacteristicProperties.Notify))
        {
            result |= GattProperty.Notify;
        }

        if (props.HasFlag(BluetoothCharacteristicProperties.Indicate))
        {
            result |= GattProperty.Indicate;
        }

        if (props.HasFlag(BluetoothCharacteristicProperties.SignedWrite))
        {
            result |= GattProperty.SignedWrite;
        }

        if (props.HasFlag(BluetoothCharacteristicProperties.ExtendedProperties))
        {
            result |= GattProperty.ExtendedProps;
        }

        return result;
    }

    #endregion

    #region Properties - Native to Shared

    /// <summary>
    ///     Converts Android <see cref="GattProperty" /> flags to the corresponding <see cref="BluetoothCharacteristicProperties" />.
    /// </summary>
    /// <param name="nativeProps">The Android GATT property flags to convert.</param>
    /// <returns>A <see cref="BluetoothCharacteristicProperties" /> value representing the specified native properties.</returns>
    public static BluetoothCharacteristicProperties ToSharedCharacteristicProperties(this GattProperty nativeProps)
    {
        BluetoothCharacteristicProperties result = 0;

        if (nativeProps.HasFlag(GattProperty.Broadcast))
        {
            result |= BluetoothCharacteristicProperties.Broadcast;
        }

        if (nativeProps.HasFlag(GattProperty.Read))
        {
            result |= BluetoothCharacteristicProperties.Read;
        }

        if (nativeProps.HasFlag(GattProperty.WriteNoResponse))
        {
            result |= BluetoothCharacteristicProperties.WriteWithoutResponse;
        }

        if (nativeProps.HasFlag(GattProperty.Write))
        {
            result |= BluetoothCharacteristicProperties.Write;
        }

        if (nativeProps.HasFlag(GattProperty.Notify))
        {
            result |= BluetoothCharacteristicProperties.Notify;
        }

        if (nativeProps.HasFlag(GattProperty.Indicate))
        {
            result |= BluetoothCharacteristicProperties.Indicate;
        }

        if (nativeProps.HasFlag(GattProperty.SignedWrite))
        {
            result |= BluetoothCharacteristicProperties.SignedWrite;
        }

        if (nativeProps.HasFlag(GattProperty.ExtendedProps))
        {
            result |= BluetoothCharacteristicProperties.ExtendedProperties;
        }

        return result;
    }

    #endregion

    #region Characteristic Permissions - Shared to Native

    /// <summary>
    ///     Converts the specified <see cref="BluetoothCharacteristicPermissions" /> to the corresponding Android <see cref="GattPermission" /> flags.
    /// </summary>
    /// <param name="perms">The Bluetooth characteristic permissions to convert.</param>
    /// <returns>A <see cref="GattPermission" /> value representing the specified permissions.</returns>
    public static GattPermission ToNative(this BluetoothCharacteristicPermissions perms)
    {
        GattPermission result = 0;

        // Read
        if (perms.HasFlag(BluetoothCharacteristicPermissions.ReadAuthenticated))
        {
            result |= GattPermission.ReadEncryptedMitm;
        }
        else if (perms.HasFlag(BluetoothCharacteristicPermissions.ReadEncrypted))
        {
            result |= GattPermission.ReadEncrypted;
        }
        else if (perms.HasFlag(BluetoothCharacteristicPermissions.Read))
        {
            result |= GattPermission.Read;
        }

        // Write
        if (perms.HasFlag(BluetoothCharacteristicPermissions.WriteSignedAuthenticated))
        {
            result |= GattPermission.WriteSignedMitm;
        }
        else if (perms.HasFlag(BluetoothCharacteristicPermissions.WriteSigned))
        {
            result |= GattPermission.WriteSigned;
        }

        if (perms.HasFlag(BluetoothCharacteristicPermissions.WriteAuthenticated))
        {
            result |= GattPermission.WriteEncryptedMitm;
        }
        else if (perms.HasFlag(BluetoothCharacteristicPermissions.WriteEncrypted))
        {
            result |= GattPermission.WriteEncrypted;
        }
        else if (perms.HasFlag(BluetoothCharacteristicPermissions.Write))
        {
            result |= GattPermission.Write;
        }

        return result;
    }

    #endregion

    #region Characteristic Permissions - Native to Shared

    /// <summary>
    ///     Converts Android <see cref="GattPermission" /> flags to the corresponding <see cref="BluetoothCharacteristicPermissions" />.
    /// </summary>
    /// <param name="nativePerms">The Android GATT permission flags to convert.</param>
    /// <returns>A <see cref="BluetoothCharacteristicPermissions" /> value representing the specified native permissions.</returns>
    public static BluetoothCharacteristicPermissions ToSharedCharacteristicPermissions(this GattPermission nativePerms)
    {
        BluetoothCharacteristicPermissions result = 0;

        // Read permissions
        if (nativePerms.HasFlag(GattPermission.ReadEncryptedMitm))
        {
            result |= BluetoothCharacteristicPermissions.ReadAuthenticated;
        }
        else if (nativePerms.HasFlag(GattPermission.ReadEncrypted))
        {
            result |= BluetoothCharacteristicPermissions.ReadEncrypted;
        }
        else if (nativePerms.HasFlag(GattPermission.Read))
        {
            result |= BluetoothCharacteristicPermissions.Read;
        }

        // Write permissions
        if (nativePerms.HasFlag(GattPermission.WriteSignedMitm))
        {
            result |= BluetoothCharacteristicPermissions.WriteSignedAuthenticated;
        }
        else if (nativePerms.HasFlag(GattPermission.WriteSigned))
        {
            result |= BluetoothCharacteristicPermissions.WriteSigned;
        }

        if (nativePerms.HasFlag(GattPermission.WriteEncryptedMitm))
        {
            result |= BluetoothCharacteristicPermissions.WriteAuthenticated;
        }
        else if (nativePerms.HasFlag(GattPermission.WriteEncrypted))
        {
            result |= BluetoothCharacteristicPermissions.WriteEncrypted;
        }
        else if (nativePerms.HasFlag(GattPermission.Write))
        {
            result |= BluetoothCharacteristicPermissions.Write;
        }

        return result;
    }

    #endregion

    #region Descriptor Permissions - Shared to Native

    /// <summary>
    ///     Converts the specified <see cref="BluetoothDescriptorPermissions" /> to the corresponding Android <see cref="GattPermission" /> flags.
    /// </summary>
    /// <param name="perms">The Bluetooth descriptor permissions to convert.</param>
    /// <returns>A <see cref="GattPermission" /> value representing the specified permissions.</returns>
    public static GattPermission ToNative(this BluetoothDescriptorPermissions perms)
    {
        GattPermission result = 0;

        // Read
        if (perms.HasFlag(BluetoothDescriptorPermissions.ReadAuthenticated))
        {
            result |= GattPermission.ReadEncryptedMitm;
        }
        else if (perms.HasFlag(BluetoothDescriptorPermissions.ReadEncrypted))
        {
            result |= GattPermission.ReadEncrypted;
        }
        else if (perms.HasFlag(BluetoothDescriptorPermissions.Read))
        {
            result |= GattPermission.Read;
        }

        // Write
        if (perms.HasFlag(BluetoothDescriptorPermissions.WriteAuthenticated))
        {
            result |= GattPermission.WriteEncryptedMitm;
        }
        else if (perms.HasFlag(BluetoothDescriptorPermissions.WriteEncrypted))
        {
            result |= GattPermission.WriteEncrypted;
        }
        else if (perms.HasFlag(BluetoothDescriptorPermissions.Write))
        {
            result |= GattPermission.Write;
        }

        return result;
    }

    #endregion

    #region Descriptor Permissions - Native to Shared

    /// <summary>
    ///     Converts Android <see cref="GattPermission" /> flags to the corresponding <see cref="BluetoothDescriptorPermissions" />.
    /// </summary>
    /// <param name="nativePerms">The Android GATT permission flags to convert.</param>
    /// <returns>A <see cref="BluetoothDescriptorPermissions" /> value representing the specified native permissions.</returns>
    public static BluetoothDescriptorPermissions ToSharedDescriptorPermissions(this GattPermission nativePerms)
    {
        BluetoothDescriptorPermissions result = 0;

        // Read permissions
        if (nativePerms.HasFlag(GattPermission.ReadEncryptedMitm))
        {
            result |= BluetoothDescriptorPermissions.ReadAuthenticated;
        }
        else if (nativePerms.HasFlag(GattPermission.ReadEncrypted))
        {
            result |= BluetoothDescriptorPermissions.ReadEncrypted;
        }
        else if (nativePerms.HasFlag(GattPermission.Read))
        {
            result |= BluetoothDescriptorPermissions.Read;
        }

        // Write permissions
        if (nativePerms.HasFlag(GattPermission.WriteEncryptedMitm))
        {
            result |= BluetoothDescriptorPermissions.WriteAuthenticated;
        }
        else if (nativePerms.HasFlag(GattPermission.WriteEncrypted))
        {
            result |= BluetoothDescriptorPermissions.WriteEncrypted;
        }
        else if (nativePerms.HasFlag(GattPermission.Write))
        {
            result |= BluetoothDescriptorPermissions.Write;
        }

        return result;
    }

    #endregion
}

