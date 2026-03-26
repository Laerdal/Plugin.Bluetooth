# Characteristic Access Service Migration Plan (Archive -> Current Library)

## 0. Agent Context (Read First)

This document is a **complete, pre-analyzed handoff**. The archive has already been read and audited. Do not re-read archive files unless you need a codec detail. Everything a coding agent needs is captured below.

**Project state**: pre-release, developing straight on branch `framinosona/WIP`. Breaking changes are fine. No backward compatibility required. No PR workflow — structure work as logical commits.

**Branch**: `framinosona/WIP` (default: `main`)

**Validation command** (run after every commit):
```bash
dotnet build Bluetooth.Maui.Sample.Scanner/Bluetooth.Maui.Sample.Scanner.csproj -f net10.0-android
```

---

## 1. Purpose

Reintroduce typed characteristic access (read/write/listen) for known Bluetooth profiles, aligned to the current `IBluetoothRemote*` abstraction layer.

---

## 2. Archive Audit Summary (Do Not Re-Read Archives)

The archive under `Archives/Scanning/` has been fully analyzed. Key findings:

### What to reuse (pattern/concept only, not code)
- Typed conversion boundary: `FromBytes(ReadOnlyMemory<byte>)` → `TRead`, `ToBytes(TWrite)` → `ReadOnlyMemory<byte>`.
- Factory helpers for primitive/enum/string codecs — cheap to rebuild, pattern is sound.
- Structured conversion exceptions with target type and source bytes.
- Characteristic-level `ValueUpdated` event wiring (archive uses `+=/-=` on subscribe/unsubscribe correctly).
- `SemaphoreSlim`-guarded `UpdateListeningStateAsync` to avoid concurrent start/stop races.

### What NOT to port (bugs / bad patterns)
| Archive flaw | Location | Fix |
|---|---|---|
| `CanReadAsync`/`CanWriteAsync`/`CanListenAsync` silently catch all exceptions, return false | `CharacteristicAccessService.Read.cs:18`, `Write.cs:18`, `Listen.cs:22` | Let exceptions propagate; callers must handle |
| `OnNotificationReceived` returns `bool` to control keep/remove, but the dispatch loop ignores that bool entirely | `IBluetoothCharacteristicAccessService.Listen.cs:12`, `CharacteristicAccessService.Listen.cs:209` | Use `Action<T>` listener; no bool return |
| Registry keys accessors only by characteristic UUID — ambiguous when same UUID is used in multiple services | `CharacteristicAccessServicesRepository.cs:70` | Key by `(ServiceId, CharacteristicId)` |
| `SetServiceInformation` mutates shared accessor state post-construction | `IBluetoothCharacteristicAccessService.cs:23`, `CharacteristicAccessServicesRepository.cs:146` | Definitions are immutable records, service info bound at construction |
| `UnknownCharacteristicAccessService` returned as fallback from registry, masking missing wiring | `CharacteristicAccessServicesRepository.cs:291` | Return `null`/throw on missing; no silent fallback |
| Registry dictionaries (`ServiceNames`, `CharacteristicsAccessServices`) are non-concurrent and unguarded | `CharacteristicAccessServicesRepository.cs:65,70` | Use `ConcurrentDictionary` |
| Battery Level characteristic typed as `sbyte` (SIG spec is `uint8`, values >127 misrepresent) | `BatteryServiceDefinition.cs:23` | Use `byte` |
| Sentinel string checks (`== "Unknown Characteristic"`) as validation guards | `CharacteristicAccessServicesRepository.cs:193,203` | Use null/empty guards |
| Reflection + `Assembly.GetCallingAssembly()` as primary registration mechanism (AOT-hostile) | `CharacteristicAccessServicesRepository.cs:78` | Explicit DI registration first; reflection optional/disabled by default |
| `CanListenAsync` probes characteristic availability eagerly; listen logic keyed at device scope not characteristic scope | `CharacteristicAccessService.Listen.cs:75,166` | Track subscriptions per characteristic instance |
| Notification handler reads from `characteristic.Value` (stale) instead of event args payload | `CharacteristicAccessService.Listen.cs:206` | Use `args` from `ValueUpdatedEventArgs` directly |

---

## 3. Current Codebase Wiring Facts (Pre-Verified)

These facts were confirmed by reading the live source and must not be re-verified:

### Name provider wiring gap (MUST FIX in Commit 1)
The base spec-based constructors in both base service and base characteristic explicitly pass `null` for the name provider, so platform factories currently never propagate names — even if a name provider is registered in DI:
- `BaseBluetoothRemoteService.cs:51`: `this(parentDevice, spec.ServiceId, null, logger)` — `null` is hardcoded
- `BaseBluetoothRemoteCharacteristic.cs:57`: `this(parentService, spec.CharacteristicId, null, logger)` — `null` is hardcoded

Platform factories (`AndroidBluetoothServiceFactory`, `AppleBluetoothRemoteServiceFactory`, `WindowsBluetoothServiceFactory`) do not accept or inject a name provider. This chain must be fixed so `IBluetoothNameProvider` actually reaches instantiated objects.

Fix path: add `IBluetoothNameProvider?` to spec-based base constructors and to all three platform service/characteristic factories.

### `IBluetoothNameProvider` resolves only by UUID, not by service+characteristic pair
Current interface:
```csharp
string? GetKnownCharacteristicName(Guid characteristic);  // no ServiceId context
```
This is ambiguous for the same characteristic UUID used in multiple services. The interface must be extended (or replaced, since this is pre-release) to support pair-based lookup.

### Factory spec constructors are `null`-passing for Apple/DotNetCore too
`AppleBluetoothRemoteService` has a separate non-spec constructor that does accept a name provider, but the factory-spec constructor does not. Same gap applies to all platforms.

### DI registration is minimal
`Bluetooth.Core.Scanning/ServiceCollectionExtensions.cs` currently only registers `IBluetoothRssiToSignalStrengthConverter`. All profile registry + name provider registrations go here.

### Platform scanning service DI files to update
- `Bluetooth.Maui.Platforms.Droid/Scanning/ServiceCollectionExtensions.cs`
- `Bluetooth.Maui.Platforms.Apple/Scanning/ServiceCollectionExtensions.cs`
- `Bluetooth.Maui.Platforms.Win/Scanning/ServiceCollectionExtensions.cs`

### No test project currently exists in this repository.

---

## 4. Committed Decisions (No Longer Open)

| Decision | Choice |
|---|---|
| Listener callback shape | `Action<T>` — no bool return |
| Reflection registration | Disabled by default; explicit DI first |
| Package split for SIG profiles | In-repo module/folder inside `Bluetooth.Core.Scanning` first; extract to separate package later if needed |
| Backward compatibility | None required (pre-release) |
| SIG profile in first commit | Yes — Battery Service as proof of workflow, typed as `byte` |
| Delivery model | Commits on `framinosona/WIP`, no PR workflow |

---

## 5. Target Architecture

### Namespace placement
- `Bluetooth.Abstractions.Scanning.Profiles` — contracts (interfaces, definition records, codec interface)
- `Bluetooth.Core.Scanning.Profiles` — implementations (registry, accessor, codecs)
- `Bluetooth.Core.Scanning.Profiles.BluetoothSig` — SIG-specific profile definitions

### Core types

```
IBluetoothProfileRegistry
    Register(BluetoothServiceDefinition)
    Register(BluetoothCharacteristicDefinition)
    GetServiceName(Guid serviceId) → string?
    GetCharacteristicName(Guid serviceId, Guid characteristicId) → string?  // pair-keyed
    GetCharacteristicName(Guid characteristicId) → string?                  // fallback, best-effort

BluetoothServiceDefinition (record)
    Guid ServiceId
    string ServiceName

BluetoothCharacteristicDefinition (record)
    Guid ServiceId
    Guid CharacteristicId
    string CharacteristicName

ICharacteristicCodec<TRead, TWrite>
    TRead FromBytes(ReadOnlyMemory<byte> bytes)
    ReadOnlyMemory<byte> ToBytes(TWrite value)

IBluetoothCharacteristicAccessor  (base, untyped)
    Guid ServiceId
    Guid CharacteristicId
    string ServiceName
    string CharacteristicName
    ValueTask<IBluetoothRemoteCharacteristic> ResolveCharacteristicAsync(
        IBluetoothRemoteDevice device,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)

IBluetoothCharacteristicAccessor<TRead, TWrite> : IBluetoothCharacteristicAccessor
    bool CanRead(IBluetoothRemoteDevice device)    // sync — characteristic already resolved
    bool CanWrite(IBluetoothRemoteDevice device)
    bool CanListen(IBluetoothRemoteDevice device)
    ValueTask<TRead> ReadAsync(IBluetoothRemoteDevice device, bool skipIfPreviouslyRead = false,
        TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    ValueTask WriteAsync(IBluetoothRemoteDevice device, TWrite value,
        bool skipIfOldValueMatchesNewValue = false,
        TimeSpan? timeout = null, CancellationToken cancellationToken = default)

IBluetoothCharacteristicAccessorListener<TRead> : IBluetoothCharacteristicAccessor  (Commit 2)
    ValueTask SubscribeAsync(IBluetoothRemoteDevice device, Action<TRead> listener,
        TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    ValueTask UnsubscribeAsync(IBluetoothRemoteDevice device, Action<TRead> listener,
        TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    ValueTask UnsubscribeAllAsync(IBluetoothRemoteDevice device,
        TimeSpan? timeout = null, CancellationToken cancellationToken = default)

BluetoothProfileRegistry : IBluetoothProfileRegistry
    ConcurrentDictionary<Guid, BluetoothServiceDefinition> _services
    ConcurrentDictionary<(Guid serviceId, Guid characteristicId), BluetoothCharacteristicDefinition> _characteristics

ProfileNameProvider : IBluetoothNameProvider
    Backed by IBluetoothProfileRegistry
    GetKnownCharacteristicName(Guid) → best-effort single-match or null if ambiguous

CharacteristicAccessor<TRead, TWrite> : IBluetoothCharacteristicAccessor<TRead, TWrite>
    Resolves via current IBluetoothRemoteDevice exploration APIs
    Uses ICharacteristicCodec<TRead, TWrite> for conversion
```

### Exploration flow inside accessor `ResolveCharacteristicAsync`
1. If service not yet explored: call `device.ExploreServicesAsync(ServiceExplorationOptions.WithCharacteristics, timeout, ct)`
2. Get service via `device.GetService(ServiceId)`
3. If characteristics not yet explored on that service: call `service.ExploreCharacteristicsAsync(null, timeout, ct)`
4. Return `service.GetCharacteristic(CharacteristicId)`

Do **not** force re-exploration if already cached (respect `UseCache = true` default).

### Exception hierarchy additions
Under `Bluetooth.Abstractions.Scanning/Exceptions`:
- `CharacteristicAccessorException : BluetoothScanningException` (base for this feature)
  - `CharacteristicAccessorResolutionException : CharacteristicAccessorException` (service/characteristic not found during resolution)
  - `CharacteristicCodecException : CharacteristicAccessorException` (conversion failure; includes target type and raw bytes)

---

## 6. Commit Breakdown

### Commit 1 — `feat (profiles): registry, name provider, and factory wiring`

**Files to create:**
- `Bluetooth.Abstractions.Scanning/Profiles/IBluetoothProfileRegistry.cs`
- `Bluetooth.Abstractions.Scanning/Profiles/BluetoothServiceDefinition.cs`
- `Bluetooth.Abstractions.Scanning/Profiles/BluetoothCharacteristicDefinition.cs`
- `Bluetooth.Abstractions.Scanning/Exceptions/CharacteristicAccessorException.cs`
- `Bluetooth.Core.Scanning/Profiles/BluetoothProfileRegistry.cs`
- `Bluetooth.Core.Scanning/Profiles/ProfileNameProvider.cs`

**Files to modify:**
- `Bluetooth.Abstractions.Scanning/IBluetoothNameProvider.cs` — add `string? GetKnownCharacteristicName(Guid serviceId, Guid characteristicId)` overload
- `Bluetooth.Core.Scanning/BaseBluetoothRemoteService.cs:51` — pass name provider through spec-based constructor instead of `null`; requires adding `IBluetoothNameProvider?` parameter
- `Bluetooth.Core.Scanning/BaseBluetoothRemoteCharacteristic.cs:57` — same fix
- `Bluetooth.Maui.Platforms.Droid/Scanning/Factories/AndroidBluetoothServiceFactory.cs` — inject and forward `IBluetoothNameProvider?`
- `Bluetooth.Maui.Platforms.Droid/Scanning/Factories/AndroidBluetoothRemoteCharacteristicFactory.cs` — same
- `Bluetooth.Maui.Platforms.Apple/Scanning/Factories/AppleBluetoothRemoteServiceFactory.cs` — same
- `Bluetooth.Maui.Platforms.Apple/Scanning/Factories/AppleBluetoothRemoteCharacteristicFactory.cs` — same
- `Bluetooth.Maui.Platforms.Win/Scanning/Factories/WindowsBluetoothServiceFactory.cs` — same
- `Bluetooth.Maui.Platforms.Win/Scanning/Factories/WindowsBluetoothRemoteCharacteristicFactory.cs` — same
- `Bluetooth.Core.Scanning/ServiceCollectionExtensions.cs` — register `IBluetoothProfileRegistry` (singleton `BluetoothProfileRegistry`) + `IBluetoothNameProvider` (singleton `ProfileNameProvider`) as defaults

**Acceptance check**: Instantiated `AndroidBluetoothRemoteService` and `AppleBluetoothRemoteService` objects receive known names when the registry is populated. Build passes.

---

### Commit 2 — `feat (profiles): typed accessor, codecs, and resolution exceptions`

**Files to create:**
- `Bluetooth.Abstractions.Scanning/Profiles/ICharacteristicCodec.cs`
- `Bluetooth.Abstractions.Scanning/Profiles/IBluetoothCharacteristicAccessor.cs`
- `Bluetooth.Abstractions.Scanning/Exceptions/CharacteristicAccessorResolutionException.cs`
- `Bluetooth.Abstractions.Scanning/Exceptions/CharacteristicCodecException.cs`
- `Bluetooth.Core.Scanning/Profiles/CharacteristicAccessor.cs` (implements `IBluetoothCharacteristicAccessor<TRead, TWrite>`)
- `Bluetooth.Core.Scanning/Profiles/CharacteristicCodecFactory.cs` (static helpers: `ForByte`, `ForUInt16`, `ForUInt32`, `ForInt16`, `ForInt32`, `ForBool`, `ForString`, `ForEnum<TEnum>`, `ForBytes`)

**Files to modify:**
- `Bluetooth.Core.Scanning/ServiceCollectionExtensions.cs` — no extra registration needed; accessors are instantiated directly by profile definitions

**Acceptance check**: A manually constructed `CharacteristicAccessor<byte, byte>` using `CharacteristicCodecFactory.ForByte()` can call `ReadAsync` on a `FakeBluetoothRemoteDevice` (or mock). Build passes.

---

### Commit 3 — `feat (profiles): Battery SIG profile`

**Files to create:**
- `Bluetooth.Core.Scanning/Profiles/BluetoothSig/BluetoothSigConstants.cs` (standard UUID suffix: `-0000-1000-8000-00805f9b34fb`)
- `Bluetooth.Core.Scanning/Profiles/BluetoothSig/BatteryProfile.cs` (static class; registers Battery Service + BatteryLevel characteristic; BatteryLevel typed as `byte` not `sbyte`)

**Files to modify:**
- `Bluetooth.Core.Scanning/ServiceCollectionExtensions.cs` — call `BatteryProfile.Register(registry)` during default setup, or expose separate `AddBluetoothSigProfiles()` extension (preferred — keeps SIG optional)

**Acceptance check**: When registry is populated via `AddBluetoothSigProfiles()`, a service with UUID `0x180F` gets name "Battery Service" and its characteristic `0x2A19` gets name "Battery Level". Build passes.

---

### Commit 4 — `feat (profiles): listen orchestration`

**Files to create:**
- `Bluetooth.Abstractions.Scanning/Profiles/IBluetoothCharacteristicAccessorListener.cs`
- `Bluetooth.Core.Scanning/Profiles/CharacteristicAccessorListener.cs`
  - Tracks subscriptions per `(IBluetoothRemoteCharacteristic, Action<TRead>)` in a `ConcurrentDictionary`
  - Subscribe: add to map, wire `ValueUpdated` once, call `StartListeningAsync` if first subscriber
  - Unsubscribe: remove from map, call `StopListeningAsync` if last subscriber
  - Dispatch: read value from `ValueUpdatedEventArgs.Value` (not from `characteristic.Value` cache)
  - Dispose: `UnsubscribeAllAsync` + event unwire

**Acceptance check**: Two listeners on the same characteristic, unsubscribing one leaves the other active. Disposing removes all. Build passes.

---

## 7. File Plan (All Commits)

```
Bluetooth.Abstractions.Scanning/
  Profiles/
    IBluetoothProfileRegistry.cs
    BluetoothServiceDefinition.cs
    BluetoothCharacteristicDefinition.cs
    ICharacteristicCodec.cs
    IBluetoothCharacteristicAccessor.cs
    IBluetoothCharacteristicAccessorListener.cs    [Commit 4]
  Exceptions/
    CharacteristicAccessorException.cs
    CharacteristicAccessorResolutionException.cs   [Commit 2]
    CharacteristicCodecException.cs                [Commit 2]

Bluetooth.Core.Scanning/
  Profiles/
    BluetoothProfileRegistry.cs
    ProfileNameProvider.cs
    CharacteristicAccessor.cs                      [Commit 2]
    CharacteristicCodecFactory.cs                  [Commit 2]
    CharacteristicAccessorListener.cs              [Commit 4]
    BluetoothSig/
      BluetoothSigConstants.cs                     [Commit 3]
      BatteryProfile.cs                            [Commit 3]
  ServiceCollectionExtensions.cs                   [modified, all commits]
```

---

## 8. Agent Prompt (Copy-Paste to Start)

```text
Implement Commit 1 of Docs/CHARACTERISTIC_ACCESS_MIGRATION_PLAN.md.

Context:
- Pre-release project. Break things freely. No backward compatibility.
- Developing straight on branch framinosona/WIP.
- Read Section 0 and Section 3 of the plan carefully before touching any file.

Constraints:
1) Fix the name provider wiring gap in BaseBluetoothRemoteService and BaseBluetoothRemoteCharacteristic spec-based constructors (they currently hardcode null for name provider).
2) Add IBluetoothNameProvider? to all three platform service factories (Android, Apple, Windows) and their characteristic factories.
3) Create IBluetoothProfileRegistry + BluetoothServiceDefinition + BluetoothCharacteristicDefinition in Bluetooth.Abstractions.Scanning/Profiles/.
4) Create BluetoothProfileRegistry (ConcurrentDictionary, pair-keyed for characteristics) + ProfileNameProvider in Bluetooth.Core.Scanning/Profiles/.
5) Register both as singletons in Bluetooth.Core.Scanning/ServiceCollectionExtensions.cs.
6) Extend IBluetoothNameProvider with a GetKnownCharacteristicName(Guid serviceId, Guid characteristicId) overload.
7) Keep all XML documentation complete on all new/modified public/protected members.

After implementation, validate with:
dotnet build Bluetooth.Maui.Sample.Scanner/Bluetooth.Maui.Sample.Scanner.csproj -f net10.0-android
```

---

## 9. Validation Command

```bash
dotnet build Bluetooth.Maui.Sample.Scanner/Bluetooth.Maui.Sample.Scanner.csproj -f net10.0-android
```

---

## 10. Completion Definition

Feature is done when:

1. Known service/characteristic names appear automatically via `Name` properties on `IBluetoothRemoteService` / `IBluetoothRemoteCharacteristic`.
2. Typed read/write accessors work for all primitive codecs.
3. Listener orchestration correctly starts/stops platform notifications and fans out to multiple subscribers without leaks.
4. Battery Service SIG profile is registered and names resolve.
5. Build passes on Android target.

