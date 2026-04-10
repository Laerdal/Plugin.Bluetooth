namespace Bluetooth.Maui.Platforms.Apple.Threading;

/// <summary>
///     Provides thread-safe event invocation by marshaling to the main thread on Apple platforms.
///     Unity-compatible replacement that uses <see cref="CoreFoundation.DispatchQueue.MainQueue"/>
///     instead of <c>Microsoft.Maui.ApplicationModel.MainThread</c>.
/// </summary>
internal static class MainThreadDispatcher
{
    /// <summary>
    ///     Invokes an event handler on the main thread.
    /// </summary>
    public static void InvokeOnMainThread(EventHandler? handler, object? sender, EventArgs args)
    {
        if (handler == null)
        {
            return;
        }

        CoreFoundation.DispatchQueue.MainQueue.DispatchAsync(() => handler.Invoke(sender, args));
    }

    /// <summary>
    ///     Invokes a generic event handler on the main thread.
    /// </summary>
    public static void InvokeOnMainThread<TEventArgs>(EventHandler<TEventArgs>? handler, object? sender, TEventArgs args)
    {
        if (handler == null)
        {
            return;
        }

        CoreFoundation.DispatchQueue.MainQueue.DispatchAsync(() => handler.Invoke(sender, args));
    }

    /// <summary>
    ///     Executes an action on the main thread.
    /// </summary>
    public static void BeginInvokeOnMainThread(Action action)
        => CoreFoundation.DispatchQueue.MainQueue.DispatchAsync(action);
}
