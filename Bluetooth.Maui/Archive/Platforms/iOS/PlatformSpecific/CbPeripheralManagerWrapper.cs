using Microsoft.Extensions.Options;

namespace Bluetooth.Maui.PlatformSpecific;

/// <summary>
/// Proxy class for CoreBluetooth peripheral manager delegate callbacks.
/// https://developer.apple.com/documentation/corebluetooth/cbperipheralmanagerdelegate
/// </summary>
public partial class CbPeripheralManagerWrapper : CBPeripheralManagerDelegate
{
    /// <summary>
    /// The underlying CBPeripheralManager instance.
    /// </summary>
    public CBPeripheralManager CbPeripheralManager { get; }

    /// <summary>
    /// The delegate proxy for handling peripheral manager events.
    /// </summary>
    private readonly CbPeripheralManagerWrapper.ICbPeripheralManagerDelegate _broadcaster;

    /// <summary>
    /// Initializes a new instance of the CbPeripheralManagerWrapper class.
    /// </summary>
    /// <param name="broadcaster">The delegate proxy for handling peripheral manager events.</param>
    /// <param name="options">The initialization options for the peripheral manager.</param>
    public CbPeripheralManagerWrapper(CbPeripheralManagerWrapper.ICbPeripheralManagerDelegate broadcaster, IOptions<BluetoothAppleOptions>? options = null)
    {
        _broadcaster = broadcaster;
        CbPeripheralManager = new CBPeripheralManager(this, options?.Value.GetPeripheralQueue(), options?.Value.CBPeripheralManagerOptions)
        {
            Delegate = this
        };
    }

    /// <summary>
    /// Releases the unmanaged resources used by the CbCentralManagerWrapper and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            CbPeripheralManager.Dispose();
        }

        base.Dispose(disposing);
    }

    /// <summary>
    /// Occurs when the state of the Core Bluetooth central manager changes.
    /// </summary>
    public event EventHandler<StateChangedEventArgs>? StateChanged;

    /// <summary>
    /// Gets or sets the current state of the Core Bluetooth peripheral manager.
    /// </summary>
    /// <remarks>
    /// The state indicates whether Bluetooth is powered on, off, unauthorized, unsupported, etc.
    /// </remarks>
    public CBManagerState State { get; private set; } = CBManagerState.Unknown;


    /// <summary>
    /// Asynchronously waits until the Core Bluetooth central manager's state is known (not Unknown).
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to cancel the wait operation.</param>
    /// <returns>A task that completes when the central manager's state is known.</returns>
    public async Task WaitForStateToBeKnownAsync(CancellationToken cancellationToken = default)
    {
        // QUICK RETURN
        if (CbPeripheralManager.State != CBManagerState.Unknown)
        {
            return;
        }

        // WAIT FOR STATE TO CHANGE
        var tcs = new TaskCompletionSource<object?>();

        try
        {
            StateChanged += Handler;
            await using (cancellationToken.Register(() => tcs.TrySetCanceled(), useSynchronizationContext: false))
            {
                await tcs.Task.ConfigureAwait(false);
            }
        }
        finally
        {
            StateChanged -= Handler;
        }
        return;

        void Handler(object? s, StateChangedEventArgs e)
        {
            if (e.NewState != CBManagerState.Unknown)
            {
                tcs.TrySetResult(null);
            }
        }
    }

    /// <summary>
    /// Asynchronously waits until the Core Bluetooth central manager is powered on.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to cancel the wait operation.</param>
    /// <returns>A task that completes when the central manager is powered on.</returns>
    public async Task WaitForPoweredOnAsync(CancellationToken cancellationToken = default)
    {
        // QUICK RETURN
        if (CbPeripheralManager.State == CBManagerState.PoweredOn)
        {
            return;
        }

        // WAIT FOR STATE TO CHANGE
        var tcs = new TaskCompletionSource<object?>();

        try
        {
            StateChanged += Handler;
            await using (cancellationToken.Register(() => tcs.TrySetCanceled(), useSynchronizationContext: false))
            {
                await tcs.Task.ConfigureAwait(false);
            }
        }
        finally
        {
            StateChanged -= Handler;
        }
        return;

        void Handler(object? s, StateChangedEventArgs e)
        {
            switch (e.NewState)
            {
                case CBManagerState.PoweredOn:
                    tcs.TrySetResult(null);
                    break;
                case CBManagerState.Unsupported:
                case CBManagerState.Unauthorized:
                case CBManagerState.PoweredOff:
                    tcs.TrySetException(new InvalidOperationException($"Bluetooth is not available. State: {e.NewState}"));
                    break;
            }
        }
    }

    #region CBPeripheralManagerDelegate

    /// <inheritdoc cref="CBPeripheralManagerDelegate.StateUpdated" />
    public override void StateUpdated(CBPeripheralManager peripheral)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(peripheral);
            State = peripheral.State;
            StateChanged?.Invoke(this, new StateChangedEventArgs(peripheral.State));
            _broadcaster.StateUpdated(peripheral.State);
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
            _broadcaster.AdvertisingStarted(error);
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
            var sharedService = _broadcaster.GetService(characteristic.Service);

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
            var sharedService = _broadcaster.GetService(characteristic.Service);

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
            _broadcaster.ServiceAdded(service);
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
            var sharedService = _broadcaster.GetService(request.Characteristic.Service);

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
            _broadcaster.WillRestoreState(dict);
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
                var sharedService = _broadcaster.GetService(request.Characteristic.Service);

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
            _broadcaster.DidOpenL2CapChannel(error, channel);
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
            _broadcaster.DidPublishL2CapChannel(error, psm);
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
            _broadcaster.DidUnpublishL2CapChannel(error, psm);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    #endregion
}

