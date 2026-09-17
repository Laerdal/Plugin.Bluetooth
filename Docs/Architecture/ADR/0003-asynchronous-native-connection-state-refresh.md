# ADR 0003: Asynchronous Native Connection-State Refresh

- Status: Accepted
- Date: 2026-09-17
- Decision Makers: Plugin.Bluetooth maintainers

## Context

On iOS, `NativeRefreshIsConnected()` updated `IsConnected` by dispatching to the main thread via
`MainThreadDispatcher.BeginInvokeOnMainThread`, which posts the action and returns immediately —
it does not wait for the dispatched action to actually run. Every caller in the base class
treated the call as if it synchronously refreshed the property (e.g. `ConnectAsync`/
`DisconnectAsync` read `IsConnected` immediately afterward to decide success/failure). Off the
main thread, this is a race: the property can still hold its pre-refresh value when read.

This produced a real, reproducible bug: `DisconnectAsync()` on iOS frequently threw
`DeviceFailedToDisconnectException` even though the device had genuinely disconnected, because
the post-disconnect `IsConnected` check ran before the main-thread dispatch had completed. The
same race applies symmetrically to `ConnectAsync()`. Android and Windows read connection state
synchronously in-process and are not affected, but the abstract member is shared across all
platforms.

## Decision

Change `BaseBluetoothRemoteDevice.NativeRefreshIsConnected()` (`protected abstract void`) to
`NativeRefreshIsConnectedAsync(CancellationToken)` (`protected abstract ValueTask`), and await it
everywhere the base class needs a guaranteed-fresh `IsConnected` value before branching:
`WaitForIsConnectedAsync`, `ConnectIfNeededAsync`, `DisconnectIfNeededAsync`, the pre/post-native
checks inside `ConnectAsync`/`DisconnectAsync`, and inside `OnConnectSucceededAsync`/
`OnConnectFailedAsync`/`OnDisconnectAsync`.

Apple's implementation dispatches to the main thread via `MainThreadDispatcher.InvokeOnMainThreadAsync`
and awaits completion. Android, Windows, and the DotNetCore fallback keep synchronous bodies
wrapped in `ValueTask.CompletedTask` — their runtime behavior does not change.

Native-callback-driven call sites that cannot become `async` (CoreBluetooth/BluetoothGatt/Windows
delegate methods are `void` by contract) fire the refresh via `.StartAndForget(onException)`,
routing any failure to `BluetoothUnhandledExceptionListener` instead of discarding it.

`ConnectAsync`/`DisconnectAsync` wrap their native-call-and-failure-dispatch logic inside the same
`try/finally` that resets `IsConnecting`/`IsDisconnecting` and clears the pending
`TaskCompletionSource`, so a refresh exception (or cancellation) surfacing during failure handling
can no longer escape past that cleanup and leave the device's connect/disconnect state machine
permanently stuck.

This is a breaking change: `BaseBluetoothRemoteDevice.NativeRefreshIsConnected()` no longer
exists. Any external subclass overriding it will fail to compile against this version and must
migrate to `NativeRefreshIsConnectedAsync(CancellationToken)`.

## Alternatives Considered

### Alternative A: Fixed delay before re-checking IsConnected

- Summary: Insert a short `Task.Delay` after `NativeRefreshIsConnected()` before reading
  `IsConnected`, as a stopgap noted in the original bug report.
- Pros: No public API change, minimal code.
- Cons: Doesn't fix the race, only narrows the window; adds arbitrary latency to every
  connect/disconnect; still fails intermittently under load or on slower devices.

### Alternative B: Keep NativeRefreshIsConnected() synchronous, block until the dispatch completes

- Summary: Leave the abstract member synchronous; have Apple's implementation block the calling
  thread with a wait handle until the main-thread dispatch finishes.
- Pros: No public API/signature change.
- Cons: Risks deadlock if ever invoked from the main thread; blocking waits are explicitly
  forbidden by this repo's own architecture guidelines ("NEVER use `.Result` or `.Wait()`").

## Consequences

### Positive

- Fixes a real, reproducible false-failure on iOS disconnect (and symmetrically on connect)
  caused by reading `IsConnected` before the native refresh had actually completed.
- Fire-and-forget refresh/callback calls no longer swallow exceptions silently; failures reach
  `BluetoothUnhandledExceptionListener`.
- `ConnectAsync`/`DisconnectAsync` can no longer get permanently stuck due to a refresh exception
  or cancellation escaping mid-cleanup.

### Negative

- Breaking API change: any external consumer subclassing `BaseBluetoothRemoteDevice` must migrate
  `NativeRefreshIsConnected()` to `NativeRefreshIsConnectedAsync(CancellationToken)`. Requires a
  major version bump per semver.
- Slightly more overhead per connect/disconnect on iOS (one awaited main-thread round trip instead
  of a fire-and-forgotten one), accepted as the cost of correctness.

### Neutral

- Android, Windows, and DotNetCore implementations remain effectively synchronous
  (`ValueTask.CompletedTask`); this ADR changes their method shape, not their runtime behavior.

## Follow-up Actions

- [ ] Bump the package major version and call out this break explicitly in the release notes.
- [ ] Confirm on real Windows hardware that the `StartAndForget` conversion behaves correctly at
      runtime (compile-verified cross-platform via `EnableWindowsTargeting`, but WinRT COM
      activation of `Windows.Devices.Bluetooth` still requires an actual Windows machine to test).
- [ ] Confirm on real hardware that iOS connect/disconnect no longer produces false
      `DeviceFailedToConnectException`/`DeviceFailedToDisconnectException`.

## References

- Code references:
  - `Bluetooth.Core.Scanning/BaseBluetoothRemoteDevice.Connection.cs`
  - `Bluetooth.Maui.Platforms.Apple/Scanning/AppleBluetoothRemoteDevice.cs`
  - `Bluetooth.Maui.Platforms.Apple/Threading/MainThreadDispatcher.cs`
- Related docs:
  - `Docs/ARCHITECTURE_GUIDELINES.md`
- Related PRs/issues:
  - PR #55
