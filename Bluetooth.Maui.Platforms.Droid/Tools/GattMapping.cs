namespace Bluetooth.Maui.Platforms.Droid.Tools;

/// <summary>
/// Provides extension methods for converting Bluetooth characteristic properties and permissions to their corresponding Android GATT flags.
/// </summary>
public static class GattMapping
{
    /// <summary>
    /// Converts the specified <see cref="BluetoothCharacteristicProperties"/> to the corresponding Android <see cref="GattProperty"/> flags.
    /// </summary>
    /// <param name="props">The Bluetooth characteristic properties to convert.</param>
    /// <returns>A <see cref="GattProperty"/> value representing the specified properties.</returns>
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

    /// <summary>
    /// Converts the specified <see cref="BluetoothCharacteristicPermissions"/> to the corresponding Android <see cref="GattPermission"/> flags.
    /// </summary>
    /// <param name="perms">The Bluetooth characteristic permissions to convert.</param>
    /// <returns>A <see cref="GattPermission"/> value representing the specified permissions.</returns>
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
    
    /// <summary>
    /// Converts the specified <see cref="BluetoothDescriptorPermissions"/> to the corresponding Android <see cref="GattPermission"/> flags.
    /// </summary>
    /// <param name="perms">The Bluetooth descriptor permissions to convert.</param>
    /// <returns>A <see cref="GattPermission"/> value representing the specified permissions.</returns>
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
}