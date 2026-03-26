namespace Bluetooth.Core.Scanning.Profiles.BluetoothSig;

/// <summary>
///     Bluetooth SIG Device Information Service profile definition.
/// </summary>
public static class DeviceInformationProfile
{
    /// <summary>
    ///     Gets the Device Information Service UUID (0x180A).
    /// </summary>
    public static readonly Guid ServiceId = Guid.Parse($"0000180a{BluetoothSigConstants.StandardGuidExtension}");

    /// <summary>
    ///     Gets an accessor for the System ID characteristic (0x2A23).
    /// </summary>
    public static IBluetoothCharacteristicAccessor<string, string> SystemId { get; } =
        CreateStringAccessor("00002a23", "System ID");

    /// <summary>
    ///     Gets an accessor for the Model Number String characteristic (0x2A24).
    /// </summary>
    public static IBluetoothCharacteristicAccessor<string, string> ModelNumber { get; } =
        CreateStringAccessor("00002a24", "Model Number String");

    /// <summary>
    ///     Gets an accessor for the Serial Number String characteristic (0x2A25).
    /// </summary>
    public static IBluetoothCharacteristicAccessor<string, string> SerialNumber { get; } =
        CreateStringAccessor("00002a25", "Serial Number String");

    /// <summary>
    ///     Gets an accessor for the Firmware Revision String characteristic (0x2A26).
    /// </summary>
    public static IBluetoothCharacteristicAccessor<Version, Version> FirmwareRevision { get; } =
        CreateVersionAccessor("00002a26", "Firmware Revision String");

    /// <summary>
    ///     Gets an accessor for the Hardware Revision String characteristic (0x2A27).
    /// </summary>
    public static IBluetoothCharacteristicAccessor<string, string> HardwareRevision { get; } =
        CreateStringAccessor("00002a27", "Hardware Revision String");

    /// <summary>
    ///     Gets an accessor for the Software Revision String characteristic (0x2A28).
    /// </summary>
    public static IBluetoothCharacteristicAccessor<Version, Version> SoftwareRevision { get; } =
        CreateVersionAccessor("00002a28", "Software Revision String");

    /// <summary>
    ///     Gets an accessor for the Manufacturer Name String characteristic (0x2A29).
    /// </summary>
    public static IBluetoothCharacteristicAccessor<string, string> ManufacturerName { get; } =
        CreateStringAccessor("00002a29", "Manufacturer Name String");

    /// <summary>
    ///     Gets an accessor for the IEEE 11073-20601 Regulatory Certification Data List characteristic (0x2A2A).
    /// </summary>
    public static IBluetoothCharacteristicAccessor<string, string> RegulatoryCertificationDataList { get; } =
        CreateStringAccessor("00002a2a", "IEEE 11073-20601 Regulatory Certification Data List");

    /// <summary>
    ///     Registers Device Information Service and characteristic definitions in the provided profile registry.
    /// </summary>
    /// <param name="registry">The profile registry that receives the profile definitions.</param>
    public static void Register(IBluetoothProfileRegistry registry)
    {
        ArgumentNullException.ThrowIfNull(registry);

        registry.Register(new BluetoothServiceDefinition(ServiceId, "Device Information"));
        RegisterCharacteristic(registry, SystemId);
        RegisterCharacteristic(registry, ModelNumber);
        RegisterCharacteristic(registry, SerialNumber);
        RegisterCharacteristic(registry, FirmwareRevision);
        RegisterCharacteristic(registry, HardwareRevision);
        RegisterCharacteristic(registry, SoftwareRevision);
        RegisterCharacteristic(registry, ManufacturerName);
        RegisterCharacteristic(registry, RegulatoryCertificationDataList);
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
            "Device Information",
            characteristicName);
    }

    private static CharacteristicAccessor<Version, Version> CreateVersionAccessor(string shortCharacteristicId, string characteristicName)
    {
        return new CharacteristicAccessor<Version, Version>(
            ServiceId,
            Guid.Parse($"{shortCharacteristicId}{BluetoothSigConstants.StandardGuidExtension}"),
            CharacteristicCodecFactory.ForVersion(),
            "Device Information",
            characteristicName);
    }
}
