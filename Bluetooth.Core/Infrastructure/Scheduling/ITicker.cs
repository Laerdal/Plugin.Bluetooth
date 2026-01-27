namespace Bluetooth.Core.Infrastructure.Scheduling;

/// <summary>
/// Interface representing a ticker that can register periodic callbacks.
/// </summary>
public interface ITicker
{
    /// <summary>
    /// Registers a callback to be invoked every <paramref name="period"/>.
    /// The callback should be fast and non-blocking; if it needs async work, use the async overload.
    /// Returns a subscription that should be disposed to unregister.
    /// </summary>
    /// <param name="name">The name of the registration.</param>
    /// <param name="period">The period between invocations.</param>
    /// <param name="tick">The callback to invoke.</param>
    /// <param name="runImmediately">Whether to run the callback immediately upon registration.</param>
    /// <returns>A subscription that should be disposed to unregister.</returns>
    IDisposable Register(
        string name,
        TimeSpan period,
        Action tick,
        bool runImmediately = false);

    /// <summary>
    /// Registers an async callback to be invoked every <paramref name="period"/>.
    /// The ticker will avoid overlapping executions for the same registration.
    /// Returns a subscription that should be disposed to unregister.
    /// </summary>
    /// <param name="name">The name of the registration.</param>
    /// <param name="period">The period between invocations.</param>
    /// <param name="tickAsync">The async callback to invoke.</param>
    /// <param name="runImmediately">Whether to run the callback immediately upon registration.</param>
    /// <returns>A subscription that should be disposed to unregister.</returns>
    IDisposable Register(
        string name,
        TimeSpan period,
        Func<CancellationToken, Task> tickAsync,
        bool runImmediately = false);
}
