using System.Collections.ObjectModel;

using Bluetooth.Abstractions.Broadcasting;
using Bluetooth.Core.Broadcasting.Exceptions;

namespace Bluetooth.Core.Broadcasting;

public abstract partial class BaseBluetoothBroadcaster
{
    /// <inheritdoc/>
    public ReadOnlyDictionary<Guid, IBluetoothBroadcastService> Services { get; }

    private Dictionary<Guid, IBluetoothBroadcastService> WritableServiceList { get; } = new Dictionary<Guid, IBluetoothBroadcastService>();

    /// <inheritdoc/>
    public async Task<IBluetoothBroadcastService> AddServiceAsync(Guid id,
        string name,
        bool isPrimary = true,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        if (WritableServiceList.ContainsKey(id))
        {
            throw new BroadcasterServiceAlreadyExistsException(this, id);
        }
        var newServiceRequest = new IBluetoothBroadcastServiceFactory.BluetoothBroadcastServiceFactoryRequest()
        {
            Id = id,
            Name = name,
            IsPrimary = isPrimary,
        };
        var newService = await ServiceFactory.CreateBroadcastServiceAsync(this, newServiceRequest, timeout, cancellationToken).ConfigureAwait(false);
        lock (WritableServiceList)
        {
            WritableServiceList.Add(id, newService);
        }
        return newService;
    }

    /// <inheritdoc/>
    public Task RemoveServiceAsync(Guid id, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        if (!WritableServiceList.TryGetValue(id, out var characteristic))
        {
            throw new BroadcasterServiceNotFoundException(this, id);
        }
        return RemoveServiceAsync(characteristic, timeout, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task RemoveServiceAsync(IBluetoothBroadcastService service, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(service);
        var id = service.Id;
        if (!WritableServiceList.ContainsKey(id))
        {
            throw new BroadcasterServiceNotFoundException(this, id);
        }
        await service.DisposeAsync().ConfigureAwait(false);
        WritableServiceList.Remove(service.Id);
    }

    /// <inheritdoc/>
    public async Task RemoveAllServicesAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        foreach (var characteristic in WritableServiceList.Values.ToList())
        {
            await RemoveServiceAsync(characteristic, timeout, cancellationToken).ConfigureAwait(false);
        }
    }

}
