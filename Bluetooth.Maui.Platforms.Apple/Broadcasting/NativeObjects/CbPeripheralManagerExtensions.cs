using Bluetooth.Abstractions.Broadcasting.Options;

namespace Bluetooth.Maui.Platforms.Apple.Broadcasting.NativeObjects;

/// <summary>
/// Extension methods for <see cref="CBPeripheralManager"/> to support Bluetooth broadcasting functionality.
/// </summary>
public static class CbPeripheralManagerExtensions
{
    /// <summary>
    /// Starts advertising with the specified options.
    /// </summary>
    public static void StartAdvertising(this CBPeripheralManager manager, BroadcastingOptions options)
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(options);

        // Prepare advertisement data
        using var advertisementData = new NSMutableDictionary();

        // Add local name if provided
        if (!string.IsNullOrEmpty(options.LocalDeviceName))
        {
            advertisementData.Add(CBAdvertisement.DataLocalNameKey, new NSString(options.LocalDeviceName));
        }

        // Add service UUIDs if provided
        if (options.AdvertisedServiceUuids?.Any() == true)
        {
            using var array = new NSMutableArray<CBUUID>();
            foreach (var serviceUuid in options.AdvertisedServiceUuids)
            {
                array.Add(CBUUID.FromString(serviceUuid.ToString()));
            }
            advertisementData.Add(CBAdvertisement.DataServiceUUIDsKey, array);
        }

        // Start advertising
        manager.StartAdvertising(advertisementData);
    }

}
