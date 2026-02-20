namespace Bluetooth.Abstractions.Scanning.Options;

/// <summary>
///     Represents options for smoothing signal strength jitter in Bluetooth scanning.
/// </summary>
public record SignalStrengthSmoothingOptions
{
    /// <summary>
    ///     The algorithm will store the last <see cref="SmoothingOnAdvertisement" /> values and
    ///     average them out.
    ///     This has the effect of smoothing out the jitter in the signal strength.
    ///     The higher the value the smoother the signal strength will be, but also the less reactive it will be.
    ///     This is the value to use when not connected : the signal strength is more jittery when not connected.
    /// </summary>
    public int SmoothingOnAdvertisement { get; init; } = 5;

    /// <summary>
    ///     The algorithm will store the last <see cref="SmoothingWhenConnected" /> values and average
    ///     them out.
    ///     This has the effect of smoothing out the jitter in the signal strength.
    ///     The higher the value the smoother the signal strength will be, but also the less reactive it will be.
    ///     This is the value to use when connected : the signal strength is more stable when connected.
    /// </summary>
    public int SmoothingWhenConnected { get; init; } = 3;
}