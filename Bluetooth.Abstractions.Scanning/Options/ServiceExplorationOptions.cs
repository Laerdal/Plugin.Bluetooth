namespace Bluetooth.Abstractions.Scanning.Options;

/// <summary>
///     Represents options for exploring Bluetooth services on a remote device.
/// </summary>
/// <remarks>
///     Service exploration discovers all services available on a connected device.
///     This is typically the first step in GATT discovery before accessing characteristics.
/// </remarks>
public record ServiceExplorationOptions
{
    /// <summary>
    ///     Gets or sets a value indicating whether to automatically explore characteristics after discovering services.
    /// </summary>
    /// <remarks>
    ///     When enabled, each discovered service will have its characteristics explored automatically.
    ///     This is equivalent to calling <see cref="IBluetoothRemoteService.ExploreCharacteristicsAsync" /> on each service.
    ///     Default: <c>false</c>
    /// </remarks>
    public bool ExploreCharacteristics { get; init; }

    /// <summary>
    ///     Gets or sets a value indicating whether to automatically explore descriptors after discovering characteristics.
    /// </summary>
    /// <remarks>
    ///     Only applicable when <see cref="ExploreCharacteristics" /> is <c>true</c>.
    ///     When enabled, each discovered characteristic will have its descriptors explored automatically.
    ///     This performs a full depth-first exploration: Services → Characteristics → Descriptors.
    ///     Default: <c>false</c>
    /// </remarks>
    public bool ExploreDescriptors { get; init; }

    /// <summary>
    ///     Gets or sets a value indicating whether to skip exploration if services have already been discovered.
    /// </summary>
    /// <remarks>
    ///     When enabled, if the device already has services in its collection, exploration is skipped.
    ///     This is useful to avoid redundant exploration calls on the same device.
    ///     Default: <c>true</c>
    /// </remarks>
    public bool UseCache { get; init; } = true;

    /// <summary>
    ///     Gets or sets the maximum depth of exploration to perform.
    /// </summary>
    /// <remarks>
    ///     This property provides a convenient way to control exploration depth:
    ///     <list type="bullet">
    ///         <item><see cref="ExplorationDepth.ServicesOnly" />: Only discover services</item>
    ///         <item><see cref="ExplorationDepth.Characteristics" />: Discover services and characteristics</item>
    ///         <item><see cref="ExplorationDepth.Descriptors" />: Discover services, characteristics, and descriptors (full exploration)</item>
    ///     </list>
    ///     Setting this property automatically sets <see cref="ExploreCharacteristics" /> and <see cref="ExploreDescriptors" /> accordingly.
    ///     Default: <see cref="ExplorationDepth.ServicesOnly" />
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
    ///     Gets or sets a filter to apply when exploring services.
    ///     Only services matching this filter will be explored.
    /// </summary>
    /// <remarks>
    ///     This can be used to limit exploration to specific services of interest.
    ///     Default: <c>null</c> (explore all services)
    /// </remarks>
    public Func<Guid, bool>? ServiceUuidFilter { get; init; }

    /// <summary>
    ///     Creates exploration options for discovering only services (no characteristics or descriptors).
    /// </summary>
    public static ServiceExplorationOptions ServicesOnly => new ServiceExplorationOptions { Depth = ExplorationDepth.ServicesOnly };

    /// <summary>
    ///     Creates exploration options for discovering services and characteristics (no descriptors).
    /// </summary>
    public static ServiceExplorationOptions WithCharacteristics => new ServiceExplorationOptions { Depth = ExplorationDepth.Characteristics };

    /// <summary>
    ///     Creates exploration options for full discovery (services, characteristics, and descriptors).
    /// </summary>
    public static ServiceExplorationOptions Full => new ServiceExplorationOptions { Depth = ExplorationDepth.Descriptors };
}