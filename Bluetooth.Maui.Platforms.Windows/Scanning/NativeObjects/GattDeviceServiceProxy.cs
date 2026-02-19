namespace Bluetooth.Maui.Platforms.Windows;


/// <summary>
/// Proxy class for Windows GATT device service that provides event handling and lifecycle management
/// for GATT service operations.
/// </summary>
public sealed partial class GattDeviceServiceProxy : IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GattDeviceServiceProxy"/> class.
    /// </summary>
    /// <param name="gattDeviceService">The native Windows GATT device service instance.</param>
    /// <param name="bluetoothServiceProxyDelegate">The delegate for handling GATT service events.</param>
    public GattDeviceServiceProxy(GattDeviceService gattDeviceService, IBluetoothServiceProxyDelegate bluetoothServiceProxyDelegate)
    {
        GattDeviceServiceProxyDelegate = bluetoothServiceProxyDelegate;
        GattDeviceService = gattDeviceService;
        GattDeviceService.DeviceAccessInformation.AccessChanged += OnAccessChanged;
    }

    /// <summary>
    /// Gets the delegate responsible for handling GATT service events.
    /// </summary>
    private IBluetoothServiceProxyDelegate GattDeviceServiceProxyDelegate { get; }

    /// <summary>
    /// Gets the native Windows GATT device service instance.
    /// </summary>
    public GattDeviceService GattDeviceService { get; }

    /// <summary>
    /// Releases all resources used by the <see cref="GattDeviceServiceProxy"/> instance.
    /// </summary>
    public void Dispose()
    {
        GattDeviceService.DeviceAccessInformation.AccessChanged -= OnAccessChanged;
        GattDeviceService.Dispose();
    }

    /// <summary>
    /// Handles device access change events and forwards them to the delegate.
    /// </summary>
    /// <param name="sender">The device access information that changed.</param>
    /// <param name="args">The device access change event arguments.</param>
    private void OnAccessChanged(DeviceAccessInformation sender, DeviceAccessChangedEventArgs args)
    {
        try
        {
            GattDeviceServiceProxyDelegate.OnAccessChanged(args.Id, args.Status);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }
}

