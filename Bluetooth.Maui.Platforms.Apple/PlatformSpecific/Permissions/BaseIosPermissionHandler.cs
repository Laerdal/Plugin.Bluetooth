namespace Bluetooth.Maui.Platforms.Apple.PlatformSpecific.Permissions;

/// <summary>
/// Base class for iOS-specific permission handling.
/// </summary>
public abstract class BaseIosPermissionHandler : Microsoft.Maui.ApplicationModel.Permissions.BasePlatformPermission
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseIosPermissionHandler"/> class.
    /// </summary>
    /// <param name="infoPlistKey">The Info.plist key associated with this permission.</param>
    protected BaseIosPermissionHandler(string infoPlistKey)
    {
        _infoPlistKey = infoPlistKey;
    }

    private readonly string _infoPlistKey;

    /// <summary>
    /// Gets the required Info.plist keys for this permission.
    /// </summary>
    protected override Func<IEnumerable<string>> RequiredInfoPlistKeys => () =>
    [
        _infoPlistKey
    ];

    /// <summary>
    /// Requests the permission if needed asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="PermissionException"></exception>
    public async Task RequestIfNeededAsync()
    {
        if (await CheckStatusAsync().ConfigureAwait(false) == PermissionStatus.Granted)
        {
            return;
        }

        var status = await RequestAsync().ConfigureAwait(false);
        if (status != PermissionStatus.Granted)
        {
            throw new PermissionException($"Permission {_infoPlistKey} was not granted. Current status: {status}.");
        }
    }
}
