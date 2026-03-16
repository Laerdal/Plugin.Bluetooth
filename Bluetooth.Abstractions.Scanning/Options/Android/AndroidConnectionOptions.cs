using Bluetooth.Abstractions.Options;

namespace Bluetooth.Abstractions.Scanning.Options.Android;

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
    public ConnectionPriority? ConnectionPriority { get; init; }

    /// <summary>
    ///     Gets the preferred transport type for the connection.
    /// </summary>
    /// <remarks>
    ///     Maps to the transport parameter in BluetoothDevice.connectGatt().
    ///     Controls whether to use LE, BR/EDR, or automatic transport selection.
    /// </remarks>
    public TransportType? TransportType { get; init; }

    /// <summary>
    ///     Gets the preferred PHY (Physical Layer) for the connection (Android 8.0+).
    /// </summary>
    /// <remarks>
    ///     Specifies the preferred PHY for connections on Android 8.0 (API 26) and higher.
    ///     Common values: LE_1M, LE_2M, LE_CODED.
    /// </remarks>
    public PhyMode? PreferredPhy { get; init; }

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
