namespace Bluetooth.Maui.Platforms.Apple.Tools;

/// <summary>
/// Extension methods for working with iOS Info.plist file entries and background modes.
/// </summary>
public static class PlistExtensions
{
    /// <summary>
    /// Checks if the specified background mode is enabled in the app's Info.plist file.
    /// </summary>
    /// <param name="bgMode">The background mode to check for.</param>
    /// <returns>True if the background mode is enabled, false otherwise.</returns>
    public static bool HasBackgroundMode(string bgMode)
    {
        var key = new NSString("UIBackgroundModes");

        if (!HasInfoPlistEntry(key))
        {
            return false;
        }

        if (!(NSBundle.MainBundle.InfoDictionary[key] is NSArray array))
        {
            return false;
        }

        using var mode = new NSString(bgMode);
        return array.Contains(mode);
    }

    /// <summary>
    /// Ensures that the specified background mode is enabled in the app's Info.plist file.
    /// Throws an exception if the background mode is not found.
    /// </summary>
    /// <param name="key">The background mode key to verify.</param>
    /// <exception cref="ArgumentException">Thrown when the background mode is not configured.</exception>
    public static void EnsureHasBackgroundMode(string key)
    {
        if (!HasBackgroundMode(key))
        {
            throw new ArgumentException($"You must add '{key}' in the 'UIBackgroundModes' node of your Info.plist file");
        }
    }

    /// <summary>
    /// Checks if the specified key exists in the app's Info.plist file.
    /// </summary>
    /// <param name="key">The Info.plist key to check for.</param>
    /// <returns>True if the key exists, false otherwise.</returns>
    public static bool HasInfoPlistEntry(string key)
    {
        using var nsKey = new NSString(key);
        return NSBundle.MainBundle.InfoDictionary.ContainsKey(nsKey);
    }

    /// <summary>
    /// Ensures that the specified key exists in the app's Info.plist file.
    /// Throws an exception if the key is not found.
    /// </summary>
    /// <param name="key">The Info.plist key to verify.</param>
    /// <exception cref="ArgumentException">Thrown when the key is not configured.</exception>
    public static void EnsureHasInfoPlistEntry(string key)
    {
        if (!HasInfoPlistEntry(key))
        {
            throw new ArgumentException($"You must set '{key}' in your Info.plist file");
        }
    }
}
