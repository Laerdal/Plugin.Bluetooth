using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

using Bluetooth.Abstractions.EventArgs;
using Bluetooth.Abstractions.Exceptions;

using Plugin.BaseTypeExtensions;

namespace Bluetooth.Core;

/// <summary>
/// Interface for managing a repository of Bluetooth objects with exploration and collection management capabilities.
/// </summary>
/// <typeparam name="T">The type of objects managed by this repository.</typeparam>
public interface IBaseObjectRepository<T> : INotifyPropertyChanged, IAsyncDisposable where T : class
{
    /// <summary>
    /// Gets a value indicating whether the repository is currently exploring/discovering items.
    /// </summary>
    bool IsExploring { get; }

    /// <summary>
    /// Asynchronously explores and discovers items for this repository.
    /// </summary>
    /// <param name="force">If true, forces re-exploration even if items already exist.</param>
    /// <param name="timeout">Optional timeout for the exploration operation.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous exploration operation.</returns>
    Task ExploreAsync(bool force = false, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Raised when the item list changes (items added, removed, or replaced).
    /// </summary>
    event EventHandler<ItemListChangedEventArgs<T>>? ListChanged;

    /// <summary>
    /// Raised when items are added to the repository.
    /// </summary>
    event EventHandler<ItemsChangedEventArgs<T>>? Added;

    /// <summary>
    /// Raised when items are removed from the repository.
    /// </summary>
    event EventHandler<ItemsChangedEventArgs<T>>? Removed;

    /// <summary>
    /// Asynchronously clears all items from the repository, disposing them if they implement IDisposable or IAsyncDisposable.
    /// </summary>
    /// <returns>A ValueTask representing the asynchronous clear operation.</returns>
    ValueTask ClearAsync();

    /// <summary>
    /// Gets a single item matching the specified filter, or throws an exception if no match is found.
    /// </summary>
    /// <param name="filter">The filter predicate to match items.</param>
    /// <returns>The matching item.</returns>
    /// <exception cref="ItemNotFoundException{T}">Thrown when no item matching the filter is found.</exception>
    /// <exception cref="MultipleItemsFoundException{T}">Thrown when multiple items match the filter.</exception>
    T GetItem(Func<T, bool> filter);

    /// <summary>
    /// Gets a single item matching the specified filter, or null if no match is found.
    /// </summary>
    /// <param name="filter">The filter predicate to match items.</param>
    /// <returns>The matching item, or null if no match is found.</returns>
    /// <exception cref="MultipleItemsFoundException{T}">Thrown when multiple items match the filter.</exception>
    T? GetItemOrDefault(Func<T, bool> filter);

    /// <summary>
    /// Gets all items matching the specified filter, or all items if no filter is provided.
    /// </summary>
    /// <param name="filter">Optional filter predicate to match items. If null, returns all items.</param>
    /// <returns>An enumerable collection of matching items.</returns>
    IEnumerable<T> GetItems(Func<T, bool>? filter = null);
}

/// <summary>
/// Base class for managing a repository of Bluetooth objects with exploration and collection management capabilities.
/// </summary>
/// <typeparam name="T">The type of objects managed by this repository.</typeparam>
public abstract class BaseObjectRepository<T> : BaseBindableObject, IAsyncDisposable, IBaseObjectRepository<T> where T : class
{
    /// <summary>
    /// Gets a value indicating whether the repository is currently exploring/discovering items.
    /// </summary>
    public bool IsExploring
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the TaskCompletionSource used to coordinate exploration operations.
    /// </summary>
    private TaskCompletionSource? ExplorationTcs
    {
        get => GetValue<TaskCompletionSource?>(null);
        set => SetValue(value);
    }

    /// <summary>
    /// Gets the observable collection of items managed by this repository.
    /// </summary>
    private ObservableCollection<T> Items
    {
        get
        {
            if (field == null)
            {
                field = [];
                field.CollectionChanged += OnCollectionChanged;
            }

            return field;
        }
    }

    /// <summary>
    /// Handles successful completion of native exploration by updating the repository items.
    /// </summary>
    /// <typeparam name="TNative">The native type returned from the platform-specific exploration.</typeparam>
    /// <param name="characteristics">The list of native objects discovered during exploration.</param>
    /// <param name="fromInputTypeToOutputTypeConversion">Function to convert from native type to repository type.</param>
    /// <param name="areRepresentingTheSameObject">Function to determine if a native object and repository object represent the same entity.</param>
    protected void OnNativeExplorationSucceeded<TNative>(IList<TNative> characteristics, Func<TNative, T> fromInputTypeToOutputTypeConversion, Func<TNative, T, bool> areRepresentingTheSameObject)
    {
        Items.UpdateFrom(characteristics, areRepresentingTheSameObject, fromInputTypeToOutputTypeConversion);

        // Attempt to dispatch exception to the TaskCompletionSource
        var success = ExplorationTcs?.TrySetResult() ?? false;
        if (!success)
        {
            throw new InvalidOperationException("Failed to set exploration exception, TaskCompletionSource was already completed.");
        }
    }

    /// <summary>
    /// Handles failed exploration by propagating the exception to waiting tasks.
    /// </summary>
    /// <param name="e">The exception that occurred during exploration.</param>
    protected void OnNativeExplorationFailed(Exception e)
    {
        // Attempt to dispatch exception to the TaskCompletionSource
        var success = ExplorationTcs?.TrySetException(e) ?? false;
        if (!success)
        {
            throw new InvalidOperationException("Failed to set exploration exception, TaskCompletionSource was already completed.", e);
        }
    }

    /// <summary>
    /// Asynchronously explores and discovers items for this repository.
    /// </summary>
    /// <param name="force">If true, forces re-exploration even if items already exist.</param>
    /// <param name="timeout">Optional timeout for the exploration operation.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous exploration operation.</returns>
    public async Task ExploreAsync(bool force = false, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        // Check if characteristics have already been explored
        if (Items.Any() && !force)
        {
            return;
        }

        // Prevents multiple calls to ReadValueAsync, if already exploring, we merge the calls
        if (ExplorationTcs is { Task.IsCompleted: false })
        {
            await ExplorationTcs.Task.ConfigureAwait(false);
            return;
        }

        ExplorationTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously); // Reset the TCS
        IsExploring = true; // Set the flag to true

        try // try-catch to dispatch exceptions rising from start
        {
            // Check if characteristics need to be cleaned
            if (Items.Any() && force)
            {
                await ClearAsync().ConfigureAwait(false);
            }

            await NativeExplorationAsync(timeout, cancellationToken).ConfigureAwait(false); // actual characteristic exploration native call
        }
        catch (Exception e)
        {
            OnNativeExplorationFailed(e); // if exception is thrown during start, we trigger the failure
        }

        // try-finally to ensure disposal and release of resources
        try
        {
            // Wait for OnCharacteristicsExplorationSucceeded to be called
            await ExplorationTcs.Task.WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            IsExploring = false; // Reset the flag
            ExplorationTcs = null;
        }
    }

    /// <summary>
    /// Performs the platform-specific exploration operation. Must be implemented by derived classes.
    /// </summary>
    /// <param name="timeout">Optional timeout for the exploration operation.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A ValueTask representing the asynchronous exploration operation.</returns>
    protected abstract ValueTask NativeExplorationAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Raised when the item list changes (items added, removed, or replaced).
    /// </summary>
    public event EventHandler<ItemListChangedEventArgs<T>>? ListChanged;

    /// <summary>
    /// Raised when items are added to the repository.
    /// </summary>
    public event EventHandler<ItemsChangedEventArgs<T>>? Added;

    /// <summary>
    /// Raised when items are removed from the repository.
    /// </summary>
    public event EventHandler<ItemsChangedEventArgs<T>>? Removed;

    /// <summary>
    /// Handles collection changed events and raises appropriate Added, Removed, and ListChanged events.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="ea">Event arguments containing change information.</param>
    protected void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs ea)
    {
        var listChangedEventArgs = new ItemListChangedEventArgs<T>(ea);
        if (listChangedEventArgs.AddedItems != null)
        {
            Added?.Invoke(this, new ItemsChangedEventArgs<T>(listChangedEventArgs.AddedItems));
        }
        if (listChangedEventArgs.RemovedItems != null)
        {
            Removed?.Invoke(this, new ItemsChangedEventArgs<T>(listChangedEventArgs.RemovedItems));
        }
        ListChanged?.Invoke(this, listChangedEventArgs);
    }

    /// <summary>
    /// Asynchronously clears all items from the repository, disposing them if they implement IDisposable or IAsyncDisposable.
    /// </summary>
    /// <returns>A ValueTask representing the asynchronous clear operation.</returns>
    public async ValueTask ClearAsync()
    {
        foreach (var item in Items)
        {
            if (item is IAsyncDisposable asyncDisposableItem)
            {
                await asyncDisposableItem.DisposeAsync().ConfigureAwait(false);
            }
            else if (item is IDisposable disposableItem)
            {
                disposableItem.Dispose();
            }
        }

        lock (Items)
        {
            Items.Clear();
        }
    }

    private readonly static Func<T, bool> _defaultAcceptAllFilter = _ => true;

    /// <summary>
    /// Gets a single item matching the specified filter, or throws an exception if no match is found.
    /// </summary>
    /// <param name="filter">The filter predicate to match items.</param>
    /// <returns>The matching item.</returns>
    /// <exception cref="ItemNotFoundException{T}">Thrown when no item matching the filter is found.</exception>
    /// <exception cref="MultipleItemsFoundException{T}">Thrown when multiple items match the filter.</exception>
    public T GetItem(Func<T, bool> filter)
    {
        try
        {
            return Items.Single(filter);
        }
        catch (InvalidOperationException e)
        {
            if (!Items.Any(filter))
            {
                throw new ItemNotFoundException<T>();
            }
            else
            {
                throw new MultipleItemsFoundException<T>(Items.Where(filter).ToList(), e);
            }
        }
    }

    /// <summary>
    /// Gets a single item matching the specified filter, or null if no match is found.
    /// </summary>
    /// <param name="filter">The filter predicate to match items.</param>
    /// <returns>The matching item, or null if no match is found.</returns>
    /// <exception cref="MultipleItemsFoundException{T}">Thrown when multiple items match the filter.</exception>
    public T? GetItemOrDefault(Func<T, bool> filter)
    {
        try
        {
            return Items.SingleOrDefault(filter);
        }
        catch (InvalidOperationException e)
        {
            throw new MultipleItemsFoundException<T>(Items.Where(filter), e);
        }
    }

    /// <summary>
    /// Gets all items matching the specified filter, or all items if no filter is provided.
    /// </summary>
    /// <param name="filter">Optional filter predicate to match items. If null, returns all items.</param>
    /// <returns>An enumerable collection of matching items.</returns>
    public IEnumerable<T> GetItems(Func<T, bool>? filter = null)
    {
        filter ??= _defaultAcceptAllFilter;
        IEnumerable<T> output;

        lock (Items)
        {
            output = Items.Where(filter).ToArray(); // ToArray() is important, creates a new array.
        }

        return output;
    }

    /// <summary>
    /// Asynchronously disposes the repository by clearing all items and releasing resources.
    /// </summary>
    /// <returns>A ValueTask representing the asynchronous dispose operation.</returns>
    protected virtual ValueTask DisposeAsyncCore()
    {
        return ClearAsync();
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// Base class for managing a repository of Bluetooth objects with typed native object conversion.
/// </summary>
/// <typeparam name="T">The type of objects managed by this repository.</typeparam>
/// <typeparam name="TNative">The native platform-specific type.</typeparam>
public abstract class BaseObjectRepository<T, TNative> : BaseObjectRepository<T> where T : class
{
    /// <summary>
    /// Converts a native platform-specific object to the repository type. Must be implemented by derived classes.
    /// </summary>
    /// <param name="native">The native object to convert.</param>
    /// <returns>The converted repository object.</returns>
    protected abstract T FromNative(TNative native);

    /// <summary>
    /// Determines whether a native object and a repository object represent the same entity. Must be implemented by derived classes.
    /// </summary>
    /// <param name="native">The native object to compare.</param>
    /// <param name="item">The repository object to compare.</param>
    /// <returns>True if the objects represent the same entity; otherwise, false.</returns>
    protected abstract bool AreRepresentingTheSameObject(TNative native, T item);

    /// <summary>
    /// Handles successful completion of native exploration by updating the repository items using the typed conversion methods.
    /// </summary>
    /// <param name="natives">The list of native objects discovered during exploration.</param>
    protected void OnNativeExplorationSucceeded(IList<TNative> natives)
    {
        base.OnNativeExplorationSucceeded(natives, FromNative, AreRepresentingTheSameObject);
    }
}
