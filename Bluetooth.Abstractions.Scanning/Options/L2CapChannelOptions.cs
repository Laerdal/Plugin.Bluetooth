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
}
