namespace Bluetooth.Core.Infrastructure.Scheduling;

/// <summary>
///     Options for configuring the Ticker.
/// </summary>
public sealed class TickerOptions
{
    /// <summary>
    ///     Internal scheduling resolution. Smaller = more accurate, more wakeups.
    /// </summary>
    public TimeSpan Resolution { get; init; } = TimeSpan.FromMilliseconds(250);
}