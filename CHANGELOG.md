# Changelog

All notable changes to this project are documented here, newest first. The format follows
[Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

> **Versioning note**: tags jump from `v1.0.3` straight to `v4.0.0` — there are no `v2.x` or
> `v3.x` releases. `v4.0.0` is the architecture rewrite described below (layered
> Abstractions/Core/Platform/Facade design, full Android/iOS/Windows implementations,
> broadcasting, L2CAP), not a series of skipped point releases.

## [Unreleased]

## [4.0.16] - 2026-08-18

### Fixed

- Prevent stop-listening attempts on characteristics of an already-disconnected device
- Enable reading `ManufacturerData` from scan responses; display manufacturer info in the sample app's device details page

## [4.0.15] - 2026-08-18

### Changed

- Revived docfx generation and wired it into CI; enabled XML doc generation on all public API projects
- Replaced the hand-written API/options reference docs with pointers into the generated API reference, removing duplication
- Fixed pervasive drift between `Docs/` and the actual code (accuracy pass)

### Fixed

- Four real-hardware DFU hang bugs found during device testing: dead `TryGattRefresh()` never wired into `ExploreServicesAsync(UseCache=false)`, a CCCD write race guarded incorrectly, `WaitForDeviceToAppearAsync` missing devices already present at call time, and related Android listening-state cleanup
- Renormalized line endings repo-wide to match `.gitattributes`

### Dependencies

- `Microsoft.SourceLink.GitHub` → 10.0.400, `Microsoft.CodeAnalysis.NetAnalyzers` → 10.0.400

## [4.0.14] - 2026-08-11

### Fixed

- Sample app: added `TwoWay` bindings for the selected characteristic/service/device so navigating back and forth between pages deselects correctly
- Android: request Bluetooth connect permission correctly and fixed a threading issue in the sample app

## [4.0.13] - 2026-08-04

### Docs

- Added a Mermaid architecture diagram to the README
- Corrected a stale claim about a Windows broadcasting limitation

## [4.0.12] - 2026-08-03

### Fixed

- Android: only request the legacy `BLUETOOTH` permission below API 31; honor `CancellationToken` in permission-request methods

### Dependencies

- `actions/checkout` → v7, `actions/setup-dotnet` → v6, `Microsoft.CodeAnalysis.NetAnalyzers` → 10.0.302

## [4.0.11] - 2026-08-03

### Fixed

- Resolve `DeviceWrapper`-substituted devices back to their underlying platform delegate; fixed two related disconnect-path bugs

## [4.0.10] - 2026-08-03

### Added

- `IsSignalStrengthProbingEnabled` toggle on remote devices
- `CleanRestartScanningAsync()` on the scanner, to recover scanning cleanly after a DFU-triggered device reboot

### Fixed

- iOS: match peripherals by identifier only (previously also matched on other fields, causing mismatches)
- Windows: validate the advertisement type when constructing a device from an advertisement

## [4.0.9] - 2026-07-02

### Added

- `DeviceWrapper` hook to customize the device instances returned by the scanner
- Logging of discovered services during service exploration

### Fixed

- Log service IDs as strings consistently during service exploration

## [4.0.8] - 2026-06-29

### Fixed

- Apple: wait for the adapter's powered-on state before starting a scan (with timeout support); throw the correct exception when scanning fails to start

## [4.0.7] - 2026-06-29

### Fixed

- Android: gracefully handle a missing `BLUETOOTH_CONNECT` permission in the adapter's state ticker instead of throwing

## [4.0.6] - 2026-05-08

### Fixed

- Android: build and startup issues; `AndroidBluetoothAdapter` constructor made public

### Changed

- Android target framework simplified to `net10.0-android`

## [4.0.5] - 2026-03-31

### Added

- Comprehensive Bluetooth SIG service/characteristic definitions, with accompanying docs

## [4.0.4] - 2026-03-29

### Added

- Resilient characteristic accessor extension methods

### Fixed

- Reading of battery and version characteristics, now falling back to sane defaults

## [4.0.3] - 2026-03-26

The bulk of the library's current feature set landed in this release.

### Added

- **Broadcasting (peripheral/GATT server role)**, fully implemented on Android, Apple, and Windows: advertising, local services/characteristics/descriptors, client subscriptions, and read/write request events with response override
- **Facade pattern**: `Bluetooth.Maui`'s `BluetoothScanner`/`BluetoothBroadcaster` now sit in front of the platform implementations as the actual `IBluetoothScanner`/`IBluetoothBroadcaster` registered by DI (see `Docs/FACADE_PATTERN_SUMMARY.md` and the architecture ADRs)
- **Profile system**: typed accessor/codec contracts and characteristic-listener orchestration for named services/characteristics, with the SIG battery profile registered by default
- Device filtering by name and signal strength; closest-device scan mode and device-disappearance handling
- Sample app: BLE broadcaster demo and write/listen "lab" pages
- ADR-based architecture documentation, commit-message format guide, and contribution definition-of-done

### Changed

- Android scanning options/enums moved into `Bluetooth.Abstractions`
- Several factory abstractions simplified or removed in favor of direct construction

## [4.0.2] - 2026-02-26

### Fixed

- Apple: ensure Bluetooth is in the powered-on state before scanning

## [4.0.1] - 2026-02-25

### Added

- Apple: background `bluetooth-central` mode for scanning; dedicated exceptions for missing `Info.plist` keys/background modes

### Fixed

- Post scanning-state notifications on the main thread; validate background modes for central/peripheral managers

## [4.0.0] - 2026-02-24

Architecture rewrite. `Plugin.Bluetooth` was renamed to `Bluetooth.Core` / `Bluetooth.Maui`, and
the current layered design (Abstractions → Core → Platform implementations → `Bluetooth.Maui`
facade) replaced the earlier structure.

### Added

- Full Android, iOS/macOS, and Windows implementations of scanning and GATT (read/write/notify)
- L2CAP channel support (factory pattern; configurable MTU, timeout, read-buffer, and background-reading options) on Apple and Android
- Configurable retry policy with exponential backoff for BLE operations
- High-performance structured logging (`LoggerMessage` source generation) across Android, Apple, and Windows
- Explicit Bluetooth permission handling and strategies
- Sample app: device details, characteristics, and scanner pages with navigation

### Changed

- **Breaking**: renamed namespaces/projects from `Plugin.Bluetooth` to `Bluetooth.Core` / `Bluetooth.Maui`
- Scanner/device APIs now return immutable snapshots of services and characteristics lists

### Fixed

- Disposed a leaked `AutoResetEvent`; removed incorrect exception filtering on multi-item checks

## [1.0.3] - 2026-01-07

### Added

- Strict service-retrieval methods that throw instead of returning `null`

## [1.0.2] - 2026-01-05

### Changed

- Reorganized logging package dependencies in the central package props

## [1.0.1] - 2026-01-05

### Docs

- Added badges and the project icon to the README

## [1.0.0] - 2026-01-05

Initial public release.

### Added

- Core cross-platform BLE abstractions: scanner, device, service, characteristic, descriptor, exceptions, event args
- Initial Android implementation (scan, connect, GATT read/write/listen) and a preliminary Windows implementation
- Sample scanner app with navigation and exception handling
- Standard GATT service definitions, configurable RSSI-to-signal-strength conversion, async retry helper

[Unreleased]: https://github.com/Laerdal/Plugin.Bluetooth/compare/v4.0.16...HEAD
[4.0.16]: https://github.com/Laerdal/Plugin.Bluetooth/compare/v4.0.15...v4.0.16
[4.0.15]: https://github.com/Laerdal/Plugin.Bluetooth/compare/v4.0.14...v4.0.15
[4.0.14]: https://github.com/Laerdal/Plugin.Bluetooth/compare/v4.0.13...v4.0.14
[4.0.13]: https://github.com/Laerdal/Plugin.Bluetooth/compare/v4.0.12...v4.0.13
[4.0.12]: https://github.com/Laerdal/Plugin.Bluetooth/compare/v4.0.11...v4.0.12
[4.0.11]: https://github.com/Laerdal/Plugin.Bluetooth/compare/v4.0.10...v4.0.11
[4.0.10]: https://github.com/Laerdal/Plugin.Bluetooth/compare/v4.0.9...v4.0.10
[4.0.9]: https://github.com/Laerdal/Plugin.Bluetooth/compare/v4.0.8...v4.0.9
[4.0.8]: https://github.com/Laerdal/Plugin.Bluetooth/compare/v4.0.7...v4.0.8
[4.0.7]: https://github.com/Laerdal/Plugin.Bluetooth/compare/v4.0.6...v4.0.7
[4.0.6]: https://github.com/Laerdal/Plugin.Bluetooth/compare/v4.0.5...v4.0.6
[4.0.5]: https://github.com/Laerdal/Plugin.Bluetooth/compare/v4.0.4...v4.0.5
[4.0.4]: https://github.com/Laerdal/Plugin.Bluetooth/compare/v4.0.3...v4.0.4
[4.0.3]: https://github.com/Laerdal/Plugin.Bluetooth/compare/v4.0.2...v4.0.3
[4.0.2]: https://github.com/Laerdal/Plugin.Bluetooth/compare/v4.0.1...v4.0.2
[4.0.1]: https://github.com/Laerdal/Plugin.Bluetooth/compare/v4.0.0...v4.0.1
[4.0.0]: https://github.com/Laerdal/Plugin.Bluetooth/compare/v1.0.3...v4.0.0
[1.0.3]: https://github.com/Laerdal/Plugin.Bluetooth/compare/v1.0.2...v1.0.3
[1.0.2]: https://github.com/Laerdal/Plugin.Bluetooth/compare/v1.0.1...v1.0.2
[1.0.1]: https://github.com/Laerdal/Plugin.Bluetooth/compare/v1.0.0...v1.0.1
[1.0.0]: https://github.com/Laerdal/Plugin.Bluetooth/releases/tag/v1.0.0
