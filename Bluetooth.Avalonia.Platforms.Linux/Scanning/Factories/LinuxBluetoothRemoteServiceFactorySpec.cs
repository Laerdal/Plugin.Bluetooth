namespace Bluetooth.Avalonia.Platforms.Linux.Scanning.Factories;

/// <summary>
///     BlueZ-specific factory spec that carries the D-Bus object path for a remote GATT service.
/// </summary>
internal sealed record LinuxBluetoothRemoteServiceFactorySpec : IBluetoothRemoteServiceFactory.BluetoothRemoteServiceFactorySpec
{
    /// <summary>
    ///     Initializes a new instance from a <see cref="BlueZObjectInfo" />.
    /// </summary>
    public LinuxBluetoothRemoteServiceFactorySpec(BlueZObjectInfo objectInfo)
        : base(ResolveServiceId(objectInfo))
    {
        ObjectPath = objectInfo.Path;
    }

    /// <summary>
    ///     Gets the D-Bus object path of the GATT service.
    /// </summary>
    public string ObjectPath { get; init; }

    private static Guid ResolveServiceId(BlueZObjectInfo obj)
    {
        var uuid = obj.GetStringProp(BlueZConstants.GattService1Interface, BlueZConstants.PropUUID);
        return uuid != null && Guid.TryParse(uuid, out var g) ? g : Guid.Empty;
    }
}
