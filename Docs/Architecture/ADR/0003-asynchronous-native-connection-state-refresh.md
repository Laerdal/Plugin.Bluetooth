# ADR 0003: Asynchronous Native Connection-State Refresh

- Status: Proposed
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
same race applies symmetrically to `ConnectAsync()`.

Windows reads connection state with a synchronous in-process property read and is not affected by
this specific race. Android is different again: its refresh does not query native state at all —
it only clears `IsConnected` to `false` when the GATT proxy is null, and otherwise no-ops,
relying entirely on the `OnConnectionStateChange` callback to update `IsConnected` out of band.
Android is not affected by *this* race either, but it does not gain a real freshness guarantee
from awaiting the refresh — if `OnConnectionStateChange` were ever missed, `IsConnected` would
stay stale indefinitely with no corrective poll. The abstract member is shared across all
platforms regardless.

## Decision

Change `BaseBluetoothRemoteDevice.NativeRefreshIsConnected()` (`protected abstract void`) to
`NativeRefreshIsConnectedAsync(CancellationToken)` (`protected abstract ValueTask`), and await it
everywhere the base class needs a guaranteed-fresh `IsConnected` value before branching:
`WaitForIsConnectedAsync`, the pre/post-native checks inside `ConnectAsync`/`DisconnectAsync`, and
inside `OnConnectSucceededAsync`/`OnConnectFailedAsync`/`OnDisconnectAsync`. `ConnectIfNeededAsync`/
`DisconnectIfNeededAsync` do *not* refresh themselves - they delegate entirely to `ConnectAsync`/
`DisconnectAsync` (which already refresh) and catch+swallow `DeviceIsAlreadyConnectedException`/
`DeviceIsAlreadyDisconnectedException` to turn the throw into a no-op; refreshing in both places
would cost two main-thread dispatches on Apple for one conditional connect/disconnect.

Apple's implementation dispatches to the main thread via `MainThreadDispatcher.InvokeOnMainThreadAsync`
and awaits completion. Android and Windows keep synchronous bodies wrapped in
`ValueTask.CompletedTask` — their runtime behavior does not change. The DotNetCore fallback keeps
throwing `PlatformNotSupportedException`, consistent with the rest of that platform's unimplemented
BLE surface — it is not a no-op success like Android/Windows.

Native-callback-driven call sites that cannot become `async` (CoreBluetooth/BluetoothGatt/Windows
delegate methods are `void` by contract) fire the refresh via `.StartAndForget(onException)`,
routing any failure to `BluetoothUnhandledExceptionListener` instead of discarding it.

`ConnectAsync`/`DisconnectAsync` wrap their native-call-and-failure-dispatch logic inside the same
`try/finally` that resets `IsConnecting`/`IsDisconnecting` and clears the pending
`TaskCompletionSource`, so a refresh exception (or cancellation) surfacing during failure handling
can no longer escape past that cleanup and leave the device's connect/disconnect state machine
permanently stuck.

`OnConnectSucceededAsync`/`OnConnectFailedAsync`/`OnDisconnectAsync` now capture the live
`ConnectionTcs`/`DisconnectionTcs` reference *before* awaiting the refresh, and only ever complete
that captured instance afterward. Awaiting inside these methods opened a window that did not exist
when they were synchronous: a native callback for an abandoned attempt (e.g. one `ConnectAsync`
call that already timed out) could resume mid-await after `ConnectAsync`'s own `finally` had
already cleared `ConnectionTcs` and a *later*, unrelated attempt had installed a new one — and,
without capturing, would complete that newer attempt's TCS with the stale attempt's outcome.
Capturing the reference up front ties each signal to the specific attempt that was live when the
native event actually fired, not whatever attempt happens to be live once the await resumes. If
the refresh itself throws, that failure is reported via a `ReportBestEffortFailure` helper instead
of calling `BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException` directly - that
listener rethrows when no listener is registered, which would otherwise skip the TCS completion
below it and hang any caller merged onto that TCS. An `OperationCanceledException` caused by the
caller's own requested cancellation is swallowed before that reporting step entirely - it's a
normal, documented outcome of `ConnectAsync`/`DisconnectAsync`, not a fault worth notifying
registered listeners about.

This does **not** attempt to correlate a native callback with the specific `ConnectAsync`/
`DisconnectAsync` call that triggered it - CoreBluetooth (and the WinRT/Android equivalents) don't
expose a per-call token to correlate against, so a stale callback for an attempt abandoned via
timeout/cancellation can still resolve whatever attempt is live when it eventually arrives. This is
the same risk already documented in `ConnectAsync`'s own comment (confirmed via real hardware); a
real fix would need an app-level generation/attempt token threaded through every native delegate
callback across all three platforms, which is out of scope for this change.

`ConnectAsync`/`DisconnectAsync` also now refresh before their own initial already-connected /
already-disconnected guard, not just via the `*IfNeededAsync` wrappers - calling either method
directly previously evaluated that guard against a potentially stale cached value. Both methods
own their `TaskCompletionSource` end to end: they capture it in a local (`ownConnectionTcs`/
`ownDisconnectionTcs`) rather than re-reading the live `ConnectionTcs`/`DisconnectionTcs` property,
and their `finally` only resets `IsConnecting`/`ConnectionTcs` (or the disconnect equivalents) - both
together, under `_connectionOperationLock` - if the live property still points at the instance they
installed. Without that, a concurrent newer attempt that had already taken over the live property
and set its own `IsConnecting`/`IsDisconnecting` could have that state wiped out by the older
attempt's cleanup. The "merge concurrent attempts" branch also now bounds its wait with the
*merging* caller's own `timeout`/`cancellationToken`, not just the owning attempt's - otherwise a
merged caller would keep waiting even after its own timeout, since only the owner's wait was ever
bounded.

Apple's `NativeConnectAsync`/`NativeDisconnectAsync` no longer refresh internally before issuing
the native call: neither method branches on `IsConnected`, so the refresh was pure overhead there,
and it was unboundable on the path where `ConnectAsync` calls `NativeDisconnectAsync` during
cleanup with `timeout: null`/`CancellationToken.None` - a stuck main-thread dispatch there could
have hung that best-effort cleanup indefinitely. The freshness guarantee they don't provide anymore
still holds through Core's own pre/post-native-call refreshes. Apple's `NativeRefreshIsConnectedAsync`
also only starts observing the dispatched task's own fault *after* cancellation actually fires
(via `CancellationToken.Register`), rather than unconditionally - otherwise an ordinary (non-cancelled)
fault would be reported twice: once by that observer and once through the normal awaited-propagation
path.

This is a breaking change with two independent surfaces:
- `BaseBluetoothRemoteDevice.NativeRefreshIsConnected()` no longer exists. Any external subclass
  overriding it will fail to compile against this version and must migrate to
  `NativeRefreshIsConnectedAsync(CancellationToken)`.
- `OnConnectSucceeded()`, `OnConnectFailed(Exception)`, and `OnDisconnect(Exception?)` were renamed
  to `OnConnectSucceededAsync(CancellationToken)`, `OnConnectFailedAsync(Exception, CancellationToken)`,
  and `OnDisconnectAsync(Exception?, CancellationToken)`. These are `protected` (not `virtual`), so
  external subclasses cannot override them, but any subclass that *calls* the old names directly
  will also fail to compile against this version.

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
- Capturing the TCS up front closes the specific race where *this call's own* await of the refresh
  let a later attempt install a new TCS before the capture happened. It does **not** correlate a
  native callback with the attempt that triggered it - a late callback for an already-abandoned
  attempt can still resolve whatever attempt is live when it eventually arrives (see the
  correlation-gap paragraph in Decision, which remains an open, accepted limitation).
- Calling `ConnectAsync`/`DisconnectAsync` directly (not through the `*IfNeededAsync` wrappers) no
  longer evaluates the already-connected/already-disconnected guard against a stale cached value.

### Negative

- Breaking API change with two surfaces: `NativeRefreshIsConnected()` →
  `NativeRefreshIsConnectedAsync(CancellationToken)`, and `OnConnectSucceeded`/`OnConnectFailed`/
  `OnDisconnect` → their `*Async` equivalents. Any external consumer subclassing or calling into
  `BaseBluetoothRemoteDevice`'s protected surface must migrate. Requires a major version bump per
  semver.
- Slightly more overhead per connect/disconnect on iOS (one awaited main-thread round trip instead
  of a fire-and-forgotten one), accepted as the cost of correctness.

### Neutral

- Android and Windows implementations remain effectively synchronous (`ValueTask.CompletedTask`);
  this ADR changes their method shape, not their runtime behavior. The DotNetCore fallback keeps
  throwing `PlatformNotSupportedException` as before, unaffected by this change.

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
