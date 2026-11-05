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
        ServiceData = ExtractServiceData(adData);

        SignalStrengthInDBm = rssi.Int32Value;

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
    public int SignalStrengthInDBm { get; }

    /// <inheritdoc />
    public int TransmitPowerLevelInDBm { get; }

    /// <inheritdoc />
    public string BluetoothAddress { get; }

    /// <inheritdoc />
    public ReadOnlyMemory<byte> ManufacturerData { get; }

    /// <inheritdoc />
    public IReadOnlyDictionary<Guid, ReadOnlyMemory<byte>> ServiceData { get; }

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

    #region Private Helper Methods

    private static IReadOnlyDictionary<Guid, ReadOnlyMemory<byte>> ExtractServiceData(AdvertisementData adData)
    {
        // Core Bluetooth reports Service Data as one dictionary (CBUUID to NSData) for the advertisement and its scan response.
        var nativeServiceData = adData.ServiceData;
        if (nativeServiceData == null || nativeServiceData.Count == 0)
        {
            return ReadOnlyDictionary<Guid, ReadOnlyMemory<byte>>.Empty;
        }

        var serviceData = new Dictionary<Guid, ReadOnlyMemory<byte>>((int) nativeServiceData.Count);
        foreach (var entry in nativeServiceData)
        {
            if (entry.Key is CBUUID uuid && entry.Value is NSData data)
            {
                serviceData[uuid.ToGuid()] = data.ToArray();
            }
        }

        return serviceData;
    }

    #endregion
}
