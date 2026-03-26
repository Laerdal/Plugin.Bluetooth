namespace Bluetooth.Abstractions.Scanning.Options;

/// <summary>
///     Represents options for exploring Bluetooth descriptors within a characteristic.
/// </summary>
/// <remarks>
///     Descriptor exploration discovers all descriptors available within a specific characteristic.
///     Descriptors provide additional metadata and configuration for characteristics.
/// </remarks>
public record DescriptorExplorationOptions
{
    /// <summary>
    ///     Gets or sets a value indicating whether to skip exploration if descriptors have already been discovered.
    /// </summary>
    /// <remarks>
    ///     When enabled, if the characteristic already has descriptors in its collection, exploration is skipped.
    ///     Default: <c>true</c>
    /// </remarks>
    public bool UseCache { get; init; } = true;

    /// <summary>
    ///     Gets or sets a filter to apply when exploring descriptors.
    ///     Only descriptors matching this filter will be explored.
    /// </summary>
    /// <remarks>
    ///     This can be used to limit exploration to specific descriptors of interest.
    ///     Default: <c>null</c> (explore all descriptors)
    /// </remarks>
    public Func<Guid, bool>? DescriptorUuidFilter { get; init; }

    /// <summary>
    ///     Gets the default descriptor exploration options.
    /// </summary>
    public static DescriptorExplorationOptions Default => new DescriptorExplorationOptions();
}
