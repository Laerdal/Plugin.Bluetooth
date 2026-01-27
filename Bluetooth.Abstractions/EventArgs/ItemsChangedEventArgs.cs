namespace Bluetooth.Abstractions.EventArgs;

/// <summary>
/// Provides data for events involving a collection of typed items.
/// </summary>
/// <typeparam name="T">The type of items in the collection.</typeparam>
public class ItemsChangedEventArgs<T> : System.EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ItemsChangedEventArgs{T}"/> class.
    /// </summary>
    /// <param name="items">The items associated with the event.</param>
    public ItemsChangedEventArgs(IEnumerable<T> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        Items = items;
    }

    /// <summary>
    /// Gets the items associated with the event.
    /// </summary>
    public IEnumerable<T> Items { get; }
}
