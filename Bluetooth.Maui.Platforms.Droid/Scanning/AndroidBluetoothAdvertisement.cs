using Bluetooth.Maui.Platforms.Droid.Scanning.NativeObjects;
using Bluetooth.Maui.Platforms.Droid.Tools;

using Array = System.Array;

namespace Bluetooth.Maui.Platforms.Droid.Scanning;

/// <summary>
///     Represents a Bluetooth Low Energy advertisement packet received from an Android device.
///     This readonly struct wraps Android's ScanResult, providing access to device information,
///     services, signal strength, and manufacturer-specific data.
/// </summary>
/// <remarks>
///     This is a readonly struct for memory efficiency. Since advertisements arrive by the thousands,
///     using a value type eliminates heap allocations and reduces GC pressure.
/// </remarks>
public readonly record struct AndroidBluetoothAdvertisement : IBluetoothAdvertisement
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AndroidBluetoothAdvertisement" /> struct from an Android scan result.
    /// </summary>
    /// <param name="scanResult">The Android scan result containing the advertisement data.</param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="scanResult" /> is <c>null</c>, or when
    ///     <see cref="ScanResult.ScanRecord" /> is <c>null</c>, or when
    ///     <see cref="ScanResult.Device" /> is <c>null</c>.
    /// </exception>
    public AndroidBluetoothAdvertisement(ScanResult scanResult)
    {
        ArgumentNullException.ThrowIfNull(scanResult);
        SignalStrengthInDBm = scanResult.Rssi;
        IsConnectable = !OperatingSystem.IsAndroidVersionAtLeast(26) || scanResult.IsConnectable;

        ArgumentNullException.ThrowIfNull(scanResult.ScanRecord, nameof(scanResult.ScanRecord));
        var scanRecord = scanResult.ScanRecord;
        DeviceName = scanRecord.DeviceName ?? string.Empty; // Advertised name here, do NOT use scanResult.Device.Name as it may be null or outdated
        ServicesGuids = ExtractServiceGuids(scanRecord);
        ManufacturerData = ExtractManufacturerData(scanRecord);
        ServiceData = ExtractServiceData(scanRecord);
        TransmitPowerLevelInDBm = scanRecord.TxPowerLevel;

        ArgumentNullException.ThrowIfNull(scanResult.Device, nameof(scanResult.Device));
        BluetoothAddress = scanResult.Device.Address ?? string.Empty;

        DateReceived = DateTimeOffset.UtcNow;
    }

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

    private static Guid[] ExtractServiceGuids(ScanRecord scanRecord)
    {
        if (scanRecord.ServiceUuids == null || scanRecord.ServiceUuids.Count == 0)
        {
            return [];
        }

        var guids = new Guid[scanRecord.ServiceUuids.Count];
        var index = 0;

        foreach (var serviceUuid in scanRecord.ServiceUuids)
        {
            if (serviceUuid.Uuid != null)
            {
                guids[index++] = serviceUuid.Uuid.ToGuid();
            }
        }

        // If some UUIDs were null, resize the array
        if (index < guids.Length)
        {
            Array.Resize(ref guids, index);
        }

        return guids;
    }

    private static IReadOnlyDictionary<Guid, ReadOnlyMemory<byte>> ExtractServiceData(ScanRecord scanRecord)
    {
        // The platform already merges the 16-bit, 32-bit and 128-bit Service Data structures of the advertisement and its
        // scan response into one map keyed by the expanded UUID.
        var nativeServiceData = scanRecord.ServiceData;
        if (nativeServiceData == null || nativeServiceData.Count == 0)
        {
            return ReadOnlyDictionary<Guid, ReadOnlyMemory<byte>>.Empty;
        }

        var serviceData = new Dictionary<Guid, ReadOnlyMemory<byte>>(nativeServiceData.Count);
        foreach (var (parcelUuid, data) in nativeServiceData)
        {
            if (parcelUuid?.Uuid == null || data == null)
            {
                continue;
            }

            serviceData[parcelUuid.Uuid.ToGuid()] = data;
        }

        return serviceData;
    }

    private static ReadOnlyMemory<byte> ExtractManufacturerData(ScanRecord scanRecord)
    {
        var bytes = scanRecord.GetBytes();
        if (bytes == null || bytes.Length == 0)
        {
            return ReadOnlyMemory<byte>.Empty;
        }

        return AndroidManufacturerDataBuilder.Build(ScanRecordPart.FromRawBytes(bytes));
    }

    #endregion
}
