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

This *does* correlate a native callback with the specific `ConnectAsync`/`DisconnectAsync` attempt
that triggered it, via an app-level attempt token - `_connectAttemptToken`/`_disconnectAttemptToken` -
threaded entirely through Core's own state rather than through the native delegate callbacks
themselves (CoreBluetooth, and the WinRT/Android equivalents, are fixed-signature `void` callbacks
with no per-call context slot to smuggle a token through). `ConnectAsync`/`DisconnectAsync` set the
token to their own `ownConnectionTcs`/`ownDisconnectionTcs` in the same lock acquisition where they
install it as the live `ConnectionTcs`/`DisconnectionTcs`. Every native-callback-driven completion
path validates and claims through a single atomic helper, `TryClaimPendingConnectAttempt` (one lock
acquisition: check the token still matches the live `ConnectionTcs` and no terminal signal has been
claimed yet, then claim by setting `ConnectAttemptTerminalSignalReceived`) - closing the
two-separate-lock-acquisitions gap `OnDisconnectAsync`'s `IsConnectionAttemptPending` check and its
subsequent `OnConnectFailedAsync` call previously had between them, and the same unsynchronized-write
race in `ConnectAttemptTerminalSignalReceived` itself, since claim-check and claim-write now happen
under the one lock instead of as two separate statements. `OnConnectSucceededAsync`/
`OnConnectFailedAsync`/`OnDisconnectAsync`'s own connect-attempt-pending branch all route through
this helper; Android's `OnConnectionStateChange` calls it directly too (replacing its own former
`IsConnectionAttemptPending` pre-check) so it can attach the native `GattStatus` as the failure
reason via the new `CompleteClaimedConnectFailureAsync`, without a second, separately-locked
re-validation that could target a different (newer) attempt than the one it just checked.

`ConnectAsync`'s abandon path (the outer `catch` that cancels a native connect request it's giving
up on) retires `_connectAttemptToken` to `null` *before* issuing that cancellation, not only in the
method's own `finally` - a new `ConnectAsync` call cannot start until this call returns to its own
caller, so retiring here happens-before any such call, closing the specific real-hardware-confirmed
race in this method's own comment (a stale connect request finally completing ~29s after being
abandoned, once a second, legitimate attempt had already started). Both methods' `finally` blocks
also now cancel their own TCS and clear `ConnectionTcs`/`DisconnectionTcs` (and the matching token)
in one lock acquisition instead of two separate ones - previously, a new call's merge-check could
observe the just-cancelled TCS as "completed" and install its own attempt in the gap between the
cancel and the separately-locked clear.

This closes the correlation gap for every attempt-abandonment path reachable through this library's
own documented usage (timeout/cancel-then-retry, concurrent connect vs. disconnect, merge races).
It does not reach a scenario outside this library's control: if a native platform were to somehow
deliver two genuinely overlapping callbacks for the *same* peripheral - one stale, for an already-
retired attempt, and one for a brand-new attempt that has since fully installed its own token - in
the exact instant between them, the stale one would find the fields already pointing at the newer
attempt and be misattributed to it. This requires CoreBluetooth/BluetoothGatt/WinRT to violate their
own single-outstanding-native-operation-per-peripheral behavior to manifest, is dramatically
narrower than the gap this ADR previously left open, and remains the only case a real per-call
native context token (which none of the three platforms' delegate APIs expose) could fully close.

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
cancellation could still land microseconds after the check, immediately before the write. This is a
different, lower-level race than the attempt-correlation one closed above (it's about
`NativeRefreshIsConnectedAsync`'s own dispatched write, not about which attempt a terminal signal
belongs to) and the attempt token doesn't apply to it - closing it fully would need cancelling the
already-queued main-thread action itself, which `MainThread.InvokeOnMainThreadAsync` doesn't support.

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

`OnDisconnectAsync` now also calls `TryClaimPendingConnectAttempt` before doing anything else: a
disconnect signal that arrives while a connect attempt is still pending (no terminal native signal
claimed for it yet) is the terminal result of a *failed connect*, not a completed disconnect - the
device never actually finished connecting. Routing that case through the normal
`TrySetResultOrException(e)` path would complete the pending `ConnectionTcs` as a success (when `e`
is `null`, the common case for a plain native disconnect callback). It now routes to
`CompleteConnectFailureAsync` instead (the same helper `OnConnectFailedAsync` uses), defaulting to a
generic `DeviceFailedToConnectException` when no more specific exception is available. This lives
in the shared base method so Apple's and Windows's disconnect callbacks - which call
`OnDisconnectAsync()` directly with no equivalent check of their own - are covered automatically,
not just Android's.

`TryClaimPendingConnectAttempt` claims under one lock acquisition:
`ReferenceEquals(_connectAttemptToken, ConnectionTcs) && ConnectionTcs is { Task.IsCompleted: false }
&& !ConnectAttemptTerminalSignalReceived`, then sets `ConnectAttemptTerminalSignalReceived` and
returns `ConnectionTcs` - three conditions, not one:
- The token check ties the claim to the specific attempt that was live when `ConnectAsync` actually
  issued the native connect call (see Decision, above) - a mismatch means a newer attempt has since
  taken over, or this one was retired after being abandoned, either way making this signal stale.
- Checking `ConnectionTcs`'s completion is deliberate instead of the existing `IsConnecting` flag,
  since `IsConnecting` stays `true` until `ConnectAsync`'s own `finally` runs, which can be well
  after a successful connect's TCS is already completed - checking it alone would misclassify a
  genuine disconnect that follows a fast, already-succeeded connect as a failed connect.
- `ConnectionTcs`'s completion alone has its own, narrower gap: both `OnConnectSucceededAsync` and
  `OnConnectFailedAsync` await their own refresh *before* completing `ConnectionTcs`, so it stays
  incomplete for that entire window too - a disconnect signal arriving while a just-succeeded (or
  just-failed) connect is still mid-refresh would still read as "not yet claimed" without also
  checking `ConnectAttemptTerminalSignalReceived`, which is set atomically alongside the claim - the
  very first (and only) caller to acquire the lock while it's still `false` wins the claim; every
  other caller (including a concurrent native signal racing the same lock) sees it already `true`
  and correctly backs off instead of racing to complete the same TCS a second time.

`OnDisconnectAsync` no longer attempts to complete `ConnectionTcs` at all once a connect-attempt
claim fails (only `DisconnectionTcs`, via the `_disconnectAttemptToken` check below): a failed claim
past this point means either there is no live connect attempt, or one exists but has already been
(or is concurrently being) claimed by `OnConnectSucceededAsync`/`OnConnectFailedAsync`, which owns
completing it based on its own, more-informed refresh. `OnDisconnectAsync` completing it instead -
e.g. with a "disconnected" outcome while that other call is still mid-refresh - would let it steal
ownership of an outcome it isn't positioned to correctly determine, and could make `ConnectAsync`
return success (or a stale failure) that the connect handler's own refresh had already contradicted.

`OnConnectFailedAsync`'s final fallback (no live TCS to deliver the exception to) now reports via
`ReportBestEffortFailure` instead of calling `BluetoothUnhandledExceptionListener` directly:
`OnBluetoothUnhandledException` rethrows when nothing is registered, and every native callback
already wraps its call to `OnConnectFailedAsync` in its own `StartAndForget(ex =>
BluetoothUnhandledExceptionListener...)` - letting the fallback's own direct call rethrow would let
that wrapper deliver the same failure to the listener a second time.

On Android, `OnConnectionStateChange`'s `ProfileState.Disconnected` case calls
`TryClaimPendingConnectAttempt` itself (exposed as `protected` from Core) before calling
`OnDisconnectAsync` at all, so it can attach the native `GattStatus` as the failure reason (via
`AndroidNativeGattCallbackStatusException`) through `CompleteClaimedConnectFailureAsync` when
non-`Success`, which the generic Core-level routing inside `OnDisconnectAsync` cannot do without
platform-specific knowledge - without needing a second, separately-locked re-validation of its own
(which could otherwise target a different, newer attempt than the one it just claimed).

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
- The `protected bool IsConnectionAttemptPending` property no longer exists, replaced by
  `protected TaskCompletionSource? TryClaimPendingConnectAttempt()` (an atomic check-and-claim, not
  a side-effect-free check - see Decision) and `protected ValueTask CompleteClaimedConnectFailureAsync(...)`.
  Any external subclass reading the old property will fail to compile against this version.

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
- A stale, abandoned connect/disconnect attempt's native callback can no longer resolve whichever
  attempt is live when it eventually arrives, via the `_connectAttemptToken`/`_disconnectAttemptToken`
  correlation described in Decision, above - narrowing this from a wide-open gap to a residual,
  compound edge case that requires the native platform to violate its own
  single-outstanding-operation-per-peripheral behavior to manifest.
- Calling `ConnectAsync`/`DisconnectAsync` directly (not through the `*IfNeededAsync` wrappers) no
  longer evaluates the already-connected/already-disconnected guard against a stale cached value.
- A refresh failure inside `OnConnectSucceededAsync`/`OnDisconnectAsync` now fails the pending
  operation instead of silently completing it as a success that was never actually verified.
- A disconnect signal that arrives while a connect attempt is still pending no longer completes
  that attempt as a success on any platform (previously only handled on Android); it now fails it
  via the same `CompleteConnectFailureAsync` helper `OnConnectFailedAsync` uses, and this
  classification is no longer racy against `OnConnectSucceededAsync`/`OnConnectFailedAsync`'s own
  awaited refresh.
- A purely timeout-triggered abandonment of the refresh on Apple is now reported the same way a
  cancellation-triggered one already was, closing what was previously an accepted gap.
- `OnConnectSucceededAsync`/`OnDisconnectAsync` no longer trust the original native callback's
  signal blindly once a fresh refresh is available - a merged caller can no longer observe success
  for a connect/disconnect that the refresh already proved didn't happen.
- A refresh failure with no live TCS to deliver it to is no longer reported to
  `BluetoothUnhandledExceptionListener` twice (once directly, once via the thrown wrapper
  exception's own `StartAndForget` reporting).
- `TryClaimPendingConnectAttempt`'s check-and-claim is now one atomic lock acquisition instead of
  a separately-locked read followed by a separately-locked capture, closing the window where a
  disconnect callback (or Android's own `OnConnectionStateChange` check) could otherwise complete a
  brand-new connect attempt's TCS instead of correctly failing the one it actually classified
  against.
- An abandoned Apple refresh whose queued main-thread action only runs after its caller gave up no
  longer overwrites `IsConnected` with a stale reading (narrowed, not fully closed - see Decision).
- `OnDisconnectAsync` no longer completes `ConnectionTcs` itself once its initial
  `TryClaimPendingConnectAttempt` call fails to claim it, so it can no longer race
  `OnConnectSucceededAsync`/`OnConnectFailedAsync` to steal ownership of a connect attempt's
  outcome once that attempt's own terminal signal has been claimed.
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
- [x] Thread an app-level generation/attempt token (`_connectAttemptToken`/`_disconnectAttemptToken`,
      validated and claimed atomically via `TryClaimPendingConnectAttempt`) through Core's own state
      so native-callback-driven completion paths can tell a stale signal from a live one - this
      closes `ConnectAttemptTerminalSignalReceived`'s former unsynchronized-write race and the
      former two-separate-lock-acquisitions gap between `OnDisconnectAsync`'s pending-connect check
      and its subsequent capture, for every abandonment path reachable through this library's own
      documented usage. Residual: a compound, timing-precise scenario requiring the native platform
      to violate its own single-outstanding-operation-per-peripheral behavior - see Decision.

## References

- Code references:
  - `Bluetooth.Core.Scanning/BaseBluetoothRemoteDevice.Connection.cs`
  - `Bluetooth.Maui.Platforms.Apple/Scanning/AppleBluetoothRemoteDevice.cs`
  - `Bluetooth.Maui.Platforms.Apple/Threading/MainThreadDispatcher.cs`
- Related docs:
  - `Docs/ARCHITECTURE_GUIDELINES.md`
- Related PRs/issues:
  - PR #55
