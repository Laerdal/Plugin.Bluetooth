namespace Bluetooth.Maui.Platforms.Droid.Broadcasting.NativeObjects;

// Mapping native APIs leads to unclean interfaces, ignoring warnings here
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
#pragma warning disable CA1716 // Identifiers should not match keywords

public partial class AdvertiseCallbackProxy
{
    /// <summary>
    ///     Interface for handling Bluetooth LE advertising callbacks.
    ///     Extends the base broadcaster interface with Android-specific callback methods.
    /// </summary>
    public interface IAdvertiseCallbackProxyDelegate
    {
        /// <summary>
        ///     Called when advertising has been started successfully.
        /// </summary>
        /// <param name="settingsInEffect">The actual advertising settings that are in effect.</param>
        void OnStartSuccess(AdvertiseSettings? settingsInEffect);

        /// <summary>
        ///     Called when advertising could not be started.
        /// </summary>
        /// <param name="errorCode">The error code indicating why advertising failed to start.</param>
        void OnStartFailure(AdvertiseFailure errorCode);
    }
}

#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
#pragma warning restore CA1716 // Identifiers should not match keywords