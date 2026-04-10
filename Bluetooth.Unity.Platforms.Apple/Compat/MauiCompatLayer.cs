// Unity-compatible shims for Microsoft.Maui.ApplicationModel APIs.
// Replaces MAUI's permission system (uses CoreBluetooth authorization) and
// MainThread dispatcher (uses CoreFoundation.DispatchQueue.MainQueue) with
// platform-native equivalents, so the linked Apple BLE source compiles without MAUI.
//
// NOTE: Types are defined in Microsoft.Maui.ApplicationModel namespaces intentionally
// so that the linked Bluetooth.Maui.Platforms.Apple source code compiles unchanged.

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
///     Base class for Apple platform permission handlers.
///     Uses CoreBluetooth's <see cref="CoreBluetooth.CBManager.Authorization"/> to determine
///     the current Bluetooth permission status instead of MAUI's permission abstractions.
/// </summary>
public abstract class BasePlatformPermission
{
    /// <summary>
    ///     Checks whether Bluetooth permission has been granted via CoreBluetooth authorization.
    /// </summary>
    /// <returns>
    ///     <see cref="PermissionStatus.Granted"/> if <see cref="CBManager.Authorization"/> is
    ///     <see cref="CBManagerAuthorization.AllowedAlways"/>; <see cref="PermissionStatus.Denied"/> otherwise.
    /// </returns>
    public virtual Task<PermissionStatus> CheckStatusAsync()
    {
        var auth = CBManager.Authorization;
        var status = auth == CBManagerAuthorization.AllowedAlways
            ? PermissionStatus.Granted
            : PermissionStatus.Denied;
        return Task.FromResult(status);
    }

    /// <summary>
    ///     Returns the current authorization status. On Apple, CoreBluetooth automatically
    ///     prompts the user when a <see cref="CBCentralManager"/> or <see cref="CBPeripheralManager"/>
    ///     is created. No additional request call is needed.
    /// </summary>
    public virtual Task<PermissionStatus> RequestAsync() => CheckStatusAsync();
}

/// <summary>
///     Static helper for checking and requesting permissions.
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
    ///     Returns the current permission status for the specified type.
    /// </summary>
    public static Task<PermissionStatus> RequestAsync<TPermission>()
        where TPermission : BasePlatformPermission, new()
        => new TPermission().RequestAsync();
}

// ─────────────────────────────────────────────────────────────────────────────

namespace Microsoft.Maui.ApplicationModel;

/// <summary>
///     Unity-compatible main-thread dispatcher shim.
///     Uses <see cref="CoreFoundation.DispatchQueue.MainQueue"/> instead of
///     MAUI's <c>Microsoft.Maui.ApplicationModel.MainThread</c>.
/// </summary>
public static class MainThread
{
    /// <summary>
    ///     Dispatches <paramref name="action"/> on the main UI thread using
    ///     <see cref="CoreFoundation.DispatchQueue.MainQueue"/>.
    /// </summary>
    public static void BeginInvokeOnMainThread(Action action)
        => CoreFoundation.DispatchQueue.MainQueue.DispatchAsync(action);
}
