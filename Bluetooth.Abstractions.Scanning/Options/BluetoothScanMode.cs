namespace Bluetooth.Abstractions.Scanning.Options;

/// <summary>
///     Defines scan modes that control the power consumption and scan latency trade-off.
/// </summary>
public enum BluetoothScanMode
{
    /// <summary>
    ///     Low power mode with infrequent scans. Optimizes for battery life.
    /// </summary>
    /// <remarks>
    ///     Android: SCAN_MODE_LOW_POWER (scan interval ~5 seconds)
    /// </remarks>
    LowPower = 0,

    /// <summary>
    ///     Balanced mode providing a compromise between power consumption and scan latency.
    /// </summary>
    /// <remarks>
    ///     Android: SCAN_MODE_BALANCED (scan interval ~2 seconds)
    /// </remarks>
    Balanced = 1,

    /// <summary>
    ///     Low latency mode with frequent scans. Optimizes for fast discovery at the cost of higher power consumption.
    /// </summary>
    /// <remarks>
    ///     Android: SCAN_MODE_LOW_LATENCY (scan continuously or near-continuously)
    /// </remarks>
    LowLatency = 2,

    /// <summary>
    ///     Opportunistic mode where scans are performed only when other apps are scanning.
    ///     Provides best battery life but unpredictable scan frequency.
    /// </summary>
    /// <remarks>
    ///     Android: SCAN_MODE_OPPORTUNISTIC (Android 7.0+)
    ///     Not supported on iOS/Windows (falls back to Balanced)
    /// </remarks>
    Opportunistic = -1
}
