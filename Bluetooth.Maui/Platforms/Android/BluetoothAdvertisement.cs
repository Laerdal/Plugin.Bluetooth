using Bluetooth.Maui.PlatformSpecific;

namespace Bluetooth.Maui;

public class BluetoothAdvertisement : BaseBluetoothAdvertisement
{
    public ScanResult ScanResult { get; }

    public ScanRecord ScanRecord { get; }

    public IReadOnlyList<ScanRecordPart> ScanRecordParts { get; }

    public Android.Bluetooth.BluetoothDevice BluetoothDevice { get; }

    public BluetoothAdvertisement(ScanResult scanResult)
    {
        ArgumentNullException.ThrowIfNull(scanResult);
        ArgumentNullException.ThrowIfNull(scanResult.ScanRecord, nameof(scanResult.ScanRecord));
        ArgumentNullException.ThrowIfNull(scanResult.Device, nameof(scanResult.Device));

        ScanResult = scanResult;
        ScanRecord = scanResult.ScanRecord;
        BluetoothDevice = scanResult.Device;

        var bytes = ScanRecord.GetBytes() ?? [];
        ScanRecordParts = bytes.Length == 0 ? [] : ScanRecordPart.FromRawBytes(bytes).ToArray();

        // note: check "added in api level..." here https://developer.android.com/reference/android/bluetooth/le/ScanResult.html before adding other get
    }

    #region BaseBluetoothAdvertisement

    /// <inheritdoc/>
    protected override string InitDeviceName()
    {
        return ScanRecord.DeviceName ?? string.Empty;
    }

    /// <inheritdoc/>
    protected override IEnumerable<Guid> InitServicesGuids()
    {
        if (ScanRecord.ServiceUuids == null || ScanRecord.ServiceUuids.Count == 0)
        {
            return [];
        }

        var guids = new Guid[ScanRecord.ServiceUuids.Count];
        var index = 0;

        foreach (var serviceUuid in ScanRecord.ServiceUuids)
        {
            if (serviceUuid.Uuid != null)
            {
                guids[index++] = serviceUuid.Uuid.ToGuid();
            }
        }

        // If some UUIDs were null, resize the array
        if (index < guids.Length)
        {
            System.Array.Resize(ref guids, index);
        }

        return guids;
    }

    /// <inheritdoc/>
    protected override bool InitIsConnectable()
    {
        // IsConnectable is not available prior to API 8.0 (API level 26)
        return !OperatingSystem.IsAndroidVersionAtLeast(26) || ScanResult.IsConnectable;
    }

    /// <inheritdoc/>
    protected override int InitRawSignalStrengthInDBm()
    {
        return ScanResult.Rssi;
    }

    /// <inheritdoc/>
    protected override int InitTransmitPowerLevelInDBm()
    {
        return ScanResult.ScanRecord?.TxPowerLevel ?? 0;
    }

    /// <inheritdoc/>
    protected override byte[] InitManufacturerData()
    {
        var manufacturerDataParts = ScanRecordParts.Where(part => part.Type == ScanRecordPart.AdvertisementRecordType.ManufacturerSpecificData).ToArray(); // Enumerate once

        if (manufacturerDataParts.Length == 0)
        {
            return [];
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

    /// <inheritdoc/>
    protected override string InitBluetoothAddress()
    {
        return BluetoothDevice.Address ?? string.Empty;
    }

    #endregion
}
