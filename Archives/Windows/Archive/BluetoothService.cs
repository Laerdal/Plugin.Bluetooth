using Bluetooth.Maui.PlatformSpecific;

namespace Bluetooth.Maui;

/// <summary>
/// Windows implementation of the Bluetooth service using Windows.Devices.Bluetooth APIs.
/// </summary>
/// <remarks>
/// This implementation wraps Windows's <see cref="GattDeviceService"/> to provide access to GATT characteristics.
/// </remarks>
public partial class BluetoothService : BaseBluetoothService, GattDeviceServiceProxy.IBluetoothServiceProxyDelegate
{
    /// <summary>
    /// Gets the native Windows GATT device service proxy.
    /// </summary>
    public GattDeviceServiceProxy NativeServiceProxy { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BluetoothService"/> class.
    /// </summary>
    /// <param name="device">The Bluetooth device associated with this service.</param>
    /// <param name="serviceUuid">The unique identifier of the service.</param>
    /// <param name="nativeService">The native Windows GATT device service.</param>
    public BluetoothService(Windows.Devices.Bluetooth.IBluetoothDevice device, Guid serviceUuid, GattDeviceService nativeService) : base(device, serviceUuid)
    {
        NativeServiceProxy = new GattDeviceServiceProxy(nativeService, this);
    }

    /// <inheritdoc/>
    protected override ValueTask DisposeAsyncCore()
    {
        NativeServiceProxy.Dispose();
        return base.DisposeAsyncCore();
    }

    #region GattDeviceServiceProxy.IBluetoothServiceProxyDelegate

    /// <summary>
    /// Called when the access status of the GATT device service changes.
    /// </summary>
    /// <param name="argsId">The device ID.</param>
    /// <param name="argsStatus">The new access status.</param>
    public void OnAccessChanged(string argsId, DeviceAccessStatus argsStatus)
    {
        // Placeholder for future implementation
    }

    #endregion
}
