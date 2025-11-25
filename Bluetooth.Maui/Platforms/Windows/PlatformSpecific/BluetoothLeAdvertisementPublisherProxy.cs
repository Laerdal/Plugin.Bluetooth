namespace Bluetooth.Maui.PlatformSpecific;


/// <summary>
/// Proxy class for Windows Bluetooth LE advertisement publisher that provides event handling
/// for advertisement broadcasting operations.
/// </summary>
public sealed partial class BluetoothLeAdvertisementPublisherProxy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BluetoothLeAdvertisementPublisherProxy"/> class.
    /// </summary>
    /// <param name="bluetoothAdapterProxyDelegate">The delegate for handling advertisement publisher events.</param>
    public BluetoothLeAdvertisementPublisherProxy(IBluetoothLeAdvertisementPublisherProxyDelegate bluetoothAdapterProxyDelegate)
    {
        BluetoothLeAdvertisementPublisherProxyDelegate = bluetoothAdapterProxyDelegate;
        BluetoothLeAdvertisementPublisher = new BluetoothLEAdvertisementPublisher();
        BluetoothLeAdvertisementPublisher.StatusChanged += BluetoothLEAdvertisementPublisher_StatusChanged;
    }

    /// <summary>
    /// Gets the delegate responsible for handling advertisement publisher events.
    /// </summary>
    private IBluetoothLeAdvertisementPublisherProxyDelegate BluetoothLeAdvertisementPublisherProxyDelegate { get; }

    /// <summary>
    /// Gets the native Windows Bluetooth LE advertisement publisher instance.
    /// </summary>
    public BluetoothLEAdvertisementPublisher BluetoothLeAdvertisementPublisher { get; }

    /// <summary>
    /// Handles advertisement publisher status change events and forwards them to the delegate.
    /// </summary>
    /// <param name="sender">The advertisement publisher whose status changed.</param>
    /// <param name="args">The status change event arguments.</param>
    private void BluetoothLEAdvertisementPublisher_StatusChanged(BluetoothLEAdvertisementPublisher sender, BluetoothLEAdvertisementPublisherStatusChangedEventArgs args)
    {
        try
        {
            BluetoothLeAdvertisementPublisherProxyDelegate.OnAdvertisementPublisherStatusChanged(args.Status);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }
}

