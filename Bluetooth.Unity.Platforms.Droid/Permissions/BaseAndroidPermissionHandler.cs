namespace Bluetooth.Maui.Platforms.Droid.Permissions;

// NOTE: 'Microsoft.Maui.ApplicationModel.Permissions' here is NOT the real MAUI assembly.
// It is a Unity-compatible compat shim defined in Bluetooth.Unity.Platforms.Droid/Compat/MauiCompatLayer.cs
// that replaces MAUI's permission framework with standard Android PackageManager calls.
using Microsoft.Maui.ApplicationModel.Permissions;

/// <summary>
///     Base class for Android-specific Bluetooth permission handlers.
///     Unity-compatible replacement that uses Android's native permission check API
///     instead of MAUI's <c>Microsoft.Maui.ApplicationModel.Permissions.BasePlatformPermission</c>.
/// </summary>
public abstract class BaseAndroidPermissionHandler : BasePlatformPermission
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

    /// <inheritdoc />
    public override (string androidPermission, bool isRuntime)[] RequiredPermissions
        => [(PermissionName, PermissionIsRuntime)];

    /// <summary>
    ///     Requests the permission if it has not already been granted.
    ///     For Unity projects, this only checks the current status; use
    ///     <c>UnityEngine.Android.Permission.RequestUserPermission()</c> to request at runtime.
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
                $"Permission {PermissionName} was not granted. Is runtime: {PermissionIsRuntime}. Current status: {status}.");
        }
    }
}
