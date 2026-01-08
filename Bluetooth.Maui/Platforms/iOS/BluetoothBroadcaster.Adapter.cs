using Bluetooth.Maui.PlatformSpecific;

namespace Bluetooth.Maui;

public partial class BluetoothBroadcaster
{
    /// <summary>
    /// Gets an auto-reset event that is signaled when the Bluetooth adapter state changes.
    /// </summary>
    public AutoResetEvent StateChangedEvent { get; } = new AutoResetEvent(false);

    /// <summary>
    /// Gets or sets the current state of the Core Bluetooth peripheral manager.
    /// </summary>
    /// <remarks>
    /// The state indicates whether Bluetooth is powered on, off, unauthorized, unsupported, etc.
    /// </remarks>
    public CBManagerState State
    {
        get => GetValue(CBManagerState.Unknown);
        protected set => SetValue(value);
    }

    /// <summary>
    /// Waits asynchronously for the peripheral manager to reach a specific state.
    /// </summary>
    /// <param name="state">The target state to wait for.</param>
    /// <param name="timeout">Optional timeout for the wait operation.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that completes when the target state is reached.</returns>
    public ValueTask WaitForStateAsync(CBManagerState state, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        return WaitForPropertyToBeOfValue(nameof(State), state, timeout, cancellationToken);
    }

    /// <summary>
    /// Waits asynchronously for the peripheral manager state to be known (not <see cref="CBManagerState.Unknown"/>).
    /// </summary>
    /// <param name="timeout">Optional timeout for the wait operation.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that completes when the state becomes known.</returns>
    public ValueTask WaitForStateToBeKnownAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        return WaitForPropertyToBeDifferentThanValue(nameof(State), CBManagerState.Unknown, timeout, cancellationToken);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// On iOS, this checks if the Core Bluetooth peripheral manager is powered on.
    /// </remarks>
    protected override void NativeRefreshIsBluetoothOn()
    {
        State = CbPeripheralManagerProxy?.CbPeripheralManager.State ?? CBManagerState.Unknown;
        IsBluetoothOn = State == CBManagerState.PoweredOn;
    }

    /// <summary>
    /// Called when the peripheral manager's state is updated.
    /// </summary>
    /// <remarks>
    /// This method updates the <see cref="State"/> property and refreshes the <see cref="BaseBluetoothActivity.IsBluetoothOn"/> flag.
    /// </remarks>
    public void StateUpdated()
    {
        NativeRefreshIsBluetoothOn();
    }

    /// <summary>
    /// Gets or sets the dispatch queue on which peripheral manager events will be dispatched.
    /// </summary>
    /// <remarks>
    /// If <c>null</c>, the default global queue will be used.
    /// </remarks>
    public DispatchQueue? DispatchQueue { get; set; }

    /// <inheritdoc/>
    /// <remarks>
    /// Initializes the Core Bluetooth peripheral manager and ensures Bluetooth permissions are declared (iOS 13+).
    /// Waits for the peripheral manager state to become known before completing.
    /// </remarks>
    protected async override ValueTask NativeInitializeAsync()
    {
        if (OperatingSystem.IsIOSVersionAtLeast(13) || OperatingSystem.IsMacCatalystVersionAtLeast(10, 15))
        {
            BluetoothPermissions.BluetoothAlways.EnsureDeclared();
        }

        CbPeripheralManagerProxy = new CbPeripheralManagerProxy(this, DispatchQueue);
        ArgumentNullException.ThrowIfNull(CbPeripheralManagerProxy);

        // Wait for state to be known
        await WaitForStateToBeKnownAsync().ConfigureAwait(false);
        NativeRefreshIsBluetoothOn();
    }
}
