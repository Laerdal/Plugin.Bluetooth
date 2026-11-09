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
    /// <param name="options">The options of the scan the advertisement belongs to, or <see langword="null" /> when none was running.</param>
    /// <returns>The newly created and added <see cref="IBluetoothRemoteDevice" /> instance.</returns>
    private IBluetoothRemoteDevice AddDeviceFromAdvertisement(IBluetoothAdvertisement advertisement, ScanningOptions? options)
    {
        var device = NativeCreateDeviceFromAdvertisement(advertisement);

        // The device starts with the signal strength smoothing of the scan it was discovered by. The options come from the callback that
        // received the advertisement, not from the scanner's current state, which a handler may already have changed by stopping the scan.
        if (options?.SignalStrengthJitterSmoothing is { } smoothing && device is BaseBluetoothRemoteDevice baseDevice)
        {
            baseDevice.SignalStrengthSmoothingOptions = smoothing;
        }

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
    ///     Tells whether an advertisement is processed. It must pass <see cref="AdvertisementFilter">the scanner's filter</see> and, while a
    ///     scan runs, the filters of that scan:
    ///     <see cref="Bluetooth.Abstractions.Scanning.Options.ScanningOptions.IgnoreNamelessAdvertisements" /> and
    ///     <see cref="Bluetooth.Abstractions.Scanning.Options.ScanningOptions.AdvertisementFilter" />.
    /// </summary>
    /// <param name="advertisement">The advertisement to judge.</param>
    /// <param name="options">The options of the scan the advertisement belongs to, or <see langword="null" /> when none was running.</param>
    /// <remarks>
    ///     Generic so that no advertisement is boxed unless a filter delegate has to be called.
    /// </remarks>
    private bool PassesAdvertisementFilters<TAdvertisement>(TAdvertisement advertisement, ScanningOptions? options)
        where TAdvertisement : IBluetoothAdvertisement
    {
        // A null filter accepts everything
        if (AdvertisementFilter?.Invoke(advertisement) == false)
        {
            return false;
        }

        if (options is null)
        {
            return true;
        }

        if (options.IgnoreNamelessAdvertisements && string.IsNullOrWhiteSpace(advertisement.DeviceName))
        {
            return false;
        }

        return options.AdvertisementFilter?.Invoke(advertisement) != false;
    }

    /// <summary>
    ///     Handles the reception of a Bluetooth advertisement, applying filtering and raising events as necessary.
    /// </summary>
    /// <param name="advertisement">The received Bluetooth advertisement.</param>
    protected void OnAdvertisementReceived<TAdvertisement>(TAdvertisement advertisement)
        where TAdvertisement : struct, IBluetoothAdvertisement
    {
        // The scan this advertisement belongs to, read before any consumer code runs: a handler may stop the scan, which clears it.
        var options = ActiveScanningOptions;

        if (!PassesAdvertisementFilters(advertisement, options))
        {
            return;
        }

        // Throw event
        AdvertisementReceived?.Invoke(this, new AdvertisementReceivedEventArgs(advertisement));

        // Get or create device. A device that was just created has already taken this advertisement.
        var device = GetOrAddDevice(advertisement, options, out var created);
        if (!created)
        {
            device.OnAdvertisementReceived(advertisement);
        }
    }

    /// <summary>
    ///     The lock of one address while a device for it is being created, and how many callbacks hold it or wait for it.
    /// </summary>
    private sealed class DeviceCreationLock
    {
        public int Users;
    }

    // Guards the entries and their Users counts. It is never held while a device is created or looked up.
    private readonly Dictionary<string, DeviceCreationLock> _deviceCreationLocks = [];

    /// <summary>
    ///     Returns the device of the advertisement's address, creating it when there is none. Two callbacks for the same new address
    ///     must not both create one: a second device with the same id makes every later lookup of that address throw
    ///     <see cref="MultipleDevicesFoundException" />. Creating a device has side effects on the native objects (Apple's peripheral
    ///     wrapper becomes its peripheral's delegate), so a loser cannot simply be thrown away: creation is serialized per address
    ///     instead, and the miss path checks again under that lock. The lock is per address so that a consumer's
    ///     <c>DeviceWrapper</c>, which runs under it, never holds up another address.
    /// </summary>
    private IBluetoothRemoteDevice GetOrAddDevice<TAdvertisement>(TAdvertisement advertisement, ScanningOptions? options, out bool created)
        where TAdvertisement : IBluetoothAdvertisement
    {
        var address = advertisement.BluetoothAddress;
        if (GetDeviceOrDefault(address) is { } existing)
        {
            created = false;
            return existing;
        }

        // The entry lives as long as one callback holds or awaits it. Dropping it earlier would let a new callback take a new lock while an
        // old one still waits on the previous, and both could create a device, for example when the device was removed in between.
        DeviceCreationLock creationLock;
        lock (_deviceCreationLocks)
        {
            if (!_deviceCreationLocks.TryGetValue(address, out creationLock!))
            {
                _deviceCreationLocks[address] = creationLock = new DeviceCreationLock();
            }

            creationLock.Users++;
        }

        try
        {
            lock (creationLock)
            {
                if (GetDeviceOrDefault(address) is { } addedMeanwhile)
                {
                    created = false;
                    return addedMeanwhile;
                }

                created = true;
                return AddDeviceFromAdvertisement(advertisement, options);
            }
        }
        finally
        {
            lock (_deviceCreationLocks)
            {
                if (--creationLock.Users == 0)
                {
                    _deviceCreationLocks.Remove(address);
                }
            }
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
        // The scan these advertisements belong to, read before any consumer code runs: a handler may stop the scan, which clears it.
        var options = ActiveScanningOptions;
        var filteredAdvertisements = advertisements.Where(adv => PassesAdvertisementFilters(adv, options)).ToList();

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
            var device = GetOrAddDevice(advertisementGroup.First(), options, out var created);
            foreach (var advertisement in created ? advertisementGroup.Skip(1) : advertisementGroup)
            {
                device.OnAdvertisementReceived(advertisement);
            }
        }
    }

    #endregion
}
