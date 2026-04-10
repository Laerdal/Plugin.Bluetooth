namespace Bluetooth.Avalonia.Platforms.Linux.Scanning.Factories;

/// <summary>
///     BlueZ-specific factory spec that carries the D-Bus object path and pre-parsed
///     interface properties for a remote device.
/// </summary>
internal sealed record LinuxBluetoothRemoteDeviceFactorySpec : IBluetoothRemoteDeviceFactory.BluetoothRemoteDeviceFactorySpec
{
    /// <summary>
    ///     Initializes a new instance from a <see cref="BlueZObjectInfo" /> discovered during scanning.
    /// </summary>
    public LinuxBluetoothRemoteDeviceFactorySpec(BlueZObjectInfo objectInfo)
        : base(
            objectInfo.GetStringProp(BlueZConstants.Device1Interface, BlueZConstants.PropAddress)
                      ?? objectInfo.Path,
            ResolveManufacturer(objectInfo))
    {
        ObjectPath = objectInfo.Path;
        ObjectInfo = objectInfo;
    }

    /// <summary>
    ///     Gets the D-Bus object path of the device (e.g. <c>/org/bluez/hci0/dev_AA_BB_CC_DD_EE_FF</c>).
    /// </summary>
    public string ObjectPath { get; init; }

    /// <summary>
    ///     Gets the pre-parsed object info from the time of discovery.
    /// </summary>
    public BlueZObjectInfo ObjectInfo { get; init; }

    private static Manufacturer ResolveManufacturer(BlueZObjectInfo obj)
    {
        // ManufacturerData is a{qv} — each entry maps a uint16 company ID to a byte array variant.
        // We take the first entry's company ID as the manufacturer.
        if (!obj.Interfaces.TryGetValue(BlueZConstants.Device1Interface, out var props))
        {
            return (Manufacturer) (-1);
        }

        if (!props.TryGetValue(BlueZConstants.PropManufacturerData, out var mdVariant))
        {
            return (Manufacturer) (-1);
        }

        if (mdVariant.Type != VariantValueType.Array)
        {
            return (Manufacturer) (-1);
        }

        // The first key in the manufacturer-data dict is the Bluetooth SIG company ID.
        var dict = mdVariant.GetDictionary<ushort, VariantValue>();
        if (dict.Count == 0)
        {
            return (Manufacturer) (-1);
        }

        return (Manufacturer) dict.Keys.First();
    }
}
