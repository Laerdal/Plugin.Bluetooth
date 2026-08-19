# ADR 0003: Windows Scan-Response Handling

- Status: Proposed
- Date: 2026-08-19
- Decision Makers: François Raminosona (Software Architect)
- Supersedes: `Laerdal/Plugin.Bluetooth` PR #47 (originally authored by David Rosenbusch, ownership
  transferred after the abstraction-level approach was rejected — see Alternatives Considered)

## Context

`IBluetoothAdvertisement` is documented as a cross-platform-normalized snapshot: one event per
discovered device per advertising interval, with a single consistent `ManufacturerData` byte
layout. Android and iOS already deliver this correctly, but not because their advertisement
protocol is simpler — because the OS does the merging for you:

- **Android**: `AndroidBluetoothAdvertisement.ExtractManufacturerData` groups the raw scan
  record's parts by manufacturer ID and concatenates all parts sharing that ID (ADV + SCAN_RSP)
  into one byte array before Plugin.Bluetooth ever sees it.
- **iOS**: CoreBluetooth does the equivalent natively. Per Laerdal's own firmware documentation
  ([BLE Advertising In Laerdal Products - 00046845](https://laerdal.atlassian.net/wiki/spaces/BLE/pages/30572556),
  "Scan response" section, updated at revision C.2 in 2020 specifically for this): "in iOS, the
  scan response data may be appended directly to the normal advertisement data (without any way
  to separate the two)."
- **Windows**: `BluetoothLEAdvertisementWatcher.Received` fires once per PDU. A device that sends
  a scannable primary ADV and a SCAN_RSP produces **two separate native events**, each carrying
  only what that PDU physically contains. Nothing coalesces them.
  `WindowsBluetoothAdvertisement.ExtractManufacturerData` already special-cases this
  (`ConnectableUndirected` → treat as ADV's own manufacturer data; `ScanResponse` → treat as the
  scan response's; everything else → empty) but returns only whichever half arrived, and
  `BaseBluetoothRemoteDevice.OnAdvertisementReceived` overwrites `LastAdvertisement` wholesale —
  no field-level merge. Whichever PDU arrives last wins; the other's fields are silently lost
  until the next interval.

The Laerdal firmware doc confirms the byte layout this needs to converge on. Normal case:
`[CompanyId(2)] [DeviceId] [Status]` from the ADV, and separately `[CompanyId(2)] [0xFF
scan-response-prefix] [scan response TLV payload]` from the SCAN_RSP. iOS's forced concatenation
produces exactly `[CompanyId(2)] [DeviceId] [Status] [0xFF] [scan response payload]` — which is
also what Android's manual grouping-and-concatenation produces. Windows is the only platform not
producing this layout at all.

This gap is what PR #47 was written against: Windows advertisements missing manufacturer data
get dropped by advertisement filters, even from devices the filter is meant to match. David's fix
(after the abstraction-level `CachedManufacturer` approach was rejected — see below) caches only
the 2-byte `Manufacturer` enum on the device and backfills it into events that lack it. That
unblocks filtering but leaves `ManufacturerData` itself fragmented — any consumer reading the full
byte layout (e.g. `Laerdal.Bluetooth`'s `LaerdalAdvertisementExtensions.GetLittleFirmwareVersionOrDefault`,
which needs device-type/status/sub-type/timestamp/version bytes) still silently gets `null` on
whichever Windows event happened to be the ADV-only half.

## Decision

Add Windows-only, opt-in scan-response merging, scoped entirely to `Bluetooth.Maui.Platforms.Win`
and additive to the Windows concrete types — no changes to `IBluetoothAdvertisement` or
`IBluetoothRemoteDevice`.

### New API surface

- `Bluetooth.Abstractions.Scanning.Options.Windows.WindowsScanningOptions` (new record, sibling to
  the existing `WindowsConnectionOptions`), attached via a new `ScanningOptions.Windows { get; init; }`
  (`object?`, mirroring the existing `Android` property):
  - `MergeScanResponses` (`bool`, default `false`)
  - `ScanResponseMergeWindow` (`TimeSpan`, default `500ms`, confirmed as the starting point — the
    firmware doc notes the central requests a scan response "most likely only... once", so it
    should land within the same or very next advertising interval; adjust once tested against
    real Laerdal hardware)
- `WindowsBluetoothRemoteDevice.ScanResponseReceived` (event) and `.LastScanResponse` (property):
  new members on the concrete Windows device type, following the same placement precedent as
  `BluetoothLeDeviceProxy`/`GattSessionProxy`/`GattSessionStatus` already on that class. Suggest a
  new partial file, `WindowsBluetoothRemoteDevice.ScanResponse.cs`, mirroring how
  `BaseBluetoothRemoteDevice` splits concerns into `.Advertisement.cs`, `.Connection.cs`, etc.

### Behavior

**Both modes share one rule**: a device is never created from a SCAN_RSP alone. A scan-response
PDU is only ever dispatched once a device already exists in the scanner's device list for that
`BluetoothAddress` (same address-lookup pattern PR #47 already established in
`WindowsBluetoothScanner.OnAdvertisementReceived`, resolved through `UnderlyingPlatformDevice`
before casting to `WindowsBluetoothRemoteDevice`, consistent with how `AppleBluetoothScanner.GetDevice`
already has to unwrap `DeviceWrapper`-substituted subtypes). If no device exists yet, the SCAN_RSP
is dropped silently, exactly as it is today.

**`MergeScanResponses = false` (default)**:
- An ADV PDU is dispatched immediately through the existing path — no buffering, no added
  latency, no side state. This may create the device if it's the first sighting.
- A SCAN_RSP PDU is looked up against the device list. If the device exists, its
  `LastScanResponse` is set and `ScanResponseReceived` fires on the device. The primary
  `AdvertisementReceived`/`LastAdvertisement`/`ManufacturerData` pipeline is untouched — Windows
  keeps today's ADV-only `ManufacturerData` on that side.
- Chosen over defaulting to merge-on because it doesn't need a pending-advertisement side buffer:
  it stays entirely inside the existing "advertisement → device creation/update" flow, using the
  device list itself as the only piece of state to correlate against. Trade-off (deliberately
  accepted): a consumer reading `IBluetoothAdvertisement.ManufacturerData` in a platform-agnostic
  way gets less on Windows by default than on Android/iOS, unless it also branches on
  `WindowsBluetoothRemoteDevice.LastScanResponse`.

**`MergeScanResponses = true`**:
- Every ADV PDU is held in a short-lived per-address pending buffer instead of dispatched
  immediately, exactly as it is on every interval — not just first sighting — to genuinely mirror
  Android/iOS's per-interval merged behavior.
- The buffered advertisement resolves either when a matching SCAN_RSP arrives (matched by
  `BluetoothAddress`) or when `ScanResponseMergeWindow` elapses, whichever is first.
- Once resolved, it follows the **same path as the other platforms**: filter → `AdvertisementReceived`
  → device creation/update, with `ManufacturerData` merged per the byte layout above:
  `[CompanyId(2, from whichever PDU had it)] + [ADV's manufacturer-data payload, if any] +
  [SCAN_RSP's manufacturer-data payload, if any]`. Other fields (`DeviceName`, `ServicesGuids`,
  `IsConnectable`, `TransmitPowerLevelInDBm`) take the ADV PDU's values, falling back to the
  SCAN_RSP's only if the ADV's were empty/default — the firmware doc states advertising data
  carries manufacturing data + name, and scan response carries manufacturing data + UUID only, so
  ADV is expected to be authoritative for those fields.
- On timeout with no SCAN_RSP received (e.g. a non-scannable device), the advertisement fires
  **as-is**, ADV-only — it is never held indefinitely or dropped.
- If a SCAN_RSP genuinely arrived (not just a timeout), `ScanResponseReceived`/`LastScanResponse`
  also fire on the device immediately afterward, same as the non-merge path — so power users get
  both the merged, cross-platform-consistent advertisement *and* the raw scan-response payload if
  they want to inspect it distinctly.
- A SCAN_RSP that arrives late (after its buffer already timed out and dispatched) falls through
  to the same "device already exists → dispatch `ScanResponseReceived`" path used when merging is
  off. This means there is really only one additional mechanism here — the pending buffer with its
  timeout — layered on top of behavior both modes otherwise share.

### Documentation

Given this is the one place Windows behaves differently from Android/iOS by design (not by gap),
both `Docs/Core-Concepts/Advertisement.md` and `Docs/Platforms/Windows.md` must explicitly document:
default behavior, what `MergeScanResponses`/`ScanResponseMergeWindow` change, that a device is
never created from a scan response alone, and that a merge-mode timeout fires the advertisement
as-is rather than dropping or blocking it.

## Alternatives Considered

### Alternative A: Split Android/iOS to match Windows (expose ADV vs SCAN_RSP everywhere)

- Summary: Give every platform a `ScanResponseReceived` distinction, making Windows's PDU-level
  granularity the cross-platform norm instead of an exception.
- Pros: Faithful to Windows's native model; no correlation/buffering logic needed anywhere.
- Cons: Android and iOS have **no API surface at all** for this distinction — there's no
  callback, no flag, nothing to split on. Implementing it there means fabricating a distinction
  the OS never gives you, less reliably than Option 1's bounded-wait correlation on Windows. It
  also pushes the merge burden onto every consumer instead of centralizing it in one platform
  folder, and is a breaking change to code (`Laerdal.Bluetooth`, `fds-mobile-app`) actively being
  built out this quarter.

### Alternative B: `CachedManufacturer` on the shared abstraction (original PR #47 approach)

- Summary: Add a `CachedManufacturer` member to `IBluetoothAdvertisement` itself, with
  Android/iOS stubbing `null`.
- Pros: Simple, minimal diff.
- Cons: Leaks a Windows-only PDU-plumbing workaround into the cross-platform contract every other
  platform has to carry as dead weight. Rejected directly (2026-08-17) in favor of keeping it
  Windows-only; David's rework already moved it off the abstraction before handing the PR over.
  Superseded here regardless, since it only ever patched the 2-byte manufacturer enum, not the
  full `ManufacturerData` byte layout.

### Alternative C: Merge unconditionally on Windows, no opt-out

- Summary: Always buffer and merge on Windows, matching Android/iOS with no `MergeScanResponses`
  flag at all.
- Pros: Zero platform leakage for any consumer reading `IBluetoothAdvertisement` alone; strongest
  cross-platform parity.
- Cons: Forces a pending-advertisement side buffer (and its added, if bounded, latency) onto every
  Windows consumer with no escape hatch, even ones that don't care about scan-response content and
  would prefer today's immediate-dispatch behavior. Rejected in favor of an opt-in default-off
  flag specifically to preserve the simpler, buffer-free path as the default.

## Consequences

### Positive

- Windows can now produce the same `ManufacturerData` byte layout as Android/iOS when
  `MergeScanResponses = true`, matching what the firmware doc and existing `Laerdal.Bluetooth`
  parsing code already assume.
- No changes to `IBluetoothAdvertisement` or `IBluetoothRemoteDevice` — every addition is
  Windows-only and additive, consistent with how `CachedManufacturer` was already redirected.
- Default behavior is unchanged and non-breaking for existing consumers; the new behavior is
  entirely opt-in.
- Makes PR #47 unnecessary as a standalone fix — `ScanResponseReceived`/`LastScanResponse` covers
  its use case more generally, and with the full byte layout rather than just the manufacturer ID.

### Negative

- With the default (`MergeScanResponses = false`), a consumer writing platform-agnostic code
  against `IBluetoothAdvertisement.ManufacturerData` alone still gets less on Windows than on
  Android/iOS unless it explicitly also reads `WindowsBluetoothRemoteDevice.LastScanResponse`.
  Accepted trade-off, not a defect — see Decision.
- Merge mode adds real state (a per-address pending-advertisement buffer with a timeout) and
  bounded latency that Android/iOS don't need, confined to `Bluetooth.Maui.Platforms.Win`.
- Two Windows-specific scanning-option surfaces now exist (`WindowsScanningOptions` here,
  `WindowsConnectionOptions` already) — acceptable, matches the Android precedent
  (`AndroidScanningOptions`/`AndroidConnectionOptions`) exactly.

### Neutral

- `ScanResponseMergeWindow`'s 500ms default is a confirmed starting point, not yet validated
  against real hardware — expected to be tuned once tested, not a blocker to `Accepted`.

## Follow-up Actions

Tracked in detail in **SC-3208** (Jira) — the ADR is the decision record, SC-3208 is the work
breakdown. Summary:

- [ ] Implement `WindowsScanningOptions` (`MergeScanResponses`, `ScanResponseMergeWindow`) and wire
      `ScanningOptions.Windows` through `WindowsBluetoothScanner`.
- [ ] Implement the per-address pending-advertisement buffer and timeout in
      `WindowsBluetoothScanner`/`BluetoothLeAdvertisementWatcherWrapper`.
- [ ] Add `ScanResponseReceived`/`LastScanResponse` to `WindowsBluetoothRemoteDevice` (new
      `WindowsBluetoothRemoteDevice.ScanResponse.cs` partial file).
- [ ] Update `Docs/Core-Concepts/Advertisement.md` and `Docs/Platforms/Windows.md` per the
      Documentation section above.
- [ ] Validate `ScanResponseMergeWindow`'s default against real hardware; adjust if needed.
- [x] PR #47 closed 2026-08-19 as superseded by this ADR (see PR comment); no branch repurposed —
      implementation starts fresh from `main`.
- [x] **In scope**: `ScanningOptions.ScanMode`, `.RssiThreshold`, and `.EnableExtendedAdvertising`
      already document Windows support ("Mapped to sampling interval and signal strength filter
      settings", "Full support via BluetoothLEAdvertisementWatcher.SignalStrengthFilter",
      "Supported on Windows 10 version 2004+") that `WindowsBluetoothScanner.NativeStartAsync`
      does not actually implement — it only calls `Watcher.BluetoothLeAdvertisementWatcher.Start()`
      with no configuration, even though `BluetoothLeAdvertisementWatcherWrapper` already exposes
      `ScanningMode`, `SignalStrengthFilter*`, and `AllowExtendedAdvertisements` as read-only
      observables. Confirmed as part of this same body of work (Windows support in this plugin
      hasn't been fully exercised yet and is likely under-implemented elsewhere too) — not spun
      off separately. Wire these through, and add to `WindowsScanningOptions` while in there:
      an explicit `ScanningMode` (Active/Passive) escape hatch, direct `SignalStrengthFilter`
      knobs (`OutOfRangeThresholdInDBm`, `SamplingInterval`, `OutOfRangeTimeout` — `RssiThreshold`
      only ever covers `InRangeThresholdInDBm`), and an `AllowExtendedAdvertisements` escape hatch.
- [x] **In scope, confirmed against the current Microsoft Learn reference (2026-08-19)** — further
      native `BluetoothLEAdvertisementWatcher`/`BluetoothLEAdvertisementFilter` capabilities with
      no equivalent anywhere in this codebase today:
  - `AdvertisementFilter` (`BluetoothLEAdvertisementFilter`) — native payload-section-based
    filtering via `.Advertisement` (ServiceUuids/ManufacturerData/LocalName) and `.BytePatterns`
    (offset-based raw byte pattern matching). Today `ScanningOptions.ServiceUuids` is filtered
    **in software after** the advertisement is received on Windows (per the existing doc comment)
    — this is a real opportunity to move to native/radio-level filtering instead, which would also
    reduce the volume of PDUs the new merge-buffer has to handle.
  - `UseHardwareFilter`, `UseCodedPhy`, `UseUncoded1MPhy`, `ScanParameters` — newer watcher
    properties not yet described in the summary reference table; each needs its own docs lookup
    during implementation before deciding whether/how to expose via `WindowsScanningOptions`.

## References

- Code references:
  - `Bluetooth.Maui.Platforms.Win/Scanning/WindowsBluetoothAdvertisement.cs`
  - `Bluetooth.Maui.Platforms.Win/Scanning/WindowsBluetoothScanner.cs`
  - `Bluetooth.Maui.Platforms.Win/Scanning/NativeObjects/BluetoothLeAdvertisementWatcherWrapper.cs`
  - `Bluetooth.Maui.Platforms.Win/Scanning/WindowsBluetoothRemoteDevice.cs`
  - `Bluetooth.Core.Scanning/BaseBluetoothRemoteDevice.Advertisement.cs`
  - `Bluetooth.Abstractions.Scanning/Options/ScanningOptions.cs`
  - `Bluetooth.Abstractions.Scanning/Options/Android/AndroidScanningOptions.cs` (naming/shape precedent)
  - `Bluetooth.Abstractions.Scanning/Options/Windows/WindowsConnectionOptions.cs` (sibling)
  - `Laerdal.Bluetooth/LaerdalAdvertisementExtensions.cs`, `Laerdal.Bluetooth/Constants/LaerdalManufacturerDataType.cs`
- Related docs:
  - `Docs/Core-Concepts/Advertisement.md`
  - `Docs/Platforms/Windows.md`
  - Confluence: [BLE Advertising In Laerdal Products - 00046845](https://laerdal.atlassian.net/wiki/spaces/BLE/pages/30572556)
- Related PRs/issues:
  - `Laerdal/Plugin.Bluetooth#44` ("Fix: Enable reading ScanResponses for ManufacturerData")
  - `Laerdal/Plugin.Bluetooth#47` ("Add manufacturer cache for Windows Scanner") — closed
    2026-08-19, superseded by this ADR
  - Jira **SC-3208** — implementation work breakdown for this ADR
