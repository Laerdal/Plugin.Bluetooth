namespace Bluetooth.Core.Scanning.CharacteristicAccess;

public abstract partial class CharacteristicAccessService<TRead, TWrite>
{
    /// <inheritdoc />
    public async ValueTask<bool> CanReadAsync(IBluetoothDevice device)
    {
        try
        {
            if (!await HasCharacteristicAsync(device).ConfigureAwait(false))
            {
                return false; // Avoid throwing exceptions if the characteristic doesn't exist
            }

            var characteristic = await GetCharacteristicAsync(device).ConfigureAwait(false);
            return characteristic.CanRead;
        }
        catch (Exception ex)
        {
            // LOG : ERROR - Error checking if characteristic can be read
            _ = ex; // Suppress unused variable warning
            return false;
        }
    }

    /// <inheritdoc />
    public async ValueTask<TRead> ReadAsync(IBluetoothDevice device, bool useLastValueIfPreviouslyRead = false, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        var characteristic = await GetCharacteristicAsync(device, timeout, cancellationToken).ConfigureAwait(false);
        var rawByteArray = await characteristic.ReadValueAsync(useLastValueIfPreviouslyRead, timeout, cancellationToken).ConfigureAwait(false);
        return FromBytes(rawByteArray);
    }
}
