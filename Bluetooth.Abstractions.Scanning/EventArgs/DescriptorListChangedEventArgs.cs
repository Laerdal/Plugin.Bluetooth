namespace Bluetooth.Abstractions.Scanning.EventArgs;

/// <summary>
/// Provides data for the DescriptorListChanged event.
/// </summary>
public class DescriptorListChangedEventArgs : ItemListChangedEventArgs<IBluetoothRemoteDescriptor>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DescriptorListChangedEventArgs"/> class.
    /// </summary>
    /// <param name="args">The collection changed event arguments.</param>
    public DescriptorListChangedEventArgs(NotifyCollectionChangedEventArgs args) : base(args)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DescriptorListChangedEventArgs"/> class.
    /// </summary>
    /// <param name="addedItems">The devices that were added.</param>
    /// <param name="removedItems">The devices that were removed.</param>
    public DescriptorListChangedEventArgs(IEnumerable<IBluetoothRemoteDescriptor>? addedItems, IEnumerable<IBluetoothRemoteDescriptor>? removedItems) : base(addedItems, removedItems)
    {
    }
}

/// <summary>
/// Provides data for the DescriptorsAdded event.
/// </summary>
/// <param name="items">The devices that were added.</param>
public class DescriptorsAddedEventArgs(IEnumerable<IBluetoothRemoteDescriptor> items) : ItemsChangedEventArgs<IBluetoothRemoteDescriptor>(items);

/// <summary>
/// Provides data for the DescriptorsRemoved event.
/// </summary>
/// <param name="items">The devices that were removed.</param>
public class DescriptorsRemovedEventArgs(IEnumerable<IBluetoothRemoteDescriptor> items) : ItemsChangedEventArgs<IBluetoothRemoteDescriptor>(items);
