namespace Bluetooth.Abstractions.Options;

/// <summary>
///     Configuration for retry behavior of Bluetooth operations.
/// </summary>
/// <remarks>
///     Provides flexible retry configuration for BLE operations that may fail intermittently.
///     Common scenarios include Android GATT error 133 (connection failures), scan start failures
///     when the adapter is busy, and transient GATT operation failures.
/// </remarks>
public record RetryOptions
{
    /// <summary>
    ///     Gets the maximum number of retry attempts (default: 3).
    /// </summary>
    /// <remarks>
    ///     Setting to 0 disables retries (single attempt only).
    ///     Higher values increase reliability but may increase operation time.
    /// </remarks>
    public int MaxRetries { get; init; } = 3;

    /// <summary>
    ///     Gets the base delay between retry attempts (default: 200ms).
    /// </summary>
    /// <remarks>
    ///     This is the base delay used between retries. When <see cref="ExponentialBackoff"/> is enabled,
    ///     the actual delay will increase exponentially: base, base*2, base*4, etc.
    /// </remarks>
    public TimeSpan DelayBetweenRetries { get; init; } = TimeSpan.FromMilliseconds(200);

    /// <summary>
    ///     Gets whether to use exponential backoff for retry delays (default: false).
    /// </summary>
    /// <remarks>
    ///     When enabled, delays between retries double with each attempt:
    ///     - Attempt 1: DelayBetweenRetries
    ///     - Attempt 2: DelayBetweenRetries * 2
    ///     - Attempt 3: DelayBetweenRetries * 4
    ///     - Etc.
    ///     This reduces load on the BLE stack during retry storms.
    /// </remarks>
    public bool ExponentialBackoff { get; init; }

    /// <summary>
    ///     Default retry configuration (3 retries, 200ms delay, no exponential backoff).
    /// </summary>
    /// <remarks>
    ///     This configuration works well for most BLE operations and matches the existing
    ///     hardcoded retry behavior in GATT write operations.
    /// </remarks>
    public static RetryOptions Default => new();

    /// <summary>
    ///     No retry configuration (0 retries, single attempt only).
    /// </summary>
    /// <remarks>
    ///     Use this when you want to disable retry logic for a specific operation.
    /// </remarks>
    public static RetryOptions None => new() { MaxRetries = 0 };

    /// <summary>
    ///     Aggressive retry configuration (5 retries, 100ms base delay, exponential backoff enabled).
    /// </summary>
    /// <remarks>
    ///     Use this for operations that frequently fail due to transient issues.
    ///     The exponential backoff helps prevent overwhelming the BLE stack.
    ///     Total max delay: 100ms + 200ms + 400ms + 800ms + 1600ms = 3100ms
    /// </remarks>
    public static RetryOptions Aggressive => new()
    {
        MaxRetries = 5,
        DelayBetweenRetries = TimeSpan.FromMilliseconds(100),
        ExponentialBackoff = true
    };
}
