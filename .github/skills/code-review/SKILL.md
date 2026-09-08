---
name: code-review
description: Use for reviewing pull requests on Plugin.Bluetooth against this repository's Contribution Definition of Done and its history of platform-callback bugs. Applies to any PR touching Bluetooth.Abstractions, Bluetooth.Maui, or the platform implementations.
---

# Code Review — Plugin.Bluetooth

The canonical checklist for this repo is already written down at
[`Docs/Best-Practices/Contribution-DoD.md`](../../../Docs/Best-Practices/Contribution-DoD.md) —
apply it in full rather than a shorter summary. It covers layering (Abstractions/Core/Platform/
Facade), XML doc / CS1591 requirements, exception types, `CancellationToken` exposure, platform
matrix impact, EventId/logging conventions, DI/composition effects, and docs-only fast path.

## Bug classes to actively look for

This repo has a real history of the following bug shapes — flag anything that looks like a
repeat, even if the DoD checklist above doesn't spell it out line by line:

- **Native-callback identity/lifetime bugs**: a native BLE callback capturing the wrong device
  identity, firing after the wrapping object was disposed, or completing twice through two
  different code paths (e.g. a CCCD write success callback *and* a timeout both resolving the
  same awaiter).
- **Stale state after disconnect**: `IsConnected`- or similar cached-state properties not being
  invalidated on a disconnect event, so a reconnect attempt reads stale state.
- **Registry/race conditions in device discovery**: methods like `WaitForDeviceToAppearAsync`
  racing against the scanner's own device registry updates.
- **Scan filter allow-lists that are too narrow**: a device-type filter that silently drops
  advertisements missing a byte the filter didn't account for (this class of bug has broken
  detection on specific Android OEM devices before — check any new/changed scan filter against
  the full advertisement payload shape, not just the happy path).
- **Windows scan-response merging**: PDU merging logic for Windows advertisements is subtle —
  changes here need to be checked against both a bare advertisement and one with a separate
  scan-response PDU.
- **Platform parity drift**: a fix or feature landing on one platform (Android/iOS/macOS/Windows)
  without the equivalent handling — or an explicit documented gap — on the others.

## What not to flag

- Missing automated tests — this repo has no unit/integration test infrastructure yet (see
  `Docs/Best-Practices/Testing.md`); manual platform validation is the current bar, per the DoD.
- Style nits already covered by `.editorconfig`/analyzers — focus review comments on behavior,
  layering, and the bug classes above.
