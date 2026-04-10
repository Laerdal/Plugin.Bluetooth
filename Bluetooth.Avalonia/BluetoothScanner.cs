namespace Bluetooth.Avalonia;

/// <summary>
///     Unified Bluetooth scanner facade providing cross-platform inheritance and extension points
///     for Avalonia applications.
/// </summary>
/// <remarks>
///     <para>
///         This class wraps platform-specific scanner implementations and provides a unified
///         API with virtual extension points for client customization.
///     </para>
///     <para>
///         <b>Client Extensibility:</b> Inherit from this class to add custom scanning logic:
///     </para>
///     <example>
///     <code>
///     public class MyCustomScanner : BluetoothScanner
///     {
///         public MyCustomScanner(...) : base(...) { }
///
///         protected override void OnAdvertisementReceived(IBluetoothAdvertisement advertisement)
///         {
///             if (advertisement.DeviceName?.StartsWith("MyDevice") == true)
///             {
///                 // Handle specific devices
///             }
///             base.OnAdvertisementReceived(advertisement);
///         }
///     }
///     </code>
///     </example>
/// </remarks>
public class BluetoothScanner : IBluetoothScanner, IAsyncDisposable
{
    #region Platform-Specific Scanner

#if ANDROID
    private readonly Platforms.Android.Scanning.AndroidBluetoothScanner _platformScanner;
#elif IOS || MACOS
    private readonly Platforms.Apple.Scanning.AppleBluetoothScanner _platformScanner;
#elif WINDOWS
    private readonly Platforms.Windows.Scanning.WindowsBluetoothScanner _platformScanner;
#else
    private readonly Platforms.Linux.Scanning.LinuxBluetoothScanner _platformScanner;
#endif

    /// <summary>
    ///     Gets the underlying platform-specific scanner implementation.
    /// </summary>
    /// <remarks>
    ///     Clients can access platform-specific APIs by casting this property using conditional compilation.
    /// </remarks>
    public IBluetoothScanner PlatformScanner => _platformScanner;

    #endregion

    #region Constructor

#if IOS || MACOS
    /// <summary>
    ///     Initializes a new instance of the <see cref="BluetoothScanner"/> class.
    /// </summary>
    /// <param name="adapter">The Bluetooth adapter associated with this scanner.</param>
    /// <param name="rssiToSignalStrengthConverter">The converter for RSSI to signal strength.</param>
    /// <param name="ticker">The ticker for scheduling periodic refresh tasks.</param>
    /// <param name="cbCentralInitOptions">Apple-specific: CBCentralManager initialization options (iOS/macOS only).</param>
    /// <param name="dispatchQueueProvider">Apple-specific: Dispatch queue provider for Core Bluetooth (iOS/macOS only).</param>
    /// <param name="deviceFactory">The factory for creating platform-specific remote device instances.</param>
    /// <param name="nameProvider">Optional provider for Bluetooth device names.</param>
    /// <param name="loggerFactory">Optional logger factory for creating loggers.</param>
#elif ANDROID || WINDOWS
    /// <summary>
    ///     Initializes a new instance of the <see cref="BluetoothScanner"/> class.
    /// </summary>
    /// <param name="adapter">The Bluetooth adapter associated with this scanner.</param>
    /// <param name="rssiToSignalStrengthConverter">The converter for RSSI to signal strength.</param>
    /// <param name="ticker">The ticker for scheduling periodic refresh tasks.</param>
    /// <param name="deviceFactory">The factory for creating platform-specific remote device instances.</param>
    /// <param name="nameProvider">Optional provider for Bluetooth device names.</param>
    /// <param name="loggerFactory">Optional logger factory for creating loggers.</param>
#else
    /// <summary>
    ///     Initializes a new instance of the <see cref="BluetoothScanner"/> class.
    /// </summary>
    /// <param name="adapter">The Bluetooth adapter associated with this scanner.</param>
    /// <param name="rssiToSignalStrengthConverter">The converter for RSSI to signal strength.</param>
    /// <param name="ticker">The ticker for scheduling periodic refresh tasks.</param>
    /// <param name="nameProvider">Optional provider for Bluetooth device names.</param>
    /// <param name="loggerFactory">Optional logger factory for creating loggers.</param>
#endif
    [ActivatorUtilitiesConstructor]
    public BluetoothScanner(
        IBluetoothAdapter adapter,
        IBluetoothRssiToSignalStrengthConverter rssiToSignalStrengthConverter,
        ITicker ticker,
#if IOS || MACOS
        IOptions<Platforms.Apple.NativeObjects.CbCentralInitOptions> cbCentralInitOptions,
        Platforms.Apple.IDispatchQueueProvider dispatchQueueProvider,
        IBluetoothRemoteDeviceFactory deviceFactory,
#elif ANDROID || WINDOWS
        IBluetoothRemoteDeviceFactory deviceFactory,
#endif
        IBluetoothNameProvider? nameProvider = null,
        ILoggerFactory? loggerFactory = null)
    {
#if ANDROID
        _platformScanner = new Platforms.Android.Scanning.AndroidBluetoothScanner(
            adapter,
            rssiToSignalStrengthConverter,
            ticker,
            deviceFactory,
            nameProvider,
            loggerFactory);
#elif IOS || MACOS
        _platformScanner = new Platforms.Apple.Scanning.AppleBluetoothScanner(
            adapter,
            rssiToSignalStrengthConverter,
            ticker,
            cbCentralInitOptions,
            dispatchQueueProvider,
            deviceFactory,
            nameProvider,
            loggerFactory);
#elif WINDOWS
        _platformScanner = new Platforms.Windows.Scanning.WindowsBluetoothScanner(
            adapter,
            rssiToSignalStrengthConverter,
            ticker,
            deviceFactory,
            nameProvider,
            loggerFactory);
#else
        _platformScanner = new Platforms.Linux.Scanning.LinuxBluetoothScanner(
            adapter,
            rssiToSignalStrengthConverter,
            ticker,
            nameProvider,
            loggerFactory);
#endif

        // Hook up events from platform scanner to this facade
        _platformScanner.AdvertisementReceived += (s, e) =>
        {
            OnAdvertisementReceived(e.Advertisement);
            AdvertisementReceived?.Invoke(this, e);
        };
        _platformScanner.RunningStateChanged += (s, e) => RunningStateChanged?.Invoke(this, e);
        _platformScanner.Starting += (s, e) => Starting?.Invoke(this, e);
        _platformScanner.Started += (s, e) => Started?.Invoke(this, e);
        _platformScanner.Stopping += (s, e) => Stopping?.Invoke(this, e);
        _platformScanner.Stopped += (s, e) => Stopped?.Invoke(this, e);
        _platformScanner.DeviceListChanged += (s, e) => DeviceListChanged?.Invoke(this, e);
        _platformScanner.DevicesAdded += (s, e) => DevicesAdded?.Invoke(this, e);
        _platformScanner.DevicesRemoved += (s, e) => DevicesRemoved?.Invoke(this, e);
    }

    #endregion

    #region Extension Points for Client Projects

    /// <summary>
    ///     Virtual method called when an advertisement is received.
    ///     Override this in client projects to add custom advertisement processing logic.
    /// </summary>
    /// <param name="advertisement">The received Bluetooth advertisement.</param>
    protected virtual void OnAdvertisementReceived(IBluetoothAdvertisement advertisement)
    {
        // Default: no-op, clients can override
    }

    #endregion

    #region IBluetoothScanner Implementation - Properties

    /// <inheritdoc />
    public IBluetoothAdapter Adapter => _platformScanner.Adapter;

    /// <inheritdoc />
    public ILoggerFactory? LoggerFactory => _platformScanner.LoggerFactory;

    /// <inheritdoc />
    public bool IsRunning => _platformScanner.IsRunning;

    /// <inheritdoc />
    public bool IsStarting => _platformScanner.IsStarting;

    /// <inheritdoc />
    public bool IsStopping => _platformScanner.IsStopping;

    #endregion

    #region IBluetoothScanner Implementation - Events

    /// <inheritdoc />
    public event EventHandler? RunningStateChanged;

    /// <inheritdoc />
    public event EventHandler? Starting;

    /// <inheritdoc />
    public event EventHandler? Started;

    /// <inheritdoc />
    public event EventHandler? Stopping;

    /// <inheritdoc />
    public event EventHandler? Stopped;

    /// <inheritdoc />
    public event EventHandler<AdvertisementReceivedEventArgs>? AdvertisementReceived;

    /// <inheritdoc />
    public event EventHandler<DeviceListChangedEventArgs>? DeviceListChanged;

    /// <inheritdoc />
    public event EventHandler<DevicesAddedEventArgs>? DevicesAdded;

    /// <inheritdoc />
    public event EventHandler<DevicesRemovedEventArgs>? DevicesRemoved;

    #endregion

    #region IBluetoothScanner Implementation - Permissions

    /// <inheritdoc />
    public ValueTask<bool> HasScannerPermissionsAsync()
    {
        return _platformScanner.HasScannerPermissionsAsync();
    }

    /// <inheritdoc />
    public ValueTask RequestScannerPermissionsAsync(
        bool requireBackgroundLocation = false,
        CancellationToken cancellationToken = default)
    {
        return _platformScanner.RequestScannerPermissionsAsync(requireBackgroundLocation, cancellationToken);
    }

    #endregion

    #region IBluetoothScanner Implementation - Scanning Control

    /// <inheritdoc />
    public ValueTask StartScanningAsync(
        ScanningOptions? scanningOptions = null,
        ScanningPermissionOptions? permissionOptions = null,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        return _platformScanner.StartScanningAsync(scanningOptions, permissionOptions, timeout, cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask StartScanningIfNeededAsync(
        ScanningOptions? scanningOptions = null,
        ScanningPermissionOptions? permissionOptions = null,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        return _platformScanner.StartScanningIfNeededAsync(scanningOptions, permissionOptions, timeout, cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask StopScanningAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        return _platformScanner.StopScanningAsync(timeout, cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask StopScanningIfNeededAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        return _platformScanner.StopScanningIfNeededAsync(timeout, cancellationToken);
    }

    #endregion

    #region IBluetoothScanner Implementation - Device Management - Get

    /// <inheritdoc />
    public IBluetoothRemoteDevice GetDevice(Func<IBluetoothRemoteDevice, bool> filter)
    {
        return _platformScanner.GetDevice(filter);
    }

    /// <inheritdoc />
    public IBluetoothRemoteDevice GetDevice(string id)
    {
        return _platformScanner.GetDevice(id);
    }

    /// <inheritdoc />
    public IBluetoothRemoteDevice? GetDeviceOrDefault(Func<IBluetoothRemoteDevice, bool> filter)
    {
        return _platformScanner.GetDeviceOrDefault(filter);
    }

    /// <inheritdoc />
    public IBluetoothRemoteDevice? GetDeviceOrDefault(string id)
    {
        return _platformScanner.GetDeviceOrDefault(id);
    }

    /// <inheritdoc />
    public IReadOnlyList<IBluetoothRemoteDevice> GetDevices(Func<IBluetoothRemoteDevice, bool>? filter = null)
    {
        return _platformScanner.GetDevices(filter);
    }

    #endregion

    #region IBluetoothScanner Implementation - Device Management - Has

    /// <inheritdoc />
    public bool HasDevice(Func<IBluetoothRemoteDevice, bool> filter)
    {
        return _platformScanner.HasDevice(filter);
    }

    /// <inheritdoc />
    public bool HasDevice(string id)
    {
        return _platformScanner.HasDevice(id);
    }

    #endregion

    #region IBluetoothScanner Implementation - Device Management - Clear

    /// <inheritdoc />
    public ValueTask ClearDevicesAsync()
    {
        return _platformScanner.ClearDevicesAsync();
    }

    #endregion

    #region IAsyncDisposable

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await _platformScanner.DisposeAsync().ConfigureAwait(false);
    }

    #endregion
}
