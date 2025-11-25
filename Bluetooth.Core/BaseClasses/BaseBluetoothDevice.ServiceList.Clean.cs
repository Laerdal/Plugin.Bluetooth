
namespace Bluetooth.Core.BaseClasses;

public abstract partial class BaseBluetoothDevice
{
    /// <summary>
    /// Clears all services and their characteristics, disposing of them properly.
    /// </summary>
    /// <returns>A task that completes when all services have been cleared and disposed.</returns>
    public async ValueTask ClearServicesAsync()
    {
        foreach (var service in Services)
        {
            await service.ClearCharacteristicsAsync().ConfigureAwait(false);
            await service.DisposeAsync().ConfigureAwait(false);
        }

        lock (Services)
        {
            Services.Clear();
        }
    }
}
