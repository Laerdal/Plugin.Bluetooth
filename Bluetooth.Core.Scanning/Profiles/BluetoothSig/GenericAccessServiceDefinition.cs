namespace Bluetooth.Core.Scanning.Profiles.BluetoothSig;

/// <summary>
///     Bluetooth SIG Generic Access Service profile definition.
/// </summary>
[BluetoothServiceDefinition]
public static class GenericAccessServiceDefinition
{
    /// <summary>
    ///     Gets the Generic Access Service display name.
    /// </summary>
    public static readonly string Name = "Generic Access Service";

    /// <summary>
    ///     Gets the Generic Access Service UUID (0x1800).
    /// </summary>
    public static readonly Guid Id = BluetoothServiceDefinitions.FromShortId(0x1800);

    /// <summary>
    ///     Gets an accessor for the Device Name characteristic (0x2A00).
    /// </summary>
    public static readonly IBluetoothCharacteristicAccessor<string, string> DeviceName = BluetoothServiceDefinitions.StringCharacteristic(Id, 0x2A00, Name, "Device Name");

    /// <summary>
    ///     Gets an accessor for the Appearance characteristic (0x2A01).
    /// </summary>
    public static readonly IBluetoothCharacteristicAccessor<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>> Appearance = BluetoothServiceDefinitions.BytesCharacteristic(Id, 0x2A01, Name, "Appearance");

    /// <summary>
    ///     Gets an accessor for the Peripheral Privacy Flag characteristic (0x2A02).
    /// </summary>
    public static readonly IBluetoothCharacteristicAccessor<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>> PeripheralPrivacyFlag = BluetoothServiceDefinitions.BytesCharacteristic(Id, 0x2A02, Name, "Peripheral Privacy Flag");

    /// <summary>
    ///     Gets an accessor for the Reconnection Address characteristic (0x2A03).
    /// </summary>
    public static readonly IBluetoothCharacteristicAccessor<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>> ReconnectionAddress = BluetoothServiceDefinitions.BytesCharacteristic(Id, 0x2A03, Name, "Reconnection Address");

    /// <summary>
    ///     Gets an accessor for the Peripheral Preferred Connection Parameters characteristic (0x2A04).
    /// </summary>
    public static readonly IBluetoothCharacteristicAccessor<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>> PeripheralPreferredConnectionParameters = BluetoothServiceDefinitions.BytesCharacteristic(Id, 0x2A04, Name, "Peripheral Preferred Connection Parameters");
}
