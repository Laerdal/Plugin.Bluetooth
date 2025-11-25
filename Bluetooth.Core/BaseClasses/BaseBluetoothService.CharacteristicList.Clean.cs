
namespace Bluetooth.Core.BaseClasses;

public abstract partial class BaseBluetoothService
{
    /// <summary>
    /// Clears all characteristics and disposes of them properly.
    /// </summary>
    /// <returns>A task that completes when all characteristics have been cleared and disposed.</returns>
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
