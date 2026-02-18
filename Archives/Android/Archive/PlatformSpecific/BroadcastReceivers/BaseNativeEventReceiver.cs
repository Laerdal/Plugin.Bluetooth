namespace Bluetooth.Maui.PlatformSpecific.BroadcastReceivers;

/// <summary>
/// Base class for Android broadcast receivers that handle Bluetooth-related events.
/// Provides common functionality for registering/unregistering receivers and filtering intents.
/// </summary>
public abstract class BaseNativeEventReceiver : BroadcastReceiver
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseNativeEventReceiver"/> class.
    /// </summary>
    /// <param name="actions">The collection of intent actions to listen for.</param>
    protected BaseNativeEventReceiver(IEnumerable<string> actions)
    {
        Actions = actions.ToList();
        RegisterReceiver();
    }

    /// <summary>
    /// Registers this receiver with the Android system to listen for the specified actions.
    /// </summary>
    protected void RegisterReceiver()
    {
        using var filter = new IntentFilter();
        foreach (var action in Actions)
        {
            filter.AddAction(action);
        }

        Android.App.Application.Context.RegisterReceiver(this, filter);
    }

    /// <summary>
    /// Unregisters this receiver from the Android system.
    /// </summary>
    protected void UnregisterReceiver()
    {
        Android.App.Application.Context.UnregisterReceiver(this);
    }

    /// <summary>
    /// Gets the collection of intent actions this receiver listens for.
    /// </summary>
    protected IEnumerable<string> Actions { get; set; }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            UnregisterReceiver();
        }

        base.Dispose(disposing);
    }

    /// <inheritdoc/>
    public override void OnReceive(Context? context, Intent? intent)
    {
        if (intent == null)
        {
            return;
        }

        if (Actions.Contains(intent.Action))
        {
            OnEventReceived(intent);
        }
    }

    /// <summary>
    /// Called when a relevant intent is received. Derived classes should override this method
    /// to handle specific intent actions.
    /// </summary>
    /// <param name="intent">The intent that was received.</param>
    protected abstract void OnEventReceived(Intent intent);
}
