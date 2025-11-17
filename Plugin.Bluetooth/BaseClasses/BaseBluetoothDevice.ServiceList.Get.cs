
namespace Plugin.Bluetooth.BaseClasses;

public abstract partial class BaseBluetoothDevice
{
    private readonly static Func<IBluetoothService, bool> _defaultAcceptAllFilter = _ => true;

    /// <inheritdoc/>
    public IBluetoothService? GetServiceOrDefault(Func<IBluetoothService, bool> filter)
    {
        try
        {
            return Services.SingleOrDefault(filter);
        }
        catch (InvalidOperationException e)
        {
            throw new MultipleServicesFoundException(this, Services.Where(filter), e);
        }
    }

    /// <inheritdoc/>
    public IBluetoothService? GetServiceOrDefault(Guid id)
    {
        return GetServiceOrDefault(service => service.Id == id);
    }

    /// <inheritdoc/>
    public IEnumerable<IBluetoothService> GetServices(Func<IBluetoothService, bool>? filter = null)
    {
        filter ??= _defaultAcceptAllFilter;

        lock (Services)
        {
            foreach (var service in Services)
            {
                if (filter.Invoke(service))
                {
                    yield return service;
                }
            }
        }
    }

    /// <inheritdoc/>
    public IEnumerable<IBluetoothService> GetServices(Guid id)
    {
        return GetServices(service => service.Id == id);
    }

    /// <inheritdoc/>
    public async ValueTask<IBluetoothService?> GetServiceOrDefaultAsync(Func<IBluetoothService, bool> filter, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        await ExploreServicesAsync(false, false, timeout, cancellationToken).ConfigureAwait(false);
        return GetServiceOrDefault(filter);
    }

    /// <inheritdoc/>
    public ValueTask<IBluetoothService?> GetServiceOrDefaultAsync(Guid id, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        return GetServiceOrDefaultAsync(service => service.Id == id, timeout, cancellationToken);
    }

    /// <inheritdoc/>
    public async ValueTask<IEnumerable<IBluetoothService>> GetServicesAsync(Func<IBluetoothService, bool>? filter = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        await ExploreServicesAsync(false, false, timeout, cancellationToken).ConfigureAwait(false);
        return GetServices(filter);
    }

    /// <inheritdoc/>
    public ValueTask<IEnumerable<IBluetoothService>> GetServicesAsync(Guid id, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        return GetServicesAsync(service => service.Id == id, timeout, cancellationToken);
    }
}
