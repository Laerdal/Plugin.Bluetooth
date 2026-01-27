namespace Bluetooth.Maui.PlatformSpecific;

/// <summary>
/// Extension methods for converting Windows <see cref="AppCapabilityAccessStatus"/> values
/// to platform-agnostic permission status values.
/// </summary>
public static class AppCapabilityAccessStatusExtensions
{
    /// <summary>
    /// Converts a Windows <see cref="AppCapabilityAccessStatus"/> to a cross-platform <see cref="PermissionStatus"/>.
    /// </summary>
    /// <param name="status">The Windows app capability access status to convert.</param>
    /// <returns>The corresponding cross-platform permission status.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the status value is not recognized.</exception>
    [SupportedOSPlatform("Windows10.0.18362.0")]
    public static PermissionStatus ToPermissionStatus(this AppCapabilityAccessStatus status)
    {
        return status switch
        {
            AppCapabilityAccessStatus.Allowed => PermissionStatus.Granted,
            AppCapabilityAccessStatus.DeniedByUser => PermissionStatus.Denied,
            AppCapabilityAccessStatus.DeniedBySystem => PermissionStatus.Disabled,
            AppCapabilityAccessStatus.NotDeclaredByApp => PermissionStatus.Denied,
            AppCapabilityAccessStatus.UserPromptRequired => PermissionStatus.Unknown,
            var _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }

}
