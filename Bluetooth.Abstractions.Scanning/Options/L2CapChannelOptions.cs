namespace Bluetooth.Abstractions.Scanning.Options;

/// <summary>
///     Options for L2CAP channel creation.
/// </summary>
public record L2CapChannelOptions
{
    /// <summary>
    ///     Gets the maximum transmission unit (MTU) for the L2CAP channel.
    ///     If null, a platform-determined default is used.
    /// </summary>
    public int? Mtu { get; init; }

    /// <summary>
    ///     Gets the default MTU to use when the platform cannot determine it automatically.
    /// </summary>
    public int DefaultMtu { get; init; } = 512;

    /// <summary>
    ///     Gets a value indicating whether background reading is enabled.
    ///     When true, a background loop raises <c>DataReceived</c> events as data arrives.
    /// </summary>
    public bool EnableBackgroundReading { get; init; } = true;

    /// <summary>
    ///     Gets a value indicating whether the output stream is automatically flushed after each write.
    /// </summary>
    public bool AutoFlushWrites { get; init; } = true;

    /// <summary>
    ///     Gets the buffer size in bytes used for reading data from the L2CAP channel.
    ///     Defaults to <see cref="DefaultMtu" /> when 0.
    /// </summary>
    public int ReadBufferSize { get; init; } = 512;
}
