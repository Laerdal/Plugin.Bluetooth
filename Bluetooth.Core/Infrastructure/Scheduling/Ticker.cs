namespace Bluetooth.Core.Infrastructure.Scheduling;

/// <summary>
///     Implementation of the ITicker interface.
/// </summary>
public sealed partial class Ticker : ITicker, IDisposable
{
    private readonly object _gate = new();
    private readonly ILogger<Ticker> _logger;
    private readonly Dictionary<Guid, Registration> _registrations = new();
    private readonly TimeSpan _resolution;
    private CancellationTokenSource? _cts;
    private Task? _loopTask;

    private PeriodicTimer? _timer;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Ticker" /> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="options">The ticker options.</param>
    public Ticker(ILogger<Ticker> logger, IOptions<TickerOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _logger = logger;
        _resolution = options.Value.Resolution <= TimeSpan.Zero
            ? TimeSpan.FromMilliseconds(250)
            : options.Value.Resolution;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        lock (_gate)
        {
            Stop_NoLock();
            _registrations.Clear();
        }
    }

    /// <inheritdoc />
    public IDisposable Register(string name, TimeSpan period, Action tick, bool runImmediately = false)
    {
        return Register(name, period, ct => {
            tick();
            return Task.CompletedTask;
        }, runImmediately);
    }

    /// <inheritdoc />
    public IDisposable Register(string name, TimeSpan period, Func<CancellationToken, Task> tickAsync, bool runImmediately = false)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            name = "unnamed";
        }

        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(period, TimeSpan.Zero);

        var id = Guid.NewGuid();
        var reg = new Registration(id, name, period, tickAsync)
        {
            NextDueUtc = runImmediately ? DateTimeOffset.UtcNow : DateTimeOffset.UtcNow + period
        };

        lock (_gate)
        {
            _registrations.Add(id, reg);
            EnsureStarted_NoLock();
        }

        // If runImmediately, schedule it; the loop will pick it up.
        return new Subscription(this, id);
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "Ticker loop crashed.")]
    private static partial void LogTickerLoopCrashed(ILogger logger, Exception ex);

    [LoggerMessage(Level = LogLevel.Error, Message = "Ticker job '{JobName}' failed.")]
    private static partial void LogTickerJobFailed(ILogger logger, string jobName, Exception ex);

    private void EnsureStarted_NoLock()
    {
        if (_loopTask is not null)
        {
            return;
        }

        _cts = new CancellationTokenSource();
        _timer = new PeriodicTimer(_resolution);
        _loopTask = Task.Run(() => LoopAsync(_cts.Token));
    }

    private async Task LoopAsync(CancellationToken ct)
    {
        try
        {
            while (!ct.IsCancellationRequested)
            {
                // Wait tick
                if (_timer is null)
                {
                    break;
                }

                var ok = await _timer.WaitForNextTickAsync(ct).ConfigureAwait(false);
                if (!ok)
                {
                    break;
                }

                List<Registration> due;

                lock (_gate)
                {
                    if (_registrations.Count == 0)
                    {
                        // No jobs: stop the timer loop to avoid background churn.
                        Stop_NoLock();
                        return;
                    }

                    var now = DateTimeOffset.UtcNow;
                    due = _registrations.Values
                        .Where(r => r.NextDueUtc <= now)
                        .ToList();
                }

                // Run due jobs (outside lock)
                foreach (var r in due)
                {
                    _ = TriggerAsync(r, ct);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // expected on shutdown
        }
        catch (Exception ex)
        {
            if (_logger.IsEnabled(LogLevel.Error))
            {
                LogTickerLoopCrashed(_logger, ex);
            }
        }
    }

    private async Task TriggerAsync(Registration r, CancellationToken ct)
    {
        // No overlapping executions per job
        if (!r.TryEnter())
        {
            return;
        }

        try
        {
            await r.TickAsync(ct).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // fine
        }
        catch (Exception ex)
        {
            if (_logger.IsEnabled(LogLevel.Error))
            {
                LogTickerJobFailed(_logger, r.Name, ex);
            }
        }
        finally
        {
            r.Exit();

            // Reschedule: always from "now" to avoid drift accumulating weirdly after long runs.
            lock (_gate)
            {
                if (_registrations.TryGetValue(r.Id, out var existing))
                {
                    existing.NextDueUtc = DateTimeOffset.UtcNow + existing.Period;
                }
            }
        }
    }

    private void Unregister(Guid id)
    {
        lock (_gate)
        {
            _registrations.Remove(id);
            if (_registrations.Count == 0)
            {
                Stop_NoLock();
            }
        }
    }

    private void Stop_NoLock()
    {
        try { _cts?.Cancel(); }
        catch
        {
            /* ignore */
        }

        _timer?.Dispose();
        _timer = null;

        _cts?.Dispose();
        _cts = null;

        _loopTask = null;
    }

    private sealed class Subscription : IDisposable
    {
        private readonly Guid _id;
        private readonly Ticker _owner;
        private int _disposed;

        public Subscription(Ticker owner, Guid id)
        {
            _owner = owner;
            _id = id;
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) != 0)
            {
                return;
            }

            _owner.Unregister(_id);
        }
    }

    private sealed class Registration
    {
        private int _running; // 0/1

        public Registration(Guid id, string name, TimeSpan period, Func<CancellationToken, Task> tickAsync)
        {
            Id = id;
            Name = name;
            Period = period;
            TickAsync = tickAsync;
        }

        public Guid Id { get; }
        public string Name { get; }
        public TimeSpan Period { get; }
        public Func<CancellationToken, Task> TickAsync { get; }
        public DateTimeOffset NextDueUtc { get; set; }

        public bool TryEnter()
        {
            return Interlocked.CompareExchange(ref _running, 1, 0) == 0;
        }

        public void Exit()
        {
            Volatile.Write(ref _running, 0);
        }
    }
}