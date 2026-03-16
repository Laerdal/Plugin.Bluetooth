namespace Bluetooth.Abstractions.Enums;

/// <summary>
///     Defines connection priority modes that affect connection parameters and power consumption.
/// </summary>
public enum ConnectionPriority
{
    /// <summary>
    ///     Balanced mode providing reasonable performance and moderate power consumption.
    /// </summary>
    /// <remarks>
    ///     Android: CONNECTION_PRIORITY_BALANCED
    ///     Connection interval: 30-50ms, Latency: 0, Timeout: 20s
    /// </remarks>
    Balanced = 0,

    /// <summary>
    ///     High performance mode optimized for low latency at the cost of higher power consumption.
    /// </summary>
    /// <remarks>
    ///     Android: CONNECTION_PRIORITY_HIGH
    ///     Connection interval: 11.25-15ms, Latency: 0, Timeout: 20s
    ///     Best for real-time data transfer or gaming
    /// </remarks>
    High = 1,

    /// <summary>
    ///     Low power mode optimized for battery life with reduced performance.
    /// </summary>
    /// <remarks>
    ///     Android: CONNECTION_PRIORITY_LOW_POWER
    ///     Connection interval: 100-125ms, Latency: 2, Timeout: 20s
    ///     Best for infrequent data updates
    /// </remarks>
    LowPower = 2
}
