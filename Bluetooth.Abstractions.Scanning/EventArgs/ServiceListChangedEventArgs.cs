namespace Bluetooth.Abstractions.Scanning.EventArgs;

/// <summary>
///     Provides data for the ServiceListChanged event.
/// </summary>
public class ServiceListChangedEventArgs : ItemListChangedEventArgs<IBluetoothRemoteService>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ServiceListChangedEventArgs" /> class.
    /// </summary>
    /// <param name="args">The collection changed event arguments.</param>
    public ServiceListChangedEventArgs(NotifyCollectionChangedEventArgs args) : base(args)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ServiceListChangedEventArgs" /> class.
    /// </summary>
    /// <param name="addedItems">The services that were added.</param>
    /// <param name="removedItems">The services that were removed.</param>
    public ServiceListChangedEventArgs(IEnumerable<IBluetoothRemoteService>? addedItems, IEnumerable<IBluetoothRemoteService>? removedItems) : base(addedItems, removedItems)
    {
    }
}

/// <summary>
///     Provides data for the ServicesAdded event.
/// </summary>
/// <param name="items">The services that were added.</param>
public class ServicesAddedEventArgs(IEnumerable<IBluetoothRemoteService> items) : ItemsChangedEventArgs<IBluetoothRemoteService>(items);

/// <summary>
///     Provides data for the ServicesRemoved event.
/// </summary>
/// <param name="items">The services that were removed.</param>
public class ServicesRemovedEventArgs(IEnumerable<IBluetoothRemoteService> items) : ItemsChangedEventArgs<IBluetoothRemoteService>(items);