
using Plugin.Bluetooth.Maui.PlatformSpecific.BroadcastReceivers;

namespace Plugin.Bluetooth.Maui.PlatformSpecific;

public partial class BluetoothDeviceEventProxy
{
    private IScanner Scanner { get; }

    private BluetoothDeviceEventReceiver BluetoothDeviceEventReceiver { get; }

    public BluetoothDeviceEventProxy(IScanner scanner)
    {
        Scanner = scanner;
        BluetoothDeviceEventReceiver = new BluetoothDeviceEventReceiver();
        BluetoothDeviceEventReceiver.BondStateChanged += ReceiverOnBondStateChanged;
        BluetoothDeviceEventReceiver.PairingRequest += ReceiverOnPairingRequest;
        BluetoothDeviceEventReceiver.AclConnected += ReceiverOnAclConnected;
        BluetoothDeviceEventReceiver.AclDisconnected += ReceiverOnAclDisconnected;
        BluetoothDeviceEventReceiver.ClassChanged += ReceiverOnClassChanged;
        BluetoothDeviceEventReceiver.DeviceFound += ReceiverOnDeviceFound;
        BluetoothDeviceEventReceiver.NameChanged += ReceiverOnNameChanged;
        BluetoothDeviceEventReceiver.UuidChanged += ReceiverOnUuidChanged;
    }

    private void ReceiverOnUuidChanged(object? sender, BluetoothDeviceEventReceiver.UuidEventArgs e)
    {
        try
        {
            // GET DEVICE
            var sharedDevice = Scanner.GetDevice(e.Device);

            // ACTION
            sharedDevice.OnUuidChanged(e.Uuids);
        }
        catch (Exception ex)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, ex);
        }
    }

    private void ReceiverOnNameChanged(object? sender, BluetoothDeviceEventReceiver.NameChangedEventArgs e)
    {
        try
        {
            // GET DEVICE
            var sharedDevice = Scanner.GetDevice(e.Device);

            // ACTION
            sharedDevice.OnNameChanged(e.NewName);
        }
        catch (Exception ex)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, ex);
        }
    }

    private void ReceiverOnDeviceFound(object? sender, BluetoothDeviceEventReceiver.DeviceFoundEventArgs e)
    {
        try
        {
            // ACTION
            Scanner.OnDeviceFound(e.Device);
        }
        catch (Exception ex)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, ex);
        }
    }

    private void ReceiverOnClassChanged(object? sender, BluetoothDeviceEventReceiver.ClassChangedEventArgs e)
    {
        try
        {
            // GET DEVICE
            var sharedDevice = Scanner.GetDevice(e.Device);

            // ACTION
            sharedDevice.OnClassChanged(e.DeviceClass);
        }
        catch (Exception ex)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, ex);
        }
    }

    private void ReceiverOnAclConnected(object? sender, BluetoothDeviceEventReceiver.AclConnectionEventArgs e)
    {
        try
        {
            // GET DEVICE
            var sharedDevice = Scanner.GetDevice(e.Device);

            // ACTION
            sharedDevice.OnAclConnected();
        }
        catch (Exception ex)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, ex);
        }
    }

    private void ReceiverOnAclDisconnected(object? sender, BluetoothDeviceEventReceiver.AclConnectionEventArgs e)
    {
        try
        {
            // GET DEVICE
            var sharedDevice = Scanner.GetDevice(e.Device);

            // ACTION
            sharedDevice.OnAclConnected();
        }
        catch (Exception ex)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, ex);
        }
    }

    private void ReceiverOnPairingRequest(object? sender, BluetoothDeviceEventReceiver.PairingRequestEventArgs e)
    {
        try
        {
            // GET DEVICE
            var sharedDevice = Scanner.GetDevice(e.Device);

            // ACTION
            sharedDevice.OnPairingRequest(e.PairingVariant, e.PassKey);
        }
        catch (Exception ex)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, ex);
        }
    }

    /// <summary>
    /// Handles the bond state changed event from the broadcast receiver.
    /// Retrieves the device and forwards the bond state change notification.
    /// </summary>
    private void ReceiverOnBondStateChanged(object? sender, BluetoothDeviceEventReceiver.BondStateChangedEventArgs args)
    {
        try
        {
            // GET DEVICE
            var sharedDevice = Scanner.GetDevice(args.Device);

            // ACTION
            sharedDevice.OnBondStateChanged(args.PreviousBondState, args.BondState);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }
}
