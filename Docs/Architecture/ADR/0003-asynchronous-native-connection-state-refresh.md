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
callback across all three platforms, which is out of scope for this change. This same root cause
(no way for a stale signal to know it's stale) also means: `ConnectAttemptTerminalSignalReceived`'s
writes in `OnConnectSucceededAsync`/`OnConnectFailedAsync` are unconditional and unsynchronized - a
late write for an abandoned attempt can still land after a newer attempt has reset the flag for
itself, marking that newer attempt as if it had already received a terminal signal it hasn't;
and `OnDisconnectAsync`'s `IsConnectionAttemptPending` check and its subsequent call into
`OnConnectFailedAsync` are two separate lock acquisitions, not one atomic operation - a newer
attempt's `ConnectAsync` can install its own `ConnectionTcs` in the gap between them, so the
`OnConnectFailedAsync` call ends up capturing and failing that newer attempt instead of the one the
disconnect was actually classified against. Both are consequences of the same fundamental gap, not
independent bugs, and are not fixed here for the same reason.

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
(via `CancellationToken.Register`, disposed once the call completes), rather than unconditionally -
otherwise an ordinary (non-cancelled) fault would be reported twice: once by that observer and once
through the normal awaited-propagation path.

`MainThread.InvokeOnMainThreadAsync`'s queued action cannot itself be cancelled once queued - it
runs regardless of what happens to the caller awaiting it. The dispatched action now checks
`cancellationToken.IsCancellationRequested` before publishing to `IsConnected`, so an abandoned
refresh that only actually runs on the main thread later (after its caller gave up) doesn't
overwrite `IsConnected`/raise state events with a stale reading that could clobber a newer, more
current connect/disconnect attempt's own state. This narrows but doesn't fully close the window -
cancellation could still land microseconds after the check, immediately before the write - closing
it completely would need the same generation/attempt correlation already out of scope for native
callbacks in general (see above).

Every Core call site that has a `timeout` (`WaitForIsConnectedAsync`, and the pre/post-native
checks in `ConnectAsync`/`DisconnectAsync`) now goes through a private `RefreshIsConnectedAsync`
helper instead of wrapping `NativeRefreshIsConnectedAsync` directly in
`WaitBetterAsync(timeout, cancellationToken)`. That old pattern bounded how long the *caller*
waited but never actually cancelled the token handed to the platform implementation, so Apple's
cancellation-triggered observer above never learned that a purely timeout-triggered abandonment
had happened, and a fault arriving after that point went unreported. The helper instead links
`timeout` into an actual `CancellationTokenSource` (via `CancelAfter`) before calling
`NativeRefreshIsConnectedAsync`, translating a timeout into a real cancellation of the same token -
making a timeout indistinguishable from an explicit cancellation to the platform implementation,
and closing the gap without any platform-specific timeout plumbing. It throws `TimeoutException`
when its own linked timeout fires, matching what `WaitBetterAsync` would have thrown. The three
callback-driven methods (`OnConnectSucceededAsync`/`OnConnectFailedAsync`/`OnDisconnectAsync`) have
no `timeout` parameter at all - only `cancellationToken` - so they call
`NativeRefreshIsConnectedAsync` directly and are unaffected by this helper.

When the refresh itself fails (a non-cancellation exception) inside `OnConnectSucceededAsync` or
`OnDisconnectAsync`, the captured TCS is now failed with that refresh exception instead of being
completed as if the original outcome (a successful connect, or `e == null` for disconnect) still
held - `IsConnected` can no longer be trusted once its own refresh has faulted, so reporting success
without being able to verify it would be misleading. `ReportBestEffortFailure` only runs when there
was *no* live TCS to deliver the failure to directly: if a live TCS absorbed it, whoever explicitly
called `ConnectAsync`/`DisconnectAsync` already receives it via that captured task, so also
reporting it to `BluetoothUnhandledExceptionListener` here would be a second delivery of the same
failure - the first being the wrapper exception these methods then throw, which every calling
native callback already routes to that same listener via its own `StartAndForget(onException)`.

Even when the refresh itself *succeeds*, `OnConnectSucceededAsync`/`OnDisconnectAsync` now validate
its result before declaring the operation's outcome, instead of trusting the native callback's
original signal blindly: `OnConnectSucceededAsync` only calls `connectionTcs.TrySetResult()` when
the just-refreshed `IsConnected` is actually `true` (faulting the TCS with
`DeviceFailedToConnectException` otherwise), and `OnDisconnectAsync`, when `e` is `null`, only
completes the captured TCS successfully when the just-refreshed `IsConnected` is actually `false`
(faulting it with `DeviceFailedToDisconnectException` otherwise). Without this, a merged caller
(see `ConnectAsync`/`DisconnectAsync`'s merge branch, which returns as soon as the same TCS
completes) could observe success for a connect/disconnect that the just-awaited refresh had already
proven didn't actually happen - the owning call would only discover that moments later via its own
separate post-native refresh/check.

`OnDisconnectAsync` now also checks a new `IsConnectionAttemptPending` property before doing
anything else: a disconnect signal that arrives while a connect attempt is still pending (no
terminal native signal received for it yet) is the terminal result of a *failed connect*, not a
completed disconnect - the device never actually finished connecting. Routing that case through the
normal `TrySetResultOrException(e)` path would complete the pending `ConnectionTcs` as a success
(when `e` is `null`, the common case for a plain native disconnect callback). It now routes to
`OnConnectFailedAsync` instead, defaulting to a generic `DeviceFailedToConnectException` when no
more specific exception is available. This lives in the shared base method so Apple's and Windows's
disconnect callbacks - which call `OnDisconnectAsync()` directly with no equivalent check of their
own - are covered automatically, not just Android's.

`IsConnectionAttemptPending` is `ConnectionTcs is { Task.IsCompleted: false } &&
!ConnectAttemptTerminalSignalReceived` - two conditions, not one:
- Checking `ConnectionTcs`'s completion alone isn't enough to know a terminal signal hasn't arrived
  yet: it's deliberately checked instead of the existing `IsConnecting` flag, since `IsConnecting`
  stays `true` until `ConnectAsync`'s own `finally` runs, which can be well after a successful
  connect's TCS is already completed - checking it alone would misclassify a genuine disconnect
  that follows a fast, already-succeeded connect as a failed connect.
- But `ConnectionTcs`'s completion alone has its own, narrower gap: both `OnConnectSucceededAsync`
  and `OnConnectFailedAsync` await their own refresh *before* completing `ConnectionTcs`, so it
  stays incomplete for that entire window too - a disconnect signal arriving while a just-succeeded
  (or just-failed) connect is still mid-refresh would still read as "no terminal signal received
  yet" and get routed to `OnConnectFailedAsync` a second time, racing to complete the same TCS a
  second time against whichever of `OnConnectSucceededAsync`/`OnConnectFailedAsync` is already
  running. `ConnectAttemptTerminalSignalReceived` closes this: it's set to `true` as the very first
  statement in both methods (before either captures the TCS or awaits anything), and reset to
  `false` only when `ConnectAsync` installs a new attempt's TCS - so it accurately reflects "has a
  terminal signal been *received*", independent of how long that signal takes to finish processing.

`IsConnectionAttemptPending`'s getter reads both conditions under `_connectionOperationLock`.
`ConnectAsync`'s merge-check block writes `ConnectionTcs` and resets
`ConnectAttemptTerminalSignalReceived` as two separate statements under that same lock; without
also taking it on the read side, a disconnect callback could observe the newly-installed, genuinely
incomplete `ConnectionTcs` alongside the *previous* attempt's stale `true` flag (a read landing
between those two writes) - concluding no attempt is pending and completing the brand-new attempt's
TCS instead of correctly routing to it as a failure. This closes the read/write race specifically
between the getter and `ConnectAsync`'s own compound write - it does **not** close the separate,
generation-correlation-shaped gap in `ConnectAttemptTerminalSignalReceived`'s *own* writes (see
Decision, above) - locking a read against one specific writer doesn't help against a different,
unsynchronized writer racing the same field for an unrelated (stale) attempt.

`OnDisconnectAsync` no longer attempts to complete `ConnectionTcs` at all past its initial
`IsConnectionAttemptPending` check (only `DisconnectionTcs`): once that check has already routed a
disconnect arriving *before* any terminal connect signal to `OnConnectFailedAsync`, any
`ConnectionTcs` still live past that point belongs to an attempt whose own
`OnConnectSucceededAsync`/`OnConnectFailedAsync` has already received its terminal signal and is
(or will shortly be) completing it based on its own, more-informed refresh. `OnDisconnectAsync`
completing it instead - e.g. with a "disconnected" outcome while that other call is still
mid-refresh - would let it steal ownership of an outcome it isn't positioned to correctly
determine, and could make `ConnectAsync` return success (or a stale failure) that the connect
handler's own refresh had already contradicted.

`OnConnectFailedAsync`'s final fallback (no live TCS to deliver the exception to) now reports via
`ReportBestEffortFailure` instead of calling `BluetoothUnhandledExceptionListener` directly:
`OnBluetoothUnhandledException` rethrows when nothing is registered, and every native callback
already wraps its call to `OnConnectFailedAsync` in its own `StartAndForget(ex =>
BluetoothUnhandledExceptionListener...)` - letting the fallback's own direct call rethrow would let
that wrapper deliver the same failure to the listener a second time.

On Android, `OnConnectionStateChange`'s `ProfileState.Disconnected` case additionally checks the
same `IsConnectionAttemptPending` property itself (exposed as `protected` from Core) before calling
`OnDisconnectAsync` at all, so it can attach the native `GattStatus` as the failure reason (via
`AndroidNativeGattCallbackStatusException`) when non-`Success`, which the generic Core-level check
above cannot do without platform-specific knowledge.

Native-callback-driven calls into `OnConnectSucceededAsync`/`OnConnectFailedAsync`/`OnDisconnectAsync`
always pass `cancellationToken: default` (delegate methods are `void` by contract and have no
per-call token to supply), and none of the three take a `timeout` parameter either (there is no
caller-supplied one to use - they're triggered by native events, not a user call). Unlike the
pre-ADR-0003 fire-and-forget refresh this replaced (which could never block a caller no matter how
long it took), this refresh is now on the operation-completion path: `ConnectAsync`/
`DisconnectAsync`, even called with their documented default (no timeout), await the TCS these
methods complete, so a refresh that never completes here means they never complete either. All
three now bound their refresh with a fixed `CallbackRefreshTimeout` (5 seconds) via
`RefreshIsConnectedAsync`, instead of calling `NativeRefreshIsConnectedAsync` directly with no
bound at all. Five seconds is a backstop against a genuinely stuck main thread, not a normal-path
timing constraint - this is normally a near-instant local property read. A refresh that exceeds
this bound is treated like any other refresh failure (see above): it faults the live TCS, or
reports via `ReportBestEffortFailure` if there's no live TCS to deliver it to.

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
- A stale, abandoned connect/disconnect attempt's native callback can still resolve whichever
  attempt is live when it eventually arrives; generation correlation is out of scope (see
  Decision).
- Calling `ConnectAsync`/`DisconnectAsync` directly (not through the `*IfNeededAsync` wrappers) no
  longer evaluates the already-connected/already-disconnected guard against a stale cached value.
- A refresh failure inside `OnConnectSucceededAsync`/`OnDisconnectAsync` now fails the pending
  operation instead of silently completing it as a success that was never actually verified.
- A disconnect signal that arrives while a connect attempt is still pending no longer completes
  that attempt as a success on any platform (previously only handled on Android); it now fails it
  via `OnConnectFailedAsync`, and this classification is no longer racy against
  `OnConnectSucceededAsync`/`OnConnectFailedAsync`'s own awaited refresh.
- A purely timeout-triggered abandonment of the refresh on Apple is now reported the same way a
  cancellation-triggered one already was, closing what was previously an accepted gap.
- `OnConnectSucceededAsync`/`OnDisconnectAsync` no longer trust the original native callback's
  signal blindly once a fresh refresh is available - a merged caller can no longer observe success
  for a connect/disconnect that the refresh already proved didn't happen.
- A refresh failure with no live TCS to deliver it to is no longer reported to
  `BluetoothUnhandledExceptionListener` twice (once directly, once via the thrown wrapper
  exception's own `StartAndForget` reporting).
- `IsConnectionAttemptPending`'s read is now atomic with `ConnectAsync`'s compound write of
  `ConnectionTcs`/`ConnectAttemptTerminalSignalReceived`, closing a narrow window where a
  disconnect callback could otherwise complete a brand-new connect attempt's TCS instead of
  correctly failing it.
- An abandoned Apple refresh whose queued main-thread action only runs after its caller gave up no
  longer overwrites `IsConnected` with a stale reading (narrowed, not fully closed - see Decision).
- `OnDisconnectAsync` no longer completes `ConnectionTcs` itself past its initial
  `IsConnectionAttemptPending` check, so it can no longer race
  `OnConnectSucceededAsync`/`OnConnectFailedAsync` to steal ownership of a connect attempt's
  outcome once that attempt's own terminal signal has been received.
- `OnConnectFailedAsync`'s no-live-TCS fallback no longer reports the same failure to
  `BluetoothUnhandledExceptionListener` twice (once directly, once via the calling native
  callback's own `StartAndForget` wrapper after the direct call rethrows).
- `OnConnectFailedAsync` now evaluates completing `connectionTcs` and `disconnectionTcs`
  independently instead of via `||`, so a disconnect routed here while both a connect attempt and
  an explicit `DisconnectAsync` call are concurrently pending completes both instead of
  short-circuiting after the first and leaving the other waiting until its own timeout.
- Native-callback-driven refreshes (`OnConnectSucceededAsync`/`OnConnectFailedAsync`/
  `OnDisconnectAsync`) are now bounded by a fixed safety-valve timeout instead of being fully
  unbounded, closing a real (not just theoretical) way for a stalled Apple main thread to hang
  `ConnectAsync`/`DisconnectAsync` indefinitely even at their documented default.

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

- [x] Bump the package major version (`.config/version.json`: `major` 4 → 5) and call out this
      break explicitly in the release notes.
- [ ] Confirm on real Windows hardware that the `StartAndForget` conversion behaves correctly at
      runtime (compile-verified cross-platform via `EnableWindowsTargeting`, but WinRT COM
      activation of `Windows.Devices.Bluetooth` still requires an actual Windows machine to test).
- [ ] Confirm on real hardware that iOS connect/disconnect no longer produces false
      `DeviceFailedToConnectException`/`DeviceFailedToDisconnectException`.
- [x] Give native-callback-driven refresh calls (`OnConnectSucceededAsync`/`OnConnectFailedAsync`/
      `OnDisconnectAsync`, all invoked with `cancellationToken: default`) a bounded lifetime
      (fixed 5s `CallbackRefreshTimeout` via `RefreshIsConnectedAsync`) instead of an unbounded
      one, so a stalled Apple main-thread queue can't hang `ConnectAsync`/`DisconnectAsync`
      indefinitely even at their documented default (no timeout).
- [ ] Thread an app-level generation/attempt token through every native delegate callback across
      all three platforms (the same redesign already needed for the callback-correlation gap in
      Decision, above) - this would also close: `ConnectAttemptTerminalSignalReceived`'s
      unsynchronized writes racing a newer attempt's reset, and the two-separate-lock-acquisitions
      gap between `OnDisconnectAsync`'s `IsConnectionAttemptPending` check and its subsequent
      `OnConnectFailedAsync` call.

## References

- Code references:
  - `Bluetooth.Core.Scanning/BaseBluetoothRemoteDevice.Connection.cs`
  - `Bluetooth.Maui.Platforms.Apple/Scanning/AppleBluetoothRemoteDevice.cs`
  - `Bluetooth.Maui.Platforms.Apple/Threading/MainThreadDispatcher.cs`
- Related docs:
  - `Docs/ARCHITECTURE_GUIDELINES.md`
- Related PRs/issues:
  - PR #55
