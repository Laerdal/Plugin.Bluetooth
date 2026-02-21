using Bluetooth.Maui.Platforms.Win.NativeObjects;

namespace Bluetooth.Maui.Platforms.Win;

/// <summary>
///     Windows implementation of the Bluetooth adapter using Windows.Devices.Bluetooth namespace.
/// </summary>
public partial class BluetoothAdapter : BaseBluetoothAdapter
{
    public IBluetoothAdapterWrapper BluetoothAdapterWrapper { get; }

    public IRadioWrapper RadioWrapper { get; }

    public BluetoothAdapter(IBluetoothAdapterWrapper bluetoothAdapterWrapper, IRadioWrapper radioWrapper, ILogger<IBluetoothAdapter>? logger = null) : base(logger)
    {
        BluetoothAdapterWrapper = bluetoothAdapterWrapper;
        RadioWrapper = radioWrapper;
    }


    /*
    /// <inheritdoc/>
    protected override void NativeRefreshValues()
    {
        // Scanner properties
        BluetoothLeAdvertisementWatcherWrapper?.RefreshValues();

        // Broadcaster properties
        BluetoothLeAdvertisementPublisherStatus = BluetoothLeAdvertisementPublisherWrapper?.BluetoothLeAdvertisementPublisher.Status ?? BluetoothLEAdvertisementPublisherStatus.Created;
        if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 22621))
        {
            BluetoothLeAdvertisementPublisherIsAnonymous = BluetoothLeAdvertisementPublisherWrapper?.BluetoothLeAdvertisementPublisher.IsAnonymous ?? false;
            BluetoothLeAdvertisementPublisherUseExtendedAdvertisement = BluetoothLeAdvertisementPublisherWrapper?.BluetoothLeAdvertisementPublisher.UseExtendedAdvertisement ?? false;
            BluetoothLeAdvertisementPublisherIncludeTransmitPowerLevel = BluetoothLeAdvertisementPublisherWrapper?.BluetoothLeAdvertisementPublisher.IncludeTransmitPowerLevel ?? false;
            BluetoothLeAdvertisementPublisherPreferredTransmitPowerLevelInDBm = BluetoothLeAdvertisementPublisherWrapper?.BluetoothLeAdvertisementPublisher.PreferredTransmitPowerLevelInDBm;
        }
    }

    #region Scanner

    /// <summary>
    /// Gets the Windows Bluetooth LE advertisement watcher.
    /// </summary>
    public Scanning.NativeObjects.BluetoothLeAdvertisementWatcherWrapper? BluetoothLeAdvertisementWatcherWrapper { get; private set; }

    /// <summary>
    /// Initializes the Bluetooth LE advertisement watcher.
    /// </summary>
    public Scanning.NativeObjects.BluetoothLeAdvertisementWatcherWrapper InitializeBluetoothLeAdvertisementWatcher(BluetoothScanner scanner)
    {
        if(BluetoothLeAdvertisementWatcherWrapper != null)
        {
            throw new InvalidOperationException("BluetoothLeAdvertisementWatcherWrapper is already initialized.");
        }
        BluetoothLeAdvertisementWatcherWrapper = new Scanning.NativeObjects.BluetoothLeAdvertisementWatcherWrapper(scanner);
        return BluetoothLeAdvertisementWatcherWrapper;
    }

    #endregion

    #region Broadcaster

    /// <summary>
    /// Gets the Windows Bluetooth LE advertisement publisher.
    /// </summary>
    public BluetoothLeAdvertisementPublisherWrapper? BluetoothLeAdvertisementPublisherWrapper { get; private set; }

    /// <summary>
    /// Initializes the Bluetooth LE advertisement publisher.
    /// </summary>
    public void InitializeBluetoothLeAdvertisementPublisher(BluetoothBroadcaster broadcaster)
    {
        BluetoothLeAdvertisementPublisherWrapper = new BluetoothLeAdvertisementPublisherWrapper(broadcaster);
    }

    /// <summary>
    /// Gets the current status of the Bluetooth LE advertisement publisher.
    /// </summary>
    public BluetoothLEAdvertisementPublisherStatus BluetoothLeAdvertisementPublisherStatus
    {
        get => GetValue(BluetoothLEAdvertisementPublisherStatus.Created);
        private set => SetValue(value);
    }

    /// <summary>
    /// Gets a value indicating whether the Bluetooth LE advertisement publisher is set to anonymous.
    /// </summary>
    public bool BluetoothLeAdvertisementPublisherIsAnonymous
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    /// <summary>
    /// Gets a value indicating whether the Bluetooth LE advertisement publisher uses extended advertisements.
    /// </summary>
    public bool BluetoothLeAdvertisementPublisherUseExtendedAdvertisement
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    /// <summary>
    /// Gets a value indicating whether the Bluetooth LE advertisement publisher includes the transmit power level.
    /// </summary>
    public bool BluetoothLeAdvertisementPublisherIncludeTransmitPowerLevel
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    /// <summary>
    /// Gets the preferred transmit power level in dBm for the Bluetooth LE advertisement publisher.
    /// </summary>
    public short? BluetoothLeAdvertisementPublisherPreferredTransmitPowerLevelInDBm
    {
        get => GetValue<short?>(null);
        private set => SetValue(value);
    }

    #endregion
*/
}