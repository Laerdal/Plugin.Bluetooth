namespace Bluetooth.Abstractions.Scanning.Options;

/// <summary>
///     Defines the depth of Bluetooth GATT exploration to perform.
/// </summary>
public enum ExplorationDepth
{
    /// <summary>
    ///     Discover only services. Does not explore characteristics or descriptors.
    /// </summary>
    ServicesOnly = 0,

    /// <summary>
    ///     Discover services and their characteristics. Does not explore descriptors.
    /// </summary>
    Characteristics = 1,

    /// <summary>
    ///     Discover services, characteristics, and descriptors. Full depth exploration.
    /// </summary>
    Descriptors = 2
}
