namespace Bluetooth.Abstractions.Options;

/// <summary>
///     Represents infrastructure-level configuration for Bluetooth services that are set at application startup via dependency injection.
///     These options define app-wide defaults and behaviors that rarely change during runtime.
/// </summary>
/// <remarks>
///     <para>
///         This class is intended to be configured during application startup using the Options pattern:
///     </para>
///     <code>
/// builder.Services.Configure&lt;BluetoothInfrastructureOptions&gt;(options =&gt;
/// {
///     options.DefaultScanningOptions = new ScanningOptions
///     {
///         ScanMode = BluetoothScanMode.LowPower
///     };
///     options.AutoCleanupOnStop = true;
/// });
/// </code>
///     <para>
///         <b>Key Distinction:</b>
///     </para>
///     <list type="bullet">
///         <item>
///             <term>Infrastructure Options (this class)</term>
///             <description>Set once at app startup via DI. Defines app-wide defaults and behaviors.</description>
///         </item>
///         <item>
///             <term>Operation Options (ScanningOptions, BroadcastingOptions, etc.)</term>
///             <description>Passed to methods like StartScanningAsync(). Can vary per operation.</description>
///         </item>
///     </list>
/// </remarks>
public record BluetoothInfrastructureOptions
{
    /// <summary>
    ///     Gets or sets a value indicating whether to automatically clean up resources when scanner or broadcaster stops.
    /// </summary>
    /// <remarks>
    ///     When enabled:
    ///     <list type="bullet">
    ///         <item>Scanner stop will disconnect all connected devices and clear device list</item>
    ///         <item>Broadcaster stop will disconnect all clients and clear service list</item>
    ///     </list>
    ///     Default: <c>true</c>
    /// </remarks>
    public bool AutoCleanupOnStop { get; init; } = true;

    /// <summary>
    ///     Gets or sets a value indicating whether to automatically dispose devices when they are removed from the scanner's device list.
    /// </summary>
    /// <remarks>
    ///     When enabled, calling <c>Scanner.RemoveDevice(device)</c> or <c>Scanner.ClearDevices()</c> will also dispose the device instances.
    ///     Default: <c>true</c>
    /// </remarks>
    public bool AutoDisposeDevicesOnRemoval { get; init; } = true;

    /// <summary>
    ///     Gets or sets a value indicating whether to automatically dispose services when they are removed from a device's service list.
    /// </summary>
    /// <remarks>
    ///     When enabled, calling <c>Device.ClearServices()</c> will also dispose all service instances (which cascades to characteristics and descriptors).
    ///     Default: <c>true</c>
    /// </remarks>
    public bool AutoDisposeServicesOnRemoval { get; init; } = true;

    /// <summary>
    ///     Gets or sets the default timeout for operations that don't explicitly specify a timeout.
    /// </summary>
    /// <remarks>
    ///     This timeout applies to operations like:
    ///     <list type="bullet">
    ///         <item>Connect/Disconnect</item>
    ///         <item>Read/Write characteristic values</item>
    ///         <item>Service/Characteristic/Descriptor exploration</item>
    ///         <item>Start/Stop scanning or broadcasting</item>
    ///     </list>
    ///     Default: <c>30 seconds</c>
    ///     Set to <c>null</c> for no timeout (wait indefinitely).
    /// </remarks>
    public TimeSpan? DefaultOperationTimeout { get; init; } = TimeSpan.FromSeconds(30);

    /// <summary>
    ///     Gets or sets the maximum number of concurrent connection attempts allowed.
    /// </summary>
    /// <remarks>
    ///     Limits how many devices can be connecting simultaneously to prevent resource exhaustion.
    ///     Default: <c>5</c>
    ///     Set to <c>0</c> for unlimited concurrent connections (not recommended).
    /// </remarks>
    public int MaxConcurrentConnections { get; init; } = 5;

    /// <summary>
    ///     Gets or sets a value indicating whether to enable verbose logging for Bluetooth operations.
    /// </summary>
    /// <remarks>
    ///     When enabled, detailed logs will be emitted for all Bluetooth operations including:
    ///     <list type="bullet">
    ///         <item>State transitions</item>
    ///         <item>Native API calls</item>
    ///         <item>Data transfers</item>
    ///     </list>
    ///     Default: <c>false</c>
    ///     <para><b>Warning:</b> Enabling this can significantly impact performance and log size.</para>
    /// </remarks>
    public bool EnableVerboseLogging { get; init; }

    /// <summary>
    ///     Gets or sets a value indicating whether to throw exceptions on unhandled Bluetooth errors.
    /// </summary>
    /// <remarks>
    ///     When <c>false</c>, unhandled exceptions are dispatched to <see cref="BluetoothUnhandledExceptionListener" /> instead of being thrown.
    ///     Default: <c>false</c>
    /// </remarks>
    public bool ThrowOnUnhandledExceptions { get; init; }
}
