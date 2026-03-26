namespace Bluetooth.Core.Scanning.Profiles.BluetoothSig;

/// <summary>
///     Bluetooth SIG Generic Attribute Service profile definition.
/// </summary>
public static class GenericAttributeProfile
{
    /// <summary>
    ///     Gets the Generic Attribute Service UUID (0x1801).
    /// </summary>
    public static readonly Guid ServiceId = Guid.Parse($"00001801{BluetoothSigConstants.StandardGuidExtension}");

    /// <summary>
    ///     Gets an accessor for the Service Changed characteristic (0x2A05).
    /// </summary>
    public static IBluetoothCharacteristicAccessor<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>> ServiceChanged { get; } =
        new CharacteristicAccessor<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>>(
            ServiceId,
            Guid.Parse($"00002a05{BluetoothSigConstants.StandardGuidExtension}"),
            CharacteristicCodecFactory.ForBytes(),
            "Generic Attribute Service",
            "Service Changed");

    /// <summary>
    ///     Registers Generic Attribute Service and characteristic definitions in the provided profile registry.
    /// </summary>
    /// <param name="registry">The profile registry that receives the profile definitions.</param>
    public static void Register(IBluetoothProfileRegistry registry)
    {
        ArgumentNullException.ThrowIfNull(registry);

        registry.Register(new BluetoothServiceDefinition(ServiceId, "Generic Attribute Service"));
        registry.Register(new BluetoothCharacteristicDefinition(ServiceChanged.ServiceId, ServiceChanged.CharacteristicId, ServiceChanged.CharacteristicName));
    }
}