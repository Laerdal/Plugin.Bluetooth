namespace Bluetooth.Abstractions.Scanning.Options;

/// <summary>
///     Represents options for exploring Bluetooth characteristics within a service.
/// </summary>
/// <remarks>
///     Characteristic exploration discovers all characteristics available within a specific service.
/// </remarks>
public record CharacteristicExplorationOptions
{
    /// <summary>
    ///     Gets or sets a value indicating whether to automatically explore descriptors after discovering characteristics.
    /// </summary>
    /// <remarks>
    ///     When enabled, each discovered characteristic will have its descriptors explored automatically.
    ///     Default: <c>false</c>
    /// </remarks>
    public bool ExploreDescriptors { get; init; }

    /// <summary>
    ///     Gets or sets a value indicating whether to skip exploration if characteristics have already been discovered.
    /// </summary>
    /// <remarks>
    ///     When enabled, if the service already has characteristics in its collection, exploration is skipped.
    ///     Default: <c>true</c>
    /// </remarks>
    public bool UseCache { get; init; } = true;

    /// <summary>
    ///     Gets or sets a filter to apply when exploring characteristics.
    ///     Only characteristics matching this filter will be explored.
    /// </summary>
    /// <remarks>
    ///     This can be used to limit exploration to specific characteristics of interest.
    ///     Default: <c>null</c> (explore all characteristics)
    /// </remarks>
    public Func<Guid, bool>? CharacteristicUuidFilter { get; init; }

    /// <summary>
    ///     Creates exploration options for discovering only characteristics (no descriptors).
    /// </summary>
    public static CharacteristicExplorationOptions CharacteristicsOnly => new CharacteristicExplorationOptions { ExploreDescriptors = false };

    /// <summary>
    ///     Creates exploration options for full discovery (characteristics and descriptors).
    /// </summary>
    public static CharacteristicExplorationOptions Full => new CharacteristicExplorationOptions { ExploreDescriptors = true };
}
