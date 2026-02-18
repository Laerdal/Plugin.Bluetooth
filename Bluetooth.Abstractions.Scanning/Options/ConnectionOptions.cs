namespace Bluetooth.Abstractions.Scanning.Options;

/// <summary>
/// Represents Bluetooth device connection options.
/// </summary>
public record ConnectionOptions
{
    /// <summary>
    /// Gets a value indicating whether to wait for an advertisement before connecting to the device.
    /// </summary>
    public bool WaitForAdvertisementBeforeConnecting { get; init; }

    /// <summary>
    /// Gets a value indicating whether to automatically reconnect if the connection is lost.
    /// </summary>
    /// <remarks>
    /// <b>Platform Support:</b>
    /// <list type="bullet">
    /// <item><b>Android</b>: Full support via autoConnect parameter in connectGatt</item>
    /// <item><b>iOS/macOS</b>: Reconnection is application-managed, not automatic</item>
    /// <item><b>Windows</b>: Partial support, depends on device behavior</item>
    /// </list>
    /// <para>
    /// When enabled on Android, the system will automatically reconnect when the device becomes available.
    /// This may result in longer initial connection times but provides automatic reconnection.
    /// </para>
    /// </remarks>
    public bool AutoConnect { get; init; }

    /// <summary>
    /// Gets the preferred connection priority/performance mode.
    /// </summary>
    /// <remarks>
    /// <b>Platform Support:</b>
    /// <list type="bullet">
    /// <item><b>Android</b>: Full support via requestConnectionPriority</item>
    /// <item><b>iOS/macOS</b>: Connection parameters negotiated automatically</item>
    /// <item><b>Windows</b>: Connection parameters negotiated automatically</item>
    /// </list>
    /// </remarks>
    public BluetoothConnectionPriority ConnectionPriority { get; init; } = BluetoothConnectionPriority.Balanced;

    /// <summary>
    /// Gets the preferred transport type for the connection.
    /// </summary>
    /// <remarks>
    /// <b>Platform Support:</b>
    /// <list type="bullet">
    /// <item><b>Android</b>: Full support (Auto, LE, or BR/EDR)</item>
    /// <item><b>iOS/macOS</b>: Only LE supported (always uses LE transport)</item>
    /// <item><b>Windows</b>: Determined automatically based on device capabilities</item>
    /// </list>
    /// </remarks>
    public BluetoothTransportType TransportType { get; init; } = BluetoothTransportType.Auto;
}

/// <summary>
/// Defines connection priority modes that affect connection parameters and power consumption.
/// </summary>
public enum BluetoothConnectionPriority
{
    /// <summary>
    /// Balanced mode providing reasonable performance and moderate power consumption.
    /// </summary>
    /// <remarks>
    /// Android: CONNECTION_PRIORITY_BALANCED
    /// Connection interval: 30-50ms, Latency: 0, Timeout: 20s
    /// </remarks>
    Balanced = 0,

    /// <summary>
    /// High performance mode optimized for low latency at the cost of higher power consumption.
    /// </summary>
    /// <remarks>
    /// Android: CONNECTION_PRIORITY_HIGH
    /// Connection interval: 11.25-15ms, Latency: 0, Timeout: 20s
    /// Best for real-time data transfer or gaming
    /// </remarks>
    High = 1,

    /// <summary>
    /// Low power mode optimized for battery life with reduced performance.
    /// </summary>
    /// <remarks>
    /// Android: CONNECTION_PRIORITY_LOW_POWER
    /// Connection interval: 100-125ms, Latency: 2, Timeout: 20s
    /// Best for infrequent data updates
    /// </remarks>
    LowPower = 2
}

/// <summary>
/// Defines the transport type for Bluetooth connections.
/// </summary>
public enum BluetoothTransportType
{
    /// <summary>
    /// Automatically select transport based on device capabilities (default).
    /// </summary>
    /// <remarks>
    /// Android: TRANSPORT_AUTO
    /// </remarks>
    Auto = 0,

    /// <summary>
    /// Use Bluetooth Low Energy (LE) transport.
    /// </summary>
    /// <remarks>
    /// Android: TRANSPORT_LE
    /// iOS/macOS: Always used (only option available)
    /// </remarks>
    Le = 2,

    /// <summary>
    /// Use classic Bluetooth (BR/EDR) transport.
    /// </summary>
    /// <remarks>
    /// Android: TRANSPORT_BREDR
    /// Not supported for BLE operations
    /// </remarks>
    BrEdr = 1
}
