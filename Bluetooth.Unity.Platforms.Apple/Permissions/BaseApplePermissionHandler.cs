namespace Bluetooth.Maui.Platforms.Apple.Permissions;

using Microsoft.Maui.ApplicationModel.Permissions;

/// <summary>
///     Base class for Apple platform Bluetooth permission handlers.
///     Unity-compatible replacement that uses CoreBluetooth's
///     <see cref="CoreBluetooth.CBManager.Authorization"/> to check permission status,
///     instead of MAUI's Info.plist-based permission framework.
/// </summary>
public abstract class BaseApplePermissionHandler : BasePlatformPermission
{
    /// <summary>
    ///     Initializes a new instance of <see cref="BaseApplePermissionHandler"/>.
    /// </summary>
    /// <param name="infoPlistKey">The Info.plist key associated with this permission (informational only).</param>
    protected BaseApplePermissionHandler(string infoPlistKey)
    {
        InfoPlistKey = infoPlistKey;
    }

    /// <summary>
    ///     Gets the Info.plist key associated with this permission.
    /// </summary>
    protected string InfoPlistKey { get; }

    /// <summary>
    ///     Requests the permission if it has not already been granted.
    ///     On Apple platforms, CoreBluetooth handles the permission prompt automatically
    ///     when the <see cref="CoreBluetooth.CBCentralManager"/> or
    ///     <see cref="CoreBluetooth.CBPeripheralManager"/> is initialized.
    /// </summary>
    public async Task RequestIfNeededAsync()
    {
        if (await CheckStatusAsync().ConfigureAwait(false) == PermissionStatus.Granted)
        {
            return;
        }

        var status = await RequestAsync().ConfigureAwait(false);
        if (status != PermissionStatus.Granted)
        {
            throw new PermissionException(
                $"Bluetooth permission ({InfoPlistKey}) was not granted. Current status: {status}.");
        }
    }
}
