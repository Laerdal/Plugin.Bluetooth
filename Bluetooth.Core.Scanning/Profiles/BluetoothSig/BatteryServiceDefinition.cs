namespace Bluetooth.Core.Scanning.Profiles.BluetoothSig;

/// <summary>
///     Bluetooth SIG Battery Service profile definition.
/// </summary>
[BluetoothServiceDefinition]
public static class BatteryServiceDefinition
{
    /// <summary>
    ///     Gets the Battery Service display name.
    /// </summary>
    public static readonly string Name = "Battery Service";

    /// <summary>
    ///     Gets the Battery Service UUID (0x180F).
    /// </summary>
    public static readonly Guid Id = BluetoothServiceDefinitions.FromShortId(0x180F);

    /// <summary>
    ///     Gets an accessor for the Battery Level characteristic.
    /// </summary>
    public static readonly IBluetoothCharacteristicAccessor<byte, byte> BatteryLevel = BluetoothServiceDefinitions.ByteCharacteristic(Id, 0x2A19, Name, "Battery Level");
}
