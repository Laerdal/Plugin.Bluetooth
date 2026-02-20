namespace Bluetooth.Abstractions.Enums;

/// <summary>
///     Represents the physical layer (PHY) modes available for Bluetooth 5.0+ connections.
///     The PHY determines the data rate and range characteristics of the connection.
/// </summary>
[Flags]
public enum PhyMode
{
    /// <summary>
    ///     No preference for PHY.
    /// </summary>
    None = 0,

    /// <summary>
    ///     1 Mbit/s PHY (LE 1M).
    ///     Bluetooth 4.0+ standard PHY, balanced between range and throughput.
    /// </summary>
    Le1M = 1 << 0,

    /// <summary>
    ///     2 Mbit/s PHY (LE 2M).
    ///     Bluetooth 5.0+ high-speed PHY, higher throughput but reduced range.
    /// </summary>
    Le2M = 1 << 1,

    /// <summary>
    ///     Coded PHY (LE Coded).
    ///     Bluetooth 5.0+ long-range PHY, extended range but lower throughput.
    ///     Available in S=2 (500 kbit/s) and S=8 (125 kbit/s) coding schemes.
    /// </summary>
    LeCoded = 1 << 2
}