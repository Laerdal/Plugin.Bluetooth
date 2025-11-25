using Array = System.Array;

namespace Bluetooth.Maui.PlatformSpecific;

/// <summary>
/// Represents a part of a Bluetooth LE advertisement scan record.
/// Contains the size, type, and payload data for a specific advertisement data element.
/// </summary>
public partial class ScanRecordPart
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ScanRecordPart"/> class.
    /// </summary>
    /// <param name="size">The size of the data element in bytes.</param>
    /// <param name="type">The type of the advertisement record.</param>
    /// <param name="payload">The payload data for this record part.</param>
    public ScanRecordPart(byte size, AdvertisementRecordType type, byte[] payload)
    {
        Size = size;
        Type = type;
        _payloadArray = payload;
    }

    #region Size

    /// <summary>
    /// Gets the size of the data element in bytes.
    /// </summary>
    public byte Size { get; init; }

    #endregion

    #region Type

    /// <summary>
    /// Gets the type of the advertisement record.
    /// </summary>
    public AdvertisementRecordType Type { get; init; }

    #endregion

    #region Payload

    private readonly byte[] _payloadArray;

    /// <summary>
    /// Gets the payload data as a read-only span for better performance.
    /// </summary>
    public ReadOnlySpan<byte> Payload => _payloadArray;

    /// <summary>
    /// Gets the manufacturer-specific data (excluding the manufacturer ID) if this is a manufacturer data record.
    /// </summary>
    public ReadOnlySpan<byte> ManufacturerData => Type != AdvertisementRecordType.ManufacturerSpecificData ? default : _payloadArray.AsSpan(2);

    /// <summary>
    /// Gets the manufacturer ID bytes if this is a manufacturer data record.
    /// </summary>
    public ReadOnlySpan<byte> ManufacturerIdBytes => Type != AdvertisementRecordType.ManufacturerSpecificData ? default : _payloadArray.AsSpan(0, 2);

    /// <summary>
    /// Gets the manufacturer ID if this is a manufacturer data record.
    /// </summary>
    public Manufacturer? ManufacturerId => Type != AdvertisementRecordType.ManufacturerSpecificData ? null : (Manufacturer)(((ManufacturerIdBytes[1] & 0xFF) << 8) + (ManufacturerIdBytes[0] & 0xFF));

    #endregion

    /// <summary>
    /// Parses the raw BLE advertisement bytes into appropriate data chunks.
    /// </summary>
    /// <param name="rawBytes">The raw bytes that comprise an entire BLE advertisement message.</param>
    /// <returns>A collection of scan record parts parsed from the raw bytes.</returns>
    /// <remarks>
    /// This mechanism was created to work around certain issues with the default Android BLE API,
    /// specifically the fact that GetManufacturerData(manufacturer_id) returns just the last
    /// manufacturer data chunk instead of all of them.
    /// </remarks>
    internal static IEnumerable<ScanRecordPart> FromRawBytes(byte[] rawBytes)
    {
        if (rawBytes.Length == 0)
        {
            yield break;
        }

        var index = 0;
        while (index < rawBytes.Length && rawBytes[index] != 0)
        {
            var size = rawBytes[index];
            if (index + 1 + size > rawBytes.Length)
            {
                break;
            }

            // Pre-allocate the payload array with exact size
            var payloadArray = new byte[size - 1];

            // Copy the payload data into the pre-allocated array
            Array.Copy(rawBytes,
                       index + 2,
                       payloadArray,
                       0,
                       size - 1);

            yield return new ScanRecordPart(size, (AdvertisementRecordType)rawBytes[index + 1], payloadArray);

            index += size + 1;
        }
    }

    /// <summary>
    /// Returns a string representation of this scan record part.
    /// </summary>
    /// <returns>A string containing the size, type, and length information.</returns>
    public override string ToString()
    {
        return $"Size = {Size}; Type = {Type}; {_payloadArray.Length} bytes long";
    }
}
