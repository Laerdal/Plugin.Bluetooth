using System.Collections.Specialized;

namespace Bluetooth.Abstractions.EventArgs;

/// <summary>
/// Provides data for collection changed events involving typed items.
/// </summary>
/// <typeparam name="T">The type of items in the collection.</typeparam>
public class ItemListChangedEventArgs<T> : System.EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ItemListChangedEventArgs{T}"/> class.
    /// </summary>
    /// <param name="args">The collection changed event arguments.</param>
    protected ItemListChangedEventArgs(NotifyCollectionChangedEventArgs args)
    {
        ArgumentNullException.ThrowIfNull(args);

        AddedItems = args.NewItems?.Cast<T>() ?? [];
        RemovedItems = args.OldItems?.Cast<T>() ?? [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemListChangedEventArgs{T}"/> class.
    /// </summary>
    /// <param name="addedItems">The items that were added.</param>
    /// <param name="removedItems">The items that were removed.</param>
    protected ItemListChangedEventArgs(IEnumerable<T>? addedItems, IEnumerable<T>? removedItems)
    {
        AddedItems = addedItems;
        RemovedItems = removedItems;
    }

    /// <summary>
    /// Gets the items that were added to the collection.
    /// </summary>
    public IEnumerable<T>? AddedItems { get; }

    /// <summary>
    /// Gets the items that were removed from the collection.
    /// </summary>
    public IEnumerable<T>? RemovedItems { get; }
}
