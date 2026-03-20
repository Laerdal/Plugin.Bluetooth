namespace Bluetooth.Abstractions.Scanning.Options;

/// <summary>
///     Defines when scan result callbacks should be triggered.
/// </summary>
[Flags]
public enum BluetoothScanCallbackType
{
    /// <summary>
    ///     Do not report any advertisement packets. This can be used when only interested in scan start/stop events or when filtering is performed in software.
    /// </summary>
    None = 0,

    /// <summary>
    ///     Report all advertisement packets as they are received.
    /// </summary>
    /// <remarks>
    ///     Android: CALLBACK_TYPE_ALL_MATCHES
    /// </remarks>
    AllMatches = 1,

    /// <summary>
    ///     Report only the first advertisement packet from each device.
    /// </summary>
    /// <remarks>
    ///     Android: CALLBACK_TYPE_FIRST_MATCH
    /// </remarks>
    FirstMatch = 2,

    /// <summary>
    ///     Report when a device is no longer detected (after a timeout period).
    /// </summary>
    /// <remarks>
    ///     Android: CALLBACK_TYPE_MATCH_LOST
    ///     Not fully supported on iOS/Windows
    /// </remarks>
    MatchLost = 4,

    /// <summary>
    ///     Report both first match and match lost events.
    /// </summary>
    /// <remarks>
    ///     Android: CALLBACK_TYPE_FIRST_MATCH | CALLBACK_TYPE_MATCH_LOST
    /// </remarks>
    FirstMatchAndMatchLost = FirstMatch | MatchLost
}
