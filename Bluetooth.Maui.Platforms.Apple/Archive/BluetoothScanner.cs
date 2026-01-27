using Bluetooth.Maui.PlatformSpecific;

using Microsoft.Extensions.Options;

namespace Bluetooth.Maui;

/// <summary>
/// iOS implementation of the Bluetooth scanner using Core Bluetooth framework.
/// </summary>
/// <remarks>
/// This implementation uses <see cref="CBCentralManager"/> to scan for BLE peripherals.
/// </remarks>
public partial class BluetoothScanner : BaseBluetoothScanner, CbCentralManagerWrapper.ICbCentralManagerProxyDelegate
{
    /// <summary>
    /// Gets the CbCentralManagerWrapper instance.
    /// </summary>
    public CBCentralManager? CbCentralManager { get; private set; }
    private readonly IOptions<BluetoothAppleOptions>? _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="BluetoothScanner"/> class.
    /// </summary>
    /// <param name="adapter">The Bluetooth adapter.</param>
    /// <param name="knownServicesAndCharacteristicsRepository">The repository for known services and characteristics.</param>
    /// <param name="options">The Bluetooth Apple options.</param>
    public BluetoothScanner(IBluetoothAdapter adapter, IBluetoothCharacteristicAccessServicesRepository knownServicesAndCharacteristicsRepository, IOptions<BluetoothAppleOptions>? options) :
        base(adapter, knownServicesAndCharacteristicsRepository)
    {
        _options = options;
    }

    /// <summary>
    /// Gets or sets the current state of the Core Bluetooth central manager.
    /// </summary>
    /// <remarks>
    /// The state indicates whether Bluetooth is powered on, off, unauthorized, unsupported, etc.
    /// </remarks>
    public CBManagerState State
    {
        get => GetValue(CBManagerState.Unknown);
        private set => SetValue(value);
    }

    #region CbCentralManagerWrapper

    /// <summary>
    /// Called when the central manager is about to restore its state.
    /// </summary>
    /// <param name="dict">A dictionary containing state restoration information.</param>
    /// <remarks>
    /// This is called during state restoration when the app is relaunched in the background.
    /// </remarks>
    public void WillRestoreState(NSDictionary dict)
    {
        // Placeholder for future implementation if needed
    }

    /// <inheritdoc/>
    public void UpdatedState(CBManagerState centralState)
    {
        State = centralState;
    }

    #endregion

    /// <inheritdoc/>
    public async override Task EnsurePermissionsAsync(CancellationToken cancellationToken = default)
    {
        if (OperatingSystem.IsIOSVersionAtLeast(13) || OperatingSystem.IsMacCatalystVersionAtLeast(10, 15))
        {
            BluetoothPermissions.BluetoothAlways.EnsureDeclared();
        }

        CbCentralManager ??= ((BluetoothAdapter)Adapter).GetCbCentralManagerWrapper(this, _options);
        await WaitForStateToBeKnownAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously waits until the Core Bluetooth central manager's state is known (not Unknown).
    /// </summary>
    /// <param name="timeout">An optional timeout for the wait operation.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the wait operation.</param>
    /// <returns>A task that completes when the central manager's state is known.</returns>
    public ValueTask<CBManagerState> WaitForStateToBeKnownAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        return WaitForPropertyToBeDifferentThanValue(nameof(State), CBManagerState.Unknown, timeout, cancellationToken);
    }

    /// <summary>
    /// Asynchronously waits until the Core Bluetooth central manager is powered on.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to cancel the wait operation.</param>
    /// <returns>A task that completes when the central manager is powered on.</returns>
    public ValueTask<CBManagerState> WaitForStateToBePoweredOnAsync(CancellationToken cancellationToken = default)
    {
        return WaitForPropertyToBeOfValue(nameof(State), CBManagerState.PoweredOn, null, cancellationToken);
    }
}
