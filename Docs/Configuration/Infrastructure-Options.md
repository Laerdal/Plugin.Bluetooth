# Infrastructure Options

There is no `BluetoothInfrastructureOptions` type in this codebase — a previous version of this document described one at length, but it was never implemented (or was removed before it shipped) and this page went undetected out of sync for a long time. There is currently no app-wide options class for things like operation timeouts, connection concurrency limits, or verbose-logging toggles. If you need that kind of behavior today, implement it at the call site (e.g. wrap calls with your own `CancellationTokenSource` for timeouts) rather than looking for a config knob that doesn't exist.

The one piece of genuine infrastructure-level configuration in the library is `TickerOptions`.

## TickerOptions

```csharp
namespace Bluetooth.Core.Infrastructure.Scheduling;

public sealed class TickerOptions
{
    public TimeSpan Resolution { get; init; } = TimeSpan.FromMilliseconds(250);
}
```

**Namespace**: `Bluetooth.Core.Infrastructure.Scheduling`

`Resolution` controls the internal scheduling tick used by the shared `ITicker` service (polling-based property refresh, timeouts, etc.) — smaller values mean more accurate timing at the cost of more wakeups. It's registered via `AddTicker()`, which `AddBluetoothCoreServices()` calls internally:

```csharp
// Bluetooth.Core/Infrastructure/Scheduling/ServiceCollectionExtensions.cs
public static IServiceCollection AddTicker(this IServiceCollection services)
{
    services.AddOptions<TickerOptions>();
    services.AddSingleton<ITicker, Ticker>();
    return services;
}
```

There is currently no public overload that lets you customize `TickerOptions.Resolution` from `MauiProgram.cs` — it registers with its default (250ms) via `services.AddOptions<TickerOptions>()` with no configure delegate. If you need a different resolution, register your own `IConfigureOptions<TickerOptions>` after calling `AddBluetoothServices()`.

## Related Documentation

- [Dependency-Injection.md](./Dependency-Injection.md) — DI configuration guide
- [Scanning-Options.md](./Scanning-Options.md) — Scanner configuration
- [Connection-Options.md](./Connection-Options.md) — Connection configuration
- [L2CAP-Options.md](./L2CAP-Options.md) — L2CAP channel configuration
- [Exploration-Options.md](./Exploration-Options.md) — Service exploration configuration
- [Broadcasting-Options.md](./Broadcasting-Options.md) — Broadcasting configuration
