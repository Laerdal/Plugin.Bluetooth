namespace Bluetooth.Abstractions.Scanning.Options;

/// <summary>
/// Represents options for exploring Bluetooth services on a remote device.
/// </summary>
/// <remarks>
/// Service exploration discovers all services available on a connected device.
/// This is typically the first step in GATT discovery before accessing characteristics.
/// </remarks>
public record ServiceExplorationOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether to automatically explore characteristics after discovering services.
    /// </summary>
    /// <remarks>
    /// When enabled, each discovered service will have its characteristics explored automatically.
    /// This is equivalent to calling <see cref="IBluetoothRemoteService.ExploreCharacteristicsAsync"/> on each service.
    /// Default: <c>false</c>
    /// </remarks>
    public bool ExploreCharacteristics { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether to automatically explore descriptors after discovering characteristics.
    /// </summary>
    /// <remarks>
    /// Only applicable when <see cref="ExploreCharacteristics"/> is <c>true</c>.
    /// When enabled, each discovered characteristic will have its descriptors explored automatically.
    /// This performs a full depth-first exploration: Services → Characteristics → Descriptors.
    /// Default: <c>false</c>
    /// </remarks>
    public bool ExploreDescriptors { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether to skip exploration if services have already been discovered.
    /// </summary>
    /// <remarks>
    /// When enabled, if the device already has services in its collection, exploration is skipped.
    /// This is useful to avoid redundant exploration calls on the same device.
    /// Default: <c>true</c>
    /// </remarks>
    public bool UseCache { get; init; } = true;

    /// <summary>
    /// Gets or sets the maximum depth of exploration to perform.
    /// </summary>
    /// <remarks>
    /// This property provides a convenient way to control exploration depth:
    /// <list type="bullet">
    /// <item><see cref="ExplorationDepth.ServicesOnly"/>: Only discover services</item>
    /// <item><see cref="ExplorationDepth.Characteristics"/>: Discover services and characteristics</item>
    /// <item><see cref="ExplorationDepth.Descriptors"/>: Discover services, characteristics, and descriptors (full exploration)</item>
    /// </list>
    /// Setting this property automatically sets <see cref="ExploreCharacteristics"/> and <see cref="ExploreDescriptors"/> accordingly.
    /// Default: <see cref="ExplorationDepth.ServicesOnly"/>
    /// </remarks>
    public ExplorationDepth Depth
    {
        get => ExploreDescriptors ? ExplorationDepth.Descriptors :
               ExploreCharacteristics ? ExplorationDepth.Characteristics :
               ExplorationDepth.ServicesOnly;
        init
        {
            ExploreCharacteristics = value >= ExplorationDepth.Characteristics;
            ExploreDescriptors = value >= ExplorationDepth.Descriptors;
        }
    }

    /// <summary>
    /// Gets or sets a filter to apply when exploring services.
    /// Only services matching this filter will be explored.
    /// </summary>
    /// <remarks>
    /// This can be used to limit exploration to specific services of interest.
    /// Default: <c>null</c> (explore all services)
    /// </remarks>
    public Func<Guid, bool>? ServiceUuidFilter { get; init; }

    /// <summary>
    /// Creates exploration options for discovering only services (no characteristics or descriptors).
    /// </summary>
    public static ServiceExplorationOptions ServicesOnly => new() { Depth = ExplorationDepth.ServicesOnly };

    /// <summary>
    /// Creates exploration options for discovering services and characteristics (no descriptors).
    /// </summary>
    public static ServiceExplorationOptions WithCharacteristics => new() { Depth = ExplorationDepth.Characteristics };

    /// <summary>
    /// Creates exploration options for full discovery (services, characteristics, and descriptors).
    /// </summary>
    public static ServiceExplorationOptions Full => new() { Depth = ExplorationDepth.Descriptors };
}

/// <summary>
/// Represents options for exploring Bluetooth characteristics within a service.
/// </summary>
/// <remarks>
/// Characteristic exploration discovers all characteristics available within a specific service.
/// </remarks>
public record CharacteristicExplorationOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether to automatically explore descriptors after discovering characteristics.
    /// </summary>
    /// <remarks>
    /// When enabled, each discovered characteristic will have its descriptors explored automatically.
    /// Default: <c>false</c>
    /// </remarks>
    public bool ExploreDescriptors { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether to skip exploration if characteristics have already been discovered.
    /// </summary>
    /// <remarks>
    /// When enabled, if the service already has characteristics in its collection, exploration is skipped.
    /// Default: <c>true</c>
    /// </remarks>
    public bool UseCache { get; init; } = true;

    /// <summary>
    /// Gets or sets a filter to apply when exploring characteristics.
    /// Only characteristics matching this filter will be explored.
    /// </summary>
    /// <remarks>
    /// This can be used to limit exploration to specific characteristics of interest.
    /// Default: <c>null</c> (explore all characteristics)
    /// </remarks>
    public Func<Guid, bool>? CharacteristicUuidFilter { get; init; }

    /// <summary>
    /// Creates exploration options for discovering only characteristics (no descriptors).
    /// </summary>
    public static CharacteristicExplorationOptions CharacteristicsOnly => new() { ExploreDescriptors = false };

    /// <summary>
    /// Creates exploration options for full discovery (characteristics and descriptors).
    /// </summary>
    public static CharacteristicExplorationOptions Full => new() { ExploreDescriptors = true };
}

/// <summary>
/// Represents options for exploring Bluetooth descriptors within a characteristic.
/// </summary>
/// <remarks>
/// Descriptor exploration discovers all descriptors available within a specific characteristic.
/// Descriptors provide additional metadata and configuration for characteristics.
/// </remarks>
public record DescriptorExplorationOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether to skip exploration if descriptors have already been discovered.
    /// </summary>
    /// <remarks>
    /// When enabled, if the characteristic already has descriptors in its collection, exploration is skipped.
    /// Default: <c>true</c>
    /// </remarks>
    public bool UseCache { get; init; } = true;

    /// <summary>
    /// Gets or sets a filter to apply when exploring descriptors.
    /// Only descriptors matching this filter will be explored.
    /// </summary>
    /// <remarks>
    /// This can be used to limit exploration to specific descriptors of interest.
    /// Default: <c>null</c> (explore all descriptors)
    /// </remarks>
    public Func<Guid, bool>? DescriptorUuidFilter { get; init; }

    /// <summary>
    /// Gets the default descriptor exploration options.
    /// </summary>
    public static DescriptorExplorationOptions Default => new();
}

/// <summary>
/// Defines the depth of Bluetooth GATT exploration to perform.
/// </summary>
public enum ExplorationDepth
{
    /// <summary>
    /// Discover only services. Does not explore characteristics or descriptors.
    /// </summary>
    ServicesOnly = 0,

    /// <summary>
    /// Discover services and their characteristics. Does not explore descriptors.
    /// </summary>
    Characteristics = 1,

    /// <summary>
    /// Discover services, characteristics, and descriptors. Full depth exploration.
    /// </summary>
    Descriptors = 2
}
