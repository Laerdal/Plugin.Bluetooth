using Bluetooth.Abstractions.Scanning;
using Bluetooth.Core.Exceptions;
using Bluetooth.Core.Scanning.Exceptions;

namespace Bluetooth.Core.Scanning;

public abstract partial class BaseBluetoothService
{

    private readonly static Func<IBluetoothCharacteristic, bool> _defaultAcceptAllFilter = _ => true;

    /// <inheritdoc/>
    /// <exception cref="MultipleCharacteristicsFoundException">Thrown when multiple characteristics match the filter.</exception>
    public IBluetoothCharacteristic? GetCharacteristicOrDefault(Func<IBluetoothCharacteristic, bool> filter)
    {
        try
        {
            return Characteristics.SingleOrDefault(filter);
        }
        catch (InvalidOperationException e)
        {
            throw new MultipleCharacteristicsFoundException(this, Characteristics.Where(filter), e);
        }
    }

    /// <inheritdoc/>
    /// <exception cref="MultipleCharacteristicsFoundException">Thrown when multiple characteristics match the specified ID.</exception>
    public IBluetoothCharacteristic? GetCharacteristicOrDefault(Guid id)
    {
        return GetCharacteristicOrDefault(characteristic => characteristic.Id == id);
    }

    /// <inheritdoc/>
    public IEnumerable<IBluetoothCharacteristic> GetCharacteristics(Func<IBluetoothCharacteristic, bool>? filter = null)
    {
        filter ??= _defaultAcceptAllFilter;
        IEnumerable<IBluetoothCharacteristic> output;

        lock (Characteristics)
        {
            output = Characteristics.Where(filter).ToArray(); // ToArray() is important, creates a new array.
        }

        return output;
    }

    /// <inheritdoc/>
    public IEnumerable<IBluetoothCharacteristic> GetCharacteristics(Guid id)
    {
        return GetCharacteristics(characteristic => characteristic.Id == id);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// This method automatically explores characteristics if they haven't been explored yet.
    /// </remarks>
    /// <exception cref="MultipleCharacteristicsFoundException">Thrown when multiple characteristics match the filter.</exception>
    public async ValueTask<IBluetoothCharacteristic?> GetCharacteristicOrDefaultAsync(Func<IBluetoothCharacteristic, bool> filter, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        await ExploreCharacteristicsAsync(false, timeout, cancellationToken).ConfigureAwait(false);
        return GetCharacteristicOrDefault(filter);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// This method automatically explores characteristics if they haven't been explored yet.
    /// </remarks>
    /// <exception cref="MultipleCharacteristicsFoundException">Thrown when multiple characteristics match the specified ID.</exception>
    public ValueTask<IBluetoothCharacteristic?> GetCharacteristicOrDefaultAsync(Guid id, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        return GetCharacteristicOrDefaultAsync(characteristic => characteristic.Id == id, timeout, cancellationToken);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// This method automatically explores characteristics if they haven't been explored yet.
    /// </remarks>
    public async ValueTask<IEnumerable<IBluetoothCharacteristic>> GetCharacteristicsAsync(Func<IBluetoothCharacteristic, bool>? filter = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        await ExploreCharacteristicsAsync(false, timeout, cancellationToken).ConfigureAwait(false);
        return GetCharacteristics(filter);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// This method automatically explores characteristics if they haven't been explored yet.
    /// </remarks>
    public ValueTask<IEnumerable<IBluetoothCharacteristic>> GetCharacteristicsAsync(Guid id, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        return GetCharacteristicsAsync(characteristic => characteristic.Id == id, timeout, cancellationToken);
    }
}
