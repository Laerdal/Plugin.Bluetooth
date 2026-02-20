namespace Bluetooth.Maui.Platforms.Droid.Permissions;

/// <summary>
///     Base class for Android-specific permission handling.
/// </summary>
public abstract class BaseAndroidPermissionHandler : Microsoft.Maui.ApplicationModel.Permissions.BasePlatformPermission
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseAndroidPermissionHandler" /> class.
    /// </summary>
    /// <param name="permissionName">The Android permission name.</param>
    /// <param name="permissionIsRuntime">Indicates whether the permission is a runtime permission.</param>
    protected BaseAndroidPermissionHandler(string permissionName, bool permissionIsRuntime)
    {
        PermissionName = permissionName;
        PermissionIsRuntime = permissionIsRuntime;
    }

    protected string PermissionName { get; }

    protected bool PermissionIsRuntime { get; }

    public override (string androidPermission, bool isRuntime)[] RequiredPermissions => [(PermissionName, PermissionIsRuntime)];

    /// <summary>
    ///     Requests the permission if needed asynchronously.
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
            throw new PermissionException($"Permission {PermissionName} was not granted. Is runtime: {PermissionIsRuntime}. Current status: {status}.");
        }
    }
}