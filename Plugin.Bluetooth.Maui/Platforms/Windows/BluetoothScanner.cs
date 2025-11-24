using Plugin.Bluetooth.Maui.PlatformSpecific;

namespace Plugin.Bluetooth.Maui;

/// <inheritdoc  />
public partial class BluetoothScanner : BaseBluetoothScanner, BluetoothLeAdvertisementWatcherProxy.IBluetoothLeAdvertisementWatcherProxyDelegate, RadioProxy.IRadioProxyDelegate, BluetoothAdapterProxy.IBluetoothAdapterProxyDelegate
{
    public BluetoothLeAdvertisementWatcherProxy? BluetoothLeAdvertisementWatcherProxy { get; protected set; }


    public BluetoothScanner()
    {
    }

    public BluetoothLEAdvertisementWatcherStatus BluetoothLeAdvertisementWatcherStatus
    {
        get => GetValue(BluetoothLEAdvertisementWatcherStatus.Stopped);
        private set
        {
            if (SetValue(value))
            {
                NativeRefreshIsRunning();
                if (value == BluetoothLEAdvertisementWatcherStatus.Started)
                {
                    OnStartSucceeded();
                }
                else if (value == BluetoothLEAdvertisementWatcherStatus.Stopped)
                {
                    OnStopSucceeded();
                }
            }
        }
    }

    public ValueTask WaitForBluetoothLeAdvertisementWatcherStatusAsync(BluetoothLEAdvertisementWatcherStatus state, TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        return WaitForPropertyToBeOfValue(nameof(BluetoothLeAdvertisementWatcherStatus), state, timeout, cancellationToken);
    }


    #region BaseBluetoothScanner

    protected override void NativeRefreshIsRunning()
    {
        BluetoothLeAdvertisementWatcherStatus = BluetoothLeAdvertisementWatcherProxy?.BluetoothLeAdvertisementWatcher.Status ?? BluetoothLEAdvertisementWatcherStatus.Stopped;
        IsRunning = BluetoothLeAdvertisementWatcherStatus == BluetoothLEAdvertisementWatcherStatus.Started;
    }

    protected override void NativeStart()
    {
        BluetoothLeAdvertisementWatcherProxy = new BluetoothLeAdvertisementWatcherProxy(this);
        BluetoothLeAdvertisementWatcherProxy.BluetoothLeAdvertisementWatcher.Start();
        NativeRefreshIsRunning();
    }

    protected override void NativeStop()
    {
        BluetoothLeAdvertisementWatcherProxy?.BluetoothLeAdvertisementWatcher.Stop();
        NativeRefreshIsRunning();
    }

    protected override IBluetoothDevice NativeCreateDevice(IBluetoothAdvertisement advertisement)
    {
        throw new NotImplementedException();
    }
    #endregion

    #region BluetoothLeAdvertisementWatcherProxy.IBluetoothLeAdvertisementWatcherProxyDelegate

    public void OnAdvertisementWatcherStopped(BluetoothError argsError)
    {
        throw new NotImplementedException();
    }

    public void OnAdvertisementReceived(BluetoothLEAdvertisementReceivedEventArgs argsAdvertisement)
    {
        throw new NotImplementedException();
    }

    #endregion



    public BluetoothAdapterProxy? BluetoothAdapterProxy { get; protected set; }

    public RadioProxy? RadioProxy { get; protected set; }


    public RadioState RadioState
    {
        get => GetValue(RadioState.Unknown);
        private set
        {
            if (SetValue(value))
            {
                NativeRefreshIsBluetoothOn();
            }
        }
    }

    #region RadioProxy.IRadioProxyDelegate

    /// <inheritdoc />
    public void OnRadioStateChanged(RadioState senderState)
    {
        RadioState = senderState;
    }

    #endregion

    public ValueTask WaitForRadioStateAsync(RadioState state, TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        return WaitForPropertyToBeOfValue(nameof(RadioState), state, timeout, cancellationToken);
    }

    public ValueTask WaitForRadioStateToBeKnownAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        return WaitForPropertyToBeDifferentThanValue(nameof(RadioState), RadioState.Unknown, timeout, cancellationToken);
    }

    protected async override ValueTask NativeInitializeAsync()
    {
        BluetoothAdapterProxy = await BluetoothAdapterProxy.GetInstanceAsync(this).ConfigureAwait(false);
        RadioProxy = await RadioProxy.GetInstanceAsync(BluetoothAdapterProxy, this).ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override void NativeRefreshIsBluetoothOn()
    {
        RadioState = RadioProxy?.Radio.State ?? RadioState.Unknown;
        IsBluetoothOn = RadioState == RadioState.On && (BluetoothAdapterProxy?.BluetoothAdapter.IsLowEnergySupported ?? false) && (BluetoothAdapterProxy?.BluetoothAdapter.IsCentralRoleSupported ?? false);
    }
}
