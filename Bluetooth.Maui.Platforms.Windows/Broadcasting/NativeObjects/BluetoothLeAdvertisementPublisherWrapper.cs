namespace Bluetooth.Maui.Platforms.Windows.Broadcasting.NativeObjects;

/// <summary>
///     Proxy class for Windows Bluetooth LE advertisement publisher that provides event handling
///     for advertisement broadcasting operations.
/// </summary>
public sealed partial class BluetoothLeAdvertisementPublisherWrapper
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BluetoothLeAdvertisementPublisherWrapper" /> class.
    /// </summary>
    /// <param name="broadcaster">The delegate for handling advertisement publisher events.</param>
    public BluetoothLeAdvertisementPublisherWrapper(IBluetoothLeAdvertisementPublisherProxyDelegate broadcaster)
    {
        BluetoothLeAdvertisementPublisherProxyDelegate = broadcaster;
        BluetoothLeAdvertisementPublisher = new BluetoothLEAdvertisementPublisher();
        BluetoothLeAdvertisementPublisher.StatusChanged += BluetoothLEAdvertisementPublisher_StatusChanged;
    }

    /// <summary>
    ///     Gets the delegate responsible for handling advertisement publisher events.
    /// </summary>
    private IBluetoothLeAdvertisementPublisherProxyDelegate BluetoothLeAdvertisementPublisherProxyDelegate { get; }

    /// <summary>
    ///     Gets the native Windows Bluetooth LE advertisement publisher instance.
    /// </summary>
    public BluetoothLEAdvertisementPublisher BluetoothLeAdvertisementPublisher { get; }

    /// <summary>
    ///     Handles advertisement publisher status change events and forwards them to the delegate.
    /// </summary>
    /// <param name="sender">The advertisement publisher whose status changed.</param>
    /// <param name="args">The status change event arguments.</param>
    private void BluetoothLEAdvertisementPublisher_StatusChanged(BluetoothLEAdvertisementPublisher sender, BluetoothLEAdvertisementPublisherStatusChangedEventArgs args)
    {
        try
        {
            if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 22621)) // Windows 10 version 2004 or later
            {
                BluetoothLeAdvertisementPublisherProxyDelegate.OnAdvertisementPublisherStatusChanged(args.Status, args.Error, args.SelectedTransmitPowerLevelInDBm);
            }
            else
            {
                BluetoothLeAdvertisementPublisherProxyDelegate.OnAdvertisementPublisherStatusChanged(args.Status, args.Error);
            }
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }
}