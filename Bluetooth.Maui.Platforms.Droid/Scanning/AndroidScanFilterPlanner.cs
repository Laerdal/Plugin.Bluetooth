namespace Bluetooth.Maui.Platforms.Droid.Scanning;

/// <summary>
///     One native scan filter to build: the conditions it requires, all of which an advertisement has to meet.
/// </summary>
/// <param name="ServiceUuid">The service UUID the advertisement has to list, or <see langword="null" /> for none.</param>
/// <param name="ManufacturerId">The company identifier the advertisement has to carry manufacturer data for, or <see langword="null" /> for none.</param>
internal readonly record struct AndroidScanFilterSpec(Guid? ServiceUuid, int? ManufacturerId);

/// <summary>
///     Decides which native scan filters a scan needs from the service UUIDs and company identifiers it asks for.
/// </summary>
/// <remarks>
///     <para>
///         Android reports an advertisement when it matches any filter of the list, and a filter matches when it meets all of its own
///         conditions. A scan that asks for service UUIDs and company identifiers wants both, so each pair gets a filter of its own
///         instead of the two lists being put side by side, which would report an advertisement that meets only one of them.
///     </para>
///     <para>This class does not touch any Android type, so it can be tested on any platform.</para>
/// </remarks>
internal static class AndroidScanFilterPlanner
{
    private const int MaxCompanyId = 0xFFFF;

    /// <summary>
    ///     Plans the native scan filters of a scan.
    /// </summary>
    /// <param name="serviceUuids">The service UUIDs to scan for, or <see langword="null" /> for any.</param>
    /// <param name="manufacturerIds">The company identifiers to scan for, or <see langword="null" /> for any.</param>
    /// <returns>
    ///     The filters to build, in the order of the lists; empty when the scan asks for neither, which means no native filtering.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">A company identifier is outside 0 to 65535, the range of a Bluetooth SIG company identifier.</exception>
    public static IReadOnlyList<AndroidScanFilterSpec> Plan(IEnumerable<Guid>? serviceUuids, IEnumerable<int>? manufacturerIds)
    {
        var uuids = serviceUuids?.Distinct().ToArray() ?? [];
        var ids = manufacturerIds?.Distinct().ToArray() ?? [];

        foreach (var id in ids)
        {
            if (id is < 0 or > MaxCompanyId)
            {
                throw new ArgumentOutOfRangeException(nameof(manufacturerIds), id, "A company identifier is between 0 and 65535.");
            }
        }

        if (ids.Length == 0)
        {
            return uuids.Select(uuid => new AndroidScanFilterSpec(uuid, null)).ToArray();
        }

        if (uuids.Length == 0)
        {
            return ids.Select(id => new AndroidScanFilterSpec(null, id)).ToArray();
        }

        return uuids.SelectMany(_ => ids, (uuid, id) => new AndroidScanFilterSpec(uuid, id)).ToArray();
    }
}
