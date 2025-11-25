
using Bluetooth.Maui.PlatformSpecific.BroadcastReceivers;

namespace Bluetooth.Maui.PlatformSpecific;

/// <summary>
/// Proxy class that bridges Android Bluetooth adapter events to the adapter interface.
/// Handles event subscription and forwarding from <see cref="BluetoothAdapterEventsReceiver"/> to <see cref="IAdapter"/>.
/// </summary>
/// <remarks>
/// This proxy subscribes to native Android Bluetooth adapter events and forwards them
/// to the adapter implementation, providing exception handling for all event handlers.
/// </remarks>
public partial class BluetoothAdapterEventProxy
{
    /// <summary>
    /// Gets the adapter instance that receives event notifications.
    /// </summary>
    private IAdapter Adapter { get; }

    /// <summary>
    /// Gets the broadcast receiver that listens for Android Bluetooth adapter events.
    /// </summary>
    private BluetoothAdapterEventsReceiver BluetoothAdapterEventsReceiver { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BluetoothAdapterEventProxy"/> class.
    /// </summary>
    /// <param name="adapter">The adapter instance that will receive event notifications.</param>
    public BluetoothAdapterEventProxy(IAdapter adapter)
    {
        Adapter = adapter;
        BluetoothAdapterEventsReceiver = new BluetoothAdapterEventsReceiver();
        BluetoothAdapterEventsReceiver.StateChanged += ReceiverOnStateChanged;
        BluetoothAdapterEventsReceiver.ScanModeChanged += ReceiverOnScanModeChanged;
        BluetoothAdapterEventsReceiver.DiscoveryStarted += ReceiverOnDiscoveryStarted;
        BluetoothAdapterEventsReceiver.DiscoveryFinished += ReceiverOnDiscoveryFinished;
        BluetoothAdapterEventsReceiver.ConnectionStateChanged += ReceiverOnConnectionStateChanged;
        BluetoothAdapterEventsReceiver.DiscoverableRequested += ReceiverOnDiscoverableRequested;
        BluetoothAdapterEventsReceiver.EnableRequested += ReceiverOnEnableRequested;
        BluetoothAdapterEventsReceiver.LocalNameChanged += ReceiverOnLocalNameChanged;
    }

    private void ReceiverOnDiscoveryStarted(object? sender, System.EventArgs e)
    {
        try
        {
            Adapter.OnDiscoveryStarted();
        }
        catch (Exception ex)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, ex);
        }
    }

    private void ReceiverOnDiscoveryFinished(object? sender, System.EventArgs e)
    {
        try
        {
            Adapter.OnDiscoveryFinished();
        }
        catch (Exception ex)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, ex);
        }
    }

    private void ReceiverOnStateChanged(object? sender, BluetoothAdapterEventsReceiver.StateChangedEventArgs e)
    {
        try
        {
            Adapter.OnStateChanged(e.PreviousState, e.NewState);
        }
        catch (Exception ex)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, ex);
        }
    }

    private void ReceiverOnScanModeChanged(object? sender, BluetoothAdapterEventsReceiver.ScanModeChangedEventArgs e)
    {
        try
        {
            Adapter.OnScanModeChanged(e.PreviousScanMode, e.ScanMode);
        }
        catch (Exception ex)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, ex);
        }
    }

    private void ReceiverOnLocalNameChanged(object? sender, BluetoothAdapterEventsReceiver.LocalNameChangedEventArgs e)
    {
        try
        {
            Adapter.OnLocalNameChanged(e.NewName);
        }
        catch (Exception ex)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, ex);
        }
    }

    private void ReceiverOnEnableRequested(object? sender, System.EventArgs e)
    {
        try
        {
            Adapter.OnEnableRequested();
        }
        catch (Exception ex)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, ex);
        }
    }

    /// <summary>
    /// Handles the discoverable requested event from the broadcast receiver.
    /// </summary>
    private void ReceiverOnDiscoverableRequested(object? sender, BluetoothAdapterEventsReceiver.DiscoverableRequestedEventArgs e)
    {
        try
        {
            Adapter.OnDiscoverableRequested(e.Duration);
        }
        catch (Exception ex)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, ex);
        }
    }

    /// <summary>
    /// Handles the connection state changed event from the broadcast receiver.
    /// </summary>
    /// <remarks>
    /// Corresponds to Android's ACTION_CONNECTION_STATE_CHANGED, which is used to broadcast the
    /// change in connection state of the local Bluetooth adapter to a profile of the remote device.
    /// </remarks>
    private void ReceiverOnConnectionStateChanged(object? sender, BluetoothAdapterEventsReceiver.ConnectionStateChangedEventArgs e)
    {
        try
        {
            Adapter.OnConnectionStateChanged(e.OldState, e.NewState, e.Device);
        }
        catch (Exception ex)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, ex);
        }
    }
}
