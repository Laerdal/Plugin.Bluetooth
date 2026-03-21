# ADR 0001: Facade Registrations Override Platform Interface Bindings

- Status: Accepted
- Date: 2026-03-21
- Decision Makers: Plugin.Bluetooth maintainers

## Context

Platform projects register platform-specific scanner and broadcaster implementations in DI.
The `Bluetooth.Maui` composition root also registers facade implementations for
`IBluetoothScanner` and `IBluetoothBroadcaster`.

Without an explicit policy, contributors can misinterpret which implementation is expected
from direct interface resolution.

## Decision

Keep the current registration order and treat facade registrations as the default bindings for:
- `IBluetoothScanner`
- `IBluetoothBroadcaster`

Platform registrations remain available for internal composition and extension paths,
but default consumer resolution uses facade wrappers.

## Alternatives Considered

### Alternative A: Remove facade defaults and resolve platform implementations directly

- Summary:
  Resolve `IBluetoothScanner` and `IBluetoothBroadcaster` to platform classes only.
- Pros:
  - Simpler to reason about immediate runtime type.
  - Fewer duplicate registrations.
- Cons:
  - Breaks the single cross-platform inheritance/extension point model.
  - Increases platform-conditional logic in client projects.

### Alternative B: Remove platform registrations and keep only facades

- Summary:
  Register only facade interfaces and construct platform dependencies privately.
- Pros:
  - Very explicit external DI surface.
- Cons:
  - Reduces flexibility for advanced internal wiring and testing scenarios.
  - Makes platform implementation substitution harder where needed.

## Consequences

### Positive

- Preserves unified facade extension model across all platforms.
- Keeps client-facing API usage consistent regardless of target platform.
- Avoids platform-conditional inheritance requirements for consumers.

### Negative

- DI graph includes both platform and facade registrations for related roles.
- Contributors must understand registration order to predict resolved types.

### Neutral

- Existing behavior remains unchanged; this ADR codifies current intent.

## Follow-up Actions

- [ ] Add one short comment in each platform service-registration entry point referencing this ADR.
- [ ] Add a short "DI Resolution Rules" note in architecture docs that references this ADR.

## References

- Code references:
  - `Bluetooth.Maui/ServiceCollectionExtensions.cs`
- Related docs:
  - `Docs/ARCHITECTURE_GUIDELINES.md`
  - `DOCUMENTATION_ALIGNMENT_REVIEW.md`
- Related PRs/issues:
  - N/A
