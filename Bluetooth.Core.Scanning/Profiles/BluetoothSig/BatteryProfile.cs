namespace Bluetooth.Core.Scanning.Profiles.BluetoothSig;

/// <summary>
///     Bluetooth SIG Battery Service profile definition.
/// </summary>
public static class BatteryProfile
{
    /// <summary>
    ///     Gets the Battery Service UUID (0x180F).
    /// </summary>
    public static readonly Guid ServiceId = Guid.Parse($"0000180f{BluetoothSigConstants.StandardGuidExtension}");

    /// <summary>
    ///     Gets the Battery Level characteristic UUID (0x2A19).
    /// </summary>
    public static readonly Guid BatteryLevelCharacteristicId = Guid.Parse($"00002a19{BluetoothSigConstants.StandardGuidExtension}");

    /// <summary>
    ///     Gets an accessor for the Battery Level characteristic.
    /// </summary>
    public static IBluetoothCharacteristicAccessor<byte, byte> BatteryLevel { get; } =
        new CharacteristicAccessor<byte, byte>(
            ServiceId,
            BatteryLevelCharacteristicId,
            CharacteristicCodecFactory.ForByte(),
            "Battery Service",
            "Battery Level");

    /// <summary>
    ///     Registers Battery Service and Battery Level characteristic definitions in the provided profile registry.
    /// </summary>
    /// <param name="registry">The profile registry that receives the profile definitions.</param>
    public static void Register(IBluetoothProfileRegistry registry)
    {
        ArgumentNullException.ThrowIfNull(registry);

        registry.Register(new BluetoothServiceDefinition(ServiceId, "Battery Service"));
        registry.Register(new BluetoothCharacteristicDefinition(BatteryLevel.ServiceId, BatteryLevel.CharacteristicId, BatteryLevel.CharacteristicName));
    }
}
