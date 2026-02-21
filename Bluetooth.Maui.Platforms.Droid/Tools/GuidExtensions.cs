namespace Bluetooth.Maui.Platforms.Droid.Tools;

/// <summary>
///     Extension methods for converting between .NET <see cref="Guid" /> and Android <see cref="UUID" /> types.
/// </summary>
public static class GuidExtensions
{
    /// <summary>
    ///     Converts an Android <see cref="UUID" /> to a .NET <see cref="Guid" />.
    /// </summary>
    /// <param name="uuid">The UUID to convert.</param>
    /// <returns>A <see cref="Guid" /> representation of the UUID, or <see cref="Guid.Empty" /> if the UUID is null.</returns>
    public static Guid ToGuid(this UUID? uuid)
    {
        return uuid == null ? Guid.Empty : Guid.Parse(uuid.ToString());
    }

    /// <summary>
    ///     Converts a .NET <see cref="Guid" /> to an Android <see cref="UUID" />.
    /// </summary>
    /// <param name="value">The Guid to convert.</param>
    /// <returns>A <see cref="UUID" /> representation of the Guid.</returns>
    public static UUID? ToUuid(this Guid value)
    {
        return UUID.FromString(value.ToString());
    }
}