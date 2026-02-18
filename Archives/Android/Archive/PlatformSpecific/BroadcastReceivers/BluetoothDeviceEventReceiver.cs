namespace Bluetooth.Maui.PlatformSpecific.BroadcastReceivers;

/// <summary>
/// Broadcast receiver for handling Bluetooth device events such as bonding, connection,
/// and device property changes.
/// </summary>
public class BluetoothDeviceEventReceiver : BaseNativeEventReceiver
{
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix

    /// <summary>
    /// Event arguments for bond state change events.
    /// </summary>
    /// <param name="Device">The Bluetooth device whose bond state changed.</param>
    /// <param name="BondState">The new bond state.</param>
    /// <param name="PreviousBondState">The previous bond state.</param>
    public record struct BondStateChangedEventArgs(Android.Bluetooth.BluetoothDevice Device, Bond BondState, Bond PreviousBondState);

    /// <summary>
    /// Event arguments for device name change events.
    /// </summary>
    /// <param name="Device">The Bluetooth device whose name changed.</param>
    /// <param name="NewName">The new name of the device.</param>
    public record struct NameChangedEventArgs(Android.Bluetooth.BluetoothDevice Device, string? NewName);

    /// <summary>
    /// Event arguments for UUID discovery events.
    /// </summary>
    /// <param name="Device">The Bluetooth device whose UUIDs were discovered.</param>
    /// <param name="Uuids">The collection of discovered UUIDs.</param>
    public record struct UuidEventArgs(Android.Bluetooth.BluetoothDevice Device, IEnumerable<ParcelUuid>? Uuids);

    /// <summary>
    /// Event arguments for pairing request events.
    /// </summary>
    /// <param name="Device">The Bluetooth device requesting pairing.</param>
    /// <param name="PairingVariant">The pairing variant type.</param>
    /// <param name="PassKey">The passkey for pairing, if applicable.</param>
    public record struct PairingRequestEventArgs(Android.Bluetooth.BluetoothDevice Device, int PairingVariant, int? PassKey = null);

    /// <summary>
    /// Event arguments for device class change events.
    /// </summary>
    /// <param name="Device">The Bluetooth device whose class changed.</param>
    /// <param name="DeviceClass">The new device class.</param>
    public record struct ClassChangedEventArgs(Android.Bluetooth.BluetoothDevice Device, BluetoothClass? DeviceClass);

    /// <summary>
    /// Event arguments for device found events.
    /// </summary>
    /// <param name="Device">The Bluetooth device that was found.</param>
    public record struct DeviceFoundEventArgs(Android.Bluetooth.BluetoothDevice Device);

    /// <summary>
    /// Event arguments for ACL connection events.
    /// </summary>
    /// <param name="Device">The Bluetooth device involved in the ACL connection.</param>
    public record struct AclConnectionEventArgs(Android.Bluetooth.BluetoothDevice Device);

#pragma warning restore CA1711 // Identifiers should not have incorrect suffix

    /// <summary>
    /// Initializes a new instance of the <see cref="BluetoothDeviceEventReceiver"/> class.
    /// </summary>
    public BluetoothDeviceEventReceiver() : base([
        Android.Bluetooth.BluetoothDevice.ActionBondStateChanged,
        Android.Bluetooth.BluetoothDevice.ActionAclConnected,
        Android.Bluetooth.BluetoothDevice.ActionAclDisconnected,
        Android.Bluetooth.BluetoothDevice.ActionPairingRequest,
        Android.Bluetooth.BluetoothDevice.ActionNameChanged,
        Android.Bluetooth.BluetoothDevice.ActionUuid
    ])
    {
        RegisterReceiver();
    }

    /// <summary>
    /// Occurs when a device's bond state changes.
    /// </summary>
    public event EventHandler<BondStateChangedEventArgs>? BondStateChanged;

    /// <summary>
    /// Occurs when a Bluetooth device is found during discovery.
    /// </summary>
    public event EventHandler<DeviceFoundEventArgs>? DeviceFound;

    /// <summary>
    /// Occurs when an ACL connection is established.
    /// </summary>
    public event EventHandler<AclConnectionEventArgs>? AclConnected;

    /// <summary>
    /// Occurs when an ACL connection is disconnected.
    /// </summary>
    public event EventHandler<AclConnectionEventArgs>? AclDisconnected;

    /// <summary>
    /// Occurs when an ACL disconnect is requested.
    /// </summary>
    public event EventHandler<AclConnectionEventArgs>? AclDisconnectRequested;

    /// <summary>
    /// Occurs when a pairing request is received.
    /// </summary>
    public event EventHandler<PairingRequestEventArgs>? PairingRequest;

    /// <summary>
    /// Occurs when a device's name changes.
    /// </summary>
    public event EventHandler<NameChangedEventArgs>? NameChanged;

    /// <summary>
    /// Occurs when a device's UUIDs are discovered.
    /// </summary>
    public event EventHandler<UuidEventArgs>? UuidChanged;

    /// <summary>
    /// Occurs when a device's class changes.
    /// </summary>
    public event EventHandler<ClassChangedEventArgs>? ClassChanged;

    /// <inheritdoc/>
    protected override void OnEventReceived(Intent intent)
    {
        if (intent.GetParcelableExtraSafe<Android.Bluetooth.BluetoothDevice>(Android.Bluetooth.BluetoothDevice.ExtraDevice) is not { } device)
        {
            return;
        }

        switch (intent.Action)
        {
            case Android.Bluetooth.BluetoothDevice.ActionFound:
                OnDeviceFound(device);
                break;

            case Android.Bluetooth.BluetoothDevice.ActionBondStateChanged:
                var bondState = (Bond) intent.GetIntExtra(Android.Bluetooth.BluetoothDevice.ExtraBondState, -1);
                var previousBondState = (Bond) intent.GetIntExtra(Android.Bluetooth.BluetoothDevice.ExtraPreviousBondState, -1);
                OnBondStateChanged(new BondStateChangedEventArgs(device, bondState, previousBondState));
                break;

            case Android.Bluetooth.BluetoothDevice.ActionAclConnected:
                OnAclConnected(device);
                break;

            case Android.Bluetooth.BluetoothDevice.ActionAclDisconnected:
                OnAclDisconnected(device);
                break;

            case Android.Bluetooth.BluetoothDevice.ActionAclDisconnectRequested:
                OnAclDisconnectRequested(device);
                break;

            case Android.Bluetooth.BluetoothDevice.ActionPairingRequest:
                var variant = intent.GetIntExtra(Android.Bluetooth.BluetoothDevice.ExtraPairingVariant, -1);
                int? passkey = intent.HasExtra(Android.Bluetooth.BluetoothDevice.ExtraPairingKey) ? intent.GetIntExtra(Android.Bluetooth.BluetoothDevice.ExtraPairingKey, -1) : null;
                OnPairingRequest(new PairingRequestEventArgs(device, variant, passkey));
                break;

            case Android.Bluetooth.BluetoothDevice.ActionNameChanged:
                var newName = intent.GetStringExtra(Android.Bluetooth.BluetoothDevice.ExtraName);
                OnNameChanged(new NameChangedEventArgs(device, newName));
                break;

            case Android.Bluetooth.BluetoothDevice.ActionUuid:
                var uuidExtras = intent.GetParcelableArrayListExtraSafe<ParcelUuid>(Android.Bluetooth.BluetoothDevice.ExtraUuid);
                OnUuidChanged(new UuidEventArgs(device, uuidExtras?.ToList()));
                break;
            case Android.Bluetooth.BluetoothDevice.ActionClassChanged:
                var deviceClass = intent.GetParcelableExtraSafe<BluetoothClass>(Android.Bluetooth.BluetoothDevice.ExtraClass);
                OnClassChanged(new ClassChangedEventArgs(device, deviceClass));
                break;
        }
    }

    /// <summary>
    /// Raises the <see cref="BondStateChanged"/> event.
    /// </summary>
    /// <param name="e">The event arguments.</param>
    protected virtual void OnBondStateChanged(BondStateChangedEventArgs e)
    {
        BondStateChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the <see cref="DeviceFound"/> event.
    /// </summary>
    /// <param name="device">The device that was found.</param>
    protected virtual void OnDeviceFound(Android.Bluetooth.BluetoothDevice device)
    {
        DeviceFound?.Invoke(this, new DeviceFoundEventArgs(device));
    }

    /// <summary>
    /// Raises the <see cref="AclConnected"/> event.
    /// </summary>
    /// <param name="device">The device that connected.</param>
    protected virtual void OnAclConnected(Android.Bluetooth.BluetoothDevice device)
    {
        AclConnected?.Invoke(this, new AclConnectionEventArgs(device));
    }

    /// <summary>
    /// Raises the <see cref="AclDisconnected"/> event.
    /// </summary>
    /// <param name="device">The device that disconnected.</param>
    protected virtual void OnAclDisconnected(Android.Bluetooth.BluetoothDevice device)
    {
        AclDisconnected?.Invoke(this, new AclConnectionEventArgs(device));
    }

    /// <summary>
    /// Raises the <see cref="AclDisconnectRequested"/> event.
    /// </summary>
    /// <param name="device">The device for which disconnect was requested.</param>
    protected virtual void OnAclDisconnectRequested(Android.Bluetooth.BluetoothDevice device)
    {
        AclDisconnectRequested?.Invoke(this, new AclConnectionEventArgs(device));
    }

    /// <summary>
    /// Raises the <see cref="PairingRequest"/> event.
    /// </summary>
    /// <param name="e">The event arguments.</param>
    protected virtual void OnPairingRequest(PairingRequestEventArgs e)
    {
        PairingRequest?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the <see cref="NameChanged"/> event.
    /// </summary>
    /// <param name="e">The event arguments.</param>
    protected virtual void OnNameChanged(NameChangedEventArgs e)
    {
        NameChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the <see cref="UuidChanged"/> event.
    /// </summary>
    /// <param name="e">The event arguments.</param>
    protected virtual void OnUuidChanged(UuidEventArgs e)
    {
        UuidChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the <see cref="ClassChanged"/> event.
    /// </summary>
    /// <param name="e">The event arguments.</param>
    protected virtual void OnClassChanged(ClassChangedEventArgs e)
    {
        ClassChanged?.Invoke(this, e);
    }
}
