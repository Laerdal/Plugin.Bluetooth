// Unity-compatible shims for Microsoft.Maui.ApplicationModel APIs.
// These replace MAUI's permission framework with standard Android platform APIs,
// allowing the platform BLE code to compile and run without a MAUI runtime dependency.
//
// NOTE: This file defines types in the Microsoft.Maui.ApplicationModel namespace
// intentionally, so that the linked Bluetooth.Maui.Platforms.Droid source code
// continues to compile unchanged under the net6.0-android TFM.

// ReSharper disable CheckNamespace
#pragma warning disable CS1591

namespace Microsoft.Maui.ApplicationModel.Permissions;

/// <summary>
///     Represents the status of a platform permission.
/// </summary>
public enum PermissionStatus
{
    Unknown = 0,
    Denied = 1,
    Disabled = 2,
    Granted = 3,
    Restricted = 4,
    Limited = 5,
}

/// <summary>
///     Exception thrown when a required permission is not granted.
/// </summary>
public class PermissionException : Exception
{
    /// <inheritdoc />
    public PermissionException(string message) : base(message) { }

    /// <inheritdoc />
    public PermissionException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
///     Base class for Android platform permission handlers.
///     Unity-compatible implementation that uses Android's native PackageManager to check
///     permission status. For Unity apps, runtime permission requesting should be handled
///     via <c>UnityEngine.Android.Permission.RequestUserPermission()</c> or declared in
///     <c>AndroidManifest.xml</c>.
/// </summary>
public abstract class BasePlatformPermission
{
    /// <summary>
    ///     Gets the list of Android permissions required by this handler, along with
    ///     whether each is a runtime permission.
    /// </summary>
    public abstract (string androidPermission, bool isRuntime)[] RequiredPermissions { get; }

    /// <summary>
    ///     Checks whether all required permissions are currently granted using the
    ///     Android <see cref="Application.Context"/>.
    /// </summary>
    /// <returns>
    ///     <see cref="PermissionStatus.Granted"/> if all permissions are granted;
    ///     <see cref="PermissionStatus.Denied"/> otherwise.
    /// </returns>
    public virtual Task<PermissionStatus> CheckStatusAsync()
    {
        var context = Application.Context;
        var allGranted = RequiredPermissions.All(p =>
            context.CheckSelfPermission(p.androidPermission) == global::Android.Content.PM.Permission.Granted);

        return Task.FromResult(allGranted ? PermissionStatus.Granted : PermissionStatus.Denied);
    }

    /// <summary>
    ///     Returns the current permission status without proactively requesting permissions.
    ///     <para>
    ///         <strong>Unity limitation:</strong> Runtime permission requesting from a library requires
    ///         an Android <c>Activity</c> reference, which is not available here. Use
    ///         <c>UnityEngine.Android.Permission.RequestUserPermission()</c> in your MonoBehaviour
    ///         before initializing Bluetooth, or declare all required permissions in
    ///         <c>AndroidManifest.xml</c>.
    ///     </para>
    /// </summary>
    public virtual Task<PermissionStatus> RequestAsync() => CheckStatusAsync();
}

/// <summary>
///     Static helper for checking and requesting permissions.
///     Unity-compatible shim that delegates to <see cref="BasePlatformPermission"/> implementations.
/// </summary>
public static class Permissions
{
    /// <summary>
    ///     Checks the status of the specified permission type.
    /// </summary>
    public static Task<PermissionStatus> CheckStatusAsync<TPermission>()
        where TPermission : BasePlatformPermission, new()
        => new TPermission().CheckStatusAsync();

    /// <summary>
    ///     Checks the status of the specified permission type; for Unity, this does not
    ///     automatically request permissions — the app must handle this via Unity's API.
    /// </summary>
    public static Task<PermissionStatus> RequestAsync<TPermission>()
        where TPermission : BasePlatformPermission, new()
        => new TPermission().RequestAsync();
}
