namespace Bluetooth.Maui.Platforms.Windows.Permissions;

/// <summary>
/// Windows implementation of the Bluetooth permission manager.
/// </summary>
/// <remarks>
/// Windows uses a capability-based permission model where Bluetooth permissions are declared
/// in Package.appxmanifest and automatically granted at install time. No runtime permission
/// prompts are shown to users.
/// </remarks>
public class BluetoothPermissionManager : IBluetoothPermissionManager
{
    /// <inheritdoc/>
    /// <remarks>
    /// On Windows, Bluetooth permissions are capability-based and granted at install time
    /// if the 'bluetooth' capability is declared in Package.appxmanifest.
    /// This method always returns true.
    /// </remarks>
    public ValueTask<bool> HasBluetoothPermissionsAsync()
    {
        // On Windows, Bluetooth permissions are capability-based and granted at install time
        return ValueTask.FromResult(true);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// On Windows, Bluetooth permissions are capability-based and granted at install time
    /// if the 'bluetooth' capability is declared in Package.appxmanifest.
    /// This method always returns true.
    /// </remarks>
    public ValueTask<bool> HasScannerPermissionsAsync()
    {
        // On Windows, Bluetooth permissions are capability-based and granted at install time
        return ValueTask.FromResult(true);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// On Windows, Bluetooth permissions are capability-based and granted at install time
    /// if the 'bluetooth' capability is declared in Package.appxmanifest.
    /// This method always returns true.
    /// </remarks>
    public ValueTask<bool> HasBroadcasterPermissionsAsync()
    {
        // On Windows, Bluetooth permissions are capability-based and granted at install time
        return ValueTask.FromResult(true);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// On Windows, no runtime permission request is needed. Bluetooth permissions are
    /// declared in Package.appxmanifest and granted at install time.
    /// This method always returns true.
    /// </remarks>
    public ValueTask<bool> RequestBluetoothPermissionsAsync()
    {
        // No runtime request needed on Windows - permissions are declared at install time
        return ValueTask.FromResult(true);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// On Windows, no runtime permission request is needed. Bluetooth permissions are
    /// declared in Package.appxmanifest and granted at install time.
    /// This method always returns true.
    /// </remarks>
    public ValueTask<bool> RequestScannerPermissionsAsync()
    {
        // No runtime request needed on Windows - permissions are declared at install time
        return ValueTask.FromResult(true);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// On Windows, no runtime permission request is needed. Bluetooth permissions are
    /// declared in Package.appxmanifest and granted at install time.
    /// This method always returns true.
    /// </remarks>
    public ValueTask<bool> RequestBroadcasterPermissionsAsync()
    {
        // No runtime request needed on Windows - permissions are declared at install time
        return ValueTask.FromResult(true);
    }
}
