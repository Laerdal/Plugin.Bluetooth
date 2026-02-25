namespace Bluetooth.Maui.Platforms.Apple.Exceptions;

/// <summary>
///     Represents an exception that occurs when a required Info.plist key is missing for Apple Bluetooth operations.
/// </summary>
/// <remarks>
///     This exception is thrown when the application's Info.plist file is missing required entries
///     such as Bluetooth usage descriptions that are necessary for Bluetooth functionality on iOS and macOS platforms.
/// </remarks>
public class AppleMissingInfoPlistKeyException : BluetoothException
{
    /// <summary>
    ///     Gets the Info.plist key that is missing.
    /// </summary>
    public string Key { get; private set; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="AppleMissingInfoPlistKeyException" /> class
    ///     for a missing Info.plist key.
    /// </summary>
    /// <param name="key">The Info.plist key that is missing.</param>
    public AppleMissingInfoPlistKeyException(string key) : base($"Your Info.plist is missing the required '{key}' entry. Add this key to your Info.plist file.")
    {
        Key = key;
    }

    /// <summary>
    ///     Ensures that the specified Info.plist key is configured, throwing an exception if not.
    /// </summary>
    /// <param name="key">The Info.plist key to verify.</param>
    /// <exception cref="AppleMissingInfoPlistKeyException">Thrown when the key is not configured.</exception>
    public static void ThrowIfMissingInfoPlistKey(string key)
    {
        if (!PlistExtensions.HasInfoPlistEntry(key))
        {
            throw new AppleMissingInfoPlistKeyException(key);
        }
    }
}
