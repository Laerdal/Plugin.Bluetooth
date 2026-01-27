using Bluetooth.Abstractions.Scanning;
using Bluetooth.Abstractions.Scanning.EventArgs;

namespace Bluetooth.Core.Scanning;

public abstract partial class BaseBluetoothScanner
{
    /// <summary>
    /// Creates a native device from the advertisement and adds it to the device list.
    /// </summary>
    /// <param name="advertisement">The advertisement from which to create and add the device.</param>
    /// <returns>The newly created and added <see cref="IBluetoothDevice"/> instance.</returns>
    protected virtual IBluetoothDevice AddDeviceFromAdvertisement(IBluetoothAdvertisement advertisement)
    {
        var newDeviceRequest = new IBluetoothDeviceFactory.BluetoothDeviceFactoryRequestWithAdvertisement()
        {
            Advertisement = advertisement
        };
        var device = DeviceFactory.CreateDevice(this, newDeviceRequest);
        lock (Devices)
        {
            Devices.Add(device);
        }
        return device;
    }

    /// <inheritdoc/>
    public event EventHandler<AdvertisementReceivedEventArgs>? AdvertisementReceived;

    /// <summary>
    /// Processes a received advertisement, applying filters and triggering events.
    /// </summary>
    /// <param name="advertisement">The advertisement to process.</param>
    /// <remarks>
    /// This method applies the <see cref="IBluetoothScannerStartScanningOptions.AdvertisementFilter"/>, raises the <see cref="AdvertisementReceived"/> event,
    /// groups advertisements by device, and optionally filters out duplicates based on <see cref="IBluetoothScannerStartScanningOptions.IgnoreDuplicateAdvertisements"/>.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="advertisement"/> is <c>null</c>.</exception>
    protected void OnAdvertisementReceived(IBluetoothAdvertisement advertisement)
    {
        ArgumentNullException.ThrowIfNull(advertisement);

        // Filter
        if (!StartScanningOptions.AdvertisementFilter.Invoke(advertisement))
        {
            return;
        }

        // Throw event
        AdvertisementReceived?.Invoke(this, new AdvertisementReceivedEventArgs(advertisement));

        // Get or create device
        if (GetDeviceOrDefault(advertisement.BluetoothAddress) is { } existingDevice)
        {
            // Filter out duplicates if needed
            if (StartScanningOptions.IgnoreDuplicateAdvertisements && existingDevice.LastAdvertisement != null && existingDevice.LastAdvertisement.Equals(advertisement))
            {
                return;
            }

            // Process advertisement infos
            existingDevice.OnAdvertisementReceived(advertisement);
        }
        else
        {
            // New device
            AddDeviceFromAdvertisement(advertisement);
        }
    }

    /// <summary>
    /// Processes multiple received advertisements in batch, primarily for Android batch advertisement processing.
    /// </summary>
    /// <param name="advertisements">The collection of advertisements to process.</param>
    /// <remarks>
    /// This method filters advertisements, raises events for each, groups them by device,
    /// and processes them accordingly. This is particularly useful for handling Android's batch scan results.
    /// </remarks>
    protected void OnAdvertisementsReceived(IEnumerable<IBluetoothAdvertisement> advertisements)
    {
        // Filter
        var filteredAdvertisements = advertisements.Where(advertisement => StartScanningOptions.AdvertisementFilter.Invoke(advertisement)).ToList();

        // Throw event
        foreach (var advertisement in filteredAdvertisements)
        {
            AdvertisementReceived?.Invoke(this, new AdvertisementReceivedEventArgs(advertisement));
        }

        // Group by device
        var groupedAdvertisements = filteredAdvertisements.GroupBy(advertisement => advertisement.BluetoothAddress);
        foreach (var advertisementGroup in groupedAdvertisements)
        {
            // Get or create device
            if (GetDeviceOrDefault(advertisementGroup.Key) is { } existingDevice)
            {
                // Process advertisement infos
                foreach (var advertisement in advertisementGroup)
                {
                    existingDevice.OnAdvertisementReceived(advertisement);
                }
            }
            else
            {
                // Process advertisement infos
                var newDevice = AddDeviceFromAdvertisement(advertisementGroup.First());
                foreach (var advertisement in advertisementGroup.Skip(1))
                {
                    newDevice.OnAdvertisementReceived(advertisement);
                }
            }
        }
    }

}
