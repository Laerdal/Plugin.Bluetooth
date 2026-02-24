using Bluetooth.Maui.Platforms.Win.Exceptions;
using Bluetooth.Maui.Platforms.Win.Logging;
using Bluetooth.Maui.Platforms.Win.Scanning.Factories;
using Bluetooth.Maui.Platforms.Win.Scanning.NativeObjects;

namespace Bluetooth.Maui.Platforms.Win.Scanning;

/// <summary>
///     Represents a Windows-specific Bluetooth Low Energy remote device.
///     This class wraps Windows's BluetoothLEDevice and GattSession, providing platform-specific
///     implementations for device connection, service discovery, and device property management.
/// </summary>
/// <remarks>
///     <seealso href="https://learn.microsoft.com/en-us/uwp/api/windows.devices.bluetooth.bluetoothledevice">BluetoothLEDevice</seealso>
///     <seealso href="https://learn.microsoft.com/en-us/uwp/api/windows.devices.bluetooth.genericattributeprofile.gattsession">GattSession</seealso>
/// </remarks>
public class WindowsBluetoothRemoteDevice : BaseBluetoothRemoteDevice, BluetoothLeDeviceProxy.IBluetoothLeDeviceProxyDelegate, NativeObjects.GattSessionWrapper.IGattSessionDelegate
{
    /// <summary>
    ///     Initializes a new instance of the Windows <see cref="WindowsBluetoothRemoteDevice" /> class.
    /// </summary>
    /// <param name="scanner">The Bluetooth scanner associated with this device.</param>
    /// <param name="spec">The device factory spec containing device information.</param>
    /// <param name="serviceFactory">The factory for creating Bluetooth services.</param>
    /// <param name="rssiToSignalStrengthConverter">Converter for RSSI to signal strength.</param>
    /// <param name="logger">Optional logger for logging device operations.</param>
    public WindowsBluetoothRemoteDevice(IBluetoothScanner scanner,
        IBluetoothRemoteDeviceFactory.BluetoothRemoteDeviceFactorySpec spec,
        IBluetoothRemoteServiceFactory serviceFactory,
        IBluetoothRssiToSignalStrengthConverter rssiToSignalStrengthConverter,
        ILogger<IBluetoothRemoteDevice>? logger = null) : base(scanner, spec, serviceFactory, rssiToSignalStrengthConverter, logger)
    {
        ArgumentNullException.ThrowIfNull(spec);
        if (spec is not WindowsBluetoothRemoteDeviceFactorySpec)
        {
            throw new ArgumentException($"Expected spec of type {typeof(WindowsBluetoothRemoteDeviceFactorySpec)}, but got {spec.GetType()}");
        }
    }

    /// <summary>
    ///     Gets the Windows Bluetooth LE device proxy used for device operations.
    ///     This is initialized when connecting to the device.
    /// </summary>
    public BluetoothLeDeviceProxy? BluetoothLeDeviceProxy { get; protected set; }

    /// <summary>
    ///     Gets the Windows GATT session proxy used for maintaining a reliable connection.
    ///     This is initialized when connecting to the device.
    /// </summary>
    public NativeObjects.GattSessionWrapper? GattSessionProxy { get; protected set; }

    /// <summary>
    ///     Gets or sets the current Windows GATT session status.
    /// </summary>
    public GattSessionStatus GattSessionStatus
    {
        get => GetValue(GattSessionStatus.Closed);
        private set => SetValue(value);
    }

    /// <summary>
    ///     Gets or sets the current Windows Bluetooth connection status.
    /// </summary>
    public BluetoothConnectionStatus BluetoothConnectionStatus
    {
        get => GetValue(BluetoothConnectionStatus.Disconnected);
        private set => SetValue(value);
    }

    #region MTU

    /// <inheritdoc />
    /// <remarks>
    ///     Windows does not support requesting MTU changes. MTU is negotiated automatically by the platform.
    ///     <seealso href="https://learn.microsoft.com/en-us/uwp/api/windows.devices.bluetooth.genericattributeprofile.gattsession.maxpdusize">GattSession.MaxPduSize</seealso>
    /// </remarks>
    protected override ValueTask NativeRequestMtuAsync(int requestedMtu)
    {
        // Windows doesn't support requesting MTU changes
        // MTU is negotiated automatically by the platform
        throw new NotSupportedException("Requesting MTU changes is not supported on Windows. MTU is automatically negotiated by the platform.");
    }

    #endregion

    #region Connection Priority

    /// <inheritdoc />
    /// <remarks>
    ///     Windows does not support connection priority requests. Connection parameters are managed automatically by the platform.
    ///     <seealso href="https://learn.microsoft.com/en-us/uwp/api/windows.devices.bluetooth.genericattributeprofile.gattsession">GattSession</seealso>
    /// </remarks>
    protected override ValueTask NativeRequestConnectionPriorityAsync(BluetoothConnectionPriority priority, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        // Windows doesn't support connection priority requests
        // Connection parameters are managed automatically by the platform
        throw new NotSupportedException("Connection priority requests are not supported on Windows. Connection parameters are managed automatically by the platform.");
    }

    #endregion

    #region PHY

    /// <inheritdoc />
    protected override ValueTask NativeSetPreferredPhyAsync(PhyMode txPhy, PhyMode rxPhy)
    {
        // Windows doesn't support PHY preference settings
        // PHY is managed automatically by the platform
        throw new NotSupportedException("PHY preference settings are not supported on Windows. PHY is managed automatically by the platform.");
    }

    #endregion

    #region L2CAP

    /// <inheritdoc />
    protected override ValueTask NativeOpenL2CapChannelAsync(int psm)
    {
        // Windows doesn't support L2CAP channels in the same way as mobile platforms
        throw new NotSupportedException("L2CAP channels are not currently supported on Windows.");
    }

    #endregion

    #region Signal Strength (RSSI)

    /// <inheritdoc />
    protected override void NativeReadSignalStrength()
    {
        // Windows doesn't support reading RSSI after connection
        // RSSI is only available during advertisement scanning
        throw new NotSupportedException("Reading RSSI is not supported on Windows after connection. RSSI is only available during device discovery.");
    }

    #endregion

    #region Connection

    /// <inheritdoc />
    protected override void NativeRefreshIsConnected()
    {
        IsConnected = BluetoothLeDeviceProxy?.BluetoothLeDevice is { ConnectionStatus: BluetoothConnectionStatus.Connected };
    }

    /// <inheritdoc />
    protected async override ValueTask NativeConnectAsync(ConnectionOptions connectionOptions, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        Logger?.LogConnecting(Id);
        NativeRefreshIsConnected();

        try
        {
            // Parse Bluetooth address from device ID (format: "XX:XX:XX:XX:XX:XX")
            var addressString = Id.Replace(":", "", StringComparison.Ordinal);
            if (!ulong.TryParse(addressString, NumberStyles.HexNumber, null, out var address))
            {
                throw new InvalidOperationException($"Invalid Bluetooth address format: {Id}");
            }

            // Create BluetoothLEDevice proxy
            BluetoothLeDeviceProxy = await BluetoothLeDeviceProxy.GetInstanceAsync(address, this, cancellationToken).ConfigureAwait(false);

            if (BluetoothLeDeviceProxy?.BluetoothLeDevice == null)
            {
                throw new InvalidOperationException("Failed to create BluetoothLEDevice");
            }

            // Update cached name
            CachedName = BluetoothLeDeviceProxy.BluetoothLeDevice.Name;

            // Create GATT session for reliable connection
            GattSessionProxy = await NativeObjects.GattSessionWrapper.GetInstanceAsync(BluetoothLeDeviceProxy.BluetoothLeDevice, this, cancellationToken).ConfigureAwait(false);

            if (GattSessionProxy != null && GattSessionProxy.GattSession.CanMaintainConnection)
            {
                GattSessionProxy.GattSession.MaintainConnection = true;
            }

            // Wait for session to become active
            await WaitForPropertyToBeOfValue(nameof(GattSessionStatus), GattSessionStatus.Active, timeout, cancellationToken).ConfigureAwait(false);

            // Request device access (permissions)
            var accessStatus = await BluetoothLeDeviceProxy.BluetoothLeDevice.RequestAccessAsync().AsTask(cancellationToken).ConfigureAwait(false);

            if (accessStatus != DeviceAccessStatus.Allowed)
            {
                throw new WindowsNativeDeviceAccessStatusException(accessStatus);
            }

            // Kick the connection by reading services
            var services = await BluetoothLeDeviceProxy.ReadGattServicesAsync(BluetoothCacheMode.Uncached, cancellationToken: cancellationToken).ConfigureAwait(false);

            Logger?.LogConnected(Id);
        }
        catch (Exception e)
        {
            Logger?.LogConnectionFailed(Id, 1, e);
            OnConnectFailed(e);
            throw;
        }
    }

    /// <inheritdoc />
    protected async override ValueTask NativeDisconnectAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        Logger?.LogDisconnecting(Id);
        NativeRefreshIsConnected();

        try
        {
            // Dispose device proxy
            BluetoothLeDeviceProxy?.Dispose();
            BluetoothLeDeviceProxy = null;

            // Dispose GATT session
            if (GattSessionProxy != null)
            {
                if (GattSessionProxy.GattSession.CanMaintainConnection)
                {
                    GattSessionProxy.GattSession.MaintainConnection = false;
                }

                await WaitForPropertyToBeOfValue(nameof(GattSessionStatus), GattSessionStatus.Closed, timeout, cancellationToken).ConfigureAwait(false);

                GattSessionProxy.Dispose();
                GattSessionProxy = null;
            }

            Logger?.LogDisconnected(Id);
        }
        catch (Exception e)
        {
            OnDisconnect(e);
            throw;
        }
    }

    #endregion

    #region Service Discovery

    /// <inheritdoc />
    protected async override ValueTask NativeServicesExplorationAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        Logger?.LogServiceDiscoveryStarting(Id);

        try
        {
            if (BluetoothLeDeviceProxy == null)
            {
                throw new InvalidOperationException("Device not connected");
            }

            var services = await BluetoothLeDeviceProxy.ReadGattServicesAsync(BluetoothCacheMode.Uncached, cancellationToken: cancellationToken).ConfigureAwait(false);

            Logger?.LogServiceDiscoveryCompleted(Id, services.Count);

            // Call OnServicesExplorationSucceeded with conversion function
            OnServicesExplorationSucceeded(services, AreRepresentingTheSameObject, FromInputTypeToOutputTypeConversion);
        }
        catch (Exception ex)
        {
            Logger?.LogServiceDiscoveryError(Id, ex.Message, ex);
            OnServicesExplorationFailed(ex);
        }
        return;

        IBluetoothRemoteService FromInputTypeToOutputTypeConversion(GattDeviceService nativeService)
        {
            var spec = new WindowsBluetoothRemoteServiceFactorySpec(nativeService);
            return ServiceFactory.Create(this, spec);
        }
    }

    private static bool AreRepresentingTheSameObject(GattDeviceService native, IBluetoothRemoteService shared)
    {
        // Compare by UUID - more detailed comparison can be done once BluetoothService is implemented
        return native.Uuid.Equals(shared.Id);
    }

    #endregion

    #region Delegate Callbacks - BluetoothLeDeviceProxy

    /// <summary>
    ///     Called when the device's GATT services change on the Windows platform.
    /// </summary>
    public void OnGattServicesChanged()
    {
        // Service list changed - could trigger re-exploration if needed
        // Currently just a placeholder
    }

    /// <summary>
    ///     Called when the device name changes on the Windows platform.
    ///     Updates the cached device name.
    /// </summary>
    /// <param name="senderName">The new device name.</param>
    public void OnNameChanged(string senderName)
    {
        Logger?.LogDeviceNameUpdated(Id, senderName);
        CachedName = senderName;
    }

    /// <summary>
    ///     Called when the device access status changes on the Windows platform.
    /// </summary>
    /// <param name="argsId">The device identifier.</param>
    /// <param name="argsStatus">The new access status.</param>
    public void OnAccessChanged(string argsId, DeviceAccessStatus argsStatus)
    {
        // Handle permission changes if needed
        // Currently just a placeholder
    }

    /// <summary>
    ///     Called when custom pairing is requested for the device on the Windows platform.
    /// </summary>
    /// <param name="args">The pairing spec event arguments.</param>
    public void OnCustomPairingRequested(DevicePairingRequestedEventArgs args)
    {
        // Handle pairing requests if needed
        // Currently just a placeholder
    }

    /// <summary>
    ///     Called when the Bluetooth connection status changes on the Windows platform.
    /// </summary>
    /// <param name="newConnectionStatus">The new connection status.</param>
    public void OnConnectionStatusChanged(BluetoothConnectionStatus newConnectionStatus)
    {
        Logger?.LogConnectionStatusChanged(Id, newConnectionStatus);
        BluetoothConnectionStatus = newConnectionStatus;
        NativeRefreshIsConnected();

        switch (newConnectionStatus)
        {
            case BluetoothConnectionStatus.Connected:
                OnConnectSucceeded();
                break;
            case BluetoothConnectionStatus.Disconnected:
                OnDisconnect();
                break;
        }
    }

    #endregion

    #region Delegate Callbacks - GattSessionWrapper

    /// <summary>
    ///     Called when the GATT session status changes on the Windows platform.
    /// </summary>
    /// <param name="argsStatus">The new GATT session status.</param>
    public void OnGattSessionStatusChanged(GattSessionStatus argsStatus)
    {
        GattSessionStatus = argsStatus;
        NativeRefreshIsConnected();
    }

    /// <summary>
    ///     Called when the maximum PDU (Protocol Data Unit) size changes on the Windows platform.
    /// </summary>
    public void OnMaxPduSizeChanged()
    {
        // Handle MTU changes if needed
        // Currently just a placeholder
    }

    #endregion

}
