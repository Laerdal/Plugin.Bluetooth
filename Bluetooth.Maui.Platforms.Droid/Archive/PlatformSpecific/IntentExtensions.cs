namespace Bluetooth.Maui.PlatformSpecific;

/// <summary>
/// Extension methods for Android <see cref="Intent"/> to safely extract parcelable data
/// with proper version handling for Android API changes.
/// </summary>
public static class IntentExtensions
{
    /// <summary>
    /// Safely extracts a parcelable extra from an intent with proper API version handling.
    /// </summary>
    /// <typeparam name="T">The type of the parcelable object to extract.</typeparam>
    /// <param name="intent">The intent to extract from.</param>
    /// <param name="key">The key of the extra to extract.</param>
    /// <returns>The parcelable object if found; otherwise, null.</returns>
    /// <exception cref="ArgumentNullException">Thrown when intent is null.</exception>
    public static T? GetParcelableExtraSafe<T>(this Intent intent, string key) where T : Java.Lang.Object
    {
        ArgumentNullException.ThrowIfNull(intent);
        if (OperatingSystem.IsAndroidVersionAtLeast(33))
        {
            return intent.GetParcelableExtra(key, Java.Lang.Class.FromType(typeof(T))) as T;
        }
        else
        {
#pragma warning disable CS0618 // Type or member is obsolete
            return intent.GetParcelableExtra(key) as T;
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }

    /// <summary>
    /// Safely extracts a parcelable array list extra from an intent with proper API version handling.
    /// </summary>
    /// <typeparam name="T">The type of the parcelable objects in the array list.</typeparam>
    /// <param name="intent">The intent to extract from.</param>
    /// <param name="key">The key of the extra to extract.</param>
    /// <returns>The parcelable array list if found; otherwise, null.</returns>
    /// <exception cref="ArgumentNullException">Thrown when intent is null.</exception>
    public static IEnumerable<T>? GetParcelableArrayListExtraSafe<T>(this Intent intent, string key) where T : Java.Lang.Object
    {
        ArgumentNullException.ThrowIfNull(intent);
        if (OperatingSystem.IsAndroidVersionAtLeast(33))
        {
            return intent.GetParcelableArrayListExtra(key, Java.Lang.Class.FromType(typeof(T))) as IEnumerable<T>;
        }
        else
        {
#pragma warning disable CS0618 // Type or member is obsolete
            return intent.GetParcelableArrayListExtra(key) as IEnumerable<T>;
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }

    /// <summary>
    /// Safely extracts a parcelable array extra from an intent with proper API version handling.
    /// </summary>
    /// <typeparam name="T">The type of the parcelable objects in the array.</typeparam>
    /// <param name="intent">The intent to extract from.</param>
    /// <param name="key">The key of the extra to extract.</param>
    /// <returns>The parcelable array if found; otherwise, null.</returns>
    /// <exception cref="ArgumentNullException">Thrown when intent is null.</exception>
    public static T? GetParcelableArrayExtraSafe<T>(this Intent intent, string key) where T : Java.Lang.Object
    {
        ArgumentNullException.ThrowIfNull(intent);
        if (OperatingSystem.IsAndroidVersionAtLeast(33))
        {
            return intent.GetParcelableArrayExtra(key, Java.Lang.Class.FromType(typeof(T))) as T;
        }
        else
        {
#pragma warning disable CS0618 // Type or member is obsolete
            return intent.GetParcelableArrayExtra(key) as T;
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}
