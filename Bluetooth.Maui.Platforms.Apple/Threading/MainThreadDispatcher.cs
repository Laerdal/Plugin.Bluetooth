namespace Bluetooth.Maui.Platforms.Apple.Threading;

/// <summary>
///     Provides thread-safe event invocation by marshaling to the main thread on Apple platforms.
///     Required because CoreBluetooth callbacks arrive on background dispatch queues,
///     but UIKit operations must happen on the main thread.
/// </summary>
internal static class MainThreadDispatcher
{
    /// <summary>
    ///     Invokes an event handler on the main thread.
    /// </summary>
    /// <param name="handler">The event handler to invoke.</param>
    /// <param name="sender">The event sender.</param>
    /// <param name="args">The event arguments.</param>
    public static void InvokeOnMainThread(EventHandler? handler, object? sender, EventArgs args)
    {
        if (handler == null)
        {
            return;
        }

        Microsoft.Maui.ApplicationModel.MainThread.BeginInvokeOnMainThread(() => {
            handler.Invoke(sender, args);
        });
    }

    /// <summary>
    ///     Invokes a generic event handler on the main thread.
    /// </summary>
    /// <typeparam name="TEventArgs">The type of event arguments.</typeparam>
    /// <param name="handler">The event handler to invoke.</param>
    /// <param name="sender">The event sender.</param>
    /// <param name="args">The event arguments.</param>
    public static void InvokeOnMainThread<TEventArgs>(EventHandler<TEventArgs>? handler, object? sender, TEventArgs args)
    {
        if (handler == null)
        {
            return;
        }

        Microsoft.Maui.ApplicationModel.MainThread.BeginInvokeOnMainThread(() => {
            handler.Invoke(sender, args);
        });
    }

    /// <summary>
    ///     Executes an action on the main thread.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    public static void BeginInvokeOnMainThread(Action action)
    {
        Microsoft.Maui.ApplicationModel.MainThread.BeginInvokeOnMainThread(action);
    }
}
