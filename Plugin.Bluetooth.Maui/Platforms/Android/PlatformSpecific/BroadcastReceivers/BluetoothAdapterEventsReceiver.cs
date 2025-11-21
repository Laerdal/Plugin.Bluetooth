using ScanMode = Android.Bluetooth.ScanMode;

namespace Plugin.Bluetooth.Maui.PlatformSpecific.BroadcastReceivers;





/// <summary>
/// Broadcast receiver for handling Bluetooth adapter events such as state changes,
/// connection state changes, and discovery events.
/// </summary>
public partial class BluetoothAdapterEventsReceiver : BaseNativeEventReceiver
{
#pragma warning disable CA1034 // Nested types should not be visible
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix

    /// <summary>
    /// Event arguments for scan mode change events.
    /// </summary>
    /// <param name="ScanMode">The new scan mode.</param>
    /// <param name="PreviousScanMode">The previous scan mode.</param>
    public record struct ScanModeChangedEventArgs(ScanMode ScanMode, ScanMode PreviousScanMode);

    /// <summary>
    /// Event arguments for Bluetooth state change events.
    /// </summary>
    /// <param name="NewState">The new Bluetooth state.</param>
    /// <param name="PreviousState">The previous Bluetooth state.</param>
    public record struct StateChangedEventArgs(State NewState, State PreviousState);

    /// <summary>
    /// Event arguments for connection state change events.
    /// </summary>
    /// <param name="NewState">The new connection state.</param>
    /// <param name="OldState">The previous connection state.</param>
    /// <param name="Device">The Bluetooth device associated with the connection state change.</param>
    public record struct ConnectionStateChangedEventArgs(ProfileState NewState, ProfileState OldState, Android.Bluetooth.BluetoothDevice? Device);

    /// <summary>
    /// Event arguments for local name change events.
    /// </summary>
    /// <param name="NewName">The new local name of the Bluetooth adapter.</param>
    public record struct LocalNameChangedEventArgs(string? NewName);

    /// <summary>
    /// Event arguments for discoverable request events.
    /// </summary>
    /// <param name="Duration">The requested discoverable duration in seconds.</param>
    public record struct DiscoverableRequestedEventArgs(int Duration);

#pragma warning restore CA1034 // Nested types should not be visible
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix

    /// <summary>
    /// Initializes a new instance of the <see cref="BluetoothAdapterEventsReceiver"/> class.
    /// </summary>
    public BluetoothAdapterEventsReceiver() : base([
        Android.Bluetooth.BluetoothAdapter.ActionScanModeChanged,
        Android.Bluetooth.BluetoothAdapter.ActionStateChanged,
        Android.Bluetooth.BluetoothAdapter.ActionConnectionStateChanged,
        Android.Bluetooth.BluetoothAdapter.ActionLocalNameChanged,
        Android.Bluetooth.BluetoothAdapter.ActionRequestDiscoverable,
        Android.Bluetooth.BluetoothAdapter.ActionRequestEnable,
        Android.Bluetooth.BluetoothAdapter.ActionDiscoveryStarted,
        Android.Bluetooth.BluetoothAdapter.ActionDiscoveryFinished
    ])
    {
    }

    /// <summary>
    /// Occurs when the Bluetooth adapter scan mode changes.
    /// </summary>
    public event EventHandler<ScanModeChangedEventArgs>? ScanModeChanged;

    /// <summary>
    /// Occurs when the Bluetooth adapter state changes.
    /// </summary>
    public event EventHandler<StateChangedEventArgs>? StateChanged;

    /// <summary>
    /// Occurs when the connection state of the Bluetooth adapter changes.
    /// </summary>
    public event EventHandler<ConnectionStateChangedEventArgs>? ConnectionStateChanged;

    /// <summary>
    /// Occurs when the local name of the Bluetooth adapter changes.
    /// </summary>
    public event EventHandler<LocalNameChangedEventArgs>? LocalNameChanged;

    /// <summary>
    /// Occurs when a request is made to make the device discoverable.
    /// </summary>
    public event EventHandler<DiscoverableRequestedEventArgs>? DiscoverableRequested;

    /// <summary>
    /// Occurs when a request is made to enable Bluetooth.
    /// </summary>
    public event EventHandler? EnableRequested;

    /// <summary>
    /// Occurs when Bluetooth device discovery starts.
    /// </summary>
    public event EventHandler? DiscoveryStarted;

    /// <summary>
    /// Occurs when Bluetooth device discovery finishes.
    /// </summary>
    public event EventHandler? DiscoveryFinished;

    /// <inheritdoc/>
    protected override void OnEventReceived(Intent intent)
    {
        ArgumentNullException.ThrowIfNull(intent);
        switch (intent.Action)
        {
            case Android.Bluetooth.BluetoothAdapter.ActionScanModeChanged:
                // Scan mode changed (e.g., from none to connectable or discoverable)
                var scanMode = (ScanMode)intent.GetIntExtra(Android.Bluetooth.BluetoothAdapter.ExtraScanMode, -1);
                var previousScanMode = (ScanMode)intent.GetIntExtra(Android.Bluetooth.BluetoothAdapter.ExtraPreviousScanMode, -1);

                OnScanModeChanged(new ScanModeChangedEventArgs(scanMode, previousScanMode));
                break;

            case Android.Bluetooth.BluetoothAdapter.ActionStateChanged:
                // Bluetooth adapter state changed (e.g., turning on/off)
                var newState = (State)intent.GetIntExtra(Android.Bluetooth.BluetoothAdapter.ExtraState, -1);
                var previousState = (State)intent.GetIntExtra(Android.Bluetooth.BluetoothAdapter.ExtraPreviousState, -1);

                OnStateChanged(new StateChangedEventArgs(newState, previousState));
                break;

            case Android.Bluetooth.BluetoothAdapter.ActionConnectionStateChanged:
                // Connection state changed for the local Bluetooth adapter
                var connectionState = (ProfileState)intent.GetIntExtra(Android.Bluetooth.BluetoothAdapter.ExtraConnectionState, -1);
                var previousConnectionState = (ProfileState)intent.GetIntExtra(Android.Bluetooth.BluetoothAdapter.ExtraPreviousConnectionState, -1);
                var device = intent.GetParcelableExtraSafe<Android.Bluetooth.BluetoothDevice>(Android.Bluetooth.BluetoothDevice.ExtraDevice);
                OnConnectionStateChanged(new ConnectionStateChangedEventArgs(connectionState, previousConnectionState, device));
                break;

            case Android.Bluetooth.BluetoothAdapter.ActionLocalNameChanged:
                // Local Bluetooth adapter name has changed
                var newName = intent.GetStringExtra(Android.Bluetooth.BluetoothAdapter.ExtraLocalName);

                OnLocalNameChanged(new LocalNameChangedEventArgs(newName));
                break;

            case Android.Bluetooth.BluetoothAdapter.ActionRequestDiscoverable:
                // Request to make the device discoverable
                var duration = intent.GetIntExtra(Android.Bluetooth.BluetoothAdapter.ExtraDiscoverableDuration, 120);

                OnDiscoverableRequested(new DiscoverableRequestedEventArgs(duration));
                break;

            case Android.Bluetooth.BluetoothAdapter.ActionRequestEnable:
                // Request to enable Bluetooth
                OnEnableRequested();
                break;

            case Android.Bluetooth.BluetoothAdapter.ActionDiscoveryStarted:
                // Bluetooth discovery has started
                OnDiscoveryStarted();
                break;

            case Android.Bluetooth.BluetoothAdapter.ActionDiscoveryFinished:
                // Bluetooth discovery has finished
                OnDiscoveryFinished();
                break;

        }
    }

    /// <summary>
    /// Raises the <see cref="ScanModeChanged"/> event.
    /// </summary>
    /// <param name="e">The event arguments.</param>
    protected virtual void OnScanModeChanged(ScanModeChangedEventArgs e)
    {
        ScanModeChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the <see cref="StateChanged"/> event.
    /// </summary>
    /// <param name="e">The event arguments.</param>
    protected virtual void OnStateChanged(StateChangedEventArgs e)
    {
        StateChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the <see cref="ConnectionStateChanged"/> event.
    /// </summary>
    /// <param name="e">The event arguments.</param>
    protected virtual void OnConnectionStateChanged(ConnectionStateChangedEventArgs e)
    {
        ConnectionStateChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the <see cref="LocalNameChanged"/> event.
    /// </summary>
    /// <param name="e">The event arguments.</param>
    protected virtual void OnLocalNameChanged(LocalNameChangedEventArgs e)
    {
        LocalNameChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the <see cref="DiscoverableRequested"/> event.
    /// </summary>
    /// <param name="e">The event arguments.</param>
    protected virtual void OnDiscoverableRequested(DiscoverableRequestedEventArgs e)
    {
        DiscoverableRequested?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the <see cref="EnableRequested"/> event.
    /// </summary>
    protected virtual void OnEnableRequested()
    {
        EnableRequested?.Invoke(this, System.EventArgs.Empty);
    }

    /// <summary>
    /// Raises the <see cref="DiscoveryStarted"/> event.
    /// </summary>
    protected virtual void OnDiscoveryStarted()
    {
        DiscoveryStarted?.Invoke(this, System.EventArgs.Empty);
    }

    /// <summary>
    /// Raises the <see cref="DiscoveryFinished"/> event.
    /// </summary>
    protected virtual void OnDiscoveryFinished()
    {
        DiscoveryFinished?.Invoke(this, System.EventArgs.Empty);
    }
}
