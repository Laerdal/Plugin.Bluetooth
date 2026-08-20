---
name: plugin-bluetooth
description: Use when writing, reviewing, or debugging code in this repo (the Bluetooth.Abstractions*/Bluetooth.Core*/Bluetooth.Maui* projects) — BLE scanning, GATT client/server, L2CAP, broadcasting — or when consuming the Bluetooth.Maui NuGet package and hitting platform-specific (Android/iOS/macOS/Windows) BLE behavior. Routes to this repo's existing architecture/conventions docs instead of re-deriving them, and flags native-callback bug classes that were only found through real-hardware testing and aren't written down anywhere else yet.
---

# Plugin.Bluetooth orientation

Cross-platform .NET MAUI BLE library (OSS, `laerdal/Plugin.Bluetooth`). Central role
(scanner/GATT client) + peripheral role (broadcaster/GATT server), one project per concern,
layered Abstractions → Core → Platform (`Droid`/`Apple`/`Win`/`DotNetCore`) → `Bluetooth.Maui`
facade. Don't re-derive any of this from scratch — it's already documented in depth in this
repo. Verify the paths below still exist before trusting them; if they don't, this skill is
stale, not the docs.

## Step 1 — do this now, before anything else

**Read `Docs/COPILOT_INSTRUCTIONS.md` in full, right now, before writing, reviewing, or
reasoning about any non-trivial code in this repo.** That file is the single source of truth
for architecture, naming conventions, logging/EventId ranges, exception hierarchy, options
pattern, cancellation/TCS patterns, thread-safety rules, per-platform API guidance, and a
60+ item code review checklist — everything below this point in this skill assumes you've
already read it and only adds what that file doesn't cover. This mirrors what
`.github/copilot-instructions.md` does for GitHub Copilot (it auto-loads a thin pointer to
the same file) — you don't get that auto-load, so treat this line as your equivalent of it.
Don't skip this because the task looks small; the file itself is what defines what "small"
safely means here (e.g. a one-line log statement still has EventId-range and structured-
logging rules attached to it).

## Where the rest of the depth lives — read before writing code

| You need | Read |
|---|---|
| Doc map / where anything else lives | `Docs/README.md` |
| Architecture rules + diagrams + why the facade pattern exists | `Docs/ARCHITECTURE_GUIDELINES.md`, `Docs/ARCHITECTURE_DIAGRAMS.md`, `Docs/FACADE_PATTERN_SUMMARY.md` |
| A past architectural decision + its alternatives | `Docs/Architecture/ADR/` — add a new ADR here if you change architectural behavior, per `CONTRIBUTING.md` |
| Consumer-facing troubleshooting (GATT 133, permissions, scanning, notifications, MTU, platform quirks) | `Docs/Troubleshooting/Common-Issues.md`, `Docs/Troubleshooting/Debugging.md` |
| Getting started / permissions / platform setup | `Docs/Getting-Started/` |
| Options records (Scanning/Connection/L2CAP/Broadcasting/Infrastructure) | `Docs/Configuration/` |
| Commit format, PR Definition of Done | `CONTRIBUTING.md`, `Docs/Best-Practices/Contribution-DoD.md` |

Quick sanity checks before relying on any doc's project/API list (things move):

```bash
find . -maxdepth 1 -type d -iname "Bluetooth.*"                       # current project list
grep -rn "protected abstract.*Native" Bluetooth.Core.Scanning/ Bluetooth.Core.Broadcasting/  # current Native* surface
```

## Native-callback bug-class watchlist

Not in `Docs/Troubleshooting/` (that's consumer-facing usage issues) — this is a distinct
class of bug in the platform-native callback plumbing itself. All of the following were silent,
**permanent hangs with no thrown exception**, and all were found only by running real DFU
firmware installs against physical hardware (Little Anne Mk1 on Android, real iOS device) —
simulators/emulators didn't reproduce any of them. When writing or reviewing `Native*`
callback code (`OnConnectionStateChange`, `OnDescriptorWrite`, TCS-completion paths, etc.),
check whether the change could reintroduce one of these shapes:

| Pattern | Real example | Commit |
|---|---|---|
| Permission/capability field not populated for this discovery path, causing false-negative gating | Android `NativeCanRead`/`NativeCanWrite` gated on `Permissions`, which Android never populates for a client-discovered descriptor — blocked every CCCD op before it was attempted | `670193a` |
| A completion path has two ways to be satisfied (state machine vs. direct call), and only one is wired | CCCD write via `WriteValueAsync()` directly (not through `StartListeningAsync`) never resolved its own pending write | `670193a`, guard fix in `372179b` |
| Platform stop/start API has no completion callback, so an internal flag the code waits on forever never flips | `AndroidBluetoothScanner.NativeStopAsync` never set `IsRunning = false` — `stopScan()` has no callback | `670193a` |
| A callback's status/reason code is treated as one thing (failure) when it can also mean another (device-initiated event) | `OnConnectionStateChange` treated any non-Success as connect-failure and returned early, so a device-initiated disconnect (e.g. DFU reboot) left `IsConnected` permanently stale, then a follow-up `DisconnectAsync()` hung waiting for a callback Android will never fire again | `670193a` |
| A cache exists at a layer below the one being bypassed | `ExploreServicesAsync(UseCache=false)` bypassed only this library's cache — Android's own GATT-stack-level cache (a rebooted DFU bootloader's stale pre-reboot services) still won this. Fix wires the already-present but dead `TryGattRefresh()` through | `23ad5f0` |
| Edge-triggered wait misses state that changed between "caller acted" and "waiter subscribed" | `WaitForDeviceToAppearAsync` only listened to `DevicesAdded` — a fast-reappearing DFU bootloader already in the registry by subscribe-time ran the full timeout regardless | `f3f397a` |
| A wrapper/substituted type breaks a direct cast to the platform-native delegate interface | `GetDevice(CBPeripheral)` cast straight to `ICbPeripheralDelegate`, which any `DeviceWrapper`-substituted custom device type never is | `ada1698` |
| Exception constructor itself throws on the common case | `AppleNativeBluetoothException` did `string.Join(";", nsError.LocalizedRecoveryOptions)` — null for most `NSError`s, so constructing the exception threw instead of the real error | `ada1698` |
| A suppression flag is checked on the wrong downstream branch | `IgnoreNextUnexpectedDisconnection` didn't gate the ERROR-level disconnect log, only a separate event further downstream — every expected DFU-reboot disconnect logged as an error anyway | `ada1698` |

Open, not yet fixed — check current status before assuming otherwise:
- OnePlus native `ScanFilter` drops every device with a scan response unless `0xFF` is in the device-type-id allow-list (`laerdal/Plugin.Bluetooth#49`).

If this table grows much further, promote it into `Docs/Troubleshooting/` (or its own ADR)
rather than letting this skill become a second copy of that folder.
