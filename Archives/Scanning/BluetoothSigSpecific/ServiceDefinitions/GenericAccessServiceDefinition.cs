namespace Bluetooth.Core.Scanning.BluetoothSigSpecific.ServiceDefinitions;

/// <summary>
///     Generic Access Service | generic_access_service | 1800
/// </summary>
[ServiceDefinition]
public static class GenericAccessServiceDefinition
{
    // https:// www.bluetooth.com/specifications/gatt/characteristics/
    /// <summary>
    /// Gets the human-readable name of the Generic Access Service.
    /// </summary>
    public readonly static string Name = "Generic Access Service";

    /// <summary>
    /// Gets the UUID of the Generic Access Service (0x1800).
    /// </summary>
    public readonly static Guid Id = new Guid($"00001800{BluetoothSigConstants.StandardGuidExtension}");

    /// <summary>
    /// Gets the Device Name characteristic access service for reading device names as UTF-8 strings.
    /// </summary>
    public readonly static CharacteristicAccessService<string> DeviceName = CharacteristicAccessServiceFactory.CreateForUtf8String(new Guid($"00002a00{BluetoothSigConstants.StandardGuidExtension}"), "Device Name");

    /// <summary>
    /// Gets the Appearance characteristic access service for reading device appearance values.
    /// </summary>
    public readonly static CharacteristicAccessService<byte[]> Appearance = CharacteristicAccessServiceFactory.CreateForByteArray(new Guid($"00002a01{BluetoothSigConstants.StandardGuidExtension}"), "Appearance");

    /// <summary>
    /// Gets the Peripheral Privacy Flag characteristic access service for reading privacy flag values.
    /// </summary>
    public readonly static CharacteristicAccessService<byte[]> PeripheralPrivacyFlag = CharacteristicAccessServiceFactory.CreateForByteArray(new Guid($"00002a02{BluetoothSigConstants.StandardGuidExtension}"), "Peripheral Privacy Flag");

    /// <summary>
    /// Gets the Reconnection Address characteristic access service for reading reconnection address values.
    /// </summary>
    public readonly static CharacteristicAccessService<byte[]> ReconnectionAddress = CharacteristicAccessServiceFactory.CreateForByteArray(new Guid($"00002a03{BluetoothSigConstants.StandardGuidExtension}"), "Reconnection Address");

    /// <summary>
    /// Gets the Peripheral Preferred Connection Parameters characteristic access service for reading connection parameters.
    /// </summary>
    public readonly static CharacteristicAccessService<byte[]> PeripheralPreferredConnectionParameters = CharacteristicAccessServiceFactory.CreateForByteArray(new Guid($"00002a04{BluetoothSigConstants.StandardGuidExtension}"), "Peripheral Preferred Connection Parameters");

}
