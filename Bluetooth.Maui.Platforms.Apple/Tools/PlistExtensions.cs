using Bluetooth.Maui.Platforms.Apple.Exceptions;

namespace Bluetooth.Maui.Platforms.Apple.Tools;

/// <summary>
///     Extension methods for working with iOS Info.plist file entries and background modes.
/// </summary>
public static class PlistExtensions
{
    /// <summary>
    ///     Checks if the specified background mode is enabled in the app's Info.plist file.
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
    ///     Ensures that the specified background mode is enabled in the app's Info.plist file.
    ///     Throws an exception if the background mode is not found.
    /// </summary>
    /// <param name="key">The background mode key to verify.</param>
    /// <exception cref="AppleMissingBackgroundModeInInfoPlistException">Thrown when the background mode is not configured.</exception>
    public static void EnsureHasBackgroundMode(string key)
    {
        AppleMissingBackgroundModeInInfoPlistException.ThrowIfMissingBackgroundMode(key);
    }

    /// <summary>
    ///     Checks if the specified key exists in the app's Info.plist file.
    /// </summary>
    /// <param name="key">The Info.plist key to check for.</param>
    /// <returns>True if the key exists, false otherwise.</returns>
    public static bool HasInfoPlistEntry(string key)
    {
        using var nsKey = new NSString(key);
        return NSBundle.MainBundle.InfoDictionary.ContainsKey(nsKey);
    }

    /// <summary>
    ///     Ensures that the specified key exists in the app's Info.plist file.
    ///     Throws an exception if the key is not found.
    /// </summary>
    /// <param name="key">The Info.plist key to verify.</param>
    /// <exception cref="AppleMissingInfoPlistKeyException">Thrown when the key is not configured.</exception>
    public static void EnsureHasInfoPlistEntry(string key)
    {
        AppleMissingInfoPlistKeyException.ThrowIfMissingInfoPlistKey(key);
    }

    /// <summary>
    ///     Retrieves all keys from the app's Info.plist file.
    /// </summary>
    /// <returns>A list of strings representing all Info.plist keys.</returns>
    public static IEnumerable<string> GetAllInfoPlistKeys()
    {
        var infoPlist = NSBundle.MainBundle.InfoDictionary;
        var keys = new List<string>();


        foreach (var key in infoPlist.Keys)
        {
            keys.Add(key.ToString());
        }

        return new ReadOnlyCollection<string>(keys);
    }

    /// <summary>
    ///     Retrieves all entries from the app's Info.plist file as a list of key:value pairs.
    /// </summary>
    /// <returns>A list of strings representing all Info.plist entries.</returns>
    public static IEnumerable<string> GetAllInfoPlistEntries()
    {
        var infoPlist = NSBundle.MainBundle.InfoDictionary;
        var entries = new List<string>();

        foreach (var key in infoPlist.Keys)
        {
            var value = infoPlist[key];
            entries.Add($"{key}: {value}");
        }

        return new ReadOnlyCollection<string>(entries);
    }
}
