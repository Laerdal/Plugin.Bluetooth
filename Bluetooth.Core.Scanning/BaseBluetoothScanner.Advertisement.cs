namespace Bluetooth.Core.Scanning;

public abstract partial class BaseBluetoothScanner
{
    #region Events

    /// <inheritdoc />
    public event EventHandler<AdvertisementReceivedEventArgs>? AdvertisementReceived;

    #endregion

    #region Device Factory

    /// <summary>
    ///     Creates a native device from the advertisement and adds it to the device list.
    /// </summary>
    /// <param name="advertisement">The advertisement from which to create and add the device.</param>
    /// <returns>The newly created and added <see cref="IBluetoothRemoteDevice" /> instance.</returns>
    protected virtual IBluetoothRemoteDevice AddDeviceFromAdvertisement(IBluetoothAdvertisement advertisement)
    {
        var spec = CreateDeviceFactoryRequestFromAdvertisement(advertisement);
        var device = DeviceFactory.Create(this, spec);
        lock (Devices)
        {
            Devices.Add(device);
        }

        return device;
    }

    /// <summary>
    ///     Creates a factory request for creating a Bluetooth device based on the received advertisement.
    /// </summary>
    /// <param name="advertisement">The received Bluetooth advertisement.</param>
    /// <returns>A factory request containing the necessary information to create a Bluetooth device.</returns>
    protected abstract IBluetoothRemoteDeviceFactory.BluetoothRemoteDeviceFactorySpec CreateDeviceFactoryRequestFromAdvertisement(IBluetoothAdvertisement advertisement);

    #endregion

    #region Advertisement Processing

    /// <summary>
    ///     Handles the reception of a Bluetooth advertisement, applying filtering and raising events as necessary.
    /// </summary>
    /// <param name="advertisement">The received Bluetooth advertisement.</param>
    protected void OnAdvertisementReceived<TAdvertisement>(TAdvertisement advertisement)
        where TAdvertisement : struct, IBluetoothAdvertisement
    {
        if (CurrentScanningOptions.IgnoreNamelessAdvertisements && string.IsNullOrEmpty(advertisement.DeviceName))
        {
            return;
        }

        // Filter
        if (!CurrentScanningOptions.AdvertisementFilter.Invoke(advertisement))
        {
            return;
        }

        // Throw event
        AdvertisementReceived?.Invoke(this, new AdvertisementReceivedEventArgs(advertisement));

        // Get or create device
        if (GetDeviceOrDefault(advertisement.BluetoothAddress) is { } existingDevice)
        {
            // Filter out duplicates if needed
            if (CurrentScanningOptions.IgnoreDuplicateAdvertisements && existingDevice.LastAdvertisement != null && existingDevice.LastAdvertisement.Equals(advertisement))
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
    ///     Processes multiple received advertisements in batch, primarily for Android batch advertisement processing.
    /// </summary>
    /// <param name="advertisements">The collection of advertisements to process.</param>
    /// <remarks>
    ///     This method filters advertisements, raises events for each, groups them by device,
    ///     and processes them accordingly. This is particularly useful for handling Android's batch scan results.
    /// </remarks>
    protected void OnAdvertisementsReceived(IEnumerable<IBluetoothAdvertisement> advertisements)
    {
        // Filter
        var filteredAdvertisements = advertisements.Where(advertisement => CurrentScanningOptions.AdvertisementFilter.Invoke(advertisement)).ToList();

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

    #endregion
}
