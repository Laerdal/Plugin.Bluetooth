using Bluetooth.Maui.Platforms.Apple.PlatformSpecific;
using Bluetooth.Maui.Platforms.Apple.PlatformSpecific.Exceptions;
using Bluetooth.Maui.Platforms.Apple.Scanning.NativeObjects;

namespace Bluetooth.Maui.Platforms.Apple.Scanning;

/// <summary>
/// Represents an iOS-specific Bluetooth Low Energy device.
/// This class wraps iOS Core Bluetooth's CBPeripheral, providing platform-specific
/// implementations for device connection, service discovery, and device property management.
/// </summary>
public class BluetoothDevice : BaseBluetoothDevice, CbPeripheralWrapper.ICbPeripheralDelegate, CbCentralManagerWrapper.ICbPeripheralDelegate
{
    /// <summary>
    /// Gets the iOS Core Bluetooth peripheral delegate proxy used for peripheral operations.
    /// </summary>
    public CbPeripheralWrapper CbPeripheralWrapper { get; }

    /// <inheritdoc/>
    public BluetoothDevice(IBluetoothScanner scanner, IBluetoothDeviceFactory.BluetoothDeviceFactoryRequest request, IBluetoothServiceFactory serviceFactory) : base(scanner, request, serviceFactory)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request is not BluetoothDeviceFactoryRequest nativeRequest)
        {
            throw new ArgumentException($"Request must be of type {nameof(BluetoothDeviceFactoryRequest)}", nameof(request));
        }

        if (request is IBluetoothDeviceFactory.BluetoothDeviceFactoryRequestWithAdvertisement { Advertisement: BluetoothAdvertisement advertisement })
        {
            CbPeripheralWrapper = new CbPeripheralWrapper(this, advertisement.Peripheral);
        }
        else if (nativeRequest.CbPeripheralWrapper != null)
        {
            CbPeripheralWrapper = nativeRequest.CbPeripheralWrapper;
        }
        else
        {
            throw new ArgumentException("Request must contain either an Advertisement or a CbPeripheralWrapper", nameof(request));
        }
    }

    /// <summary>
    /// Gets or sets the current iOS Core Bluetooth peripheral state.
    /// </summary>
    public CBPeripheralState CbPeripheralState
    {
        get => GetValue(CBPeripheralState.Disconnected);
        protected set => SetValue(value);
    }

    #region CbPeripheralWrapper.ICbPeripheralDelegate

    /// <inheritdoc/>
    /// <remarks>
    /// Updates the cached device name when the peripheral's name changes on iOS.
    /// </remarks>
    public void UpdatedName()
    {
        if (CbPeripheralWrapper.CbPeripheral.Name != null)
        {
            CachedName = CbPeripheralWrapper.CbPeripheral.Name;
        }
    }

    /// <inheritdoc/>
    /// <exception cref="AppleNativeBluetoothException">Thrown when the error parameter indicates a Bluetooth error.</exception>
    /// <remarks>
    /// Placeholder for future L2CAP channel implementation.
    /// </remarks>
    public void DidOpenL2CapChannel(NSError? error, CBL2CapChannel? channel)
    {
        AppleNativeBluetoothException.ThrowIfError(error);
        // Placeholder for future implementation
    }

    #endregion

    #region Connection

    /// <inheritdoc/>
    /// <remarks>
    /// On iOS, this checks the <see cref="CBPeripheral.State"/> property of the Core Bluetooth peripheral.
    /// </remarks>
    protected override void NativeRefreshIsConnected()
    {
        CbPeripheralState = CbPeripheralWrapper.CbPeripheral.State;
        IsConnected = CbPeripheralState == CBPeripheralState.Connected;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// On iOS, this initiates a connection by calling <c>CBCentralManager.ConnectPeripheral</c> with the configured <see cref="PeripheralConnectionOptions"/>.
    /// The connection result is delivered asynchronously via the <see cref="ConnectedPeripheral"/> or <see cref="FailedToConnectPeripheral"/> callbacks.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when the central manager is <c>null</c>.</exception>
    protected override ValueTask NativeConnectAsync(IBluetoothDeviceConnectionOptions connectionOptions, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        NativeRefreshIsConnected();
        if (Scanner is not BluetoothScanner scanner)
        {
            throw new InvalidOperationException("Scanner is not a BluetoothScanner");
        }
        if (connectionOptions is not BluetoothDeviceConnectionOptions iosConnectionOptions)
        {
            throw new ArgumentException($"Connection options must be of type {nameof(PeripheralConnectionOptions)} for iOS platform.", nameof(connectionOptions));
        }

        ArgumentNullException.ThrowIfNull(scanner.CbCentralManager);
        scanner.CbCentralManager.ConnectPeripheral(CbPeripheralWrapper.CbPeripheral, iosConnectionOptions.ToCBConnectPeripheralOptions());
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// On iOS, this initiates a disconnection by calling <c>CBCentralManager.CancelPeripheralConnection</c>.
    /// The disconnection result is delivered asynchronously via the <see cref="DisconnectedPeripheral"/> callback.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when the central manager is <c>null</c>.</exception>
    protected override ValueTask NativeDisconnectAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        NativeRefreshIsConnected();
        if (Scanner is not BluetoothScanner scanner)
        {
            throw new InvalidOperationException("Scanner is not a BluetoothScanner");
        }
        ArgumentNullException.ThrowIfNull(scanner.CbCentralManager);
        scanner.CbCentralManager.CancelPeripheralConnection(CbPeripheralWrapper.CbPeripheral);
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Called when a connection attempt to the peripheral fails on the iOS platform.
    /// </summary>
    /// <param name="error">The error that occurred during the connection attempt.</param>
    /// <exception cref="AppleNativeBluetoothException">Thrown when the error parameter indicates a Bluetooth error.</exception>
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

    /// <summary>
    /// Called when the peripheral disconnects on the iOS platform.
    /// </summary>
    /// <param name="error">The error that occurred during disconnection, or <c>null</c> if disconnection was intentional.</param>
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

    /// <summary>
    /// Called when the peripheral successfully connects on the iOS platform.
    /// </summary>
    public void ConnectedPeripheral()
    {
        NativeRefreshIsConnected();
        OnConnectSucceeded();
    }

    /// <summary>
    /// Called when a connection event occurs on the iOS platform.
    /// </summary>
    /// <param name="connectionEvent">The type of connection event that occurred.</param>
    /// <remarks>
    /// Placeholder for future implementation.
    /// </remarks>
    public void ConnectionEventDidOccur(CBConnectionEvent connectionEvent)
    {
        NativeRefreshIsConnected();
    }

    /// <summary>
    /// Called when the ANCS (Apple Notification Center Service) authorization status changes on the iOS platform.
    /// </summary>
    /// <remarks>
    /// Placeholder for future implementation.
    /// </remarks>
    public void DidUpdateAncsAuthorization()
    {
        // Placeholder for future implementation
    }

    /// <summary>
    /// Called when the peripheral disconnects on the iOS platform with additional information.
    /// </summary>
    /// <param name="timestamp">The timestamp of the disconnection event.</param>
    /// <param name="isReconnecting">Indicates whether the system is attempting to reconnect.</param>
    /// <param name="error">The error that occurred during disconnection, or <c>null</c> if disconnection was intentional.</param>
    /// <remarks>
    /// Available on iOS 13.0 and later. Placeholder for future implementation.
    /// </remarks>
    public void DidDisconnectPeripheral(double timestamp, bool isReconnecting, NSError? error)
    {
        // Unclear how we are supposed to use this method ... Apple documentation is not clear
        DisconnectedPeripheral(error);
    }

    #endregion

    #region Service Exploration

    /// <inheritdoc/>
    /// <remarks>
    /// On iOS, this initiates service discovery by calling <c>CBPeripheral.DiscoverServices</c>.
    /// Results are delivered asynchronously via the <see cref="DiscoveredService"/> callback.
    /// </remarks>
    protected override ValueTask NativeServicesExplorationAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        CbPeripheralWrapper.CbPeripheral.DiscoverServices();
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Called when service discovery completes on the iOS platform.
    /// </summary>
    /// <param name="error">The error that occurred during service discovery, or <c>null</c> if successful.</param>
    /// <exception cref="AppleNativeBluetoothException">Thrown when the error parameter indicates a Bluetooth error.</exception>
    public void DiscoveredService(NSError? error)
    {
        try
        {
            AppleNativeBluetoothException.ThrowIfError(error);
            var services = CbPeripheralWrapper.CbPeripheral.Services ?? [];
            OnServicesExplorationSucceeded(services, FromInputTypeToOutputTypeConversion, AreRepresentingTheSameObject);
        }
        catch (Exception e)
        {
            OnServicesExplorationFailed(e);
        }
        return;

        IBluetoothService FromInputTypeToOutputTypeConversion(CBService native)
        {
            var request = new BluetoothServiceFactoryRequest
            {
                ServiceId = native.UUID.ToGuid(),
                NativeService = native
            };
            return ServiceFactory.CreateService(this, request);
        }
    }

    private static bool AreRepresentingTheSameObject(CBService native, IBluetoothService shared)
    {
        return shared is BluetoothService s && native.UUID.Equals(s.NativeService.UUID) && native.Handle.Handle.Equals(s.NativeService.Handle.Handle);
    }

    /// <summary>
    /// Called when the device's services are modified on the iOS platform.
    /// </summary>
    /// <param name="services">The services that were modified.</param>
    /// <remarks>
    /// Placeholder for future implementation.
    /// </remarks>
    public void ModifiedServices(CBService[] services)
    {
        // Placeholder for future implementation
    }

    /// <summary>
    /// Gets the corresponding <see cref="IBluetoothService"/> wrapper for a native iOS Core Bluetooth service.
    /// </summary>
    /// <param name="characteristicService">The native iOS Core Bluetooth service.</param>
    /// <returns>The corresponding Bluetooth service wrapper.</returns>
    /// <exception cref="ServiceNotFoundException">Thrown when <paramref name="characteristicService"/> is <c>null</c> or when no matching service is found.</exception>
    /// <exception cref="MultipleServicesFoundException">Thrown when multiple services match the criteria.</exception>
    public CbPeripheralWrapper.ICbServiceDelegate GetService(CBService? characteristicService)
    {
        ArgumentNullException.ThrowIfNull(characteristicService);
        try
        {
            var match = GetServiceOrDefault(service => AreRepresentingTheSameObject(characteristicService, service));
            return match as CbPeripheralWrapper.ICbServiceDelegate ?? throw new ServiceNotFoundException(this, characteristicService.UUID.ToGuid());
        }
        catch (InvalidOperationException e)
        {
            var matches = GetServices(service => AreRepresentingTheSameObject(characteristicService, service)).ToArray();
            throw new MultipleServicesFoundException(this, matches, e);
        }
    }

    #endregion

    #region Signal Strength

    /// <inheritdoc/>
    protected override void NativeReadSignalStrength()
    {
        CbPeripheralWrapper.CbPeripheral.ReadRSSI();
    }

    /// <summary>
    /// Called when an RSSI read operation completes on the iOS platform.
    /// </summary>
    /// <param name="error">The error that occurred during the read operation, or <c>null</c> if successful.</param>
    /// <param name="rssi">The RSSI value in dBm.</param>
    /// <exception cref="AppleNativeBluetoothException">Thrown when the error parameter indicates a Bluetooth error.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="rssi"/> is <c>null</c>.</exception>
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

    /// <summary>
    /// Called when the RSSI value is updated on the iOS platform (deprecated callback).
    /// </summary>
    /// <param name="error">The error that occurred during the update, or <c>null</c> if successful.</param>
    /// <remarks>
    /// This method is for compatibility with iOS versions prior to iOS 8.
    /// The peripheral's RSSI property is obsolete in iOS 8 and later.
    /// </remarks>
    public void RssiUpdated(NSError? error)
    {
        // CbPeripheralWrapper.CbPeripheral.RSSI is Obsolete in iOS 8
        if (!OperatingSystem.IsIOSVersionAtLeast(8) && CbPeripheralWrapper.CbPeripheral.RSSI != null)
        {
            RssiRead(error, CbPeripheralWrapper.CbPeripheral.RSSI);
        }
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
}
