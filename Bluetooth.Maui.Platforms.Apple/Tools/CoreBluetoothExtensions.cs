namespace Bluetooth.Maui.Platforms.Apple.Tools;

/// <summary>
///     Extension methods for converting Core Bluetooth types to abstraction types.
/// </summary>
public static class CoreBluetoothExtensions
{
    /// <summary>
    ///     Converts iOS CBCharacteristicProperties to the cross-platform BluetoothCharacteristicProperties enum.
    /// </summary>
    /// <param name="properties">The iOS characteristic properties.</param>
    /// <returns>The cross-platform characteristic properties.</returns>
    public static BluetoothCharacteristicProperties ToCharacteristicProperties(this CBCharacteristicProperties properties)
    {
        var result = BluetoothCharacteristicProperties.None;

        if (properties.HasFlag(CBCharacteristicProperties.Broadcast))
        {
            result |= BluetoothCharacteristicProperties.Broadcast;
        }

        if (properties.HasFlag(CBCharacteristicProperties.Read))
        {
            result |= BluetoothCharacteristicProperties.Read;
        }

        if (properties.HasFlag(CBCharacteristicProperties.WriteWithoutResponse))
        {
            result |= BluetoothCharacteristicProperties.WriteWithoutResponse;
        }

        if (properties.HasFlag(CBCharacteristicProperties.Write))
        {
            result |= BluetoothCharacteristicProperties.Write;
        }

        if (properties.HasFlag(CBCharacteristicProperties.Notify))
        {
            result |= BluetoothCharacteristicProperties.Notify;
        }

        if (properties.HasFlag(CBCharacteristicProperties.Indicate))
        {
            result |= BluetoothCharacteristicProperties.Indicate;
        }

        if (properties.HasFlag(CBCharacteristicProperties.AuthenticatedSignedWrites))
        {
            result |= BluetoothCharacteristicProperties.SignedWrite;
        }

        if (properties.HasFlag(CBCharacteristicProperties.ExtendedProperties))
        {
            result |= BluetoothCharacteristicProperties.ExtendedProperties;
        }

        // Note: NotifyEncryptionRequired and IndicateEncryptionRequired are iOS-specific
        // and are handled through permissions in the cross-platform abstraction

        return result;
    }

    /// <summary>
    ///     Converts iOS CBAttributePermissions to the cross-platform BluetoothCharacteristicPermissions enum.
    /// </summary>
    /// <param name="permissions">The iOS attribute permissions.</param>
    /// <returns>The cross-platform characteristic permissions.</returns>
    public static BluetoothCharacteristicPermissions ToCharacteristicPermissions(this CBAttributePermissions permissions)
    {
        var result = BluetoothCharacteristicPermissions.None;

        if (permissions.HasFlag(CBAttributePermissions.Readable))
        {
            result |= BluetoothCharacteristicPermissions.Read;
        }

        if (permissions.HasFlag(CBAttributePermissions.Writeable))
        {
            result |= BluetoothCharacteristicPermissions.Write;
        }

        if (permissions.HasFlag(CBAttributePermissions.ReadEncryptionRequired))
        {
            result |= BluetoothCharacteristicPermissions.ReadEncrypted;
        }

        if (permissions.HasFlag(CBAttributePermissions.WriteEncryptionRequired))
        {
            result |= BluetoothCharacteristicPermissions.WriteEncrypted;
        }

        return result;
    }

    /// <summary>
    ///     Converts iOS CBAttributePermissions to the cross-platform BluetoothDescriptorPermissions enum.
    /// </summary>
    /// <param name="permissions">The iOS attribute permissions.</param>
    /// <returns>The cross-platform descriptor permissions.</returns>
    public static BluetoothDescriptorPermissions ToDescriptorPermissions(this CBAttributePermissions permissions)
    {
        var result = BluetoothDescriptorPermissions.None;

        if (permissions.HasFlag(CBAttributePermissions.Readable))
        {
            result |= BluetoothDescriptorPermissions.Read;
        }

        if (permissions.HasFlag(CBAttributePermissions.Writeable))
        {
            result |= BluetoothDescriptorPermissions.Write;
        }

        if (permissions.HasFlag(CBAttributePermissions.ReadEncryptionRequired))
        {
            result |= BluetoothDescriptorPermissions.ReadEncrypted;
        }

        if (permissions.HasFlag(CBAttributePermissions.WriteEncryptionRequired))
        {
            result |= BluetoothDescriptorPermissions.WriteEncrypted;
        }

        return result;
    }
}