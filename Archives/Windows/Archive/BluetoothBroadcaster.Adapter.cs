using Bluetooth.Maui.PlatformSpecific;

namespace Bluetooth.Maui;

public partial class BluetoothBroadcaster
{

    #region RadioState

    /// <summary>
    /// Gets the radio proxy for monitoring Bluetooth radio state.
    /// </summary>
    public RadioProxy? RadioProxy { get; protected set; }

    /// <summary>
    /// Gets the current state of the Bluetooth radio.
    /// </summary>
    /// <remarks>
    /// Changing this property automatically updates <see cref="BaseBluetoothActivity.IsBluetoothOn"/>.
    /// </remarks>
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
    /// <summary>
    /// Called when the Bluetooth radio state changes.
    /// </summary>
    /// <param name="senderState">The new radio state.</param>
    public void OnRadioStateChanged(RadioState senderState)
    {
        RadioState = senderState;
    }

    /// <summary>
    /// Waits asynchronously for the radio to reach a specific state.
    /// </summary>
    /// <param name="state">The target radio state to wait for.</param>
    /// <param name="timeout">The timeout for the wait operation.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that completes when the target state is reached.</returns>
    public ValueTask WaitForRadioStateAsync(RadioState state, TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        return WaitForPropertyToBeOfValue(nameof(RadioState), state, timeout, cancellationToken);
    }

    /// <summary>
    /// Waits asynchronously for the radio state to be known (not <see cref="RadioState.Unknown"/>).
    /// </summary>
    /// <param name="timeout">The timeout for the wait operation.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that completes when the state becomes known.</returns>
    public ValueTask WaitForRadioStateToBeKnownAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        return WaitForPropertyToBeDifferentThanValue(nameof(RadioState), RadioState.Unknown, timeout, cancellationToken);
    }

    #endregion

    /// <summary>
    /// Gets the Bluetooth adapter proxy.
    /// </summary>
    public BluetoothAdapterProxy? BluetoothAdapterProxy { get; protected set; }

    /// <inheritdoc/>
    /// <remarks>
    /// Initializes the Windows Bluetooth adapter and radio proxies.
    /// </remarks>
    protected async override ValueTask NativeInitializeAsync()
    {
        BluetoothAdapterProxy = await BluetoothAdapterProxy.GetInstanceAsync(this).ConfigureAwait(false);
        RadioProxy = await RadioProxy.GetInstanceAsync(BluetoothAdapterProxy, this).ConfigureAwait(false);
    }

    /// <inheritdoc />
    /// <remarks>
    /// On Windows, this checks if the radio is on and the adapter supports BLE peripheral role.
    /// </remarks>
    protected override void NativeRefreshIsBluetoothOn()
    {
        RadioState = RadioProxy?.Radio.State ?? RadioState.Unknown;
        IsBluetoothOn = RadioState == RadioState.On && (BluetoothAdapterProxy?.BluetoothAdapter.IsLowEnergySupported ?? false) && (BluetoothAdapterProxy?.BluetoothAdapter.IsCentralRoleSupported ?? false);
    }
}
