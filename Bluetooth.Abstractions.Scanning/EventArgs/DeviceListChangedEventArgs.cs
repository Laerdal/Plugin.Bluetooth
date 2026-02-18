namespace Bluetooth.Abstractions.Scanning.EventArgs;

/// <summary>
/// Provides data for the DeviceListChanged event.
/// </summary>
public class DeviceListChangedEventArgs : ItemListChangedEventArgs<IBluetoothRemoteDevice>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeviceListChangedEventArgs"/> class.
    /// </summary>
    /// <param name="args">The collection changed event arguments.</param>
    public DeviceListChangedEventArgs(NotifyCollectionChangedEventArgs args) : base(args)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DeviceListChangedEventArgs"/> class.
    /// </summary>
    /// <param name="addedItems">The devices that were added.</param>
    /// <param name="removedItems">The devices that were removed.</param>
    public DeviceListChangedEventArgs(IEnumerable<IBluetoothRemoteDevice>? addedItems, IEnumerable<IBluetoothRemoteDevice>? removedItems) : base(addedItems, removedItems)
    {
    }
}

/// <summary>
/// Provides data for the DevicesAdded event.
/// </summary>
/// <param name="items">The devices that were added.</param>
public class DevicesAddedEventArgs(IEnumerable<IBluetoothRemoteDevice> items) : ItemsChangedEventArgs<IBluetoothRemoteDevice>(items);

/// <summary>
/// Provides data for the DevicesRemoved event.
/// </summary>
/// <param name="items">The devices that were removed.</param>
public class DevicesRemovedEventArgs(IEnumerable<IBluetoothRemoteDevice> items) : ItemsChangedEventArgs<IBluetoothRemoteDevice>(items);
