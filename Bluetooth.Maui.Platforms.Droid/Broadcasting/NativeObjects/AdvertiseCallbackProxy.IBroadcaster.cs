namespace Bluetooth.Maui.Platforms.Droid.Broadcasting.NativeObjects;
#pragma warning disable CA1034 // Nested types should not be visible

public partial class AdvertiseCallbackProxy
{
    /// <summary>
    /// Interface for handling Bluetooth LE advertising callbacks.
    /// Extends the base broadcaster interface with Android-specific callback methods.
    /// </summary>
    public interface IBroadcaster : IBluetoothBroadcaster
    {
        /// <summary>
        /// Called when advertising has been started successfully.
        /// </summary>
        /// <param name="settingsInEffect">The actual advertising settings that are in effect.</param>
        void OnStartSuccess(AdvertiseSettings? settingsInEffect);

        /// <summary>
        /// Called when advertising could not be started.
        /// </summary>
        /// <param name="errorCode">The error code indicating why advertising failed to start.</param>
        void OnStartFailure(AdvertiseFailure errorCode);
    }
}

#pragma warning restore CA1034 // Nested types should not be visible
