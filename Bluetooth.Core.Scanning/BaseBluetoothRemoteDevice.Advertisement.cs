namespace Bluetooth.Core.Scanning;

public abstract partial class BaseBluetoothRemoteDevice
{
    /// <inheritdoc />
    public IBluetoothAdvertisement? LastAdvertisement
    {
        get => GetValue<IBluetoothAdvertisement?>(null);
        private set => SetValue(value);
    }

    /// <inheritdoc />
    public TimeSpan IntervalBetweenAdvertisement
    {
        get => GetValue(TimeSpan.Zero);
        private set => SetValue(value);
    }

    /// <inheritdoc />
    public event EventHandler<AdvertisementReceivedEventArgs>? AdvertisementReceived;

    /// <inheritdoc />
    public ValueTask<IBluetoothAdvertisement> WaitForAdvertisementAsync(Func<IBluetoothAdvertisement?, bool> filter, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        return WaitForPropertyToBe(nameof(LastAdvertisement), filter, timeout, cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask<IBluetoothAdvertisement> WaitForAdvertisementAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        return WaitForPropertyToChangeNotNull<IBluetoothAdvertisement>(nameof(LastAdvertisement), timeout, cancellationToken);
    }

    /// <inheritdoc />
    public void OnAdvertisementReceived(IBluetoothAdvertisement advertisement)
    {
        // Last vs. New
        LastAdvertisement = advertisement ?? throw new ArgumentNullException(nameof(advertisement));
        IntervalBetweenAdvertisement = LastAdvertisement.DateReceived - LastSeen;
        LastSeen = LastAdvertisement.DateReceived;

        // Name
        if (!string.IsNullOrWhiteSpace(advertisement.DeviceName))
        {
            AdvertisedName = advertisement.DeviceName;
        }

        // SignalStrength
        OnSignalStrengthRead(advertisement.RawSignalStrengthInDBm);

        // Throw event
        AdvertisementReceived?.Invoke(this, new AdvertisementReceivedEventArgs(advertisement));
    }
}