namespace Bluetooth.Core.BaseClasses;

public abstract partial class BaseBluetoothService : BaseBindableObject, IBluetoothService
{
    /// <inheritdoc/>
    public bool HasCharacteristic(Guid id)
    {
        return HasCharacteristic(characteristic => characteristic.Id == id);
    }

    /// <inheritdoc/>
    public bool HasCharacteristic(Func<IBluetoothCharacteristic, bool> filter)
    {
        return Characteristics.Any(filter);
    }

    /// <inheritdoc/>
    public ValueTask<bool> HasCharacteristicAsync(Guid id, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        return HasCharacteristicAsync(characteristic => characteristic.Id == id, timeout, cancellationToken);
    }

    /// <inheritdoc/>
    public async ValueTask<bool> HasCharacteristicAsync(Func<IBluetoothCharacteristic, bool> filter, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        if (HasCharacteristic(filter))
        {
            return true;
        }
        await ExploreCharacteristicsAsync(false, timeout, cancellationToken).ConfigureAwait(false);
        return HasCharacteristic(filter);
    }
}
