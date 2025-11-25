namespace Bluetooth.Core.BluetoothSigSpecific.ServiceDefinitions;

/// <summary>
///     Battery Service | battery_service | 180F
/// </summary>
[ServiceDefinition]
public static class BatteryServiceDefinition
{
    // https:// www.bluetooth.com/specifications/gatt/characteristics/
    /// <summary>
    /// Gets the human-readable name of the Battery Service.
    /// </summary>
    public readonly static string Name = "Battery Service";

    /// <summary>
    /// Gets the UUID of the Battery Service (0x180F).
    /// </summary>
    public readonly static Guid Id = new Guid($"0000180F{BluetoothSigConstants.StandardGuidExtension}");

    /// <summary>
    /// Gets the Battery Level characteristic access service for reading battery level as a signed byte.
    /// </summary>
    public readonly static CharacteristicAccessService<sbyte> BatteryLevel = CharacteristicAccessServiceFactory.CreateForSByte(new Guid($"00002a19{BluetoothSigConstants.StandardGuidExtension}"), "Battery Level");
}
