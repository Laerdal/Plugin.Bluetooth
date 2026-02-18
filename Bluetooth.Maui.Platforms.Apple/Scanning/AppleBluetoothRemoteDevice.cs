using Bluetooth.Abstractions.Scanning.Exceptions;
using Bluetooth.Abstractions.Scanning.Factories;
using Bluetooth.Maui.Platforms.Apple.Exceptions;
using Bluetooth.Maui.Platforms.Apple.Scanning.Factories;
using Bluetooth.Maui.Platforms.Apple.Scanning.NativeObjects;
using Bluetooth.Maui.Platforms.Apple.Scanning.Options;
using Bluetooth.Maui.Platforms.Apple.Tools;

namespace Bluetooth.Maui.Platforms.Apple.Scanning;

/// <inheritdoc cref="BaseBluetoothRemoteDevice" />
public class AppleBluetoothRemoteDevice : BaseBluetoothRemoteDevice, CbPeripheralWrapper.ICbPeripheralDelegate, CbCentralManagerWrapper.ICbPeripheralDelegate
{
    /// <summary>
    /// Gets the iOS Core Bluetooth peripheral delegate proxy used for peripheral operations.
    /// </summary>
    public CbPeripheralWrapper CbPeripheralWrapper { get; }

    /// <summary>
    /// Gets the Bluetooth scanner that discovered this device, cast to the Apple-specific implementation.
    /// </summary>
    public AppleBluetoothScanner AppleBluetoothScanner => (AppleBluetoothScanner) Scanner;

    /// <inheritdoc />
    public AppleBluetoothRemoteDevice(IBluetoothScanner scanner, IBluetoothDeviceFactory.BluetoothDeviceFactoryRequest request, IBluetoothServiceFactory serviceFactory, IBluetoothRssiToSignalStrengthConverter rssiToSignalStrengthConverter) :
        base(scanner, request, serviceFactory, rssiToSignalStrengthConverter)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request is not AppleBluetoothDeviceFactoryRequest appleRequest)
        {
            throw new ArgumentException($"Expected request of type {typeof(AppleBluetoothDeviceFactoryRequest)}, but got {request.GetType()}");
        }
        CbPeripheralWrapper = new CbPeripheralWrapper(this, appleRequest.CbPeripheral);
    }

    #region L2Cap

    /// <inheritdoc />
    protected override ValueTask NativeOpenL2CapChannelAsync(int psm)
    {
        CbPeripheralWrapper.CbPeripheral.OpenL2CapChannel((ushort)psm);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public void DidOpenL2CapChannel(NSError? error, CBL2CapChannel? channel)
    {

        try
        {
            AppleNativeBluetoothException.ThrowIfError(error);

            // TODO : Implement BluetoothL2CapChannel and return it in the event args. This requires implementing the read/write operations and handling the channel lifecycle, which is non-trivial and may require significant additional code to manage correctly.
            // OnL2CapChannelOpened(new BluetoothL2CapChannel(this, channel));
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
        SetValue(CbPeripheralWrapper.CbPeripheral.State == CBPeripheralState.Connected, nameof(IsConnected));
    }

    /// <inheritdoc />
    public void ConnectionEventDidOccur(CBConnectionEvent connectionEvent)
    {
        NativeRefreshIsConnected();
    }

    #region Connect

    /// <inheritdoc />
    protected override ValueTask NativeConnectAsync(Abstractions.Scanning.Options.ConnectionOptions connectionOptions, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        NativeRefreshIsConnected();
        if (Scanner is not AppleBluetoothScanner scanner)
        {
            throw new InvalidOperationException("Scanner is not a BluetoothScanner");
        }
        if (connectionOptions is not Bluetooth.Maui.Platforms.Apple.Scanning.Options.ConnectionOptions iosConnectionOptions)
        {
            throw new ArgumentException($"Connection options must be of type {nameof(PeripheralConnectionOptions)} for iOS platform.", nameof(connectionOptions));
        }

        scanner.CbCentralManagerWrapper.CbCentralManager.ConnectPeripheral(CbPeripheralWrapper.CbPeripheral, iosConnectionOptions);
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
            OnConnectFailed(e);
        }
    }

    /// <inheritdoc />
    public void ConnectedPeripheral()
    {
        NativeRefreshIsConnected();
        OnConnectSucceeded();
    }

    #endregion

    #region Disconnection

    /// <inheritdoc />
    protected override ValueTask NativeDisconnectAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
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
            OnDisconnect();
        }
        catch (Exception e)
        {
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
    /// On iOS/macOS, connection priority is automatically managed by the system.
    /// This method is a no-op and immediately completes successfully.
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
    protected override ValueTask NativeServicesExplorationAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
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
            OnServicesExplorationSucceeded(services, AreRepresentingTheSameObject, FromInputTypeToOutputTypeConversion);
        }
        catch (Exception e)
        {
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
        // TODO : Implement if needed. This method is called when the services of the peripheral are modified.
        // If your application needs to handle service modifications (e.g., if the peripheral's services can change dynamically), you may want to implement this method to update your application's state accordingly.
        // Placeholder for future implementation
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
    /// Gets the auto-reset event used to signal when the peripheral is ready to send write-without-response commands.
    /// </summary>
    private AutoResetEvent ReadyToSendWriteWithoutResponse { get; } = new AutoResetEvent(false);

    /// <summary>
    /// Called when the peripheral is ready to send more write-without-response commands on the iOS platform.
    /// </summary>
    public void IsReadyToSendWriteWithoutResponse()
    {
        ReadyToSendWriteWithoutResponse.Set();
    }

    /// <summary>
    /// Waits for the peripheral to be ready to send write-without-response commands.
    /// </summary>
    /// <param name="timeout">The timeout for the operation.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that completes when the peripheral is ready or immediately if already ready.</returns>
    public Task WaitForReadyToSendWriteWithoutResponseAsync(TimeSpan timeout, CancellationToken cancellationToken)
    {
        return CbPeripheralWrapper.CbPeripheral.CanSendWriteWithoutResponse ? Task.CompletedTask : Task.Run(() => ReadyToSendWriteWithoutResponse.WaitOne(timeout), cancellationToken);
    }

    #endregion

    /// <inheritdoc />
    public void UpdatedName()
    {
        if (CbPeripheralWrapper.CbPeripheral.Name != null)
        {
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
}
