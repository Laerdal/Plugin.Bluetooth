namespace Bluetooth.Core.Scanning;

public abstract partial class BaseBluetoothScanner
{
    #region Events

    /// <inheritdoc />
    public event EventHandler<AdvertisementReceivedEventArgs>? AdvertisementReceived;

    /// <inheritdoc />
    public Func<IBluetoothAdvertisement, bool>? AdvertisementFilter { get; set; }

    /// <inheritdoc cref="IBluetoothScanner.DeviceWrapper"/>
    public Func<IBluetoothRemoteDevice, IBluetoothAdvertisement, IBluetoothRemoteDevice?>? DeviceWrapper { get; set; }

    #endregion

    #region Advertisement - Extras

    private readonly static Func<IBluetoothAdvertisement, bool> _defaultAcceptAllAdvertisementFilter = _ => true;

    /// <inheritdoc />
    public ValueTask<IBluetoothAdvertisement> WaitForAdvertisementAsync(string bluetoothAddress, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        return WaitForAdvertisementAsync(ad => ad.BluetoothAddress == bluetoothAddress, timeout, cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask<IBluetoothAdvertisement> WaitForAdvertisementAsync(Func<IBluetoothAdvertisement, bool>? filter = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        filter ??= _defaultAcceptAllAdvertisementFilter;
        var tcs = new TaskCompletionSource<IBluetoothAdvertisement>();

        void OnAdvertisementReceivedForWait(object? sender, AdvertisementReceivedEventArgs ea)
        {
            if (filter.Invoke(ea.Advertisement))
            {
                tcs.TrySetResult(ea.Advertisement);
            }
        }

        try
        {
            AdvertisementReceived += OnAdvertisementReceivedForWait;

            return await tcs.Task.WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            AdvertisementReceived -= OnAdvertisementReceivedForWait;
        }
    }

    #endregion

    #region Device Factory

    /// <summary>
    ///     Creates a native device from the advertisement and adds it to the device list.
    /// </summary>
    /// <param name="advertisement">The advertisement from which to create and add the device.</param>
    /// <returns>The newly created and added <see cref="IBluetoothRemoteDevice" /> instance.</returns>
    private IBluetoothRemoteDevice AddDeviceFromAdvertisement(IBluetoothAdvertisement advertisement)
    {
        var device = NativeCreateDeviceFromAdvertisement(advertisement);
        if (DeviceWrapper != null)
            device = DeviceWrapper(device, advertisement) ?? device;
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
    protected abstract IBluetoothRemoteDevice NativeCreateDeviceFromAdvertisement(IBluetoothAdvertisement advertisement);

    #endregion

    #region Advertisement Processing

    /// <summary>
    ///     Handles the reception of a Bluetooth advertisement, applying filtering and raising events as necessary.
    /// </summary>
    /// <param name="advertisement">The received Bluetooth advertisement.</param>
    protected void OnAdvertisementReceived<TAdvertisement>(TAdvertisement advertisement)
        where TAdvertisement : struct, IBluetoothAdvertisement
    {
        // Filter (null means accept all)
        if (AdvertisementFilter?.Invoke(advertisement) == false)
        {
            return;
        }

        // Throw event
        AdvertisementReceived?.Invoke(this, new AdvertisementReceivedEventArgs(advertisement));

        // Get or create device. A device that was just created has already taken this advertisement.
        var device = GetOrAddDevice(advertisement, out var created);
        if (!created)
        {
            device.OnAdvertisementReceived(advertisement);
        }
    }

    private readonly object _deviceCreationGate = new();

    /// <summary>
    ///     Returns the device of the advertisement's address, creating it when there is none. Two callbacks for the same new address
    ///     must not both create one: a second device with the same id makes every later lookup of that address throw
    ///     <see cref="MultipleDevicesFoundException" />, so the miss path checks again under a gate before it creates.
    /// </summary>
    private IBluetoothRemoteDevice GetOrAddDevice(IBluetoothAdvertisement advertisement, out bool created)
    {
        if (GetDeviceOrDefault(advertisement.BluetoothAddress) is { } existing)
        {
            created = false;
            return existing;
        }

        lock (_deviceCreationGate)
        {
            if (GetDeviceOrDefault(advertisement.BluetoothAddress) is { } addedMeanwhile)
            {
                created = false;
                return addedMeanwhile;
            }

            created = true;
            return AddDeviceFromAdvertisement(advertisement);
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
        // Filter (null means accept all)
        var filteredAdvertisements = advertisements.Where(adv => AdvertisementFilter?.Invoke(adv) ?? true).ToList();

        // Throw event
        foreach (var advertisement in filteredAdvertisements)
        {
            AdvertisementReceived?.Invoke(this, new AdvertisementReceivedEventArgs(advertisement));
        }

        // Group by device
        var groupedAdvertisements = filteredAdvertisements.GroupBy(advertisement => advertisement.BluetoothAddress);
        foreach (var advertisementGroup in groupedAdvertisements)
        {
            // Get or create device; a device that was just created has already taken the first advertisement
            var device = GetOrAddDevice(advertisementGroup.First(), out var created);
            foreach (var advertisement in created ? advertisementGroup.Skip(1) : advertisementGroup)
            {
                device.OnAdvertisementReceived(advertisement);
            }
        }
    }

    #endregion
}
