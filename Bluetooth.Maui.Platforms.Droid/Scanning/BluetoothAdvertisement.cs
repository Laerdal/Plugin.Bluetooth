using Bluetooth.Maui.Platforms.Droid.Tools;

namespace Bluetooth.Maui.Platforms.Droid.Scanning;

public class BluetoothAdvertisement : BaseBluetoothAdvertisement
{
    /// <summary>
    /// Gets the native Android scan result containing the advertisement data.
    /// </summary>
    public ScanResult ScanResult { get; }

    /// <summary>
    /// Gets the scan record containing the parsed advertisement data such as device name,
    /// service UUIDs, manufacturer data, and transmit power level.
    /// </summary>
    public ScanRecord ScanRecord { get; }

    /// <summary>
    /// Gets the collection of individual advertisement data parts parsed from the raw scan record bytes.
    /// Each part represents a specific type of data (e.g., manufacturer data, service UUIDs, local name).
    /// </summary>
    public IReadOnlyList<NativeObjects.ScanRecordPart> ScanRecordParts { get; }

    /// <summary>
    /// Gets the native Android Bluetooth device that sent this advertisement.
    /// </summary>
    public Android.Bluetooth.BluetoothDevice BluetoothDevice { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BluetoothAdvertisement"/> class from an Android scan result.
    /// </summary>
    /// <param name="scanResult">The Android scan result containing the advertisement data.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="scanResult"/> is <c>null</c>, or when
    /// <see cref="ScanResult.ScanRecord"/> is <c>null</c>, or when
    /// <see cref="ScanResult.Device"/> is <c>null</c>.
    /// </exception>
    public BluetoothAdvertisement(ScanResult scanResult)
    {
        ArgumentNullException.ThrowIfNull(scanResult);
        ArgumentNullException.ThrowIfNull(scanResult.ScanRecord, nameof(scanResult.ScanRecord));
        ArgumentNullException.ThrowIfNull(scanResult.Device, nameof(scanResult.Device));

        ScanResult = scanResult;
        ScanRecord = scanResult.ScanRecord;
        BluetoothDevice = scanResult.Device;

        var bytes = ScanRecord.GetBytes() ?? [];
        ScanRecordParts = bytes.Length == 0 ? [] : NativeObjects.ScanRecordPart.FromRawBytes(bytes).ToArray();

        // note: check "added in api level..." here https://developer.android.com/reference/android/bluetooth/le/ScanResult.html before adding other get
    }

    #region BaseBluetoothAdvertisement

    /// <inheritdoc/>
    /// <remarks>
    /// Retrieves the device name from the Android scan record.
    /// Returns an empty string if no device name is available in the advertisement.
    /// </remarks>
    protected override string InitDeviceName()
    {
        return ScanRecord.DeviceName ?? string.Empty;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Extracts service UUIDs from the Android scan record and converts them to GUIDs.
    /// Returns an empty collection if no service UUIDs are advertised or if any UUID conversion fails.
    /// </remarks>
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
    /// <remarks>
    /// On Android versions prior to API level 26 (Android 8.0), this property always returns <c>true</c>
    /// because the IsConnectable property is not available. On API level 26 and above, returns the
    /// actual connectable status from the scan result.
    /// </remarks>
    protected override bool InitIsConnectable()
    {
        // IsConnectable is not available prior to API 8.0 (API level 26)
        return !OperatingSystem.IsAndroidVersionAtLeast(26) || ScanResult.IsConnectable;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Returns the RSSI (Received Signal Strength Indicator) value in dBm from the Android scan result.
    /// </remarks>
    protected override int InitRawSignalStrengthInDBm()
    {
        return ScanResult.Rssi;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Returns the transmit power level from the Android scan record, or 0 if not available.
    /// </remarks>
    protected override int InitTransmitPowerLevelInDBm()
    {
        return ScanResult.ScanRecord?.TxPowerLevel ?? 0;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Extracts and consolidates all manufacturer-specific data from the scan record parts.
    /// The data is formatted with a 2-byte manufacturer ID followed by the manufacturer data.
    /// If multiple manufacturer data parts exist, they are concatenated. Returns an empty array
    /// if no manufacturer data is present.
    /// </remarks>
    protected override byte[] InitManufacturerData()
    {
        var manufacturerDataParts = ScanRecordParts.Where(part => part.Type == NativeObjects.ScanRecordPart.AdvertisementRecordType.ManufacturerSpecificData).ToArray(); // Enumerate once

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
    /// <remarks>
    /// Returns the Bluetooth MAC address of the device from the Android BluetoothDevice.
    /// Returns an empty string if the address is not available.
    /// </remarks>
    protected override string InitBluetoothAddress()
    {
        return BluetoothDevice.Address ?? string.Empty;
    }

    #endregion

    #region Equality

    /// <summary>
    /// Determines whether the specified object is equal to the current advertisement.
    /// </summary>
    /// <param name="obj">The object to compare with the current advertisement.</param>
    /// <returns><c>true</c> if the specified object is equal to the current advertisement; otherwise, <c>false</c>.</returns>
    /// <remarks>
    /// Two advertisements are considered equal if they have the same Bluetooth device address and
    /// identical raw scan record bytes.
    /// </remarks>
    public override bool Equals(object? obj)
    {
        if (obj is not BluetoothAdvertisement other)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        // Compare Bluetooth device address
        if (BluetoothDevice.Address != other.BluetoothDevice.Address)
        {
            return false;
        }

        // Compare raw scan record bytes
        var thisBytes = ScanRecord.GetBytes();
        var otherBytes = other.ScanRecord.GetBytes();

        if (thisBytes == null && otherBytes == null)
        {
            return true;
        }

        if (thisBytes == null || otherBytes == null)
        {
            return false;
        }

        return thisBytes.AsSpan().SequenceEqual(otherBytes.AsSpan());
    }

    /// <summary>
    /// Returns a hash code for the current advertisement.
    /// </summary>
    /// <returns>A hash code for the current advertisement.</returns>
    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(BluetoothDevice.Address);

        var bytes = ScanRecord.GetBytes();
        if (bytes != null)
        {
            foreach (var b in bytes)
            {
                hash.Add(b);
            }
        }

        return hash.ToHashCode();
    }

    #endregion
}
