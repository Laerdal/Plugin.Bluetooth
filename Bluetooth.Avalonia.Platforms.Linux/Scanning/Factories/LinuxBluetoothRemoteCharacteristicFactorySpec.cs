namespace Bluetooth.Avalonia.Platforms.Linux.Scanning.Factories;

/// <summary>
///     BlueZ-specific factory spec that carries the D-Bus object path and pre-parsed flags for a GATT characteristic.
/// </summary>
internal sealed record LinuxBluetoothRemoteCharacteristicFactorySpec : IBluetoothRemoteCharacteristicFactory.BluetoothRemoteCharacteristicFactorySpec
{
    /// <summary>
    ///     Initializes a new instance from a <see cref="BlueZObjectInfo" />.
    /// </summary>
    public LinuxBluetoothRemoteCharacteristicFactorySpec(BlueZObjectInfo objectInfo)
        : base(ResolveCharacteristicId(objectInfo))
    {
        ObjectPath = objectInfo.Path;
        Flags = objectInfo.GetStringArrayProp(BlueZConstants.GattCharacteristic1Interface, BlueZConstants.PropFlags) ?? [];
    }

    /// <summary>
    ///     Gets the D-Bus object path of the characteristic.
    /// </summary>
    public string ObjectPath { get; init; }

    /// <summary>
    ///     Gets the characteristic flags (e.g. <c>"read"</c>, <c>"write"</c>, <c>"notify"</c>).
    /// </summary>
    public string[] Flags { get; init; }

    private static Guid ResolveCharacteristicId(BlueZObjectInfo obj)
    {
        var uuid = obj.GetStringProp(BlueZConstants.GattCharacteristic1Interface, BlueZConstants.PropUUID);
        return uuid != null && Guid.TryParse(uuid, out var g) ? g : Guid.Empty;
    }
}
