namespace Bluetooth.Core.BaseClasses;

public abstract partial class BaseBluetoothScanner
{
    /// <summary>
    /// Gets or sets a value indicating whether to ignore duplicate advertisements from the same device.
    /// </summary>
    /// <remarks>
    /// When set to <c>true</c>, only the first advertisement from each device will be processed.
    /// Subsequent advertisements with identical content will be filtered out.
    /// </remarks>
    public bool IgnoreDuplicateAdvertisements
    {
        get => GetValue(false);
        set => SetValue(value);
    }

    /// <inheritdoc/>
    public Func<IBluetoothAdvertisement, bool> AdvertisementFilter { get; set; } = DefaultAdvertisementFilter;

    /// <summary>
    /// Default advertisement filter that accepts all advertisements.
    /// </summary>
    /// <param name="arg">The advertisement to filter.</param>
    /// <returns><c>true</c> for all advertisements.</returns>
    /// <remarks>
    /// This default filter accepts all advertisements without any filtering. It might return too much data in environments with many Bluetooth devices.
    /// </remarks>
    private static bool DefaultAdvertisementFilter(IBluetoothAdvertisement arg)
    {
        return true;
    }

    /// <inheritdoc/>
    public event EventHandler<AdvertisementReceivedEventArgs>? AdvertisementReceived;

    /// <summary>
    /// Processes a received advertisement, applying filters and triggering events.
    /// </summary>
    /// <param name="advertisement">The advertisement to process.</param>
    /// <remarks>
    /// This method applies the <see cref="AdvertisementFilter"/>, raises the <see cref="AdvertisementReceived"/> event,
    /// groups advertisements by device, and optionally filters out duplicates based on <see cref="IgnoreDuplicateAdvertisements"/>.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="advertisement"/> is <c>null</c>.</exception>
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

        // Get or create device
        if (GetDeviceOrDefault(advertisement.BluetoothAddress) is { } existingDevice)
        {
            // Filter out duplicates if needed
            if (IgnoreDuplicateAdvertisements && existingDevice.LastAdvertisement != null && existingDevice.LastAdvertisement.Equals(advertisement))
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
