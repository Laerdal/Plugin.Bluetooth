using Plugin.Bluetooth.Maui.PlatformSpecific;

namespace Plugin.Bluetooth.Maui;

/// <inheritdoc />
public partial class BluetoothBroadcaster : BaseBluetoothBroadcaster, BluetoothLeAdvertisementPublisherProxy.IBluetoothLeAdvertisementPublisherProxyDelegate, RadioProxy.IRadioProxyDelegate, BluetoothAdapterProxy.IBluetoothAdapterProxyDelegate
{

    public BluetoothLeAdvertisementPublisherProxy? BluetoothLeAdvertisementPublisherProxy { get; protected set; }

    #region BaseBluetoothBroadcaster


    protected override void NativeRefreshIsRunning()
    {
        throw new NotImplementedException();
    }

    protected override void NativeStart()
    {
        throw new NotImplementedException();
    }

    protected override void NativeStop()
    {
        throw new NotImplementedException();
    }

    #endregion

    #region BluetoothLeAdvertisementPublisherProxy.IBluetoothLeAdvertisementPublisherProxyDelegate

    public void OnAdvertisementPublisherStatusChanged(BluetoothLEAdvertisementPublisherStatus status)
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
