namespace Bluetooth.Core.Scanning.Profiles.BluetoothSig;

/// <summary>
///     Bluetooth SIG Generic Access Service profile definition.
/// </summary>
public static class GenericAccessProfile
{
    /// <summary>
    ///     Gets the Generic Access Service UUID (0x1800).
    /// </summary>
    public static readonly Guid ServiceId = Guid.Parse($"00001800{BluetoothSigConstants.StandardGuidExtension}");

    /// <summary>
    ///     Gets an accessor for the Device Name characteristic (0x2A00).
    /// </summary>
    public static IBluetoothCharacteristicAccessor<string, string> DeviceName { get; } =
        CreateStringAccessor("00002a00", "Device Name");

    /// <summary>
    ///     Gets an accessor for the Appearance characteristic (0x2A01).
    /// </summary>
    public static IBluetoothCharacteristicAccessor<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>> Appearance { get; } =
        CreateByteAccessor("00002a01", "Appearance");

    /// <summary>
    ///     Gets an accessor for the Peripheral Privacy Flag characteristic (0x2A02).
    /// </summary>
    public static IBluetoothCharacteristicAccessor<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>> PeripheralPrivacyFlag { get; } =
        CreateByteAccessor("00002a02", "Peripheral Privacy Flag");

    /// <summary>
    ///     Gets an accessor for the Reconnection Address characteristic (0x2A03).
    /// </summary>
    public static IBluetoothCharacteristicAccessor<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>> ReconnectionAddress { get; } =
        CreateByteAccessor("00002a03", "Reconnection Address");

    /// <summary>
    ///     Gets an accessor for the Peripheral Preferred Connection Parameters characteristic (0x2A04).
    /// </summary>
    public static IBluetoothCharacteristicAccessor<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>> PeripheralPreferredConnectionParameters { get; } =
        CreateByteAccessor("00002a04", "Peripheral Preferred Connection Parameters");

    /// <summary>
    ///     Registers Generic Access Service and characteristic definitions in the provided profile registry.
    /// </summary>
    /// <param name="registry">The profile registry that receives the profile definitions.</param>
    public static void Register(IBluetoothProfileRegistry registry)
    {
        ArgumentNullException.ThrowIfNull(registry);

        registry.Register(new BluetoothServiceDefinition(ServiceId, "Generic Access Service"));
        RegisterCharacteristic(registry, DeviceName);
        RegisterCharacteristic(registry, Appearance);
        RegisterCharacteristic(registry, PeripheralPrivacyFlag);
        RegisterCharacteristic(registry, ReconnectionAddress);
        RegisterCharacteristic(registry, PeripheralPreferredConnectionParameters);
    }

    private static void RegisterCharacteristic(IBluetoothProfileRegistry registry, IBluetoothCharacteristicAccessor accessor)
    {
        registry.Register(new BluetoothCharacteristicDefinition(accessor.ServiceId, accessor.CharacteristicId, accessor.CharacteristicName));
    }

    private static CharacteristicAccessor<string, string> CreateStringAccessor(string shortCharacteristicId, string characteristicName)
    {
        return new CharacteristicAccessor<string, string>(
            ServiceId,
            Guid.Parse($"{shortCharacteristicId}{BluetoothSigConstants.StandardGuidExtension}"),
            CharacteristicCodecFactory.ForString(),
            "Generic Access Service",
            characteristicName);
    }

    private static CharacteristicAccessor<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>> CreateByteAccessor(string shortCharacteristicId, string characteristicName)
    {
        return new CharacteristicAccessor<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>>(
            ServiceId,
            Guid.Parse($"{shortCharacteristicId}{BluetoothSigConstants.StandardGuidExtension}"),
            CharacteristicCodecFactory.ForBytes(),
            "Generic Access Service",
            characteristicName);
    }
}
