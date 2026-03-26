namespace Bluetooth.Maui;

/// <summary>
///     Unified Bluetooth scanner facade providing cross-platform inheritance and extension points.
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
///             // Custom filtering/processing
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

#if __ANDROID__
    private readonly Platforms.Droid.Scanning.AndroidBluetoothScanner _platformScanner;
#elif __IOS__ || __MACCATALYST__
    private readonly Platforms.Apple.Scanning.AppleBluetoothScanner _platformScanner;
#elif WINDOWS
    private readonly Platforms.Win.Scanning.WindowsBluetoothScanner _platformScanner;
#else
    private readonly Platforms.DotNetCore.Scanning.DotNetCoreBluetoothScanner _platformScanner;
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

#if __IOS__ || __MACCATALYST__
    /// <summary>
    ///     Initializes a new instance of the <see cref="BluetoothScanner"/> class.
    /// </summary>
    /// <param name="adapter">The Bluetooth adapter associated with this scanner.</param>
    /// <param name="rssiToSignalStrengthConverter">The converter for RSSI to signal strength.</param>
    /// <param name="ticker">The ticker for scheduling periodic refresh tasks.</param>
    /// <param name="cbCentralInitOptions">Apple-specific: CBCentralManager initialization options.</param>
    /// <param name="dispatchQueueProvider">Apple-specific: Dispatch queue provider for Core Bluetooth.</param>
    /// <param name="deviceFactory">The factory for creating platform-specific remote device instances.</param>
    /// <param name="nameProvider">Optional provider for Bluetooth device names.</param>
    /// <param name="loggerFactory">Optional logger factory for creating loggers.</param>
#elif __ANDROID__ || WINDOWS
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
#if __IOS__ || __MACCATALYST__
        IOptions<CBCentralInitOptions> cbCentralInitOptions,
        IDispatchQueueProvider dispatchQueueProvider,
        IBluetoothRemoteDeviceFactory deviceFactory,
#elif __ANDROID__ || WINDOWS
        IBluetoothRemoteDeviceFactory deviceFactory,
#endif
        IBluetoothNameProvider? nameProvider = null,
        ILoggerFactory? loggerFactory = null)
    {
        // Create platform-specific scanner instance
#if __ANDROID__
        _platformScanner = new Platforms.Droid.Scanning.AndroidBluetoothScanner(
            adapter,
            rssiToSignalStrengthConverter,
            ticker,
            deviceFactory,
            nameProvider,
            loggerFactory);
#elif __IOS__ || __MACCATALYST__
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
        _platformScanner = new Platforms.Win.Scanning.WindowsBluetoothScanner(
            adapter,
            rssiToSignalStrengthConverter,
            ticker,
            deviceFactory,
            nameProvider,
            loggerFactory);
#else
        _platformScanner = new Platforms.DotNetCore.Scanning.DotNetCoreBluetoothScanner(
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
    /// <remarks>
    ///     <para>
    ///         This method is called on the platform's native thread/dispatcher.
    ///         Use this to filter, transform, or react to advertisements before they're
    ///         processed by the base scanner logic.
    ///     </para>
    ///     <para>
    ///         <b>Example Use Cases:</b>
    ///     </para>
    ///     <list type="bullet">
    ///         <item>Filter advertisements by manufacturer data</item>
    ///         <item>Parse custom advertisement payloads</item>
    ///         <item>Log specific device discoveries</item>
    ///         <item>Trigger custom events or notifications</item>
    ///     </list>
    /// </remarks>
    /// <example>
    /// <code>
    /// protected override void OnAdvertisementReceived(IBluetoothAdvertisement advertisement)
    /// {
    ///     if (advertisement.DeviceName?.StartsWith("MyDevice") == true)
    ///     {
    ///         Logger?.LogInformation("Found target device: {Name}", advertisement.DeviceName);
    ///     }
    ///     base.OnAdvertisementReceived(advertisement);
    /// }
    /// </code>
    /// </example>
    protected virtual void OnAdvertisementReceived(IBluetoothAdvertisement advertisement)
    {
        // Default: no-op, clients can override
    }

    /// <summary>
    ///     Virtual method called when a new device is detected and about to be added to the device list.
    ///     Override this to create custom device types based on advertisement data.
    /// </summary>
    /// <param name="device">The device created by the platform scanner.</param>
    /// <param name="advertisement">The advertisement that triggered device creation.</param>
    /// <returns>
    ///     The device to add to the list. Return the original device, a wrapped device,
    ///     or a completely custom device implementation. Return null to prevent the device from being added.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         This method enables custom device factory patterns where you can:
    ///     </para>
    ///     <list type="bullet">
    ///         <item>Parse manufacturer data to identify device types</item>
    ///         <item>Create typed device subclasses (e.g., SimManDevice, HeartStartDevice)</item>
    ///         <item>Pre-configure expected services based on device type</item>
    ///         <item>Add device-specific behavior or metadata</item>
    ///         <item>Filter out devices by returning null</item>
    ///     </list>
    ///     <para>
    ///         <b>Important:</b> The returned device must have the same Id as the original.
    ///     </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// protected override IBluetoothRemoteDevice? OnDeviceCreated(
    ///     IBluetoothRemoteDevice device,
    ///     IBluetoothAdvertisement advertisement)
    /// {
    ///     // Parse manufacturer data to determine device type
    ///     var manufacturerData = advertisement.ManufacturerData;
    ///     if (manufacturerData != null &amp;&amp; manufacturerData.Length > 0)
    ///     {
    ///         var deviceType = manufacturerData[0]; // First byte indicates device type
    ///
    ///         return deviceType switch
    ///         {
    ///             0x01 => new SimManDevice(device), // Wrap with SimMan-specific behavior
    ///             0x02 => new HeartStartDevice(device), // Wrap with HeartStart-specific behavior
    ///             _ => device // Unknown type, use default
    ///         };
    ///     }
    ///
    ///     return device;
    /// }
    /// </code>
    /// </example>
    protected virtual IBluetoothRemoteDevice? OnDeviceCreated(
        IBluetoothRemoteDevice device,
        IBluetoothAdvertisement advertisement)
    {
        // Default: return device unmodified
        return device;
    }

    /// <summary>
    ///     Virtual method called before scanning starts.
    ///     Override this in client projects to add custom pre-scan logic.
    /// </summary>
    /// <param name="options">The scanning options being used.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    ///     <para>
    ///         This method is called after permissions are checked but before the native
    ///         platform scanner starts. Use this to perform setup, validation, or logging.
    ///     </para>
    ///     <para>
    ///         <b>Example Use Cases:</b>
    ///     </para>
    ///     <list type="bullet">
    ///         <item>Configure platform-specific scan settings</item>
    ///         <item>Initialize custom state or caches</item>
    ///         <item>Validate scanning prerequisites</item>
    ///         <item>Start performance monitoring</item>
    ///     </list>
    /// </remarks>
    protected virtual ValueTask OnScanStartingAsync(
        ScanningOptions? options,
        CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

    /// <summary>
    ///     Virtual method called after scanning stops.
    ///     Override this in client projects to add custom post-scan cleanup logic.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    ///     <para>
    ///         This method is called after the native platform scanner has stopped.
    ///         Use this to perform cleanup, logging, or final processing.
    ///     </para>
    ///     <para>
    ///         <b>Example Use Cases:</b>
    ///     </para>
    ///     <list type="bullet">
    ///         <item>Flush cached data</item>
    ///         <item>Log scan statistics</item>
    ///         <item>Clean up resources</item>
    ///         <item>Stop performance monitoring</item>
    ///     </list>
    /// </remarks>
    protected virtual ValueTask OnScanStoppedAsync(CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

    #endregion

    #region IBluetoothScanner Implementation - Properties

    /// <inheritdoc />
    public bool IsRunning => _platformScanner.IsRunning;

    /// <inheritdoc />
    public bool IsStarting => _platformScanner.IsStarting;

    /// <inheritdoc />
    public bool IsStopping => _platformScanner.IsStopping;

    /// <inheritdoc />
    public Func<IBluetoothAdvertisement, bool>? AdvertisementFilter
    {
        get => _platformScanner.AdvertisementFilter;
        set => _platformScanner.AdvertisementFilter = value;
    }

    #endregion

    #region IBluetoothScanner Implementation - Events

    /// <inheritdoc />
    public event EventHandler<AdvertisementReceivedEventArgs>? AdvertisementReceived;

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
    public event EventHandler<DeviceListChangedEventArgs>? DeviceListChanged;

    /// <inheritdoc />
    public event EventHandler<DevicesAddedEventArgs>? DevicesAdded;

    /// <inheritdoc />
    public event EventHandler<DevicesRemovedEventArgs>? DevicesRemoved;

    #endregion

    #region IBluetoothScanner Implementation - Scanning Control

    /// <inheritdoc />
    public async Task StartScanningAsync(
        ScanningOptions? scanningOptions = null,
        Abstractions.Scanning.Options.PermissionOptions? permissionOptions = null,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        await OnScanStartingAsync(scanningOptions, cancellationToken).ConfigureAwait(false);
        await _platformScanner.StartScanningAsync(
            scanningOptions,
            permissionOptions,
            timeout,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask StartScanningIfNeededAsync(
        ScanningOptions? scanningOptions = null,
        Abstractions.Scanning.Options.PermissionOptions? permissionOptions = null,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        await _platformScanner.StartScanningIfNeededAsync(
            scanningOptions,
            permissionOptions,
            timeout,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task StopScanningAsync(
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        await _platformScanner.StopScanningAsync(timeout, cancellationToken).ConfigureAwait(false);
        await OnScanStoppedAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask StopScanningIfNeededAsync(
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        await _platformScanner.StopScanningIfNeededAsync(timeout, cancellationToken).ConfigureAwait(false);
    }

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
        return _platformScanner.RequestScannerPermissionsAsync(
            requireBackgroundLocation,
            cancellationToken);
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
    public IReadOnlyList<IBluetoothRemoteDevice> GetDevices(
        Func<IBluetoothRemoteDevice, bool>? filter = null)
    {
        return _platformScanner.GetDevices(filter);
    }

    /// <inheritdoc />
    public ValueTask<IBluetoothRemoteDevice> WaitForDeviceToAppearAsync(
        string id,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        return _platformScanner.WaitForDeviceToAppearAsync(id, timeout, cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask<IBluetoothRemoteDevice> WaitForDeviceToAppearAsync(
        Func<IBluetoothRemoteDevice, bool>? filter = null,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        return _platformScanner.WaitForDeviceToAppearAsync(filter, timeout, cancellationToken);
    }

    /// <inheritdoc />
    public IBluetoothRemoteDevice? GetClosestDeviceOrDefault(
        Func<IBluetoothRemoteDevice, bool>? filter = null)
    {
        return _platformScanner.GetClosestDeviceOrDefault(filter);
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
    public ValueTask ClearDevicesAsync(
        IEnumerable<IBluetoothRemoteDevice>? devices = null)
    {
        return _platformScanner.ClearDevicesAsync(devices);
    }

    /// <inheritdoc />
    public ValueTask ClearDeviceAsync(IBluetoothRemoteDevice? device)
    {
        return _platformScanner.ClearDeviceAsync(device);
    }

    /// <inheritdoc />
    public ValueTask ClearDeviceAsync(string deviceId)
    {
        return _platformScanner.ClearDeviceAsync(deviceId);
    }

    #endregion

    #region IAsyncDisposable

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_platformScanner is IAsyncDisposable disposable)
        {
            await disposable.DisposeAsync().ConfigureAwait(false);
        }
    }

    #endregion
}
