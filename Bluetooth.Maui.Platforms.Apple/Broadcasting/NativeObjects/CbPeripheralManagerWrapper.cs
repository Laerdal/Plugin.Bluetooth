namespace Bluetooth.Maui.Platforms.Apple.Broadcasting.NativeObjects;

/// <summary>
///     Proxy class for CoreBluetooth peripheral manager delegate callbacks.
///     https://developer.apple.com/documentation/corebluetooth/cbperipheralmanagerdelegate
/// </summary>
public partial class CbPeripheralManagerWrapper : CBPeripheralManagerDelegate
{
    private readonly ICbPeripheralManagerDelegate _cbPeripheralManagerDelegate;
    private readonly IDispatchQueueProvider _dispatchQueueProvider;
    private readonly CbPeripheralManagerOptions _options;
    private readonly ITicker _ticker;
    private readonly object _lock = new object();
    private CBPeripheralManager? _cbPeripheralManager;
    private IDisposable? _refreshSubscription;


    /// <summary>
    ///     Initializes a new instance of the CbPeripheralManagerWrapper class.
    /// </summary>
    /// <param name="cbPeripheralManagerDelegate">The delegate proxy for handling peripheral manager events.</param>
    /// <param name="options">The initialization options for the peripheral manager.</param>
    /// <param name="dispatchQueueProvider">The provider for dispatch queues.</param>
    /// <param name="ticker">The ticker for scheduling periodic tasks.</param>
    public CbPeripheralManagerWrapper(ICbPeripheralManagerDelegate cbPeripheralManagerDelegate,
        IOptions<CbPeripheralManagerOptions> options,
        IDispatchQueueProvider dispatchQueueProvider,
        ITicker ticker)
    {
        ArgumentNullException.ThrowIfNull(cbPeripheralManagerDelegate);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(dispatchQueueProvider);
        ArgumentNullException.ThrowIfNull(ticker);

        _cbPeripheralManagerDelegate = cbPeripheralManagerDelegate;
        _options = options.Value;
        _dispatchQueueProvider = dispatchQueueProvider;
        _ticker = ticker;
    }

    /// <summary>
    ///     The underlying CBCentralManager instance.
    /// </summary>
    public CBPeripheralManager CbPeripheralManager
    {
        get
        {
            if (_cbPeripheralManager == null)
            {
                lock (_lock)
                {
                    if (_cbPeripheralManager == null)
                    {
                        _cbPeripheralManager = new CBPeripheralManager(this, _dispatchQueueProvider.GetCbCentralManagerDispatchQueue(), _options)
                        {
                            Delegate = this
                        };
                        _refreshSubscription = _ticker.Register("refresh_peripheral_manager_properties", TimeSpan.FromSeconds(1), RefreshIsAdvertising, true);
                    }
                }
            }

            return _cbPeripheralManager;
        }
    }

    /// <summary>
    ///     Gets a value indicating whether the Core Bluetooth peripheral manager is currently advertising.
    /// </summary>
    private bool CbPeripheralManagerIsAdvertising { get; set; }

    private void RefreshIsAdvertising()
    {
        if (_cbPeripheralManager == null)
        {
            return;
        }

        if (CbPeripheralManagerIsAdvertising != _cbPeripheralManager.Advertising)
        {
            CbPeripheralManagerIsAdvertising = _cbPeripheralManager.Advertising;
            if (!CbPeripheralManagerIsAdvertising)
            {
                AdvertisingStopped();
            }
        }
    }

    /// <summary>
    ///     Releases the unmanaged resources used by the CbCentralManagerWrapper and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _refreshSubscription?.Dispose();
            _cbPeripheralManager?.StopAdvertising();
            _cbPeripheralManager?.Dispose();
            CbPeripheralManager.Dispose();
        }

        base.Dispose(disposing);
    }

    private void AdvertisingStopped()
    {
        try
        {
            // ACTION
            _cbPeripheralManagerDelegate.AdvertisingStopped();
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    #region CBPeripheralManagerDelegate

    /// <inheritdoc cref="CBPeripheralManagerDelegate.StateUpdated" />
    public override void StateUpdated(CBPeripheralManager peripheral)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(peripheral);
            _cbPeripheralManagerDelegate.StateUpdated(peripheral.State);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc cref="CBPeripheralManagerDelegate.AdvertisingStarted" />
    public override void AdvertisingStarted(CBPeripheralManager peripheral, NSError? error)
    {
        try
        {
            // ACTION
            _cbPeripheralManagerDelegate.AdvertisingStarted(error);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc cref="CBPeripheralManagerDelegate.CharacteristicSubscribed" />
    public override void CharacteristicSubscribed(CBPeripheralManager peripheral, CBCentral central, CBCharacteristic characteristic)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(characteristic);
            ArgumentNullException.ThrowIfNull(characteristic.Service, nameof(characteristic.Service));

            // GET SERVICE
            var sharedService = _cbPeripheralManagerDelegate.GetService(characteristic.Service);

            // GET CHARACTERISTIC
            var sharedCharacteristic = sharedService.GetCharacteristic(characteristic);

            // ACTION
            sharedCharacteristic.CharacteristicSubscribed(central, characteristic);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc cref="CBPeripheralManagerDelegate.CharacteristicUnsubscribed" />
    public override void CharacteristicUnsubscribed(CBPeripheralManager peripheral, CBCentral central, CBCharacteristic characteristic)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(characteristic);
            ArgumentNullException.ThrowIfNull(characteristic.Service, nameof(characteristic.Service));

            // GET SERVICE
            var sharedService = _cbPeripheralManagerDelegate.GetService(characteristic.Service);

            // GET CHARACTERISTIC
            var sharedCharacteristic = sharedService.GetCharacteristic(characteristic);

            // ACTION
            sharedCharacteristic.CharacteristicUnsubscribed(central, characteristic);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc cref="CBPeripheralManagerDelegate.ServiceAdded" />
    public override void ServiceAdded(CBPeripheralManager peripheral, CBService service, NSError? error)
    {
        try
        {
            // ACTION
            _cbPeripheralManagerDelegate.ServiceAdded(service);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc cref="CBPeripheralManagerDelegate.ReadRequestReceived" />
    public override void ReadRequestReceived(CBPeripheralManager peripheral, CBATTRequest request)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(request.Characteristic);
            ArgumentNullException.ThrowIfNull(request.Characteristic.Service, nameof(request.Characteristic.Service));

            // GET SERVICE
            var sharedService = _cbPeripheralManagerDelegate.GetService(request.Characteristic.Service);

            // GET CHARACTERISTIC
            var sharedCharacteristic = sharedService.GetCharacteristic(request.Characteristic);

            // ACTION
            sharedCharacteristic.ReadRequestReceived(request);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc cref="CBPeripheralManagerDelegate.WillRestoreState" />
    public override void WillRestoreState(CBPeripheralManager peripheral, NSDictionary dict)
    {
        try
        {
            // ACTION
            _cbPeripheralManagerDelegate.WillRestoreState(dict);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc cref="CBPeripheralManagerDelegate.WriteRequestsReceived" />
    public override void WriteRequestsReceived(CBPeripheralManager peripheral, CBATTRequest[] requests)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(requests);
            foreach (var request in requests)
            {
                ArgumentNullException.ThrowIfNull(request);
                ArgumentNullException.ThrowIfNull(request.Characteristic);
                ArgumentNullException.ThrowIfNull(request.Characteristic.Service, nameof(request.Characteristic.Service));

                // GET SERVICE
                var sharedService = _cbPeripheralManagerDelegate.GetService(request.Characteristic.Service);

                // GET CHARACTERISTIC
                var sharedCharacteristic = sharedService.GetCharacteristic(request.Characteristic);

                // ACTION
                sharedCharacteristic.WriteRequestsReceived(request);
            }
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc cref="CBPeripheralManagerDelegate.DidOpenL2CapChannel" />
    public override void DidOpenL2CapChannel(CBPeripheralManager peripheral, CBL2CapChannel? channel, NSError? error)
    {
        try
        {
            // ACTION
            _cbPeripheralManagerDelegate.DidOpenL2CapChannel(error, channel);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc cref="CBPeripheralManagerDelegate.DidPublishL2CapChannel" />
    public override void DidPublishL2CapChannel(CBPeripheralManager peripheral, ushort psm, NSError? error)
    {
        try
        {
            // ACTION
            _cbPeripheralManagerDelegate.DidPublishL2CapChannel(error, psm);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc cref="CBPeripheralManagerDelegate.DidUnpublishL2CapChannel" />
    public override void DidUnpublishL2CapChannel(CBPeripheralManager peripheral, ushort psm, NSError? error)
    {
        try
        {
            // ACTION
            _cbPeripheralManagerDelegate.DidUnpublishL2CapChannel(error, psm);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    #endregion
}