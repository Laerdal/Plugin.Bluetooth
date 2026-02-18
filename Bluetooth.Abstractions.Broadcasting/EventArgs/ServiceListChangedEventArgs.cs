namespace Bluetooth.Abstractions.Broadcasting.EventArgs;

/// <summary>
/// Provides data for the ServiceListChanged event.
/// </summary>
public class ServiceListChangedEventArgs : ItemListChangedEventArgs<IBluetoothLocalService>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceListChangedEventArgs"/> class.
    /// </summary>
    /// <param name="args">The collection changed event arguments.</param>
    public ServiceListChangedEventArgs(NotifyCollectionChangedEventArgs args) : base(args)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceListChangedEventArgs"/> class.
    /// </summary>
    /// <param name="addedItems">The services that were added.</param>
    /// <param name="removedItems">The services that were removed.</param>
    public ServiceListChangedEventArgs(IEnumerable<IBluetoothLocalService>? addedItems, IEnumerable<IBluetoothLocalService>? removedItems) : base(addedItems, removedItems)
    {
    }
}

/// <summary>
/// Provides data for the ServicesAdded event.
/// </summary>
/// <param name="items">The services that were added.</param>
public class ServicesAddedEventArgs(IEnumerable<IBluetoothLocalService> items) : ItemsChangedEventArgs<IBluetoothLocalService>(items);

/// <summary>
/// Provides data for the ServicesRemoved event.
/// </summary>
/// <param name="items">The services that were removed.</param>
public class ServicesRemovedEventArgs(IEnumerable<IBluetoothLocalService> items) : ItemsChangedEventArgs<IBluetoothLocalService>(items);
