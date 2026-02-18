namespace Bluetooth.Maui.Platforms.Apple.Tools;

/// <summary>
/// Extension methods for converting between System.Guid and iOS CoreBluetooth UUID types.
/// </summary>
public static class GuidExtensions
{
    private const string StandardGuidExtension = "-0000-1000-8000-00805F9B34FB";

    /// <summary>
    /// Converts an NSUuid to a System.Guid.
    /// </summary>
    /// <param name="uuid">The NSUuid to convert.</param>
    /// <returns>A System.Guid representation of the NSUuid.</returns>
    public static Guid ToGuid(this NSUuid uuid)
    {
        ArgumentNullException.ThrowIfNull(uuid);

        return Guid.ParseExact(uuid.AsString(), "d");
    }

    /// <summary>
    /// Converts a CBUUID to a System.Guid, handling both 16-bit and 128-bit UUIDs.
    /// </summary>
    /// <param name="uuid">The CBUUID to convert.</param>
    /// <returns>A System.Guid representation of the CBUUID.</returns>
    public static Guid ToGuid(this CBUUID uuid)
    {
        ArgumentNullException.ThrowIfNull(uuid);

        // this sometimes returns only the significant bits, e.g.
        // 180d or whatever    so we need to add the full string
        var idString = uuid.ToString();

        idString = idString.Length switch
        {
            4 => $"0000{idString}{StandardGuidExtension}",
            8 => $"{idString}{StandardGuidExtension}",
            _ => idString
        };

        return Guid.ParseExact(idString, "d");
    }

    /// <summary>
    /// Converts a System.Guid to an NSUuid.
    /// </summary>
    /// <param name="value">The System.Guid to convert.</param>
    /// <returns>An NSUuid representation of the System.Guid.</returns>
    public static NSUuid ToNSUuid(this Guid value)
    {
        return new NSUuid(value.ToString());
    }

    // with shorten=true
    //
    // guid 00001234-0000-1000-8000-00805F9B34FB -> cbuuid 1234      (special case for 16bit ble uuids)
    // guid 12345678-0000-1000-8000-00805F9B34FB -> cbuuid 12345678  (special case for 32bit ble uuids)
    // guid ADC23234-3209-3240-2308-209ABDE0932F -> cbuuid ADC23234-3209-3240-2308-209ABDE0932F
    /// <summary>
    /// Converts a System.Guid to a CBUUID, with optional shortening for standard Bluetooth UUIDs.
    /// </summary>
    /// <param name="value">The System.Guid to convert.</param>
    /// <param name="shorten">Whether to shorten standard Bluetooth UUIDs to their 16-bit or 32-bit forms.</param>
    /// <returns>A CBUUID representation of the System.Guid.</returns>
    public static CBUUID ToCBUuid(this Guid value, bool shorten = true)
    {
        if (!shorten)
        {
            return CBUUID.FromString(value.ToString());
        }

        var guidStringUppercased = value.ToString().ToUpperInvariant();
        if (guidStringUppercased.EndsWith(StandardGuidExtension, StringComparison.InvariantCultureIgnoreCase)) //0 vital
        {
            guidStringUppercased = guidStringUppercased[..^StandardGuidExtension.Length]; //remove the standardized postfix
        }

        if (guidStringUppercased.Length == 8 && guidStringUppercased.StartsWith("0000", StringComparison.Ordinal)) //1
        {
            guidStringUppercased = guidStringUppercased.Substring(4, 4); // 00001234 -> 1234
        }

        return CBUUID.FromString(guidStringUppercased);

        //0 its vital to remove the industry standard postfix   *-0000-1000-8000-00805F9B34FB    when we detect it otherwise the resulting
        //  cbuuid will be transmitted over as 128bits instead of 16/32bits
        //
        //1 if the prefix got trimmed down to 8 chars and it starts with 0000 then we are dealing with just a 16bit short-uuid
    }
}