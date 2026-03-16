using Bluetooth.Abstractions.Options;

namespace Bluetooth.Core.Infrastructure.Retries;

/// <summary>
///     Provides utility methods for executing actions with retry logic.
/// </summary>
public static class RetryTools
{
    /// <summary>
    ///     Shared core retry logic for all overloads.
    /// </summary>
    private async static Task RunWithRetriesCoreAsync(
        Func<Task<bool>> tryOnceAsync,
        RetryOptions? options,
        CancellationToken cancellationToken)
    {
        options ??= RetryOptions.Default;
        var attempts = 0;
        var exceptions = new List<Exception>();
        var currentDelay = options.DelayBetweenRetries;
        var success = false;

        while (!success && attempts < options.MaxRetries)
        {
            attempts++;
            try
            {
                success = await tryOnceAsync().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                exceptions.Add(e);
                // Don't delay after the last attempt
                if (attempts < options.MaxRetries)
                {
                    await Task.Delay(currentDelay, cancellationToken).ConfigureAwait(false);
                    if (options.ExponentialBackoff)
                    {
                        currentDelay = TimeSpan.FromMilliseconds(currentDelay.TotalMilliseconds * 2);
                    }
                }
            }
            if (!success && attempts < options.MaxRetries && exceptions.Count == 0)
            {
                // For non-exception retries (e.g., Func<bool> returns false), still delay
                await Task.Delay(currentDelay, cancellationToken).ConfigureAwait(false);
                if (options.ExponentialBackoff)
                {
                    currentDelay = TimeSpan.FromMilliseconds(currentDelay.TotalMilliseconds * 2);
                }
            }
        }
        if (!success)
        {
            throw new AggregateException($"Operation failed after {attempts} attempts with retry configuration: "
                                       + $"MaxRetries={options.MaxRetries}, BaseDelay={options.DelayBetweenRetries.TotalMilliseconds}ms, "
                                       + $"ExponentialBackoff={options.ExponentialBackoff}",
                                         exceptions);
        }
    }

    /// <summary>
    ///     Executes a function with retry logic, repeating the action until it returns true or the maximum retry count is reached.
    /// </summary>
    /// <param name="action">The function to execute. Should return true on success, false to retry.</param>
    /// <param name="options">The retry configuration options.</param>
    /// <param name="cancellationToken">Token to cancel the retry operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="action" /> is null.</exception>
    /// <exception cref="AggregateException">Thrown when all retry attempts fail, containing all exceptions encountered.</exception>
    public async static Task RunWithRetriesAsync(Func<bool> action, RetryOptions? options = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(action);
        await RunWithRetriesCoreAsync(
            () => Task.FromResult(action()),
            options,
            cancellationToken
        ).ConfigureAwait(false);
    }
    
    /// <summary>
    ///     Executes an action with configurable retry logic, using the provided <see cref="RetryOptions"/>.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="options">The retry configuration options.</param>
    /// <param name="cancellationToken">Token to cancel the retry operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="action" /> or <paramref name="options" /> is null.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled via <paramref name="cancellationToken"/>.</exception>
    /// <exception cref="AggregateException">Thrown when all retry attempts fail, containing all exceptions encountered.</exception>
    public async static Task RunWithRetriesAsync(Action action, RetryOptions? options = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(action);
        await RunWithRetriesCoreAsync(
            () => { action(); return Task.FromResult(true); },
            options,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <summary>
    ///     Executes an asynchronous action with configurable retry logic, using the provided <see cref="RetryOptions"/>.
    /// </summary>
    /// <param name="action">The asynchronous action to execute.</param>
    /// <param name="options">The retry configuration options.</param>
    /// <param name="cancellationToken">Token to cancel the retry operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="action" /> or <paramref name="options" /> is null.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled via <paramref name="cancellationToken"/>.</exception>
    /// <exception cref="AggregateException">Thrown when all retry attempts fail, containing all exceptions encountered.</exception>
    public async static Task RunWithRetriesAsync(Func<Task> action, RetryOptions? options = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(action);
        await RunWithRetriesCoreAsync(
            async () => { await action().ConfigureAwait(false); return true; },
            options,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <summary>
    ///     Executes an asynchronous function with configurable retry logic, using the provided <see cref="RetryOptions"/>.
    /// </summary>
    /// <typeparam name="T">The return type of the function.</typeparam>
    /// <param name="func">The asynchronous function to execute.</param>
    /// <param name="options">The retry configuration options.</param>
    /// <param name="cancellationToken">Token to cancel the retry operation.</param>
    /// <returns>A task representing the asynchronous operation, with the result from the function.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="func" /> or <paramref name="options" /> is null.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled via <paramref name="cancellationToken"/>.</exception>
    /// <exception cref="AggregateException">Thrown when all retry attempts fail, containing all exceptions encountered.</exception>
    public async static Task<T> RunWithRetriesAsync<T>(Func<Task<T>> func, RetryOptions? options = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(func);
        T? result = default;
        await RunWithRetriesCoreAsync(
            async () => { result = await func().ConfigureAwait(false); return true; },
            options,
            cancellationToken
        ).ConfigureAwait(false);
        return result!;
    }
}
