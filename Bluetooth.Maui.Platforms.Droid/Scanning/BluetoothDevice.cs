using Bluetooth.Maui.Platforms.Droid.Exceptions;
using Bluetooth.Maui.Platforms.Droid.Scanning.NativeObjects;
using Bluetooth.Maui.Platforms.Droid.Tools;

using Microsoft.Extensions.Logging;

namespace Bluetooth.Maui.Platforms.Droid.Scanning;

/// <inheritdoc/>
public partial class BluetoothDevice : BaseBluetoothDevice, BluetoothGattProxy.IDevice
{
    /// <summary>
    /// Gets the native Android Bluetooth device.
    /// </summary>
    public Android.Bluetooth.BluetoothDevice NativeDevice { get; }

    /// <summary>
    /// Gets the Bluetooth GATT proxy for managing the connection.
    /// </summary>
    private BluetoothGattProxy? BluetoothGattProxy { get; set; }

    /// <summary>
    /// Gets the Bluetooth GATT instance.
    /// </summary>
    private BluetoothGatt? BluetoothGatt => BluetoothGattProxy?.BluetoothGatt;

    /// <inheritdoc/>
    public BluetoothDevice(IBluetoothScanner scanner, IBluetoothDeviceFactory.BluetoothDeviceFactoryRequest request, IBluetoothServiceFactory serviceFactory) : base(scanner, request, serviceFactory)
    {
        if (request is not BluetoothDeviceFactoryRequest androidRequest)
        {
            throw new ArgumentException($"Request must be of type {nameof(BluetoothDeviceFactoryRequest)}", nameof(request));
        }

        NativeDevice = androidRequest.NativeDevice ?? throw new ArgumentNullException(nameof(androidRequest.NativeDevice));
    }

    /// <inheritdoc/>
    protected override void NativeRefreshIsConnected()
    {
        var gatt = BluetoothGatt;
        if (gatt == null)
        {
            IsConnected = false;
            return;
        }

        // Check connection state through Android Bluetooth Manager
        if (Scanner.Adapter is BluetoothAdapter androidAdapter)
        {
            var connectionState = androidAdapter.BluetoothManager.GetConnectionState(NativeDevice, ProfileType.Gatt);
            IsConnected = connectionState == ProfileState.Connected;
        }
        else
        {
            IsConnected = false;
        }
    }

    /// <inheritdoc/>
    protected override ValueTask NativeConnectAsync(IBluetoothDeviceConnectionOptions connectionOptions, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        // Create connection options
        var androidOptions = connectionOptions as BluetoothDeviceConnectionOptions ?? new BluetoothDeviceConnectionOptions();

        // Create GATT proxy which automatically connects
        BluetoothGattProxy = new BluetoothGattProxy(this, androidOptions, NativeDevice);

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    protected override ValueTask NativeDisconnectAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        try
        {
            BluetoothGatt?.Disconnect();
            BluetoothGattProxy?.Dispose();
            BluetoothGattProxy = null;
        }
        catch (Exception e)
        {
            Logger?.LogError(e, "Error disconnecting from device {DeviceId}", Id);
        }

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    protected override ValueTask NativeServicesExplorationAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(BluetoothGatt);

        var result = BluetoothGatt.DiscoverServices();
        if (!result)
        {
            throw new InvalidOperationException("Failed to start service discovery");
        }

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    protected override void NativeReadSignalStrength()
    {
        ArgumentNullException.ThrowIfNull(BluetoothGatt);

        var result = BluetoothGatt.ReadRemoteRssi();
        if (!result)
        {
            Logger?.LogWarning("Failed to read remote RSSI for device {DeviceId}", Id);
        }
    }

    /// <summary>
    /// Updates the device with new advertisement data.
    /// </summary>
    internal void UpdateAdvertisement(IBluetoothAdvertisement advertisement, int rssi)
    {
        Advertisement = advertisement;
        SignalStrength = rssi;
    }

    #region BluetoothGattProxy.IDevice Implementation

    /// <inheritdoc/>
    public BluetoothGattProxy.IService GetService(BluetoothGattService? nativeService)
    {
        ArgumentNullException.ThrowIfNull(nativeService);

        var serviceId = Java.Util.UUID.FromString(nativeService.Uuid?.ToString())?.ToGuid()
            ?? throw new InvalidOperationException("Service UUID is null");

        var service = GetServiceOrDefault(serviceId);
        if (service is BluetoothGattProxy.IService androidService)
        {
            return androidService;
        }

        throw new InvalidOperationException($"Service {serviceId} not found or is not an Android service");
    }

    /// <inheritdoc/>
    public void OnConnectionStateChange(GattStatus status, ProfileState newState)
    {
        try
        {
            if (status != GattStatus.Success)
            {
                AndroidNativeGattCallbackStatusConnectionException.ThrowIfError(status, newState);
            }

            if (newState == ProfileState.Connected)
            {
                IsConnected = true;
                Logger?.LogDebug("Device {DeviceId} connected", Id);
            }
            else if (newState == ProfileState.Disconnected)
            {
                IsConnected = false;
                Logger?.LogDebug("Device {DeviceId} disconnected", Id);
            }
        }
        catch (Exception e)
        {
            Logger?.LogError(e, "Error handling connection state change for device {DeviceId}", Id);
        }
    }

    /// <inheritdoc/>
    public void OnServicesDiscovered(GattStatus status)
    {
        try
        {
            AndroidNativeGattCallbackStatusException.ThrowIfError(status);

            var nativeServices = BluetoothGatt?.Services;
            if (nativeServices == null || nativeServices.Count == 0)
            {
                OnServicesExplorationFailed(new InvalidOperationException("No services found"));
                return;
            }

            OnServicesExplorationSucceeded(nativeServices, FromInputTypeToOutputTypeConversion, AreRepresentingTheSameObject);
        }
        catch (Exception e)
        {
            OnServicesExplorationFailed(e);
        }

        return;

        IBluetoothService FromInputTypeToOutputTypeConversion(BluetoothGattService native)
        {
            var serviceId = Java.Util.UUID.FromString(native.Uuid?.ToString())?.ToGuid()
                ?? throw new InvalidOperationException("Service UUID is null");

            var request = new BluetoothServiceFactoryRequest
            {
                ServiceId = serviceId,
                NativeService = native
            };

            // Note: This blocks, but is necessary since OnServicesExplorationSucceeded expects a synchronous function
            return ServiceFactory.CreateServiceAsync(this, request).GetAwaiter().GetResult();
        }

        bool AreRepresentingTheSameObject(BluetoothGattService native, IBluetoothService shared)
        {
            return shared is BluetoothService androidService &&
                   native.Uuid?.ToString() == androidService.NativeService.Uuid?.ToString() &&
                   native.InstanceId == androidService.NativeService.InstanceId;
        }
    }

    /// <inheritdoc/>
    public void OnServiceChanged()
    {
        Logger?.LogDebug("Services changed for device {DeviceId}", Id);

        // Optionally re-discover services
        try
        {
            BluetoothGatt?.DiscoverServices();
        }
        catch (Exception e)
        {
            Logger?.LogError(e, "Error rediscovering services for device {DeviceId}", Id);
        }
    }

    /// <inheritdoc/>
    public void OnReadRemoteRssi(GattStatus status, int rssi)
    {
        try
        {
            AndroidNativeGattCallbackStatusException.ThrowIfError(status);
            SignalStrength = rssi;
        }
        catch (Exception e)
        {
            Logger?.LogError(e, "Error reading remote RSSI for device {DeviceId}", Id);
        }
    }

    /// <inheritdoc/>
    public void OnReliableWriteCompleted(GattStatus status)
    {
        try
        {
            AndroidNativeGattCallbackStatusException.ThrowIfError(status);
            Logger?.LogDebug("Reliable write completed for device {DeviceId}", Id);
        }
        catch (Exception e)
        {
            Logger?.LogError(e, "Error completing reliable write for device {DeviceId}", Id);
        }
    }

    /// <inheritdoc/>
    public void OnMtuChanged(GattStatus status, int mtu)
    {
        try
        {
            AndroidNativeGattCallbackStatusException.ThrowIfError(status);
            Logger?.LogDebug("MTU changed to {Mtu} for device {DeviceId}", mtu, Id);
        }
        catch (Exception e)
        {
            Logger?.LogError(e, "Error handling MTU change for device {DeviceId}", Id);
        }
    }

    /// <inheritdoc/>
    public void OnPhyRead(GattStatus status, Android.Bluetooth.BluetoothPhy txPhy, Android.Bluetooth.BluetoothPhy rxPhy)
    {
        try
        {
            AndroidNativeGattCallbackStatusException.ThrowIfError(status);
            Logger?.LogDebug("PHY read: TX={TxPhy}, RX={RxPhy} for device {DeviceId}", txPhy, rxPhy, Id);
        }
        catch (Exception e)
        {
            Logger?.LogError(e, "Error reading PHY for device {DeviceId}", Id);
        }
    }

    /// <inheritdoc/>
    public void OnPhyUpdate(GattStatus status, Android.Bluetooth.BluetoothPhy txPhy, Android.Bluetooth.BluetoothPhy rxPhy)
    {
        try
        {
            AndroidNativeGattCallbackStatusException.ThrowIfError(status);
            Logger?.LogDebug("PHY updated: TX={TxPhy}, RX={RxPhy} for device {DeviceId}", txPhy, rxPhy, Id);
        }
        catch (Exception e)
        {
            Logger?.LogError(e, "Error updating PHY for device {DeviceId}", Id);
        }
    }

    #endregion
}
