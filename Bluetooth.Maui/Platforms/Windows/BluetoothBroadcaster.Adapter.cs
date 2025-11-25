using Bluetooth.Maui.PlatformSpecific;

namespace Bluetooth.Maui;

public partial class BluetoothBroadcaster
{

    #region RadioState
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

    /// <inheritdoc />
    public void OnRadioStateChanged(RadioState senderState)
    {
        RadioState = senderState;
    }

    public ValueTask WaitForRadioStateAsync(RadioState state, TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        return WaitForPropertyToBeOfValue(nameof(RadioState), state, timeout, cancellationToken);
    }

    public ValueTask WaitForRadioStateToBeKnownAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        return WaitForPropertyToBeDifferentThanValue(nameof(RadioState), RadioState.Unknown, timeout, cancellationToken);
    }

    #endregion

    public BluetoothAdapterProxy? BluetoothAdapterProxy { get; protected set; }

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
