using Bluetooth.Maui.Platforms.Apple.Logging;
using Bluetooth.Maui.Platforms.Apple.Scanning.Factories;
using Bluetooth.Maui.Platforms.Apple.Scanning.NativeObjects;
using Bluetooth.Maui.Platforms.Apple.Threading;

using MultipleServicesFoundException = Bluetooth.Abstractions.Scanning.Exceptions.MultipleServicesFoundException;
using ServiceNotFoundException = Bluetooth.Abstractions.Scanning.Exceptions.ServiceNotFoundException;

namespace Bluetooth.Maui.Platforms.Apple.Scanning;

/// <inheritdoc cref="BaseBluetoothRemoteDevice" />
public class AppleBluetoothRemoteDevice : BaseBluetoothRemoteDevice, CbPeripheralWrapper.ICbPeripheralDelegate, CbCentralManagerWrapper.ICbPeripheralDelegate
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AppleBluetoothRemoteDevice" /> class from an Apple advertisement.
    /// </summary>
    /// <param name="parentScanner">The Bluetooth scanner that discovered this device.</param>
    /// <param name="advertisement">The Apple-specific Bluetooth advertisement containing the Core Bluetooth peripheral.</param>
    /// <param name="signalStrengthSmoothingOptions">The options for smoothing signal strength jitter.</param>
    /// <param name="rssiToSignalStrengthConverter">The converter for RSSI to signal strength.</param>
    /// <param name="logger">An optional logger for logging device-related events and errors.</param>
    public AppleBluetoothRemoteDevice(IBluetoothScanner parentScanner,
        AppleBluetoothAdvertisement advertisement,
        SignalStrengthSmoothingOptions signalStrengthSmoothingOptions,
        IBluetoothRssiToSignalStrengthConverter rssiToSignalStrengthConverter,
        ILogger<IBluetoothRemoteDevice>? logger = null) : base(parentScanner,
                                                               advertisement,
                                                               signalStrengthSmoothingOptions,
                                                               rssiToSignalStrengthConverter,
                                                               logger)
    {
        CbPeripheralWrapper = new CbPeripheralWrapper(this, advertisement.CbPeripheral);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="AppleBluetoothRemoteDevice" /> class with the specified Core Bluetooth peripheral wrapper, parent scanner, advertisement, and logger.
    /// </summary>
    /// <param name="cbPeripheral">The native iOS Core Bluetooth peripheral represented by this remote device.</param>
    /// <param name="parentScanner">The Bluetooth scanner that discovered this device.</param>
    /// <param name="advertisement">The Bluetooth advertisement associated with this device.</param>
    /// <param name="signalStrengthSmoothingOptions">The options for smoothing signal strength jitter.</param>
    /// <param name="rssiToSignalStrengthConverter">The converter for RSSI to signal strength.</param>
    /// <param name="logger">An optional logger for logging device-related events and errors.</param>
    public AppleBluetoothRemoteDevice(CBPeripheral cbPeripheral,
        IBluetoothScanner parentScanner,
        IBluetoothAdvertisement advertisement,
        SignalStrengthSmoothingOptions signalStrengthSmoothingOptions,
        IBluetoothRssiToSignalStrengthConverter rssiToSignalStrengthConverter,
        ILogger<IBluetoothRemoteDevice>? logger = null) : base(parentScanner,
                                                               advertisement,
                                                               signalStrengthSmoothingOptions,
                                                               rssiToSignalStrengthConverter,
                                                               logger)
    {
        CbPeripheralWrapper = new CbPeripheralWrapper(this, cbPeripheral);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="AppleBluetoothRemoteDevice" /> class with the specified Core Bluetooth peripheral wrapper, parent scanner, ID, manufacturer, and logger.
    /// </summary>
    /// <param name="cbPeripheral">The native iOS Core Bluetooth peripheral represented by this remote device.</param>
    /// <param name="parentScanner">The Bluetooth scanner that discovered this device.</param>
    /// <param name="id">The unique identifier for this device.</param>
    /// <param name="manufacturer">The manufacturer of this device, if known.</param>
    /// <param name="signalStrengthSmoothingOptions">The options for smoothing signal strength jitter.</param>
    /// <param name="rssiToSignalStrengthConverter">The converter for RSSI to signal strength.</param>
    /// <param name="logger">An optional logger for logging device-related events and errors.</param>
    public AppleBluetoothRemoteDevice(CBPeripheral cbPeripheral,
        IBluetoothScanner parentScanner,
        string id,
        Manufacturer manufacturer,
        SignalStrengthSmoothingOptions signalStrengthSmoothingOptions,
        IBluetoothRssiToSignalStrengthConverter rssiToSignalStrengthConverter,
        ILogger<IBluetoothRemoteDevice>? logger = null) : base(parentScanner,
                                                               id,
                                                               manufacturer,
                                                               signalStrengthSmoothingOptions,
                                                               rssiToSignalStrengthConverter,
                                                               logger)
    {
        CbPeripheralWrapper = new CbPeripheralWrapper(this, cbPeripheral);
    }

    /// <summary>
    ///     Initializes a new instance using a factory spec.
    /// </summary>
    /// <param name="parentScanner">The Bluetooth scanner that discovered this device.</param>
    /// <param name="spec">The Apple-specific factory spec containing the native peripheral.</param>
    /// <param name="serviceFactory">The factory for creating remote services.</param>
    /// <param name="rssiToSignalStrengthConverter">The converter for RSSI to signal strength.</param>
    /// <param name="logger">An optional logger for logging device-related events and errors.</param>
    public AppleBluetoothRemoteDevice(
        IBluetoothScanner parentScanner,
        AppleBluetoothRemoteDeviceFactorySpec spec,
        IBluetoothRemoteServiceFactory serviceFactory,
        IBluetoothRssiToSignalStrengthConverter rssiToSignalStrengthConverter,
        ILogger<IBluetoothRemoteDevice>? logger = null) : base(parentScanner, spec, serviceFactory, rssiToSignalStrengthConverter, logger)
    {
        ArgumentNullException.ThrowIfNull(spec);
        CbPeripheralWrapper = new CbPeripheralWrapper(this, spec.CbPeripheral);
    }

    /// <summary>
    ///     Gets the iOS Core Bluetooth peripheral delegate proxy used for peripheral operations.
    /// </summary>
    public CbPeripheralWrapper CbPeripheralWrapper { get; }

    /// <summary>
    ///     Gets the Bluetooth scanner that discovered this device, cast to the Apple-specific implementation.
    /// </summary>
    public AppleBluetoothScanner AppleBluetoothScanner => (AppleBluetoothScanner) Scanner;

    /// <inheritdoc />
    public void UpdatedName()
    {
        if (CbPeripheralWrapper.CbPeripheral.Name != null)
        {
            Logger?.LogDeviceNameUpdated(Id, CbPeripheralWrapper.CbPeripheral.Name);
            CachedName = CbPeripheralWrapper.CbPeripheral.Name;
        }
    }

    /// <inheritdoc />
    public void DidUpdateAncsAuthorization()
    {
        // TODO : Implement if needed. This method is called when the ANCS (Apple Notification Center Service) authorization status changes. If your application needs to interact with ANCS, you may want to handle this event to update your application's state accordingly.
        // Placeholder for future implementation if needed
    }

    /// <inheritdoc />
    protected override ValueTask NativeSetPreferredPhyAsync(PhyMode txPhy, PhyMode rxPhy)
    {
        // iOS does not allow setting preferred PHY, it is determined by the system and the connected peripheral. The current PHY can be read after a successful connection.
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    protected override ValueTask NativeRequestMtuAsync(int requestedMtu)
    {
        // iOS does not allow requesting a specific MTU, it is determined by the system and the connected peripheral. The MTU can be read after a successful connection.
        return ValueTask.CompletedTask;
    }

    #region L2Cap

    /// <inheritdoc />
    protected override ValueTask NativeOpenL2CapChannelAsync(int psm)
    {
        CbPeripheralWrapper.CbPeripheral.OpenL2CapChannel((ushort) psm);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public void DidOpenL2CapChannel(NSError? error, CBL2CapChannel? channel)
    {
        try
        {
            AppleNativeBluetoothException.ThrowIfError(error);

            if (channel == null)
            {
                throw new IOException("L2CAP channel is null");
            }

            var logger = AppleBluetoothScanner?.LoggerFactory?.CreateLogger<IBluetoothRemoteDevice>() ?? new NullLogger<IBluetoothRemoteDevice>();
#pragma warning disable CA2000 // Channel is passed to OnL2CapChannelOpened which takes ownership
            var appleChannel = new AppleBluetoothRemoteL2CapChannel(this, channel, logger);
#pragma warning restore CA2000

            OnL2CapChannelOpened(appleChannel);
        }
        catch (Exception e)
        {
            OnOpenL2CapChannelFailed(e);
        }
    }

    #endregion

    #region Connection

    /// <inheritdoc />
    protected override ValueTask NativeRefreshIsConnectedAsync(CancellationToken cancellationToken = default)
    {
        var dispatchTask = MainThreadDispatcher.InvokeOnMainThreadAsync(() => {
            IsConnected = CbPeripheralWrapper.CbPeripheral.State == CBPeripheralState.Connected;
        });

        if (cancellationToken.CanBeCanceled)
        {
            // WaitAsync(cancellationToken) below only cancels the *wait* - the dispatched action
            // keeps running regardless. Only start observing the dispatch task's own outcome once
            // cancellation actually fires: at that point the WaitAsync call has already returned
            // (with OperationCanceledException) and nothing else is left to observe a late fault on
            // it. Registering unconditionally here would double-report every ordinary (non-cancelled)
            // fault, since the awaited ValueTask below would also propagate it to its own caller.
            cancellationToken.Register(() => dispatchTask.StartAndForget(ex => BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, ex)));
        }

        return new ValueTask(dispatchTask.WaitAsync(cancellationToken));
    }

    /// <inheritdoc />
    public void ConnectionEventDidOccur(CBConnectionEvent connectionEvent)
    {
        NativeRefreshIsConnectedAsync().StartAndForget(ex => BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, ex));
    }

    #region Connect

    /// <inheritdoc />
    /// <seealso href="https://developer.apple.com/documentation/corebluetooth/cbcentralmanager/1518766-connect">iOS CBCentralManager.connect</seealso>
    protected override ValueTask NativeConnectAsync(ConnectionOptions connectionOptions, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(connectionOptions);

        Logger?.LogConnecting(Id);

        // No refresh here - this method doesn't branch on IsConnected, and the caller (Core's
        // ConnectAsync) already refreshes both before its already-connected guard and again after
        // this native call completes. Awaiting one more here would also be unbounded when this is
        // invoked from the "abandon the connect attempt" cleanup path with no timeout at all - see
        // that path's comment for why it deliberately uses CancellationToken.None.
        if (Scanner is not AppleBluetoothScanner scanner)
        {
            throw new InvalidOperationException("Scanner is not a BluetoothScanner");
        }

        // Convert abstract ConnectionOptions to Apple-specific options
        var appleOptions = new PeripheralConnectionOptions
        {
            // Read Apple-specific sub-options (or use defaults if not provided)
            NotifyOnConnection = connectionOptions.Apple?.NotifyOnConnection ?? true,
            NotifyOnDisconnection = connectionOptions.Apple?.NotifyOnDisconnection ?? true,
            NotifyOnNotification = connectionOptions.Apple?.NotifyOnNotification ?? true
        };
        scanner.CbCentralManagerWrapper.CbCentralManager.ConnectPeripheral(CbPeripheralWrapper.CbPeripheral, appleOptions);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public void FailedToConnectPeripheral(NSError? error)
    {
        // No standalone refresh here - OnConnectFailedAsync below already refreshes internally,
        // and firing a second main-thread dispatch for the same event would be pure overhead.
        try
        {
            AppleNativeBluetoothException.ThrowIfError(error);
            throw new DeviceFailedToConnectException(this, "Failed to connect to peripheral, Unknown error");
        }
        catch (Exception e)
        {
            Logger?.LogConnectionFailed(Id, 1, e);
            OnConnectFailedAsync(e).StartAndForget(ex => BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, ex));
        }
    }

    /// <inheritdoc />
    public void ConnectedPeripheral()
    {
        // No standalone refresh here - OnConnectSucceededAsync below already refreshes internally,
        // and firing a second main-thread dispatch for the same event would be pure overhead.
        Logger?.LogConnected(Id);
        OnConnectSucceededAsync().StartAndForget(ex => BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, ex));
    }

    #endregion

    #region Disconnection

    /// <inheritdoc />
    /// <seealso href="https://developer.apple.com/documentation/corebluetooth/cbcentralmanager/1518952-cancelperipheralconnection">iOS CBCentralManager.cancelPeripheralConnection</seealso>
    protected override ValueTask NativeDisconnectAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        Logger?.LogDisconnecting(Id);

        // No refresh here - see NativeConnectAsync. This is also invoked from ConnectAsync's
        // "abandon the attempt" cleanup path with timeout: null and CancellationToken.None, where
        // an unbounded refresh could hang that best-effort cleanup indefinitely.
        if (Scanner is not AppleBluetoothScanner scanner)
        {
            throw new InvalidOperationException("Scanner is not a BluetoothScanner");
        }

        scanner.CbCentralManagerWrapper.CbCentralManager.CancelPeripheralConnection(CbPeripheralWrapper.CbPeripheral);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    protected override ValueTask NativeRequestConnectionPriorityAsync(ConnectionPriority priority, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        // iOS does not allow setting connection priority, it is determined by the system and the connected peripheral. The connection priority can be read after a successful connection.
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public void DisconnectedPeripheral(NSError? error)
    {
        // No standalone refresh here - OnDisconnectAsync below already refreshes internally in
        // both branches, and firing a second main-thread dispatch for the same event would be
        // pure overhead.
        try
        {
            AppleNativeBluetoothException.ThrowIfError(error);
            Logger?.LogDisconnected(Id);
            OnDisconnectAsync().StartAndForget(ex => BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, ex));
        }
        catch (Exception e)
        {
            Logger?.LogDisconnected(Id);
            OnDisconnectAsync(e).StartAndForget(ex => BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, ex));
        }
    }

    /// <inheritdoc />
    public void DidDisconnectPeripheral(double timestamp, bool isReconnecting, NSError? error)
    {
        // No standalone refresh here - OnDisconnectAsync below already refreshes internally in
        // every branch, and firing a second main-thread dispatch for the same event would be
        // pure overhead.
        try
        {
            AppleNativeBluetoothException.ThrowIfError(error);
            if (isReconnecting)
            {
                OnDisconnectAsync(new DeviceReconnectingException(this)).StartAndForget(ex => BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, ex));
            }
            else
            {
                OnDisconnectAsync().StartAndForget(ex => BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, ex));
            }
        }
        catch (Exception e)
        {
            OnDisconnectAsync(e).StartAndForget(ex => BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, ex));
        }
    }

    #endregion

    #endregion

    #region SignalStrength

    /// <inheritdoc />
    protected override void NativeReadSignalStrength()
    {
        CbPeripheralWrapper.CbPeripheral.ReadRSSI();
    }

    /// <inheritdoc />
    public void RssiRead(NSError? error, NSNumber rssi)
    {
        try
        {
            AppleNativeBluetoothException.ThrowIfError(error);
            ArgumentNullException.ThrowIfNull(rssi);

            OnSignalStrengthRead(rssi.Int32Value);
        }
        catch (Exception e)
        {
            OnSignalStrengthReadFailed(e);
        }
    }

    /// <inheritdoc />
    public void RssiUpdated(NSError? error)
    {
        // CbPeripheralWrapper.CbPeripheral.RSSI is Obsolete in iOS 8
        if (!OperatingSystem.IsIOSVersionAtLeast(8) && CbPeripheralWrapper.CbPeripheral.RSSI != null)
        {
            RssiRead(error, CbPeripheralWrapper.CbPeripheral.RSSI);
        }
    }

    #endregion

    #region Services

    /// <inheritdoc />
    /// <seealso href="https://developer.apple.com/documentation/corebluetooth/cbperipheral/1518706-discoverservices">iOS CBPeripheral.discoverServices</seealso>
    protected override ValueTask NativeServicesExplorationAsync(bool useCache, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        Logger?.LogServiceDiscoveryStarting(Id);
        CbPeripheralWrapper.CbPeripheral.DiscoverServices();
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public void DiscoveredService(NSError? error)
    {
        try
        {
            AppleNativeBluetoothException.ThrowIfError(error);
            var services = CbPeripheralWrapper.CbPeripheral.Services ?? [];
            Logger?.LogServiceDiscoveryCompleted(Id, services.Length);
            OnServicesExplorationSucceeded(services, AreRepresentingTheSameObject, FromInputTypeToOutputTypeConversion);
        }
        catch (Exception e)
        {
            Logger?.LogServiceDiscoveryError(Id, e.Message, e);
            OnServicesExplorationFailed(e);
        }

        return;

        IBluetoothRemoteService FromInputTypeToOutputTypeConversion(CBService native)
        {
            var spec = new AppleBluetoothRemoteServiceFactorySpec(native);
            return (ServiceFactory ?? throw new InvalidOperationException("ServiceFactory must be initialized via the spec-based constructor.")).Create(this, spec);
        }
    }

    /// <inheritdoc />
    public void ModifiedServices(CBService[] services)
    {
        if (services == null)
        {
            return;
        }

        Logger?.LogServicesModified(Id, services.Length);

        foreach (var nativeService in services)
        {
            var matchingService = GetServiceOrDefault(service => AreRepresentingTheSameObject(nativeService, service));
            if (matchingService != null)
            {
                // TODO :
                // Update the matching service with the new native service information
                //matchingService.Update(nativeService);
            }
        }
    }

    /// <inheritdoc />
    public CbPeripheralWrapper.ICbServiceDelegate GetService(CBService? native)
    {
        ArgumentNullException.ThrowIfNull(native);
        try
        {
            var match = GetServiceOrDefault(service => AreRepresentingTheSameObject(native, service));
            return match as CbPeripheralWrapper.ICbServiceDelegate ?? throw new ServiceNotFoundException(this, native.UUID.ToGuid());
        }
        catch (InvalidOperationException e)
        {
            var matches = GetServices(service => AreRepresentingTheSameObject(native, service)).ToArray();
            throw new MultipleServicesFoundException(this, matches, e);
        }
    }

    private static bool AreRepresentingTheSameObject(CBService native, IBluetoothRemoteService shared)
    {
        return shared is AppleBluetoothRemoteService s && native.UUID.Equals(s.CbService.UUID) && native.Handle.Handle.Equals(s.CbService.Handle.Handle);
    }

    #endregion

    #region Ready to Send Write Without Response

    /// <summary>
    ///     Gets the auto-reset event used to signal when the peripheral is ready to send write-without-response commands.
    /// </summary>
    private AutoResetEvent ReadyToSendWriteWithoutResponse { get; } = new AutoResetEvent(false);

    /// <summary>
    ///     Called when the peripheral is ready to send more write-without-response commands on the iOS platform.
    /// </summary>
    public void IsReadyToSendWriteWithoutResponse()
    {
        Logger?.LogReadyToSendWriteWithoutResponse(Id);
        ReadyToSendWriteWithoutResponse.Set();
    }

    /// <summary>
    ///     Waits for the peripheral to be ready to send write-without-response commands.
    /// </summary>
    /// <param name="timeout">The timeout for the operation.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that completes when the peripheral is ready or immediately if already ready.</returns>
    public Task WaitForReadyToSendWriteWithoutResponseAsync(TimeSpan timeout, CancellationToken cancellationToken)
    {
        return CbPeripheralWrapper.CbPeripheral.CanSendWriteWithoutResponse ?
                   Task.CompletedTask :
                   Task.Run(() => ReadyToSendWriteWithoutResponse.WaitOne(timeout), cancellationToken);
    }

    #endregion

}
