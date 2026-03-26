namespace Bluetooth.Core.Scanning.Profiles;

/// <summary>
///     Factory helpers for concise Bluetooth service definition declarations.
/// </summary>
public static class BluetoothServiceDefinitions
{
    /// <summary>
    ///     Creates a SIG-assigned Bluetooth UUID from a 16-bit short UUID.
    /// </summary>
    /// <param name="shortUuid">The 16-bit SIG short UUID.</param>
    /// <returns>The full 128-bit Bluetooth UUID.</returns>
    public static Guid FromShortId(ushort shortUuid)
    {
        return Guid.Parse($"0000{shortUuid:x4}{BluetoothSig.BluetoothSigConstants.StandardGuidExtension}");
    }

    /// <summary>
    ///     Creates a typed characteristic accessor using a 16-bit SIG short UUID.
    /// </summary>
    public static CharacteristicAccessor<TRead, TWrite> Characteristic<TRead, TWrite>(
        Guid serviceId,
        ushort shortCharacteristicId,
        ICharacteristicCodec<TRead, TWrite> codec,
        string serviceName,
        string characteristicName)
    {
        return new CharacteristicAccessor<TRead, TWrite>(
            serviceId,
            FromShortId(shortCharacteristicId),
            codec,
            serviceName,
            characteristicName);
    }

    /// <summary>
    ///     Creates a UTF-8 string characteristic accessor using a 16-bit SIG short UUID.
    /// </summary>
    public static CharacteristicAccessor<string, string> StringCharacteristic(Guid serviceId, ushort shortCharacteristicId, string serviceName, string characteristicName)
    {
        return Characteristic(serviceId, shortCharacteristicId, CharacteristicCodecFactory.ForString(), serviceName, characteristicName);
    }

    /// <summary>
    ///     Creates a version characteristic accessor using a 16-bit SIG short UUID.
    /// </summary>
    public static CharacteristicAccessor<Version, Version> VersionCharacteristic(Guid serviceId, ushort shortCharacteristicId, string serviceName, string characteristicName)
    {
        return Characteristic(serviceId, shortCharacteristicId, CharacteristicCodecFactory.ForVersion(), serviceName, characteristicName);
    }

    /// <summary>
    ///     Creates a raw byte characteristic accessor using a 16-bit SIG short UUID.
    /// </summary>
    public static CharacteristicAccessor<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>> BytesCharacteristic(Guid serviceId, ushort shortCharacteristicId, string serviceName, string characteristicName)
    {
        return Characteristic(serviceId, shortCharacteristicId, CharacteristicCodecFactory.ForBytes(), serviceName, characteristicName);
    }

    /// <summary>
    ///     Creates a byte characteristic accessor using a 16-bit SIG short UUID.
    /// </summary>
    public static CharacteristicAccessor<byte, byte> ByteCharacteristic(Guid serviceId, ushort shortCharacteristicId, string serviceName, string characteristicName)
    {
        return Characteristic(serviceId, shortCharacteristicId, CharacteristicCodecFactory.ForByte(), serviceName, characteristicName);
    }
}
