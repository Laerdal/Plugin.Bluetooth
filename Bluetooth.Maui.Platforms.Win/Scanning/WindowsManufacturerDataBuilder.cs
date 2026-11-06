namespace Bluetooth.Maui.Platforms.Win.Scanning;

/// <summary>
///     Builds the <c>ManufacturerData</c> value of a Windows advertisement from its "Manufacturer Specific Data" sections (AD type 0xFF).
/// </summary>
/// <remarks>
///     <para>
///         Windows hands every advertisement data section over as its AD type and the bytes that follow the type. For Manufacturer
///         Specific Data those bytes are the company id, least significant byte first, and then the data of that manufacturer.
///         This class does not touch any Windows type, so it can be tested on any platform.
///     </para>
///     <para>
///         The result has the layout Android delivers: for every company id, in the order it first appears, the company id once,
///         followed by the data of all the sections that carry it. With a single section that is the section itself.
///     </para>
/// </remarks>
internal static class WindowsManufacturerDataBuilder
{
    private const int CompanyIdLength = 2;

    /// <summary>
    ///     Builds the manufacturer data of one advertisement, or of an advertisement and its scan response.
    /// </summary>
    /// <param name="sections">
    ///     The bytes of every manufacturer-specific data section, each starting with its company id, in the order they were received:
    ///     the sections of the advertisement first, then those of its scan response.
    /// </param>
    /// <returns>
    ///     The manufacturer data, or an empty value when there is no section. A section too short to hold a company id cannot be
    ///     attributed to a manufacturer and is skipped.
    /// </returns>
    internal static ReadOnlyMemory<byte> Build(IEnumerable<byte[]> sections)
    {
        ArgumentNullException.ThrowIfNull(sections);

        // Most advertisements have no section or one. The one is returned as it is, without copying it.
        byte[]? only = null;
        List<(ushort CompanyId, List<byte[]> Sections)>? groups = null;

        foreach (var section in sections)
        {
            if (section.Length < CompanyIdLength)
            {
                continue;
            }

            if (only == null && groups == null)
            {
                only = section;
                continue;
            }

            if (groups == null)
            {
                groups = [(CompanyIdOf(only!), [only!])];
                only = null;
            }

            var companyId = CompanyIdOf(section);
            var index = groups.FindIndex(group => group.CompanyId == companyId);
            if (index < 0)
            {
                groups.Add((companyId, [section]));
            }
            else
            {
                groups[index].Sections.Add(section);
            }
        }

        if (groups == null)
        {
            return only ?? ReadOnlyMemory<byte>.Empty;
        }

        var length = 0;
        foreach (var (_, grouped) in groups)
        {
            length += CompanyIdLength;
            foreach (var section in grouped)
            {
                length += section.Length - CompanyIdLength;
            }
        }

        var result = new byte[length];
        var offset = 0;
        foreach (var (_, grouped) in groups)
        {
            // The company id is written once per group, from its first section.
            result[offset++] = grouped[0][0];
            result[offset++] = grouped[0][1];
            foreach (var section in grouped)
            {
                section.AsSpan(CompanyIdLength).CopyTo(result.AsSpan(offset));
                offset += section.Length - CompanyIdLength;
            }
        }

        return result;
    }

    private static ushort CompanyIdOf(byte[] section)
    {
        return (ushort) (section[0] | (section[1] << 8));
    }
}
