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
    private readonly IBluetoothRemoteL2CapChannelFactory _l2CapChannelFactory;

    /// <inheritdoc />
    public AppleBluetoothRemoteDevice(IBluetoothScanner scanner, IBluetoothDeviceFactory.BluetoothDeviceFactoryRequest request, IBluetoothServiceFactory serviceFactory,
        IBluetoothRemoteL2CapChannelFactory l2CapChannelFactory,
        IBluetoothRssiToSignalStrengthConverter rssiToSignalStrengthConverter) :
        base(scanner, request, serviceFactory, rssiToSignalStrengthConverter)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(l2CapChannelFactory);
        if (request is not AppleBluetoothDeviceFactoryRequest appleRequest)
        {
            throw new ArgumentException($"Expected request of type {typeof(AppleBluetoothDeviceFactoryRequest)}, but got {request.GetType()}");
        }

        CbPeripheralWrapper = new CbPeripheralWrapper(this, appleRequest.CbPeripheral);
        _l2CapChannelFactory = l2CapChannelFactory;
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

            // Create channel using factory
            var psm = (int)channel.Psm;
            var request = new AppleBluetoothRemoteL2CapChannelFactoryRequest(psm, channel);
            var wrappedChannel = _l2CapChannelFactory.CreateL2CapChannel(this, request);

            OnL2CapChannelOpened(wrappedChannel);
        }
        catch (Exception e)
        {
            OnOpenL2CapChannelFailed(e);
        }
    }

    #endregion

    #region Connection

    /// <inheritdoc />
    protected override void NativeRefreshIsConnected()
    {
        MainThreadDispatcher.BeginInvokeOnMainThread(() => {
            IsConnected = CbPeripheralWrapper.CbPeripheral.State == CBPeripheralState.Connected;
        });
    }

    /// <inheritdoc />
    public void ConnectionEventDidOccur(CBConnectionEvent connectionEvent)
    {
        NativeRefreshIsConnected();
    }

    #region Connect

    /// <inheritdoc />
    /// <seealso href="https://developer.apple.com/documentation/corebluetooth/cbcentralmanager/1518766-connect">iOS CBCentralManager.connect</seealso>
    protected override ValueTask NativeConnectAsync(ConnectionOptions connectionOptions, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(connectionOptions);

        Logger?.LogConnecting(Id);

        NativeRefreshIsConnected();
        if (Scanner is not AppleBluetoothScanner scanner)
        {
            throw new InvalidOperationException("Scanner is not a BluetoothScanner");
        }

        // Convert abstract ConnectionOptions to Apple-specific options
        var appleOptions = new Options.ConnectionOptions
        {
            // Copy common properties
            PermissionStrategy = connectionOptions.PermissionStrategy,
            WaitForAdvertisementBeforeConnecting = connectionOptions.WaitForAdvertisementBeforeConnecting,

            // Read Apple-specific sub-options (or use defaults if not provided)
            NotifyOnConnection = connectionOptions.Apple?.NotifyOnConnection ?? true,
            NotifyOnDisconnection = connectionOptions.Apple?.NotifyOnDisconnection ?? true,
            NotifyOnNotification = connectionOptions.Apple?.NotifyOnNotification ?? true,
            EnableTransportBridging = connectionOptions.Apple?.EnableTransportBridging,
            RequiresAncs = connectionOptions.Apple?.RequiresAncs
        };

        scanner.CbCentralManagerWrapper.CbCentralManager.ConnectPeripheral(CbPeripheralWrapper.CbPeripheral, appleOptions);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public void FailedToConnectPeripheral(NSError? error)
    {
        NativeRefreshIsConnected();
        try
        {
            AppleNativeBluetoothException.ThrowIfError(error);
            throw new DeviceFailedToConnectException(this, "Failed to connect to peripheral, Unknown error");
        }
        catch (Exception e)
        {
            Logger?.LogConnectionFailed(Id, 1, e);
            OnConnectFailed(e);
        }
    }

    /// <inheritdoc />
    public void ConnectedPeripheral()
    {
        NativeRefreshIsConnected();
        Logger?.LogConnected(Id);
        OnConnectSucceeded();
    }

    #endregion

    #region Disconnection

    /// <inheritdoc />
    /// <seealso href="https://developer.apple.com/documentation/corebluetooth/cbcentralmanager/1518952-cancelperipheralconnection">iOS CBCentralManager.cancelPeripheralConnection</seealso>
    protected override ValueTask NativeDisconnectAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        Logger?.LogDisconnecting(Id);

        NativeRefreshIsConnected();
        if (Scanner is not AppleBluetoothScanner scanner)
        {
            throw new InvalidOperationException("Scanner is not a BluetoothScanner");
        }

        scanner.CbCentralManagerWrapper.CbCentralManager.CancelPeripheralConnection(CbPeripheralWrapper.CbPeripheral);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public void DisconnectedPeripheral(NSError? error)
    {
        NativeRefreshIsConnected();
        try
        {
            AppleNativeBluetoothException.ThrowIfError(error);
            Logger?.LogDisconnected(Id);
            OnDisconnect();
        }
        catch (Exception e)
        {
            Logger?.LogDisconnected(Id);
            OnDisconnect(e);
        }
    }

    /// <inheritdoc />
    public void DidDisconnectPeripheral(double timestamp, bool isReconnecting, NSError? error)
    {
        NativeRefreshIsConnected();
        try
        {
            AppleNativeBluetoothException.ThrowIfError(error);
            if (isReconnecting)
            {
                OnDisconnect(new DeviceReconnectingException(this));
            }
            else
            {
                OnDisconnect();
            }
        }
        catch (Exception e)
        {
            OnDisconnect(e);
        }
    }

    #endregion

    #region Connection Priority

    /// <inheritdoc />
    /// <remarks>
    ///     On iOS/macOS, connection priority is automatically managed by the system.
    ///     This method is a no-op and immediately completes successfully.
    /// </remarks>
    protected override ValueTask NativeRequestConnectionPriorityAsync(BluetoothConnectionPriority priority, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        // iOS automatically manages connection priority
        return ValueTask.CompletedTask;
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
    protected override ValueTask NativeServicesExplorationAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
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
            var request = new AppleBluetoothServiceFactoryRequest(native);
            return ServiceFactory.CreateService(this, request);
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
        return CbPeripheralWrapper.CbPeripheral.CanSendWriteWithoutResponse ? Task.CompletedTask : Task.Run(() => ReadyToSendWriteWithoutResponse.WaitOne(timeout), cancellationToken);
    }

    #endregion

    #region Disposal

    /// <inheritdoc />
    public async new ValueTask DisposeAsync()
    {
        // Dispose AutoResetEvent to prevent memory leak
        ReadyToSendWriteWithoutResponse?.Dispose();

        await base.DisposeAsync().ConfigureAwait(false);
    }

    #endregion
}
