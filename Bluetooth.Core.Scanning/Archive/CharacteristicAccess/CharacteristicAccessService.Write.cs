namespace Bluetooth.Core.Scanning.CharacteristicAccess;

public abstract partial class CharacteristicAccessService<TRead, TWrite>
{
    /// <inheritdoc />
    public async ValueTask<bool> CanWriteAsync(IBluetoothDevice device)
    {
        try
        {
            if (!await HasCharacteristicAsync(device).ConfigureAwait(false))
            {
                return false; // Avoid throwing exceptions if the characteristic doesn't exist
            }

            var characteristic = await GetCharacteristicAsync(device).ConfigureAwait(false);
            return characteristic.CanWrite;
        }
        catch (Exception)
        {
            // LOG : ERROR - Error checking if characteristic can be written {ex}
            return false;
        }
    }

    /// <inheritdoc />
    public async ValueTask WriteAsync(IBluetoothDevice device,
        TWrite value,
        bool skipIfOldValueMatchesNewValue = false,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(device);

        var characteristic = await GetCharacteristicAsync(device, timeout, cancellationToken).ConfigureAwait(false);
        if (!characteristic.CanWrite)
        {
            throw new CharacteristicWriteException(characteristic, message: "Characteristic does not support writing");
        }

        await characteristic.WriteValueAsync(ToBytes(value), skipIfOldValueMatchesNewValue, timeout, cancellationToken).ConfigureAwait(false);
    }
}
