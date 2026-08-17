using Microsoft.Extensions.Caching.Memory;

namespace Bluetooth.Maui.Platforms.Win.Scanning;

/// <summary>
/// LRU(-ish) cache for associating bluetooth-addresses with manufacturers, with a size limit and expiration time.
/// </summary>
public sealed partial class ManufacturerCache : IDisposable
{
    private readonly MemoryCache _cachedManufacturerData;
    private readonly TimeSpan _absoluteExpirationRelativeToNow;

    /// <summary>
    /// LRU(-ish) cache for associating bluetooth-addresses with manufacturers, with a size limit and expiration time.
    /// </summary>
    /// <param name="sizeLimit">Maximum number of entries in the cache.</param>
    /// <param name="expirationTime">The time after which a cache entry expires.</param>
    public ManufacturerCache(int sizeLimit, TimeSpan expirationTime)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(sizeLimit);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(expirationTime, TimeSpan.Zero);

        _cachedManufacturerData = new MemoryCache(new MemoryCacheOptions
        {
            SizeLimit = sizeLimit,
        });
        _absoluteExpirationRelativeToNow = expirationTime;
    }

    /// <summary>
    /// Retrieves the manufacturer data for a given Bluetooth address from the cache, if it exists.
    /// </summary>
    /// <param name="bluetoothAddress">The Bluetooth address for which to retrieve manufacturer data.</param>
    /// <returns>The manufacturer if found; otherwise, null.</returns>
    public Manufacturer? GetManufacturerData(ulong bluetoothAddress)
    {
        if (_cachedManufacturerData.TryGetValue(bluetoothAddress, out Manufacturer manufacturer))
        {
            return manufacturer;
        }

        return null;
    }

    /// <summary>
    /// Caches the manufacturer data for a given Bluetooth address.
    /// </summary>
    /// <param name="bluetoothAddress">The Bluetooth address for which to cache manufacturer data.</param>
    /// <param name="manufacturer">The manufacturer to cache.</param>
    public void SetManufacturerData(ulong bluetoothAddress, Manufacturer manufacturer)
    {
        _cachedManufacturerData.Set(bluetoothAddress, manufacturer, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _absoluteExpirationRelativeToNow,
            Size = 1
        });
    }

    /// <summary>
    ///     Disposes the underlying memory cache.
    /// </summary>
    public void Dispose()
    {
        _cachedManufacturerData.Dispose();
    }
}
