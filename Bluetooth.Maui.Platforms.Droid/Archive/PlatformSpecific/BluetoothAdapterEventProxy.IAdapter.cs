namespace Bluetooth.Maui.PlatformSpecific;

#pragma warning disable CA1034 // Nested types should not be visible

public partial class BluetoothAdapterEventProxy
{
    /// <summary>
    /// Interface for adapter implementations that handle Android Bluetooth adapter events.
    /// Extends <see cref="IBluetoothActivity"/> to provide adapter-specific event handlers.
    /// </summary>
    /// <remarks>
    /// <para>Implementations handle events from the local Bluetooth adapter, which represents the
    /// Bluetooth hardware on the device. These events include state changes, discovery events,
    /// and connection state updates.</para>
    /// <para>See Android documentation:
    /// <see href="https://developer.android.com/reference/android/bluetooth/BluetoothAdapter">BluetoothAdapter</see>
    /// </para>
    /// </remarks>
    public interface IAdapter : IBluetoothScanner
    {
        /// <summary>
        /// Called when the Bluetooth adapter connection state changes.
        /// </summary>
        /// <param name="oldState">The previous connection state.</param>
        /// <param name="newState">The new connection state.</param>
        /// <param name="eDevice">The Bluetooth device associated with the connection state change, if applicable.</param>
        /// <remarks>
        /// Corresponds to Android's ACTION_CONNECTION_STATE_CHANGED. This indicates a change in the
        /// connection state of the local Bluetooth adapter to a profile of a remote device.
        /// </remarks>
        void OnConnectionStateChanged(ProfileState oldState, ProfileState newState, Android.Bluetooth.BluetoothDevice? eDevice);

        /// <summary>
        /// Called when a request is made to make the device discoverable.
        /// </summary>
        /// <param name="duration">The requested discoverable duration in seconds.</param>
        /// <remarks>
        /// Corresponds to Android's ACTION_REQUEST_DISCOVERABLE. This is a system activity that
        /// requests the device be made discoverable to other Bluetooth devices.
        /// </remarks>
        void OnDiscoverableRequested(int duration);

        /// <summary>
        /// Called when Bluetooth device discovery finishes.
        /// </summary>
        /// <remarks>
        /// Corresponds to Android's ACTION_DISCOVERY_FINISHED. This indicates the local Bluetooth
        /// adapter has finished the device discovery process initiated by startDiscovery().
        /// </remarks>
        void OnDiscoveryFinished();

        /// <summary>
        /// Called when Bluetooth device discovery starts.
        /// </summary>
        /// <remarks>
        /// Corresponds to Android's ACTION_DISCOVERY_STARTED. This indicates the local Bluetooth
        /// adapter has started the remote device discovery process.
        /// </remarks>
        void OnDiscoveryStarted();

        /// <summary>
        /// Called when a request is made to enable Bluetooth.
        /// </summary>
        /// <remarks>
        /// Corresponds to Android's ACTION_REQUEST_ENABLE. This is a system activity that allows
        /// the user to turn on Bluetooth without leaving the application.
        /// </remarks>
        void OnEnableRequested();

        /// <summary>
        /// Called when the local name of the Bluetooth adapter changes.
        /// </summary>
        /// <param name="newName">The new local name of the adapter.</param>
        /// <remarks>
        /// Corresponds to Android's ACTION_LOCAL_NAME_CHANGED. This indicates the local Bluetooth
        /// adapter has changed its friendly Bluetooth name visible to other devices.
        /// </remarks>
        void OnLocalNameChanged(string? newName);

        /// <summary>
        /// Called when the Bluetooth adapter scan mode changes.
        /// </summary>
        /// <param name="previousScanMode">The previous scan mode.</param>
        /// <param name="newScanMode">The new scan mode.</param>
        /// <remarks>
        /// Corresponds to Android's ACTION_SCAN_MODE_CHANGED. Scan modes control whether the adapter
        /// is discoverable and/or connectable by other Bluetooth devices.
        /// </remarks>
        void OnScanModeChanged(Android.Bluetooth.ScanMode previousScanMode, Android.Bluetooth.ScanMode newScanMode);

        /// <summary>
        /// Called when the Bluetooth adapter state changes.
        /// </summary>
        /// <param name="previousState">The previous adapter state.</param>
        /// <param name="newState">The new adapter state.</param>
        /// <remarks>
        /// Corresponds to Android's ACTION_STATE_CHANGED. States include turning off, off, turning on,
        /// and on. This is the primary way to monitor the overall Bluetooth adapter state.
        /// </remarks>
        void OnStateChanged(State previousState, State newState);
    }
}

#pragma warning restore CA1034 // Nested types should not be visible
