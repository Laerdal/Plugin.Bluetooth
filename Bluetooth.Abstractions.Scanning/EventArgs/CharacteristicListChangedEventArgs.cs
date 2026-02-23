namespace Bluetooth.Abstractions.Scanning.EventArgs;

/// <summary>
///     Provides data for the CharacteristicListChanged event.
/// </summary>
public class CharacteristicListChangedEventArgs : ItemListChangedEventArgs<IBluetoothRemoteCharacteristic>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicListChangedEventArgs" /> class.
    /// </summary>
    /// <param name="args">The collection changed event arguments.</param>
    public CharacteristicListChangedEventArgs(NotifyCollectionChangedEventArgs args) : base(args)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicListChangedEventArgs" /> class.
    /// </summary>
    /// <param name="addedItems">The characteristics that were added.</param>
    /// <param name="removedItems">The characteristics that were removed.</param>
    public CharacteristicListChangedEventArgs(IEnumerable<IBluetoothRemoteCharacteristic>? addedItems, IEnumerable<IBluetoothRemoteCharacteristic>? removedItems) : base(addedItems, removedItems)
    {
    }
}

/// <summary>
///     Provides data for the CharacteristicsAdded event.
/// </summary>
/// <param name="items">The characteristics that were added.</param>
public class CharacteristicsAddedEventArgs(IEnumerable<IBluetoothRemoteCharacteristic> items) : ItemsChangedEventArgs<IBluetoothRemoteCharacteristic>(items);

/// <summary>
///     Provides data for the CharacteristicsRemoved event.
/// </summary>
/// <param name="items">The characteristics that were removed.</param>
public class CharacteristicsRemovedEventArgs(IEnumerable<IBluetoothRemoteCharacteristic> items) : ItemsChangedEventArgs<IBluetoothRemoteCharacteristic>(items);
