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
        RawSignalStrengthInDBm = scanResult.Rssi;
        IsConnectable = !OperatingSystem.IsAndroidVersionAtLeast(26) || scanResult.IsConnectable;

        ArgumentNullException.ThrowIfNull(scanResult.ScanRecord, nameof(scanResult.ScanRecord));
        var scanRecord = scanResult.ScanRecord;
        DeviceName = scanRecord.DeviceName ?? string.Empty; // Advertised name here, do NOT use scanResult.Device.Name as it may be null or outdated
        ServicesGuids = ExtractServiceGuids(scanRecord);
        ManufacturerData = ExtractManufacturerData(scanRecord);
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

    private static ReadOnlyMemory<byte> ExtractManufacturerData(ScanRecord scanRecord)
    {
        var bytes = scanRecord.GetBytes();
        if (bytes == null || bytes.Length == 0)
        {
            return ReadOnlyMemory<byte>.Empty;
        }

        var scanRecordParts = ScanRecordPart.FromRawBytes(bytes).ToArray();
        var manufacturerDataParts = scanRecordParts
            .Where(part => part.Type == ScanRecordPart.AdvertisementRecordType.ManufacturerSpecificData)
            .ToArray();

        if (manufacturerDataParts.Length == 0)
        {
            return ReadOnlyMemory<byte>.Empty;
        }

        // Pre-calculate total size
        var totalSize = manufacturerDataParts.Sum(p => p.ManufacturerData.Length + 2); // +2 for manufacturer ID

        var resultArray = new byte[totalSize];
        var currentOffset = 0;

        // Group by manufacturer ID once
        foreach (var group in manufacturerDataParts.GroupBy(p => p.ManufacturerId))
        {
            if (group.Key == null)
            {
                continue; // Skip if no manufacturer ID
            }

            // Write manufacturer ID (2 bytes)
            var idBytes = (short) group.Key;
            resultArray[currentOffset++] = (byte) (idBytes & 0xFF);
            resultArray[currentOffset++] = (byte) ((idBytes >> 8) & 0xFF);

            // Write all manufacturer data for this ID
            foreach (var part in group)
            {
                var data = part.ManufacturerData;
                data.CopyTo(resultArray.AsSpan(currentOffset));
                currentOffset += data.Length;
            }
        }

        return resultArray;
    }

    #endregion
}