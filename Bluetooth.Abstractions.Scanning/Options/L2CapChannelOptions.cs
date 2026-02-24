namespace Bluetooth.Abstractions.Scanning.Options;

/// <summary>
///     Configuration options for L2CAP channel operations.
/// </summary>
/// <remarks>
///     <para>
///         L2CAP (Logical Link Control and Adaptation Protocol) channels provide connection-oriented
///         data channels over Bluetooth. These options control timeout behavior for various L2CAP operations.
///     </para>
///     <para>
///         Configure via the Options pattern:
///     </para>
///     <code>
/// builder.Services.Configure&lt;L2CapChannelOptions&gt;(options =&gt;
/// {
///     options.OpenTimeout = TimeSpan.FromSeconds(60); // For slow devices
///     options.SendTimeout = TimeSpan.FromSeconds(30);  // For large transfers
/// });
/// </code>
/// </remarks>
public record L2CapChannelOptions
{
    /// <summary>
    ///     Gets or sets the timeout for opening an L2CAP channel.
    /// </summary>
    /// <remarks>
    ///     This timeout applies when calling <c>OpenAsync()</c> on an L2CAP channel.
    ///     Opening a channel involves establishing the L2CAP connection with the remote device.
    ///     <para>
    ///         <b>Considerations for adjusting this timeout:</b>
    ///     </para>
    ///     <list type="bullet">
    ///         <item>Increase for devices with slow response times or poor signal quality</item>
    ///         <item>Increase when connecting through multiple hops or gateways</item>
    ///         <item>Decrease in testing scenarios for faster failure detection</item>
    ///     </list>
    ///     Default: <c>30 seconds</c>
    /// </remarks>
    public TimeSpan OpenTimeout { get; init; } = TimeSpan.FromSeconds(30);

    /// <summary>
    ///     Gets or sets the timeout for closing an L2CAP channel.
    /// </summary>
    /// <remarks>
    ///     This timeout applies when calling <c>CloseAsync()</c> on an L2CAP channel.
    ///     Closing involves gracefully shutting down the L2CAP connection.
    ///     <para>
    ///         <b>Considerations for adjusting this timeout:</b>
    ///     </para>
    ///     <list type="bullet">
    ///         <item>Usually shorter than open timeout since close is a simpler operation</item>
    ///         <item>Increase if you observe incomplete cleanup or resource leaks</item>
    ///         <item>Decrease in testing scenarios where fast cleanup is desired</item>
    ///     </list>
    ///     Default: <c>10 seconds</c>
    /// </remarks>
    public TimeSpan CloseTimeout { get; init; } = TimeSpan.FromSeconds(10);

    /// <summary>
    ///     Gets or sets the timeout for reading data from an L2CAP channel.
    /// </summary>
    /// <remarks>
    ///     This timeout applies when calling <c>ReadAsync()</c> to receive data from the channel.
    ///     <para>
    ///         <b>Considerations for adjusting this timeout:</b>
    ///     </para>
    ///     <list type="bullet">
    ///         <item>Increase for devices that may have delayed responses</item>
    ///         <item>Increase when reading large amounts of data</item>
    ///         <item>Decrease for interactive applications where responsiveness is critical</item>
    ///     </list>
    ///     Default: <c>10 seconds</c>
    /// </remarks>
    public TimeSpan ReadTimeout { get; init; } = TimeSpan.FromSeconds(10);

    /// <summary>
    ///     Gets or sets the timeout for writing data to an L2CAP channel.
    /// </summary>
    /// <remarks>
    ///     This timeout applies when calling <c>WriteAsync()</c> to transmit data over the channel.
    ///     <para>
    ///         <b>Considerations for adjusting this timeout:</b>
    ///     </para>
    ///     <list type="bullet">
    ///         <item>Increase for large data transfers (e.g., firmware updates, file transfers)</item>
    ///         <item>Increase for devices with limited throughput or flow control constraints</item>
    ///         <item>Increase when operating in environments with high interference</item>
    ///         <item>Decrease for small control messages where fast failure is preferred</item>
    ///     </list>
    ///     Default: <c>10 seconds</c>
    /// </remarks>
    public TimeSpan WriteTimeout { get; init; } = TimeSpan.FromSeconds(10);

    /// <summary>
    ///     Gets or sets the default MTU (Maximum Transmission Unit) to use when the platform
    ///     cannot determine the actual MTU from the device.
    /// </summary>
    /// <remarks>
    ///     The MTU determines the maximum size of a single L2CAP packet that can be transmitted.
    ///     This value is used as a fallback when:
    ///     <list type="bullet">
    ///         <item>On iOS/macOS - always used (CoreBluetooth doesn't expose actual MTU)</item>
    ///         <item>On Android API 29-32 - used when MaxTransmitPacketSize is unavailable</item>
    ///         <item>On Windows - depends on platform capabilities</item>
    ///     </list>
    ///     <para>
    ///         <b>Recommended values:</b>
    ///     </para>
    ///     <list type="bullet">
    ///         <item>672 bytes - Bluetooth spec minimum (safest, works everywhere)</item>
    ///         <item>1024-4096 bytes - Good balance for most applications</item>
    ///         <item>23170 bytes - Common maximum for BLE 5.0+ devices</item>
    ///         <item>65535 bytes - Theoretical maximum (rarely supported)</item>
    ///     </list>
    ///     <para>
    ///         <b>Trade-offs:</b>
    ///     </para>
    ///     <list type="bullet">
    ///         <item>Larger MTU = Higher throughput, fewer packets, lower overhead</item>
    ///         <item>Smaller MTU = Better compatibility, less memory usage, faster retries</item>
    ///     </list>
    ///     <para>
    ///         <b>Example use cases:</b>
    ///     </para>
    ///     <list type="bullet">
    ///         <item>Firmware updates: Set to 4096+ bytes for 6-8x faster transfers</item>
    ///         <item>Memory-constrained IoT devices: Set to 512 bytes to reduce memory footprint</item>
    ///         <item>General use: Keep default 672 bytes for maximum compatibility</item>
    ///     </list>
    ///     Default: <c>672 bytes</c> (Bluetooth L2CAP minimum guaranteed)
    /// </remarks>
    public int DefaultMtu { get; init; } = 672;

    /// <summary>
    ///     Gets or sets the size of the read buffer used in the background read loop.
    /// </summary>
    /// <remarks>
    ///     This controls how much memory is allocated for reading incoming data in the background.
    ///     <para>
    ///         <b>Behavior:</b>
    ///     </para>
    ///     <list type="bullet">
    ///         <item>When <c>null</c>: Uses the MTU size (default behavior)</item>
    ///         <item>When specified: Uses the exact buffer size provided</item>
    ///     </list>
    ///     <para>
    ///         <b>Considerations:</b>
    ///     </para>
    ///     <list type="bullet">
    ///         <item>Set smaller for memory-constrained devices with frequent small messages</item>
    ///         <item>Set larger than MTU to batch multiple packets (may increase latency)</item>
    ///         <item>Should generally be at least as large as your expected packet size</item>
    ///     </list>
    ///     <para>
    ///         <b>Platform Support:</b>
    ///     </para>
    ///     <list type="bullet">
    ///         <item>Android: Used for background read loop buffer allocation</item>
    ///         <item>iOS/macOS: Not used (NSStream handles buffering internally)</item>
    ///         <item>Windows: Depends on implementation</item>
    ///     </list>
    ///     <para>
    ///         <b>Example use cases:</b>
    ///     </para>
    ///     <list type="bullet">
    ///         <item>High-frequency small packets: Set to 256 bytes to save memory</item>
    ///         <item>Bulk transfers: Set equal to or larger than MTU</item>
    ///         <item>General use: Keep default (null) to automatically use MTU</item>
    ///     </list>
    ///     Default: <c>null</c> (uses MTU size)
    /// </remarks>
    public int? ReadBufferSize { get; init; }

    /// <summary>
    ///     Gets or sets whether to enable automatic background reading for DataReceived events.
    /// </summary>
    /// <remarks>
    ///     When enabled, a background task continuously reads from the channel and raises
    ///     <c>DataReceived</c> events when data arrives (push model).
    ///     <para>
    ///         When disabled, you must call <c>ReadAsync()</c> manually to receive data (pull model).
    ///     </para>
    ///     <para>
    ///         <b>Enable if:</b>
    ///     </para>
    ///     <list type="bullet">
    ///         <item>You rely on the <c>DataReceived</c> event for notifications</item>
    ///         <item>You want push-based notifications when data arrives</item>
    ///         <item>Your application is event-driven</item>
    ///     </list>
    ///     <para>
    ///         <b>Disable if:</b>
    ///     </para>
    ///     <list type="bullet">
    ///         <item>You only use <c>ReadAsync()</c> (pull model)</item>
    ///         <item>You want to reduce background task overhead</item>
    ///         <item>You want explicit control over when reads occur</item>
    ///     </list>
    ///     <para>
    ///         <b>Platform Support:</b>
    ///     </para>
    ///     <list type="bullet">
    ///         <item>Android: Starts/stops background read loop</item>
    ///         <item>iOS/macOS: Not applicable (NSStreamDelegate provides push automatically)</item>
    ///         <item>Windows: Depends on implementation</item>
    ///     </list>
    ///     <para>
    ///         <b>Resource Impact:</b>
    ///     </para>
    ///     <list type="bullet">
    ///         <item>Enabled: One background task per channel consuming a thread pool thread</item>
    ///         <item>Disabled: No background overhead, manual read calls only</item>
    ///     </list>
    ///     Default: <c>true</c> (enable push-based reading)
    /// </remarks>
    public bool EnableBackgroundReading { get; init; } = true;

    /// <summary>
    ///     Gets or sets whether to automatically flush the output stream after each write.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <b>When enabled (true):</b>
    ///     </para>
    ///     <list type="bullet">
    ///         <item>Data is sent immediately after each <c>WriteAsync()</c> call</item>
    ///         <item>Lower latency - best for real-time/interactive applications</item>
    ///         <item>Lower throughput - more protocol overhead per packet</item>
    ///     </list>
    ///     <para>
    ///         <b>When disabled (false):</b>
    ///     </para>
    ///     <list type="bullet">
    ///         <item>Data may be buffered before transmission</item>
    ///         <item>Higher throughput - can achieve 20-30% improvement for bulk transfers</item>
    ///         <item>Higher latency - data may be delayed until buffer fills or stream closes</item>
    ///     </list>
    ///     <para>
    ///         <b>Platform Support:</b>
    ///     </para>
    ///     <list type="bullet">
    ///         <item>Android: Controls whether <c>FlushAsync()</c> is called after write</item>
    ///         <item>iOS/macOS: NSStream handles flushing automatically (option ignored)</item>
    ///         <item>Windows: Depends on implementation</item>
    ///     </list>
    ///     <para>
    ///         <b>Use Cases:</b>
    ///     </para>
    ///     <list type="bullet">
    ///         <item>Real-time control/telemetry: Keep enabled (true) for low latency</item>
    ///         <item>File transfers/firmware updates: Disable (false) for maximum throughput</item>
    ///         <item>Mixed workloads: Keep default (true) and batch writes manually if needed</item>
    ///     </list>
    ///     Default: <c>true</c> (auto-flush for lowest latency)
    /// </remarks>
    public bool AutoFlushWrites { get; init; } = true;
}
