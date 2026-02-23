namespace Bluetooth.Maui.Platforms.Droid.Scanning.Options;

#pragma warning disable CA1008 // Enums should have zero value
/// <summary>
///     Android scan match modes (API 23+).
/// </summary>
/// <remarks>
///     Maps to Android ScanSettings match mode constants.
/// </remarks>
public enum ScanMatchMode
{
    /// <summary>
    ///     No matching, all advertisements are reported (default).
    ///     Maps to ScanSettings.MATCH_MODE_AGGRESSIVE (value: 1) without filtering.
    /// </summary>
    None = 0,

    /// <summary>
    ///     Aggressive mode with few matches per filter (fewer false positives).
    ///     Maps to ScanSettings.MATCH_MODE_AGGRESSIVE (value: 1).
    /// </summary>
    Aggressive = 1,

    /// <summary>
    ///     Sticky mode with more matches per filter (higher discovery rate).
    ///     Maps to ScanSettings.MATCH_MODE_STICKY (value: 2).
    /// </summary>
    Sticky = 2
}

#pragma warning restore CA1008 // Enums should have zero value
