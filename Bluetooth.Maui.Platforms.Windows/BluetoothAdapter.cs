using Bluetooth.Maui.Platforms.Windows.NativeObjects;

namespace Bluetooth.Maui.Platforms.Windows;

/// <summary>
///     Windows implementation of the Bluetooth adapter using Windows.Devices.Bluetooth namespace.
/// </summary>
public partial class BluetoothAdapter : BaseBluetoothAdapter, RadioProxy.IRadioProxyDelegate, BluetoothAdapterProxy.IBluetoothAdapterProxyDelegate
{

    public BluetoothAdapter(ILogger<IBluetoothAdapter>? logger = null) : base(logger)
    {
    }

    #region BluetoothAdapterProxy

    private BluetoothAdapterProxy? BluetoothAdapterProxy { get; set; }

    private TaskCompletionSource<BluetoothAdapterProxy>? BluetoothAdapterInitializationTcs { get; set; }


    /// <summary>
    ///    Gets the Bluetooth adapter proxy, ensuring that only one instance is created and shared across the application.
    /// </summary>
    public async ValueTask<BluetoothAdapterProxy> GetBluetoothAdapterProxyAsync()
    {
        if (BluetoothAdapterProxy != null)
        {
            return BluetoothAdapterProxy;
        }
        if (BluetoothAdapterInitializationTcs != null)
        {
            return await BluetoothAdapterInitializationTcs.Task.ConfigureAwait(false);
        }
        BluetoothAdapterInitializationTcs = new TaskCompletionSource<BluetoothAdapterProxy>();
        BluetoothAdapterProxy = await BluetoothAdapterProxy.GetInstanceAsync(this).ConfigureAwait(false);
        BluetoothAdapterInitializationTcs.TrySetResult(BluetoothAdapterProxy);
        BluetoothAdapterInitializationTcs = null;
        return BluetoothAdapterProxy;
    }

    #endregion


    #region BluetoothAdapterProxy

    private RadioProxy? RadioProxy { get; set; }

    private TaskCompletionSource<RadioProxy>? RadioInitializationTcs { get; set; }


    /// <summary>
    ///    Gets the Radio proxy, ensuring that only one instance is created and shared across the application.
    /// </summary>
    public async ValueTask<RadioProxy> GetRadioProxyAsync()
    {
        if (RadioProxy != null)
        {
            return RadioProxy;
        }
        if (RadioInitializationTcs != null)
        {
            return await RadioInitializationTcs.Task.ConfigureAwait(false);
        }
        RadioInitializationTcs = new TaskCompletionSource<RadioProxy>();
        var bluetoothAdapterProxy = await GetBluetoothAdapterProxyAsync();
        RadioProxy = await RadioProxy.GetInstanceAsync(bluetoothAdapterProxy.BluetoothAdapter,this).ConfigureAwait(false);
        RadioInitializationTcs.TrySetResult(RadioProxy);
        RadioInitializationTcs = null;
        return RadioProxy;
    }

    #endregion

    /*
    /// <inheritdoc/>
    protected override void NativeRefreshValues()
    {
        // Adapter properties
        BluetoothAdapterIsAdvertisementOffloadSupported = BluetoothAdapterInstance?.IsAdvertisementOffloadSupported ?? false;
        BluetoothAdapterIsLowEnergySupported = BluetoothAdapterInstance?.IsLowEnergySupported ?? false;
        BluetoothAdapterIsClassicSupported = BluetoothAdapterInstance?.IsClassicSupported ?? false;
        BluetoothAdapterAreLowEnergySecureConnectionsSupported = BluetoothAdapterInstance?.AreLowEnergySecureConnectionsSupported ?? false;
        BluetoothAdapterIsPeripheralRoleSupported = BluetoothAdapterInstance?.IsPeripheralRoleSupported ?? false;
        BluetoothAdapterAreClassicSecureConnectionsSupported = BluetoothAdapterInstance?.AreClassicSecureConnectionsSupported ?? false;

        // Radio properties
        RadioState = BluetoothRadio?.State ?? RadioState.Unknown;

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

    #region BluetoothAdapter

    private BluetoothAdapterProxy? BluetoothAdapterProxy { get; set; }

    private Bluetooth_BluetoothAdapter? BluetoothAdapterInstance => BluetoothAdapterProxy?.BluetoothAdapter;

    /// <summary>
    /// Gets the Windows Bluetooth adapter.
    /// </summary>
    public async ValueTask<Bluetooth_BluetoothAdapter> GetBluetoothAdapterAsync()
    {
        BluetoothAdapterProxy ??= await BluetoothAdapterProxy.GetInstanceAsync(this).ConfigureAwait(false);
        if(BluetoothAdapterInstance == null)
        {
            throw new PermissionException("BluetoothAdapter.GetDefaultAsync = null, Did you forget to add '<DeviceCapability Name=\"bluetooth\" />' in your Manifest's Capabilities?");
        }
        return BluetoothAdapterInstance;
    }

    /// <summary>
    /// Gets a value indicating whether Bluetooth advertisement offload is supported.
    /// </summary>
    public bool BluetoothAdapterIsAdvertisementOffloadSupported
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    /// <summary>
    /// Gets a value indicating whether Bluetooth Low Energy is supported.
    /// </summary>
    public bool BluetoothAdapterIsLowEnergySupported
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    /// <summary>
    /// Gets a value indicating whether classic Bluetooth is supported.
    /// </summary>
    public bool BluetoothAdapterIsClassicSupported
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    /// <summary>
    /// Gets a value indicating whether classic Bluetooth secure connections are supported.
    /// </summary>
    public bool BluetoothAdapterAreClassicSecureConnectionsSupported
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    /// <summary>
    /// Gets a value indicating whether Bluetooth Low Energy secure connections are supported.
    /// </summary>
    public bool BluetoothAdapterAreLowEnergySecureConnectionsSupported
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    /// <summary>
    /// Gets a value indicating whether the peripheral role is supported.
    /// </summary>
    public bool BluetoothAdapterIsPeripheralRoleSupported
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    #endregion

    #region BluetoothRadio

    private RadioProxy? BluetoothRadioProxy { get; set; }

    private Radio? BluetoothRadio => BluetoothRadioProxy?.Radio;

    /// <summary>
    /// Gets the Windows Bluetooth radio.
    /// </summary>
    public async ValueTask<Radio> GetBluetoothRadioAsync(Bluetooth_BluetoothAdapter bluetoothAdapter)
    {
        BluetoothRadioProxy ??= await RadioProxy.GetInstanceAsync(bluetoothAdapter, this).ConfigureAwait(false);
        if(BluetoothRadio == null)
        {
            throw new PermissionException("Radio.FromIdAsync = null, Did you forget to add '<DeviceCapability Name=\"bluetooth\" />' in your Manifest's Capabilities?");
        }
        return BluetoothRadio;
    }

    /// <summary>
    /// Gets the current state of the Bluetooth radio.
    /// </summary>
    public RadioState RadioState
    {
        get => GetValue(RadioState.Unknown);
        private set => SetValue(value);
    }

    /// <summary>
    /// Handles radio state changes.
    /// </summary>
    public void OnRadioStateChanged(RadioState senderState)
    {
        RadioState = senderState;
        IsBluetoothOn = RadioState == RadioState.On && (BluetoothAdapterInstance?.IsLowEnergySupported ?? false);
    }

    #endregion

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

    /// <summary>
    ///     Handles radio state changes.
    /// </summary>
    /// <param name="senderState">The new radio state.</param>
    /// <remarks>
    ///     This is a placeholder for future implementation. The RadioProxy is not currently instantiated,
    ///     so this method is not called in the current scanning implementation.
    /// </remarks>
    public void OnRadioStateChanged(RadioState senderState)
    {
        // No-op: RadioProxy is not currently used in the scanning implementation
        // Future enhancement: Update adapter state properties when radio state changes
    }
}