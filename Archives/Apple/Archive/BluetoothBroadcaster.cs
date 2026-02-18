using Bluetooth.Abstractions.Broadcasting.EventArgs;
using Bluetooth.Maui.PlatformSpecific;

using Microsoft.Extensions.Options;

namespace Bluetooth.Maui;

/// <summary>
/// iOS implementation of the Bluetooth broadcaster using Core Bluetooth's CBPeripheralManager.
/// </summary>
/// <remarks>
/// This implementation allows the device to act as a BLE peripheral and advertise services.
/// </remarks>
public partial class BluetoothBroadcaster : BaseBluetoothBroadcaster, CbPeripheralManagerWrapper.ICbPeripheralManagerDelegate
{
    /// <summary>
    /// Gets the CbPeripheralManagerWrapper instance.
    /// </summary>
    public CbPeripheralManagerWrapper CbPeripheralManagerWrapper => ((BluetoothAdapter)Adapter).CbPeripheralManagerWrapper ?? throw new InvalidOperationException("BluetoothAdapter.CbPeripheralManagerWrapper is not available");

    private readonly IOptions<BluetoothAppleOptions>? _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="BluetoothScanner"/> class.
    /// </summary>
    /// <param name="adapter">The Bluetooth adapter.</param>
    /// <param name="options">The Bluetooth Apple options.</param>
    public BluetoothBroadcaster(IBluetoothAdapter adapter, IOptions<BluetoothAppleOptions>? options) :
        base(adapter)
    {
        _options = options;
    }

    #region CbPeripheralManagerWrapper.ICbPeripheralManagerDelegate

    /// <inheritdoc/>
    public void WillRestoreState(NSDictionary dict)
    {
        // Placeholder for future implementation if needed
    }

    /// <inheritdoc/>
    public void DidOpenL2CapChannel(NSError? error, CBL2CapChannel? channel)
    {
        // Placeholder for future implementation if needed
    }

    /// <inheritdoc/>
    public void DidPublishL2CapChannel(NSError? error, ushort psm)
    {
        // Placeholder for future implementation if needed
    }

    /// <inheritdoc/>
    public void DidUnpublishL2CapChannel(NSError? error, ushort psm)
    {
        // Placeholder for future implementation if needed
    }

    /// <inheritdoc/>
    public void StateUpdated(CBManagerState peripheralState)
    {
        if (Adapter is not BluetoothAdapter bluetoothAdapter)
        {
            throw new InvalidOperationException("Adapter is not a BluetoothAdapter");
        }
        bluetoothAdapter.OnStateChanged();
    }

    #endregion

    /// <inheritdoc/>
    public async override Task EnsurePermissionsAsync(CancellationToken cancellationToken = default)
    {
        if (OperatingSystem.IsIOSVersionAtLeast(13) || OperatingSystem.IsMacCatalystVersionAtLeast(10, 15))
        {
            BluetoothPermissions.BluetoothAlways.EnsureDeclared();
        }

        if (Adapter is not BluetoothAdapter bluetoothAdapter)
        {
            throw new InvalidOperationException("Adapter is not a BluetoothAdapter");
        }

        bluetoothAdapter.CreateCbPeripheralManagerWrapper(this, _options);
        await CbPeripheralManagerWrapper.WaitForStateToBeKnownAsync(cancellationToken).ConfigureAwait(false);
    }
}
