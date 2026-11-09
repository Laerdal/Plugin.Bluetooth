using Bluetooth.Maui.Platforms.Droid.Scanning.NativeObjects;

namespace Bluetooth.Maui.Platforms.Droid.Scanning;

/// <summary>
///     Builds the <c>ManufacturerData</c> value of an Android advertisement from the "Manufacturer Specific Data" parts of its scan record.
/// </summary>
/// <remarks>
///     <para>
///         The result has the layout the rest of the library expects: for every company id, in the order it first appears, the company
///         id once (least significant byte first), followed by the data of all the parts that carry it. With a single part that is the
///         part itself. The scan record holds the advertisement and its scan response one after the other, so a company id that shows up
///         in both is written once and followed by both payloads.
///     </para>
///     <para>This class does not touch any Android type, so it can be tested on any platform.</para>
/// </remarks>
internal static class AndroidManufacturerDataBuilder
{
    private const int CompanyIdLength = 2;

    /// <summary>
    ///     Builds the manufacturer data of one scan record.
    /// </summary>
    /// <param name="parts">The parts of the scan record, in the order they were received. Parts of another type are ignored.</param>
    /// <returns>
    ///     The manufacturer data, with exactly the bytes that were written, or an empty value when there is no part to write. A
    ///     manufacturer part too short to hold a company id cannot be attributed to a manufacturer and is skipped.
    /// </returns>
    internal static ReadOnlyMemory<byte> Build(IEnumerable<ScanRecordPart> parts)
    {
        ArgumentNullException.ThrowIfNull(parts);

        var manufacturerParts = parts
                                .Where(part => part.Type == ScanRecordPart.AdvertisementRecordType.ManufacturerSpecificData
                                               && part.Payload.Length >= CompanyIdLength)
                                .ToArray();
        if (manufacturerParts.Length == 0)
        {
            return ReadOnlyMemory<byte>.Empty;
        }

        // The company ids in the order they first appear. A part is attributed to a company id by its first two bytes.
        var companyIds = new List<ushort>();
        var totalSize = 0;
        foreach (var part in manufacturerParts)
        {
            totalSize += part.Payload.Length;
            var companyId = CompanyIdOf(part);
            if (!companyIds.Contains(companyId))
            {
                companyIds.Add(companyId);
            }
        }

        // Every part carries its company id, but the result carries each id once.
        totalSize -= CompanyIdLength * (manufacturerParts.Length - companyIds.Count);

        var result = new byte[totalSize];
        var offset = 0;
        foreach (var companyId in companyIds)
        {
            result[offset++] = (byte) (companyId & 0xFF);
            result[offset++] = (byte) (companyId >> 8);

            foreach (var part in manufacturerParts)
            {
                if (CompanyIdOf(part) != companyId)
                {
                    continue;
                }

                var data = part.ManufacturerData;
                data.CopyTo(result.AsSpan(offset));
                offset += data.Length;
            }
        }

        return result;
    }

    private static ushort CompanyIdOf(ScanRecordPart part)
    {
        var idBytes = part.ManufacturerIdBytes;
        return (ushort) (idBytes[0] | (idBytes[1] << 8));
    }
}
