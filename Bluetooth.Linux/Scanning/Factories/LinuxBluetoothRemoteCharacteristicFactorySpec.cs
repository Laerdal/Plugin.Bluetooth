namespace Bluetooth.Linux.Scanning.Factories;

/// <summary>
///     Linux-specific factory spec for creating remote GATT characteristics.
///     Extends the base spec with the native BlueZ proxy and pre-fetched flags.
/// </summary>
public record LinuxBluetoothRemoteCharacteristicFactorySpec : IBluetoothRemoteCharacteristicFactory.BluetoothRemoteCharacteristicFactorySpec
{
    /// <summary>
    ///     Initialises a new spec from a BlueZ characteristic proxy.
    /// </summary>
    /// <param name="characteristicId">The characteristic UUID.</param>
    /// <param name="nativeCharacteristic">The native BlueZ D-Bus proxy.</param>
    /// <param name="flags">
    ///     The BlueZ flags string array (e.g. "read", "write", "notify").
    ///     Pre-fetched to avoid async calls inside constructors.
    /// </param>
    public LinuxBluetoothRemoteCharacteristicFactorySpec(Guid characteristicId, IGattCharacteristic1 nativeCharacteristic, string[] flags)
        : base(characteristicId)
    {
        ArgumentNullException.ThrowIfNull(nativeCharacteristic);
        NativeCharacteristic = nativeCharacteristic;
        Flags = flags ?? [];
    }

    /// <summary>
    ///     The native BlueZ GATT characteristic D-Bus proxy.
    /// </summary>
    public IGattCharacteristic1 NativeCharacteristic { get; }

    /// <summary>
    ///     BlueZ property flags such as "read", "write", "notify", "indicate", "write-without-response".
    /// </summary>
    public string[] Flags { get; }
}
