using Bluetooth.Abstractions.Scanning;

namespace Bluetooth.Core.Scanning;

public abstract partial class BaseBluetoothService
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
    /// <remarks>
    /// This method automatically explores characteristics if they haven't been explored yet.
    /// </remarks>
    public ValueTask<bool> HasCharacteristicAsync(Guid id, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        return HasCharacteristicAsync(characteristic => characteristic.Id == id, timeout, cancellationToken);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// This method automatically explores characteristics if they haven't been explored yet.
    /// </remarks>
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
