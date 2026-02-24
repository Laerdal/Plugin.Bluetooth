using Bluetooth.Abstractions.Options;

namespace Bluetooth.Core.Infrastructure.Retries;

/// <summary>
///     Provides utility methods for executing actions with retry logic.
/// </summary>
public static class RetryTools
{
    /// <summary>
    ///     Executes a function with retry logic, repeating the action until it returns true or the maximum retry count is reached.
    /// </summary>
    /// <param name="action">The function to execute. Should return true on success, false to retry.</param>
    /// <param name="maxRetries">The maximum number of retry attempts.</param>
    /// <param name="delayBetweenRetries">The time to wait between retry attempts.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="action" /> is null.</exception>
    /// <exception cref="AggregateException">Thrown when all retry attempts fail, containing all exceptions encountered.</exception>
    public async static Task RunWithRetriesAsync(Func<bool> action, int maxRetries, TimeSpan delayBetweenRetries)
    {
        ArgumentNullException.ThrowIfNull(action);
        var success = false;
        var attempts = 0;
        var exceptions = new List<Exception>();
        while (!success && attempts < maxRetries)
        {
            attempts++;
            try
            {
                success = action();
            }
            catch (Exception e)
            {
                await Task.Delay(delayBetweenRetries).ConfigureAwait(false);
                exceptions.Add(e);
            }
        }

        if (!success)
        {
            throw new AggregateException(exceptions);
        }
    }

    /// <summary>
    ///     Executes an action with retry logic, repeating the action until it succeeds or the maximum retry count is reached.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="maxRetries">The maximum number of retry attempts.</param>
    /// <param name="delayBetweenRetries">The time to wait between retry attempts.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="action" /> is null.</exception>
    /// <exception cref="AggregateException">Thrown when all retry attempts fail, containing all exceptions encountered.</exception>
    public async static Task RunWithRetriesAsync(Action action, int maxRetries, TimeSpan delayBetweenRetries)
    {
        ArgumentNullException.ThrowIfNull(action);
        var success = false;
        var attempts = 0;
        var exceptions = new List<Exception>();
        while (!success && attempts < maxRetries)
        {
            attempts++;
            try
            {
                action();
                success = true;
            }
            catch (Exception e)
            {
                await Task.Delay(delayBetweenRetries).ConfigureAwait(false);
                exceptions.Add(e);
            }
        }

        if (!success)
        {
            throw new AggregateException(exceptions);
        }
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
    public async static Task RunWithRetriesAsync(
        Action action,
        RetryOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(action);
        ArgumentNullException.ThrowIfNull(options);

        var success = false;
        var attempts = 0;
        var exceptions = new List<Exception>();
        var currentDelay = options.DelayBetweenRetries;

        while (!success && attempts < options.MaxRetries)
        {
            attempts++;
            try
            {
                action();
                success = true;
            }
            catch (Exception e)
            {
                exceptions.Add(e);

                // Don't delay after the last attempt
                if (attempts < options.MaxRetries)
                {
                    await Task.Delay(currentDelay, cancellationToken).ConfigureAwait(false);

                    // Apply exponential backoff if enabled
                    if (options.ExponentialBackoff)
                    {
                        currentDelay = TimeSpan.FromMilliseconds(currentDelay.TotalMilliseconds * 2);
                    }
                }
            }
        }

        if (!success)
        {
            throw new AggregateException(
                $"Operation failed after {attempts} attempts with retry configuration: " +
                $"MaxRetries={options.MaxRetries}, BaseDelay={options.DelayBetweenRetries.TotalMilliseconds}ms, " +
                $"ExponentialBackoff={options.ExponentialBackoff}",
                exceptions);
        }
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
    public async static Task RunWithRetriesAsync(
        Func<Task> action,
        RetryOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(action);
        ArgumentNullException.ThrowIfNull(options);

        var success = false;
        var attempts = 0;
        var exceptions = new List<Exception>();
        var currentDelay = options.DelayBetweenRetries;

        while (!success && attempts < options.MaxRetries)
        {
            attempts++;
            try
            {
                await action().ConfigureAwait(false);
                success = true;
            }
            catch (Exception e)
            {
                exceptions.Add(e);

                // Don't delay after the last attempt
                if (attempts < options.MaxRetries)
                {
                    await Task.Delay(currentDelay, cancellationToken).ConfigureAwait(false);

                    // Apply exponential backoff if enabled
                    if (options.ExponentialBackoff)
                    {
                        currentDelay = TimeSpan.FromMilliseconds(currentDelay.TotalMilliseconds * 2);
                    }
                }
            }
        }

        if (!success)
        {
            throw new AggregateException(
                $"Operation failed after {attempts} attempts with retry configuration: " +
                $"MaxRetries={options.MaxRetries}, BaseDelay={options.DelayBetweenRetries.TotalMilliseconds}ms, " +
                $"ExponentialBackoff={options.ExponentialBackoff}",
                exceptions);
        }
    }
}
