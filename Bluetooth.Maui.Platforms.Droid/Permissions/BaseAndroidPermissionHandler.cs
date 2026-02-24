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

    /// <summary>
    ///     Gets the Android permission name.
    /// </summary>
    protected string PermissionName { get; }

    /// <summary>
    ///     Gets a value indicating whether this is a runtime permission that requires user approval.
    /// </summary>
    protected bool PermissionIsRuntime { get; }

    /// <summary>
    ///     Gets the list of required Android permissions with their runtime status.
    /// </summary>
    public override (string androidPermission, bool isRuntime)[] RequiredPermissions => [(PermissionName, PermissionIsRuntime)];

    /// <summary>
    ///     Requests the permission from the user if it has not already been granted.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="PermissionException">Thrown when the permission is not granted after the spec.</exception>
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
