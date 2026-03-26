using Bluetooth.Abstractions;

namespace Bluetooth.Core;

/// <summary>
/// Extension methods for <see cref="BaseObjectRepository{T}"/> to simplify working with items that have identifiers.
/// </summary>
public static class BaseObjectRepositoryExtensions
{
    #region ID-based GetItem and GetItems overloads

    /// <summary>
    /// Gets the item with the specified identifier, or null if no match is found.
    /// </summary>
    /// <typeparam name="T">The type of objects in the repository.</typeparam>
    /// <typeparam name="TId">The type of the identifier.</typeparam>
    /// <param name="repo">The repository to search.</param>
    /// <param name="id">The identifier to search for.</param>
    /// <returns>The item with the specified identifier, or null if no match is found.</returns>
    public static T? GetItemOrDefault<T, TId>(this BaseObjectRepository<T> repo, TId id) where T : class, IBluetoothObjectWithId<TId>
                                                                                         where TId : IEquatable<TId>
    {
        ArgumentNullException.ThrowIfNull(repo);
        return repo.GetItemOrDefault(i => i.Id.Equals(id));
    }

    /// <summary>
    /// Gets all items with the specified identifier.
    /// </summary>
    /// <typeparam name="T">The type of objects in the repository.</typeparam>
    /// <typeparam name="TId">The type of the identifier.</typeparam>
    /// <param name="repo">The repository to search.</param>
    /// <param name="id">The identifier to search for.</param>
    /// <returns>An enumerable collection of items with the specified identifier.</returns>
    public static IEnumerable<T> GetItems<T, TId>(this BaseObjectRepository<T> repo, TId id) where T : class, IBluetoothObjectWithId<TId>
                                                                                             where TId : IEquatable<TId>
    {
        ArgumentNullException.ThrowIfNull(repo);
        return repo.GetItems(i => i.Id.Equals(id));
    }

    #endregion

}
