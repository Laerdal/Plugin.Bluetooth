using Bluetooth.Abstractions.Options;
using Bluetooth.Core.Infrastructure.Retries;
using Bluetooth.Maui.Platforms.Droid.Enums;
using Bluetooth.Maui.Platforms.Droid.Exceptions;
using Bluetooth.Maui.Platforms.Droid.Logging;
using Bluetooth.Maui.Platforms.Droid.Scanning.Factories;
using Bluetooth.Maui.Platforms.Droid.Scanning.NativeObjects;
using Bluetooth.Maui.Platforms.Droid.Tools;

using BluetoothPhy = Android.Bluetooth.BluetoothPhy;
using MultipleServicesFoundException = Bluetooth.Abstractions.Scanning.Exceptions.MultipleServicesFoundException;
using ServiceNotFoundException = Bluetooth.Abstractions.Scanning.Exceptions.ServiceNotFoundException;

namespace Bluetooth.Maui.Platforms.Droid.Scanning;

/// <summary>
///     Android implementation of a Bluetooth Low Energy remote device.
///     This class wraps Android's BluetoothDevice and BluetoothGatt, providing platform-specific
///     implementations for device connection, service discovery, and GATT operations.
/// </summary>
public class AndroidBluetoothRemoteDevice : BaseBluetoothRemoteDevice,
    BluetoothGattProxy.IBluetoothGattDelegate,
    IAsyncDisposable
{
    private BluetoothGattProxy? _bluetoothGattProxy;
    private BluetoothDevice? _nativeDevice;
    private ConnectionOptions? _connectionOptions;
    private readonly IBluetoothRemoteL2CapChannelFactory _l2CapChannelFactory;

    /// <summary>
    ///     Initializes a new instance of the <see cref="AndroidBluetoothRemoteDevice" /> class.
    /// </summary>
    /// <param name="scanner">The Bluetooth scanner associated with this device.</param>
    /// <param name="spec">The factory spec containing device information.</param>
    /// <param name="serviceFactory">The factory for creating Bluetooth services.</param>
    /// <param name="l2CapChannelFactory">The factory for creating L2CAP channels.</param>
    /// <param name="rssiToSignalStrengthConverter">Converter for RSSI to signal strength.</param>
    /// <param name="logger">An optional logger for logging device operations.</param>
    public AndroidBluetoothRemoteDevice(
        IBluetoothScanner scanner,
        IBluetoothRemoteDeviceFactory.BluetoothRemoteDeviceFactorySpec spec,
        IBluetoothRemoteServiceFactory serviceFactory,
        IBluetoothRemoteL2CapChannelFactory l2CapChannelFactory,
        IBluetoothRssiToSignalStrengthConverter rssiToSignalStrengthConverter,
        ILogger<IBluetoothRemoteDevice>? logger = null)
        : base(scanner, spec, serviceFactory, rssiToSignalStrengthConverter, logger)
    {
        ArgumentNullException.ThrowIfNull(spec);
        ArgumentNullException.ThrowIfNull(l2CapChannelFactory);
        if (spec is not AndroidBluetoothRemoteDeviceFactorySpec nativeSpec)
        {
            throw new ArgumentException(
                $"Expected spec of type {typeof(AndroidBluetoothRemoteDeviceFactorySpec)}, but got {spec.GetType()}");
        }

        _nativeDevice = nativeSpec.NativeDevice;
        _l2CapChannelFactory = l2CapChannelFactory;
    }

    /// <summary>
    ///     Gets the Android GATT proxy used for communication with the remote device.
    ///     This is initialized when connecting to the device.
    /// </summary>
    public BluetoothGattProxy? BluetoothGattProxy => _bluetoothGattProxy;

    /// <summary>
    ///     Gets the connection options used when connecting to this device.
    ///     This is set during the connection process and can be used by GATT operations.
    /// </summary>
    public ConnectionOptions? ConnectionOptions => _connectionOptions;

    /// <inheritdoc />
    public async new ValueTask DisposeAsync()
    {
        if (_bluetoothGattProxy != null)
        {
            await CastAndDispose(_bluetoothGattProxy).ConfigureAwait(false);
            _bluetoothGattProxy = null;
        }

        _nativeDevice?.Dispose();
        _nativeDevice = null;

        // Dispose AutoResetEvent to prevent memory leak
        ReliableWriteCompleted?.Dispose();

        await base.DisposeAsync().ConfigureAwait(false);

        return;

        static async ValueTask CastAndDispose(IDisposable resource)
        {
            if (resource is IAsyncDisposable resourceAsyncDisposable)
            {
                await resourceAsyncDisposable.DisposeAsync().ConfigureAwait(false);
            }
            else
            {
                resource.Dispose();
            }
        }
    }

    #region Signal Strength (RSSI)

    /// <inheritdoc />
    protected override void NativeReadSignalStrength()
    {
        if (_bluetoothGattProxy == null)
        {
            throw new InvalidOperationException("Device not connected - GATT proxy is null");
        }

        var success = _bluetoothGattProxy.BluetoothGatt.ReadRemoteRssi();
        if (!success)
        {
            throw new InvalidOperationException("Failed to initiate RSSI read operation");
        }
    }

    /// <inheritdoc />
    public void OnReadRemoteRssi(GattStatus status, int rssi)
    {
        if (status != GattStatus.Success)
        {
            OnSignalStrengthReadFailed(new AndroidNativeGattCallbackStatusException((GattCallbackStatus) status));
            return;
        }

        OnSignalStrengthRead(rssi);
    }

    #endregion

    #region MTU

    /// <inheritdoc />
    protected override ValueTask NativeRequestMtuAsync(int requestedMtu)
    {
        if (_bluetoothGattProxy == null)
        {
            throw new InvalidOperationException("Device not connected - GATT proxy is null");
        }

        if (!OperatingSystem.IsAndroidVersionAtLeast(21))
        {
            throw new PlatformNotSupportedException("MTU negotiation requires Android 5.0 (API 21) or higher");
        }

        var success = _bluetoothGattProxy.BluetoothGatt.RequestMtu(requestedMtu);
        if (!success)
        {
            throw new InvalidOperationException("Failed to initiate MTU spec");
        }

        return ValueTask.CompletedTask;
    }


    /// <inheritdoc />
    public void OnMtuChanged(GattStatus status, int mtu)
    {
        if (status != GattStatus.Success)
        {
            OnRequestMtuFailed(new AndroidNativeGattCallbackStatusException((GattCallbackStatus) status));
            return;
        }

        OnMtuChanged(mtu);
    }

    #endregion

    #region Connection Priority

    /// <inheritdoc />
    /// <seealso href="https://developer.android.com/reference/android/bluetooth/BluetoothGatt#requestConnectionPriority(int)">Android BluetoothGatt.requestConnectionPriority(int)</seealso>
    protected override ValueTask NativeRequestConnectionPriorityAsync(
        BluetoothConnectionPriority priority,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        if (_bluetoothGattProxy == null)
        {
            throw new InvalidOperationException("Device not connected - GATT proxy is null");
        }

        // Convert abstract priority to Android GattConnectionPriority using converter
        var androidPriority = priority.ToAndroidGattConnectionPriority();

        var success = _bluetoothGattProxy.BluetoothGatt.RequestConnectionPriority(androidPriority);
        if (!success)
        {
            throw new InvalidOperationException($"Failed to spec connection priority: {priority}");
        }

        return ValueTask.CompletedTask;
    }

    #endregion

    #region PHY

    /// <inheritdoc />
    protected override ValueTask NativeSetPreferredPhyAsync(PhyMode txPhy, PhyMode rxPhy)
    {
        if (_bluetoothGattProxy == null)
        {
            throw new InvalidOperationException("Device not connected - GATT proxy is null");
        }

        if (!OperatingSystem.IsAndroidVersionAtLeast(26))
        {
            throw new PlatformNotSupportedException("PHY update requires Android 8.0 (API 26) or higher");
        }
        _bluetoothGattProxy.BluetoothGatt.SetPreferredPhy(txPhy.ToAndroidPhyMode(), rxPhy.ToAndroidPhyMode(), BluetoothPhyOption.NoPreferred);

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public void OnPhyRead(GattStatus status, BluetoothPhy txPhy, BluetoothPhy rxPhy)
    {
        if (status != GattStatus.Success)
        {
            // PHY read failed - this is informational, just return
            return;
        }

        OnPhyChanged(txPhy.ToSharedPhyMode(), rxPhy.ToSharedPhyMode());
    }

    /// <inheritdoc />
    public void OnPhyUpdate(GattStatus status, BluetoothPhy txPhy, BluetoothPhy rxPhy)
    {
        if (status != GattStatus.Success)
        {
            OnSetPreferredPhyFailed(new AndroidNativeGattCallbackStatusException((GattCallbackStatus) status));
            return;
        }

        OnPhyChanged(txPhy.ToSharedPhyMode(), rxPhy.ToSharedPhyMode());
    }

    #endregion

    #region L2CAP

    /// <inheritdoc />
    protected override async ValueTask NativeOpenL2CapChannelAsync(int psm)
    {
        if (!OperatingSystem.IsAndroidVersionAtLeast(29))
        {
            throw new PlatformNotSupportedException(
                "L2CAP channels require Android 10+ (API 29). Current version does not support this feature.");
        }

        if (_nativeDevice == null)
        {
            throw new InvalidOperationException("Native device is null");
        }

        // Create channel using factory
        var spec = new AndroidBluetoothRemoteL2CapChannelFactorySpec(psm, _nativeDevice);
        var channel = _l2CapChannelFactory.Create(this, spec);

        // Open the channel which will trigger the callback
        _ = Task.Run(async () =>
        {
            try
            {
                await channel.OpenAsync().ConfigureAwait(false);
                OnL2CapChannelOpened(channel);
            }
            catch (Exception e)
            {
                OnOpenL2CapChannelFailed(e);
                await channel.CloseAsync().ConfigureAwait(false);
                await channel.DisposeAsync().ConfigureAwait(false);
            }
        });
    }

    #endregion

    #region Connection

    /// <inheritdoc />
    protected override void NativeRefreshIsConnected()
    {
        if (_bluetoothGattProxy == null)
        {
            IsConnected = false;
            return;
        }

        // On Android, we rely on connection state callbacks
        // The IsConnected property is updated via OnConnectionStateChange
    }

    /// <inheritdoc />
    /// <seealso href="https://developer.android.com/reference/android/bluetooth/BluetoothGatt#connect()">Android BluetoothGatt.connect()</seealso>
    protected override async ValueTask NativeConnectAsync(
        ConnectionOptions connectionOptions,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(connectionOptions);

        // Store connection options for GATT operations
        _connectionOptions = connectionOptions;

        NativeRefreshIsConnected();

        Logger?.LogConnecting(Id);

        var retryOptions = connectionOptions.ConnectionRetry ?? RetryOptions.None;
        var attempt = 0;

        try
        {
            if (retryOptions.MaxRetries > 0)
            {
                await RetryTools.RunWithRetriesAsync(
                    async () =>
                    {
                        attempt++;
                        try
                        {
                            await ConnectInternalAsync(connectionOptions, cancellationToken).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            if (attempt < retryOptions.MaxRetries)
                            {
                                Logger?.LogConnectionRetry(attempt, retryOptions.MaxRetries, Id, ex);
                            }
                            throw;
                        }
                    },
                    retryOptions,
                    cancellationToken
                ).ConfigureAwait(false);
            }
            else
            {
                await ConnectInternalAsync(connectionOptions, cancellationToken).ConfigureAwait(false);
            }

            Logger?.LogConnected(Id);

            // Auto-apply ConnectionPriority after successful connection if specified
            if (connectionOptions.Android?.ConnectionPriority.HasValue == true)
            {
                try
                {
                    await RequestConnectionPriorityAsync(
                        connectionOptions.Android.ConnectionPriority.Value,
                        timeout,
                        cancellationToken
                    ).ConfigureAwait(false);

                    Logger?.LogConnectionPriorityApplied(
                        connectionOptions.Android.ConnectionPriority.Value,
                        Id);
                }
                catch (Exception ex)
                {
                    // Log but don't fail the connection if priority spec fails
                    Logger?.LogConnectionPriorityFailed(
                        connectionOptions.Android.ConnectionPriority.Value,
                        Id,
                        ex);
                }
            }
        }
        catch (Exception e)
        {
            Logger?.LogConnectionFailed(Id, Math.Max(attempt, 1), e);
            OnConnectFailed(e);
            throw;
        }
    }

    /// <summary>
    ///     Internal method that performs the actual connection to the device.
    ///     Separated from NativeConnectAsync to support retry logic.
    /// </summary>
    private async Task ConnectInternalAsync(ConnectionOptions connectionOptions, CancellationToken cancellationToken)
    {
        // Get native device if not already available
        if (_nativeDevice == null)
        {
            var androidScanner = (AndroidBluetoothScanner) Scanner;
            var androidAdapter = (AndroidBluetoothAdapter) androidScanner.Adapter;
            _nativeDevice = androidAdapter.NativeBluetoothAdapter.GetRemoteDevice(Id);
            if (_nativeDevice == null)
            {
                throw new InvalidOperationException($"Failed to get native device for address: {Id}");
            }
        }

        // Convert to Droid-specific connectionOptions which adds PreferredPhy property
        var droidConnectionOptions = new Options.ConnectionOptions
        {
            // Copy properties from abstract ConnectionOptions
            PermissionStrategy = connectionOptions.PermissionStrategy,
            WaitForAdvertisementBeforeConnecting = connectionOptions.WaitForAdvertisementBeforeConnecting,
            Apple = connectionOptions.Apple,
            Android = connectionOptions.Android,
            Windows = connectionOptions.Windows,

            // Add Droid-specific PreferredPhy (extracted from Android sub-options)
            PreferredPhy = connectionOptions.Android?.PreferredPhy as BluetoothPhy?
        };

        // Create GATT connection
        _bluetoothGattProxy = new BluetoothGattProxy(this, droidConnectionOptions, _nativeDevice);

        await Task.CompletedTask.ConfigureAwait(false);
    }

    /// <inheritdoc />
    /// <seealso href="https://developer.android.com/reference/android/bluetooth/BluetoothGatt#disconnect()">Android BluetoothGatt.disconnect()</seealso>
    protected override async ValueTask NativeDisconnectAsync(
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        NativeRefreshIsConnected();

        Logger?.LogDisconnecting(Id);

        try
        {
            if (_bluetoothGattProxy != null)
            {
                _bluetoothGattProxy.BluetoothGatt.Disconnect();
                // Note: OnConnectionStateChange callback will handle setting IsConnected = false
                Logger?.LogDisconnected(Id);
            }
        }
        catch (Exception e)
        {
            Logger?.LogDisconnectError(Id, e);
            OnDisconnect(e);
            throw;
        }
    }

    /// <summary>
    ///     Gets or sets the current connection state of the device.
    ///     This is updated based on Android's GATT connection state changes.
    /// </summary>
    public ProfileState CurrentConnectionState
    {
        get => GetValue(ProfileState.Disconnected);
        private set => SetValue(value);
    }

    /// <inheritdoc />
    public void OnConnectionStateChange(GattStatus status, ProfileState newState)
    {
        CurrentConnectionState = newState;
        if (status != GattStatus.Success)
        {
            // Connection failed
            OnConnectFailed(new AndroidNativeGattCallbackStatusException((GattCallbackStatus) status));
            return;
        }

        switch (newState)
        {
            case ProfileState.Connected:
                IsConnected = true;
                OnConnectSucceeded();
                break;

            case ProfileState.Disconnected:
                IsConnected = false;
                OnDisconnect();
                break;

            case ProfileState.Connecting:
                // Transitional state, no action needed
                break;

            case ProfileState.Disconnecting:
                // Transitional state, no action needed
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, "Unknown profile state");
        }
    }


    #endregion

    #region Service Discovery

    /// <inheritdoc />
    /// <seealso href="https://developer.android.com/reference/android/bluetooth/BluetoothGatt#discoverServices()">Android BluetoothGatt.discoverServices()</seealso>
    protected override async ValueTask NativeServicesExplorationAsync(
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        if (_bluetoothGattProxy == null)
        {
            throw new AndroidNativeBluetoothException("Device not connected - GATT proxy is null");
        }

        Logger?.LogServiceDiscoveryStarting(Id);

        var retryOptions = _connectionOptions?.Android?.ServiceDiscoveryRetry ?? RetryOptions.None;
        var attempt = 0;

        try
        {
            if (retryOptions.MaxRetries > 0)
            {
                await RetryTools.RunWithRetriesAsync(
                    () =>
                    {
                        attempt++;
                        try
                        {
                            return DiscoverServicesInternal();
                        }
                        catch (Exception ex)
                        {
                            if (attempt < retryOptions.MaxRetries)
                            {
                                Logger?.LogServiceDiscoveryRetry(attempt, retryOptions.MaxRetries, Id, ex);
                            }
                            throw;
                        }
                    },
                    retryOptions,
                    cancellationToken
                ).ConfigureAwait(false);
            }
            else
            {
                await DiscoverServicesInternal().ConfigureAwait(false);
            }

            // Log success in OnServicesDiscovered callback where we know the count
        }
        catch (AggregateException ex)
        {
            Logger?.LogServiceDiscoveryFailed(Id, Math.Max(attempt, 1), ex);
            throw;
        }
    }

    /// <summary>
    ///     Internal method that initiates service discovery.
    ///     Separated from NativeServicesExplorationAsync to support retry logic.
    /// </summary>
    private Task DiscoverServicesInternal()
    {
        if (_bluetoothGattProxy == null)
        {
            throw new AndroidNativeBluetoothException("Device not connected - GATT proxy is null");
        }

        var success = _bluetoothGattProxy.BluetoothGatt.DiscoverServices();
        if (!success)
        {
            throw new AndroidNativeBluetoothException("Failed to initiate service discovery");
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public BluetoothGattProxy.IBluetoothGattServiceDelegate GetService(BluetoothGattService? nativeService)
    {
        ArgumentNullException.ThrowIfNull(nativeService);
        ArgumentNullException.ThrowIfNull(nativeService.Uuid);

        var guid = nativeService.Uuid.ToGuid();
        try
        {
            var match = GetServiceOrDefault(service => AreRepresentingTheSameObject(nativeService, service));
            return match as BluetoothGattProxy.IBluetoothGattServiceDelegate
                   ?? throw new ServiceNotFoundException(this, guid);
        }
        catch (InvalidOperationException e)
        {
            var matches = GetServices(service => AreRepresentingTheSameObject(nativeService, service));
            throw new MultipleServicesFoundException(this, matches, e);
        }
    }

    private static bool AreRepresentingTheSameObject(BluetoothGattService native, IBluetoothRemoteService service)
    {
        return native.Uuid?.ToGuid().Equals(service.Id) ?? false;
    }

    /// <inheritdoc />
    public void OnServicesDiscovered(GattStatus status)
    {
        if (status != GattStatus.Success)
        {
            OnServicesExplorationFailed(new AndroidNativeGattCallbackStatusException((GattCallbackStatus) status));
            return;
        }

        try
        {
            if (_bluetoothGattProxy == null)
            {
                throw new InvalidOperationException("GATT proxy is null");
            }

            var services = _bluetoothGattProxy.BluetoothGatt.Services;
            if (services == null)
            {
                throw new InvalidOperationException("Services list is null");
            }

            Logger?.LogServiceDiscoveryCompleted(Id, services.Count);

            OnServicesExplorationSucceeded(services, AreRepresentingTheSameObject, FromInputTypeToOutputTypeConversion);
        }
        catch (Exception ex)
        {
            OnServicesExplorationFailed(ex);
        }
        return;

        IBluetoothRemoteService FromInputTypeToOutputTypeConversion(BluetoothGattService nativeService)
        {
            var spec = new AndroidBluetoothRemoteServiceFactorySpec(nativeService);
            return ServiceFactory.Create(this, spec);
        }
    }


    /// <inheritdoc />
    public void OnServiceChanged()
    {
        BluetoothGattProxy?.BluetoothGatt.DiscoverServices();
    }


    #endregion

    #region ReliableWrite
    /// <summary>
    /// Gets the auto-reset event used to signal completion of reliable write operations.
    /// </summary>
    private AutoResetEvent ReliableWriteCompleted { get; } = new AutoResetEvent(false);

    /// <summary>
    /// Called when a reliable write operation completes on the Android platform.
    /// </summary>
    /// <param name="status">The status of the reliable write operation.</param>
    /// <exception cref="AndroidNativeGattStatusException">Thrown when the status indicates an error.</exception>
    public void OnReliableWriteCompleted(GattStatus status)
    {
        AndroidNativeGattStatusException.ThrowIfNotSuccess(status);
        ReliableWriteCompleted.Set();
    }

    /// <summary>
    /// Waits for a reliable write operation to complete.
    /// </summary>
    /// <param name="timeout">The timeout for the operation.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that returns true if the operation completed within the timeout, false otherwise.</returns>
    public Task<bool> WaitForReliableWriteCompletedAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        return Task.Run(() => ReliableWriteCompleted.WaitOne(timeout), cancellationToken);
    }

    #endregion
}
