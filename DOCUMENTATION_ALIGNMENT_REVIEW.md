# Documentation Alignment Review

This file tracks implementation misalignments found while refreshing documentation.

Scope:

- Current repository code state
- Cross-platform differences that affect architecture clarity
- Class/file-structure divergences that can confuse contributors

## 1. Broadcasting Factory Pattern Is Not Uniform

Observed:

- Abstraction side documents broadcaster factory interfaces as pending/stub infrastructure.
- Android broadcaster path uses factory implementations for local GATT objects.
- Apple broadcaster path creates some local objects directly in broadcaster flow.
- Windows broadcaster factories are registered but runtime behavior remains not implemented.

Impact:

- Factory usage rules are not currently deterministic across platforms.
- Contributor expectations differ depending on platform folder they inspect first.

Suggested follow-up:

- Decide one of:
  - Standardize all platforms on factory-based creation.
  - Keep mixed strategy and explicitly document where direct creation is intentional.

## 2. Facade DI Overrides Platform Registrations

Observed:

- Platform projects register platform scanner/broadcaster services.
- Bluetooth.Maui then registers facade scanner/broadcaster as default interface bindings.

Impact:

- Interface resolution returns facade wrappers by default.
- It is easy to assume platform classes are resolved directly from IBluetoothScanner and IBluetoothBroadcaster.

Suggested follow-up:

- Keep existing behavior but add a short design note near DI registration points explaining intentional override order.

## 3. Core Broadcasting DI Extension Is Empty

Observed:

- Bluetooth.Core.Broadcasting service extension exists but currently does not add shared registrations.

Impact:

- The extension suggests a shared core broadcasting registration layer that does not yet provide concrete behavior.

Suggested follow-up:

- Either add shared registrations there, or document it as reserved for future cross-platform broadcasting infrastructure.

## 4. Advanced Capability Semantics Differ By Platform

Observed:

- Windows throws NotSupportedException for broadcaster runtime APIs, L2CAP, connected RSSI reads, MTU requests, connection-priority requests, and PHY requests.
- iOS/macOS supports broadcasting but has platform restrictions on some operations (for example reliable write transactions are not supported).
- DotNetCore fallback implementations throw PlatformNotSupportedException by design.

Impact:

- Shared interface surface is broader than any single-platform capability set.
- Cross-platform apps need explicit capability guards around advanced operations.

Suggested follow-up:

- Keep the broad shared contracts, but ensure each advanced operation docs section states unsupported-exception behavior clearly.

## 5. Optional: Scanning Options Monitor Wiring Check

Observed:

- Base scanner includes a property for options monitor usage path.
- This is easy to read as active runtime options monitoring; wiring may be partial depending on constructor paths.

Impact:

- Contributors can misread this as fully active dynamic option reload behavior.

Suggested follow-up:

- Confirm whether dynamic options monitoring is fully wired in production code paths.
- If not, document current behavior and intended direction.
