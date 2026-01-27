namespace Bluetooth.Maui.PlatformSpecific;

/// <summary>
/// Extension methods for Windows <see cref="AppCapability"/> objects to simplify
/// permission request workflows.
/// </summary>
public static class AppCapabilityExtensions
{
    /// <summary>
    /// Requests access to the app capability if it's not already allowed.
    /// If access is denied, throws a <see cref="NativePermissionException"/>.
    /// </summary>
    /// <param name="appCapability">The app capability to request access for.</param>
    /// <returns>The resulting access status after the request.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="appCapability"/> is null.</exception>
    /// <exception cref="NativePermissionException">Thrown when access is not granted.</exception>
    [SupportedOSPlatform("Windows10.0.18362.0")]
    public async static Task<AppCapabilityAccessStatus> RequestIfNeededAsync(this AppCapability appCapability)
    {
        ArgumentNullException.ThrowIfNull(appCapability);
        if (appCapability.CheckAccess() == AppCapabilityAccessStatus.Allowed)
        {
            return AppCapabilityAccessStatus.Allowed;
        }

        var accessStatus = await appCapability.RequestAccessAsync(); // AsTask() ??

        if (accessStatus != AppCapabilityAccessStatus.Allowed)
        {
            throw new NativePermissionException(appCapability.CapabilityName, accessStatus.ToPermissionStatus());
        }

        return accessStatus;
    }
}
