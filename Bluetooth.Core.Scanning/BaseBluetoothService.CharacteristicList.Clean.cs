
namespace Bluetooth.Core.Scanning;

public abstract partial class BaseBluetoothService
{
    /// <summary>
    /// Clears all characteristics and disposes of them properly.
    /// </summary>
    /// <returns>A task that completes when all characteristics have been cleared and disposed.</returns>
    /// <remarks>
    /// This method iterates through all characteristics, disposes each one, and then clears the collection.
    /// It ensures proper cleanup of resources associated with each characteristic.
    /// </remarks>
    public async ValueTask ClearCharacteristicsAsync()
    {
        foreach (var characteristic in Characteristics)
        {
            await characteristic.DisposeAsync().ConfigureAwait(false);
        }

        lock (Characteristics)
        {
            Characteristics.Clear();
        }
    }
}
