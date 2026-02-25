namespace Bluetooth.Maui.Platforms.Apple.Exceptions;

/// <summary>
///     Represents an exception that occurs when a required background mode is missing from the Info.plist file for Apple Bluetooth operations.
/// </summary>
/// <remarks>
///     This exception is thrown when the application's Info.plist file is missing required UIBackgroundModes entries that are necessary for Bluetooth functionality on iOS and macOS platforms, such as "bluetooth-central" for state restoration.
/// </remarks>
public class AppleMissingBackgroundModeInInfoPlistException : BluetoothException
{
    /// <summary>
    ///     Gets the background mode that is missing from the Info.plist file.
    /// </summary>
    public string BackgroundMode { get; private set; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="AppleMissingBackgroundModeInInfoPlistException" /> class
    ///     for a missing background mode.
    /// </summary>
    /// <param name="backgroundMode">The background mode that is missing (e.g., "bluetooth-central").</param>
    public AppleMissingBackgroundModeInInfoPlistException(string backgroundMode) :
        base($"Your Info.plist is missing the required UIBackgroundModes entry. Add '{backgroundMode}' to the 'UIBackgroundModes' array in your Info.plist file to enable Bluetooth state restoration.")
    {
        BackgroundMode = backgroundMode;
    }

    /// <summary>
    ///     Ensures that the specified background mode is configured, throwing an exception if not.
    /// </summary>
    /// <param name="backgroundMode">The background mode to verify.</param>
    /// <exception cref="AppleMissingBackgroundModeInInfoPlistException">Thrown when the background mode is not configured.</exception>
    public static void ThrowIfMissingBackgroundMode(string backgroundMode)
    {
        if (!PlistExtensions.HasBackgroundMode(backgroundMode))
        {
            throw new AppleMissingBackgroundModeInInfoPlistException(backgroundMode);
        }
    }
}