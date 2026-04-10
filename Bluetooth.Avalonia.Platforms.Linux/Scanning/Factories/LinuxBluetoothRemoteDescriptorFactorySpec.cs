namespace Bluetooth.Avalonia.Platforms.Linux.Scanning.Factories;

/// <summary>
///     BlueZ-specific factory spec that carries the D-Bus object path for a GATT descriptor.
/// </summary>
internal sealed record LinuxBluetoothRemoteDescriptorFactorySpec : IBluetoothRemoteDescriptorFactory.BluetoothRemoteDescriptorFactorySpec
{
    /// <summary>
    ///     Initializes a new instance from a <see cref="BlueZObjectInfo" />.
    /// </summary>
    public LinuxBluetoothRemoteDescriptorFactorySpec(BlueZObjectInfo objectInfo)
        : base(ResolveDescriptorId(objectInfo))
    {
        ObjectPath = objectInfo.Path;
    }

    /// <summary>
    ///     Gets the D-Bus object path of the descriptor.
    /// </summary>
    public string ObjectPath { get; init; }

    private static Guid ResolveDescriptorId(BlueZObjectInfo obj)
    {
        var uuid = obj.GetStringProp(BlueZConstants.GattDescriptor1Interface, BlueZConstants.PropUUID);
        return uuid != null && Guid.TryParse(uuid, out var g) ? g : Guid.Empty;
    }
}
