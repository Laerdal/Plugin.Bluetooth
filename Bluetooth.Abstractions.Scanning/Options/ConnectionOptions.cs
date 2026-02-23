using Bluetooth.Abstractions.Options;

namespace Bluetooth.Abstractions.Scanning.Options;

/// <summary>
///     Represents Bluetooth device connection options.
/// </summary>
public record ConnectionOptions
{
    #region Permission Handling

    /// <summary>
    ///     Gets the permission request strategy for this connection operation.
    /// </summary>
    /// <remarks>
    ///     Defaults to <see cref="PermissionRequestStrategy.RequestAutomatically"/> which automatically
    ///     requests BLUETOOTH_CONNECT permission (Android 12+) before connecting if not already granted.
    /// </remarks>
    public PermissionRequestStrategy PermissionStrategy { get; init; } = PermissionRequestStrategy.RequestAutomatically;

    #endregion

    /// <summary>
    ///     Gets a value indicating whether to wait for an advertisement before connecting to the device.
    /// </summary>
    public bool WaitForAdvertisementBeforeConnecting { get; init; }

    #region Retry Configuration

    /// <summary>
    ///     Gets the retry configuration for device connection operations.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Retry configuration applied when initial connection attempts fail due to transient issues.
    ///         Critical for Android GATT error 133 (connection failures), which is common and often
    ///         resolves with retry.
    ///     </para>
    ///     <b>Platform Support:</b>
    ///     <list type="bullet">
    ///         <item><b>Android</b>: Retries critical for GATT error 133 (connection failures)</item>
    ///         <item><b>iOS/macOS</b>: Retries on connection timeout or peripheral unavailable</item>
    ///         <item><b>Windows</b>: Retries on device connection failures</item>
    ///     </list>
    ///     <para>
    ///         Defaults to <see cref="RetryOptions.Default"/> (3 retries with 200ms delay).
    ///         Set to <see cref="RetryOptions.None"/> to disable retry logic.
    ///     </para>
    /// </remarks>
    public RetryOptions? ConnectionRetry { get; init; } = RetryOptions.Default;

    #endregion

    #region Platform-Specific Options

    /// <summary>
    ///     Gets the Apple platform-specific connection options.
    /// </summary>
    /// <remarks>
    ///     These options are only used on iOS/macOS platforms and are ignored on other platforms.
    /// </remarks>
    public AppleConnectionOptions? Apple { get; init; }

    /// <summary>
    ///     Gets the Android platform-specific connection options.
    /// </summary>
    /// <remarks>
    ///     These options are only used on Android platforms and are ignored on other platforms.
    /// </remarks>
    public AndroidConnectionOptions? Android { get; init; }

    /// <summary>
    ///     Gets the Windows platform-specific connection options.
    /// </summary>
    /// <remarks>
    ///     These options are only used on Windows platforms and are ignored on other platforms.
    /// </remarks>
    public WindowsConnectionOptions? Windows { get; init; }

    #endregion
}

/// <summary>
///     Defines connection priority modes that affect connection parameters and power consumption.
/// </summary>
public enum BluetoothConnectionPriority
{
    /// <summary>
    ///     Balanced mode providing reasonable performance and moderate power consumption.
    /// </summary>
    /// <remarks>
    ///     Android: CONNECTION_PRIORITY_BALANCED
    ///     Connection interval: 30-50ms, Latency: 0, Timeout: 20s
    /// </remarks>
    Balanced = 0,

    /// <summary>
    ///     High performance mode optimized for low latency at the cost of higher power consumption.
    /// </summary>
    /// <remarks>
    ///     Android: CONNECTION_PRIORITY_HIGH
    ///     Connection interval: 11.25-15ms, Latency: 0, Timeout: 20s
    ///     Best for real-time data transfer or gaming
    /// </remarks>
    High = 1,

    /// <summary>
    ///     Low power mode optimized for battery life with reduced performance.
    /// </summary>
    /// <remarks>
    ///     Android: CONNECTION_PRIORITY_LOW_POWER
    ///     Connection interval: 100-125ms, Latency: 2, Timeout: 20s
    ///     Best for infrequent data updates
    /// </remarks>
    LowPower = 2
}

/// <summary>
///     Defines the transport type for Bluetooth connections.
/// </summary>
public enum BluetoothTransportType
{
    /// <summary>
    ///     Automatically select transport based on device capabilities (default).
    /// </summary>
    /// <remarks>
    ///     Android: TRANSPORT_AUTO
    /// </remarks>
    Auto = 0,

    /// <summary>
    ///     Use Bluetooth Low Energy (LE) transport.
    /// </summary>
    /// <remarks>
    ///     Android: TRANSPORT_LE
    ///     iOS/macOS: Always used (only option available)
    /// </remarks>
    Le = 2,

    /// <summary>
    ///     Use classic Bluetooth (BR/EDR) transport.
    /// </summary>
    /// <remarks>
    ///     Android: TRANSPORT_BREDR
    ///     Not supported for BLE operations
    /// </remarks>
    BrEdr = 1
}

#region Platform-Specific Connection Options

/// <summary>
///     Apple (iOS/macOS) platform-specific connection options.
/// </summary>
public record AppleConnectionOptions
{
    /// <summary>
    ///     Gets a value indicating whether to notify on connection events.
    /// </summary>
    /// <remarks>
    ///     Corresponds to CBConnectPeripheralOptionNotifyOnConnectionKey.
    ///     When true, the system will display an alert when the peripheral connects while
    ///     the app is suspended.
    /// </remarks>
    public bool? NotifyOnConnection { get; init; }

    /// <summary>
    ///     Gets a value indicating whether to notify on disconnection events.
    /// </summary>
    /// <remarks>
    ///     Corresponds to CBConnectPeripheralOptionNotifyOnDisconnectionKey.
    ///     When true, the system will display an alert when the peripheral disconnects while
    ///     the app is suspended.
    /// </remarks>
    public bool? NotifyOnDisconnection { get; init; }

    /// <summary>
    ///     Gets a value indicating whether to notify on notification events.
    /// </summary>
    /// <remarks>
    ///     Corresponds to CBConnectPeripheralOptionNotifyOnNotificationKey.
    ///     When true, the system will display an alert for incoming notifications while
    ///     the app is suspended.
    /// </remarks>
    public bool? NotifyOnNotification { get; init; }

    /// <summary>
    ///     Gets a value indicating whether to enable transport bridging (iOS 13+).
    /// </summary>
    /// <remarks>
    ///     Corresponds to CBConnectPeripheralOptionEnableTransportBridgingKey.
    ///     When true, allows the peripheral to be used over both Classic and LE transports.
    ///     Available on iOS 13.0+ and tvOS 13.0+.
    /// </remarks>
    public bool? EnableTransportBridging { get; init; }

    /// <summary>
    ///     Gets a value indicating whether to require ANCS (Apple Notification Center Service) support (iOS 13+).
    /// </summary>
    /// <remarks>
    ///     Corresponds to CBConnectPeripheralOptionRequiresANCS.
    ///     When true, the connection will only succeed if the peripheral supports ANCS.
    ///     Available on iOS 13.0+ and tvOS 13.0+.
    /// </remarks>
    public bool? RequiresAncs { get; init; }
}

/// <summary>
///     Android platform-specific connection options.
/// </summary>
public record AndroidConnectionOptions
{
    /// <summary>
    ///     Gets a value indicating whether to automatically reconnect if the connection is lost.
    /// </summary>
    /// <remarks>
    ///     Maps to the autoConnect parameter in BluetoothDevice.connectGatt().
    ///     When true, the system will automatically reconnect when the device becomes available.
    ///     This may result in longer initial connection times but provides automatic reconnection.
    /// </remarks>
    public bool? AutoConnect { get; init; }

    /// <summary>
    ///     Gets the preferred connection priority/performance mode.
    /// </summary>
    /// <remarks>
    ///     Sets the preferred connection parameters via BluetoothGatt.requestConnectionPriority().
    ///     This affects latency, throughput, and power consumption.
    /// </remarks>
    public BluetoothConnectionPriority? ConnectionPriority { get; init; }

    /// <summary>
    ///     Gets the preferred transport type for the connection.
    /// </summary>
    /// <remarks>
    ///     Maps to the transport parameter in BluetoothDevice.connectGatt().
    ///     Controls whether to use LE, BR/EDR, or automatic transport selection.
    /// </remarks>
    public BluetoothTransportType? TransportType { get; init; }

    /// <summary>
    ///     Gets the preferred PHY (Physical Layer) for the connection (Android 8.0+).
    /// </summary>
    /// <remarks>
    ///     Specifies the preferred PHY for connections on Android 8.0 (API 26) and higher.
    ///     Common values: LE_1M, LE_2M, LE_CODED.
    ///     Requires explicit platform type due to native Android enum.
    /// </remarks>
    public object? PreferredPhy { get; init; } // Object type to avoid direct Android dependency in abstractions

    #region GATT Operation Retry Configuration

    /// <summary>
    ///     Gets the retry configuration for service discovery operations.
    /// </summary>
    /// <remarks>
    ///     Used when DiscoverServices fails or times out.
    ///     Android BLE stack can fail service discovery due to timing issues.
    ///     Defaults to 2 retries with 300ms delay.
    /// </remarks>
    public RetryOptions? ServiceDiscoveryRetry { get; init; } = new RetryOptions
    {
        MaxRetries = 2,
        DelayBetweenRetries = TimeSpan.FromMilliseconds(300)
    };

    /// <summary>
    ///     Gets the retry configuration for GATT write operations.
    /// </summary>
    /// <remarks>
    ///     Replaces hardcoded 3 retries with 200ms delay in AndroidBluetoothRemoteCharacteristic.
    ///     Defaults to <see cref="RetryOptions.Default"/> (3 retries with 200ms delay).
    /// </remarks>
    public RetryOptions? GattWriteRetry { get; init; } = RetryOptions.Default;

    /// <summary>
    ///     Gets the retry configuration for GATT read operations.
    /// </summary>
    /// <remarks>
    ///     Defaults to 2 retries with 100ms delay.
    /// </remarks>
    public RetryOptions? GattReadRetry { get; init; } = new RetryOptions
    {
        MaxRetries = 2,
        DelayBetweenRetries = TimeSpan.FromMilliseconds(100)
    };

    #endregion
}

/// <summary>
///     Windows platform-specific connection options.
/// </summary>
/// <remarks>
///     Windows currently does not expose connection options through the WinRT API.
///     Connection parameters are managed automatically by the Windows Bluetooth stack.
///     This class is provided for consistency and future extensibility.
/// </remarks>
public record WindowsConnectionOptions
{
    // Reserved for future Windows-specific connection options
}

#endregion
