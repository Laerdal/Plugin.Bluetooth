
namespace Plugin.Bluetooth.BaseClasses;

public abstract partial class BaseBluetoothScanner
{
    /// <summary>
    /// Gets or sets a value indicating whether to ignore duplicate advertisements from the same device.
    /// </summary>
    public bool IgnoreDuplicateAdvertisements
    {
        get => GetValue(false);
        set => SetValue(value);
    }

    /// <inheritdoc/>
    public Func<IBluetoothAdvertisement, bool> AdvertisementFilter { get; set; } = DefaultAdvertisementFilter;

    private static bool DefaultAdvertisementFilter(IBluetoothAdvertisement arg)
    {
        return false;
    }

    /// <inheritdoc/>
    public event EventHandler<AdvertisementReceivedEventArgs>? AdvertisementReceived;

    /// <summary>
    /// Processes a received advertisement, applying filters and triggering events.
    /// </summary>
    /// <param name="advertisement">The advertisement to process.</param>
    protected void OnAdvertisementReceived(IBluetoothAdvertisement advertisement)
    {
        ArgumentNullException.ThrowIfNull(advertisement);

        // Filter
        if (!AdvertisementFilter.Invoke(advertisement))
        {
            return;
        }

        // Throw event
        AdvertisementReceived?.Invoke(this, new AdvertisementReceivedEventArgs(advertisement));

        // Group by device
        var device = GetDeviceOrDefault(advertisement.BluetoothAddress) ?? AddDeviceFromAdvertisement(advertisement);

        // Filter out duplicates if needed
        if (IgnoreDuplicateAdvertisements && device.LastAdvertisement != null && device.LastAdvertisement.Equals(advertisement))
        {
            return;
        }

        // Process advertisement infos
        device.OnAdvertisementReceived(advertisement);
    }


    /// <summary>
    /// Processes multiple received advertisements in batch, primarily for Android batch advertisement processing.
    /// </summary>
    /// <param name="advertisements">The collection of advertisements to process.</param>
    protected void OnAdvertisementsReceived(IEnumerable<IBluetoothAdvertisement> advertisements)
    {
        // Filter
        var filteredAdvertisements = advertisements.Where(advertisement => AdvertisementFilter.Invoke(advertisement)).ToList();

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
            var device = GetDeviceOrDefault(advertisementGroup.Key) ?? AddDeviceFromAdvertisement(advertisementGroup.First());

            // Process advertisement infos
            foreach (var advertisement in advertisementGroup)
            {
                device.OnAdvertisementReceived(advertisement);
            }
        }
    }
}
