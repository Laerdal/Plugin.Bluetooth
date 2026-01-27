
using Bluetooth.Abstractions.Scanning;

namespace Bluetooth.Core.Scanning;

public abstract partial class BaseBluetoothDevice
{
    /// <inheritdoc/>
    public bool HasService(Guid id)
    {
        return HasService(service => service.Id == id);
    }

    /// <inheritdoc/>
    public bool HasService(Func<IBluetoothService, bool> filter)
    {
        return Services.Any(filter);
    }

    /// <inheritdoc/>
    public ValueTask<bool> HasServiceAsync(Guid id, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        return HasServiceAsync(service => service.Id == id, timeout, cancellationToken);
    }

    /// <inheritdoc/>
    public async ValueTask<bool> HasServiceAsync(Func<IBluetoothService, bool> filter, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        if (HasService(filter))
        {
            return true;
        }
        await ExploreServicesAsync(false, true, timeout, cancellationToken).ConfigureAwait(false);

        return HasService(filter);
    }

}
