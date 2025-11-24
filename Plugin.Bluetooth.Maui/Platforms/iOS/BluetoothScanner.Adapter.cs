using Plugin.Bluetooth.Maui.PlatformSpecific;

namespace Plugin.Bluetooth.Maui;

public partial class BluetoothScanner
{
    public AutoResetEvent StateChangedEvent { get; } = new AutoResetEvent(false);

    public CBManagerState State
    {
        get => GetValue(CBManagerState.Unknown);
        protected set => SetValue(value);
    }

    public ValueTask WaitForStateAsync(CBManagerState state, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        return WaitForPropertyToBeOfValue(nameof(State), state, timeout, cancellationToken);
    }

    public ValueTask WaitForStateToBeKnownAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        return WaitForPropertyToBeDifferentThanValue(nameof(State), CBManagerState.Unknown, timeout, cancellationToken);
    }

    protected override void NativeRefreshIsBluetoothOn()
    {
        State = CbCentralManagerProxy?.CbCentralManager.State ?? CBManagerState.Unknown;
        IsBluetoothOn = State == CBManagerState.PoweredOn;
    }

    public void UpdatedState()
    {
        NativeRefreshIsBluetoothOn();
    }


    public CBCentralInitOptions? CbCentralInitOptions { get; set; }
    public DispatchQueue? DispatchQueue { get; set; }

    protected async override ValueTask NativeInitializeAsync()
    {
        if (OperatingSystem.IsIOSVersionAtLeast(13))
        {
            BluetoothPermissions.BluetoothAlways.EnsureDeclared();
        }

        CbCentralManagerProxy = new CbCentralManagerProxy(this, CbCentralInitOptions, DispatchQueue);
        ArgumentNullException.ThrowIfNull(CbCentralManagerProxy);
        // Initial State is Unknown
        await WaitForStateToBeKnownAsync().ConfigureAwait(false);
        NativeRefreshIsBluetoothOn();
    }

}
