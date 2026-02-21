using Bluetooth.Maui.Platforms.Win.NativeObjects;

namespace Bluetooth.Maui.Platforms.Win.Permissions;

/// <summary>
///     Windows implementation of the Bluetooth permission manager.
/// </summary>
/// <remarks>
///     Windows uses a capability-based permission model where Bluetooth permissions are declared
///     in Package.appxmanifest and automatically granted at install time. No runtime permission
///     prompts are shown to users.
/// </remarks>
public class BluetoothPermissionManager : IBluetoothPermissionManager
{
    /// <summary>
    ///     Gets the Bluetooth adapter wrapper that provides access to the Bluetooth adapter properties and state.
    /// </summary>
    private IBluetoothAdapterWrapper BluetoothAdapterWrapper { get; }

    /// <summary>
    ///    Gets the Bluetooth radio wrapper that provides access to the Bluetooth radio properties and state.
    /// </summary>
    private IRadioWrapper RadioWrapper { get; }

    /// <summary>
    ///    Initializes a new instance of the <see cref="BluetoothPermissionManager" /> class with the specified Bluetooth adapter and radio wrappers.
    /// </summary>
    public BluetoothPermissionManager(IBluetoothAdapterWrapper bluetoothAdapterWrapper, IRadioWrapper radioWrapper)
    {
        BluetoothAdapterWrapper = bluetoothAdapterWrapper;
        RadioWrapper = radioWrapper;
    }
    
    /// <inheritdoc />
    /// <remarks>
    ///     On Windows, Bluetooth permissions are capability-based and granted at install time
    ///     if the 'bluetooth' capability is declared in Package.appxmanifest.
    ///     This method always returns true.
    /// </remarks>
    public ValueTask<bool> HasBluetoothPermissionsAsync()
    {
        // On Windows, Bluetooth permissions are capability-based and granted at install time
        return ValueTask.FromResult(true);
    }

    /// <inheritdoc />
    /// <remarks>
    ///     On Windows, Bluetooth permissions are capability-based and granted at install time
    ///     if the 'bluetooth' capability is declared in Package.appxmanifest.
    ///     This method always returns true.
    /// </remarks>
    public ValueTask<bool> HasScannerPermissionsAsync()
    {
        // On Windows, Bluetooth permissions are capability-based and granted at install time
        return ValueTask.FromResult(true);
    }

    /// <inheritdoc />
    /// <remarks>
    ///     On Windows, Bluetooth permissions are capability-based and granted at install time
    ///     if the 'bluetooth' capability is declared in Package.appxmanifest.
    ///     This method always returns true.
    /// </remarks>
    public ValueTask<bool> HasBroadcasterPermissionsAsync()
    {
        // On Windows, Bluetooth permissions are capability-based and granted at install time
        return ValueTask.FromResult(true);
    }

    /// <param name="cancellationToken"></param>
    /// <inheritdoc />
    /// <remarks>
    ///     On Windows, no runtime permission request is needed. Bluetooth permissions are
    ///     declared in Package.appxmanifest and granted at install time.
    ///     This method always returns true.
    /// </remarks>
    public async ValueTask<bool> RequestBluetoothPermissionsAsync(CancellationToken cancellationToken = default)
    {
        var adapter = await BluetoothAdapterWrapper.GetBluetoothAdapterAsync(cancellationToken).ConfigureAwait(false);
        var radio = await RadioWrapper.GetRadioAsync(cancellationToken).ConfigureAwait(false);

        // Is Peripheral role supported ?
        if (!adapter.IsPeripheralRoleSupported)
        {
            throw new PermissionException($"BluetoothAdapter.IsPeripheralRoleSupported = false");
        }

        if (radio.State != RadioState.On) // trying to turn on BT Radio
        {
            var access = await Windows.Devices.Radios.Radio.RequestAccessAsync();
            if (access != RadioAccessStatus.Allowed)
            {
                throw new PermissionException($"Radio.RequestAccessAsync = {access}, Did you forget to add '<DeviceCapability Name=\"radios\" />' in your Manifest's Capabilities ?");
            }

            var success = await radio.SetStateAsync(RadioState.On);
            if (success != RadioAccessStatus.Allowed)
            {
                throw new PermissionException($"Radio.SetStateAsync(RadioState.On) = {success}, Did you forget to add '<DeviceCapability Name=\"radios\" />' in your Manifest's Capabilities ?");
            }
        }
        return true;
    }

    /// <inheritdoc />
    /// <remarks>
    ///     On Windows, no runtime permission request is needed. Bluetooth permissions are
    ///     declared in Package.appxmanifest and granted at install time.
    ///     This method always returns true.
    /// </remarks>
    public ValueTask<bool> RequestScannerPermissionsAsync()
    {
        // No runtime request needed on Windows - permissions are declared at install time
        return ValueTask.FromResult(true);
    }

    /// <inheritdoc />
    /// <remarks>
    ///     On Windows, no runtime permission request is needed. Bluetooth permissions are
    ///     declared in Package.appxmanifest and granted at install time.
    ///     This method always returns true.
    /// </remarks>
    public ValueTask<bool> RequestBroadcasterPermissionsAsync()
    {
        // No runtime request needed on Windows - permissions are declared at install time
        return ValueTask.FromResult(true);
    }
}