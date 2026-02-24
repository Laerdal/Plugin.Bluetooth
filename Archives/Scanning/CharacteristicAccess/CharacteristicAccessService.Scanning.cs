namespace Bluetooth.Core.Scanning.CharacteristicAccess;

public abstract partial class CharacteristicAccessService
{
    /// <inheritdoc />
    public async ValueTask<IBluetoothCharacteristic> GetCharacteristicAsync(IBluetoothDevice device, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(device);

        // Explore services if not already explored
        await device.ExploreServicesIfNeededAsync(exploreCharacteristicsToo: false, timeout: timeout, cancellationToken: cancellationToken).ConfigureAwait(false);

        // Get the service
        var service = device.GetService(ServiceId);

        // Explore characteristics if not already explored
        await service.ExploreCharacteristicsIfNeededAsync(timeout, cancellationToken).ConfigureAwait(false);

        // Get and return the characteristic
        return service.GetCharacteristic(CharacteristicId);
    }

    /// <inheritdoc />
    public async ValueTask<bool> HasCharacteristicAsync(IBluetoothDevice device, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(device);

        // Check if device has the service
        if (!device.HasService(ServiceId))
        {
            // Explore services if not already explored
            await device.ExploreServicesIfNeededAsync(exploreCharacteristicsToo: false, timeout: timeout, cancellationToken: cancellationToken).ConfigureAwait(false);

            // Check again after exploration
            if (!device.HasService(ServiceId))
            {
                return false;
            }
        }

        // Get the service
        var service = device.GetService(ServiceId);

        // Check if service has the characteristic
        if (!service.HasCharacteristic(CharacteristicId))
        {
            // Explore characteristics if not already explored
            await service.ExploreCharacteristicsIfNeededAsync(timeout, cancellationToken).ConfigureAwait(false);

            // Check again after exploration
            return service.HasCharacteristic(CharacteristicId);
        }

        return true;
    }
}
