namespace Bluetooth.Abstractions.Broadcasting.EventArgs;

/// <summary>
///     Provides data for the ClientDeviceListChanged event.
/// </summary>
public class ClientDeviceListChangedEventArgs : ItemListChangedEventArgs<IBluetoothConnectedDevice>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ClientDeviceListChangedEventArgs" /> class.
    /// </summary>
    /// <param name="args">The collection changed event arguments.</param>
    public ClientDeviceListChangedEventArgs(NotifyCollectionChangedEventArgs args) : base(args)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ClientDeviceListChangedEventArgs" /> class.
    /// </summary>
    /// <param name="addedItems">The devices that were added.</param>
    /// <param name="removedItems">The devices that were removed.</param>
    public ClientDeviceListChangedEventArgs(IEnumerable<IBluetoothConnectedDevice>? addedItems, IEnumerable<IBluetoothConnectedDevice>? removedItems) : base(addedItems, removedItems)
    {
    }
}

/// <summary>
///     Provides data for the DevicesAdded event.
/// </summary>
/// <param name="items">The devices that were added.</param>
public class ClientDevicesAddedEventArgs(IEnumerable<IBluetoothConnectedDevice> items) : ItemsChangedEventArgs<IBluetoothConnectedDevice>(items);

/// <summary>
///     Provides data for the DevicesRemoved event.
/// </summary>
/// <param name="items">The devices that were removed.</param>
public class ClientDevicesRemovedEventArgs(IEnumerable<IBluetoothConnectedDevice> items) : ItemsChangedEventArgs<IBluetoothConnectedDevice>(items);