namespace Bluetooth.Maui.PlatformSpecific;

/// <summary>
/// Event arguments for the StateChanged event.
/// </summary>
public class StateChangedEventArgs : EventArgs
{
    /// <summary>
    /// Gets the new state of the Core Bluetooth central manager.
    /// </summary>
    public CBManagerState NewState { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StateChangedEventArgs"/> class.
    /// </summary>
    /// <param name="newState">The new state of the Core Bluetooth central manager.</param>
    public StateChangedEventArgs(CBManagerState newState)
    {
        NewState = newState;
    }
}
