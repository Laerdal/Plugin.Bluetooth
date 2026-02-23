namespace Bluetooth.Maui.Platforms.Apple.Scanning;

/// <summary>
///     Represents a Bluetooth Low Energy advertisement packet received from an iOS device.
///     This readonly struct wraps iOS's CBPeripheral and advertisement data, providing access to
///     device information, services, signal strength, and manufacturer-specific data.
/// </summary>
/// <remarks>
///     This is a readonly struct for memory efficiency. Since advertisements arrive by the thousands,
///     using a value type eliminates heap allocations and reduces GC pressure.
/// </remarks>
public readonly record struct AppleBluetoothAdvertisement : IBluetoothAdvertisement
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AppleBluetoothAdvertisement" /> struct from iOS Core Bluetooth objects.
    /// </summary>
    /// <param name="cbPeripheral">The Core Bluetooth peripheral that sent the advertisement.</param>
    /// <param name="advertisementData">The native iOS advertisement data dictionary.</param>
    /// <param name="rssi">The received signal strength indicator value.</param>
    public AppleBluetoothAdvertisement(CBPeripheral cbPeripheral, NSDictionary advertisementData, NSNumber rssi)
    {
        ArgumentNullException.ThrowIfNull(rssi);
        ArgumentNullException.ThrowIfNull(cbPeripheral);

        CbPeripheral = cbPeripheral;

        var adData = new AdvertisementData(advertisementData);

        DeviceName = adData.LocalName ?? string.Empty;
        ServicesGuids = adData.ServiceUuids?.Select(serviceUuid => serviceUuid.ToGuid()).ToArray() ?? [];
        IsConnectable = adData.IsConnectable ?? false;
        TransmitPowerLevelInDBm = adData.TxPowerLevel?.Int32Value ?? 0;
        ManufacturerData = adData.ManufacturerData?.ToArray() ?? [];

        RawSignalStrengthInDBm = rssi.Int32Value;

        BluetoothAddress = cbPeripheral.Identifier.AsString();

        DateReceived = DateTimeOffset.UtcNow;
    }

    /// <summary>
    ///     Gets the iOS Core Bluetooth peripheral that sent this advertisement.
    /// </summary>
    public CBPeripheral CbPeripheral { get; }

    #region IBluetoothAdvertisement Members

    /// <inheritdoc />
    public DateTimeOffset DateReceived { get; }

    /// <inheritdoc />
    public string DeviceName { get; }

    /// <inheritdoc />
    public IEnumerable<Guid> ServicesGuids { get; }

    /// <inheritdoc />
    public bool IsConnectable { get; }

    /// <inheritdoc />
    public int RawSignalStrengthInDBm { get; }

    /// <inheritdoc />
    public int TransmitPowerLevelInDBm { get; }

    /// <inheritdoc />
    public string BluetoothAddress { get; }

    /// <inheritdoc />
    public ReadOnlyMemory<byte> ManufacturerData { get; }

    /// <inheritdoc />
    public Manufacturer Manufacturer => ManufacturerData.Length >= 2
        ? (Manufacturer) ManufacturerId
        : (Manufacturer) (-1);

    /// <inheritdoc />
    public int ManufacturerId
    {
        get
        {
            if (ManufacturerData.Length < 2)
            {
                return -1;
            }

            return BitConverter.ToInt16(ManufacturerData[..2].Span);
        }
    }

    #endregion
}
