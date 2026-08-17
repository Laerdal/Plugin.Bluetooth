# ADR 0002: Broadcasting Factory Consistency Strategy

- Status: Accepted
- Date: 2026-03-21
- Decision Makers: Plugin.Bluetooth maintainers

## Context

Broadcasting object creation currently differs across platforms:
- Android broadcaster paths use broadcasting factories.
- Apple broadcaster creates local services directly in broadcaster flow.
- Windows broadcaster-side factories exist, while runtime broadcasting behavior is not implemented.

This divergence can confuse contributors and reviewers about expected construction patterns.

## Decision

Adopt a transition strategy with explicit target state:

1. Short term (current accepted state):
- Mixed construction patterns are allowed.
- Each platform must document which path it uses (factory vs direct creation).
- New broadcasting code must not introduce additional undocumented construction patterns.

2. Target state:
- Standardize broadcaster-side object creation on factory-based construction across platforms
  where broadcasting is implemented.
- Keep unsupported platforms explicit with `NotSupportedException` or
  `PlatformNotSupportedException`, depending on platform policy.

## Alternatives Considered

### Alternative A: Keep permanent mixed strategy with no standardization target

- Summary:
  Treat per-platform creation style as unconstrained.
- Pros:
  - Minimal refactoring effort.
  - Maximum short-term platform autonomy.
- Cons:
  - Ongoing contributor confusion.
  - Higher maintenance burden for cross-platform architectural consistency.

### Alternative B: Enforce immediate standardization in one change set

- Summary:
  Convert all broadcaster-side creation paths to factories now.
- Pros:
  - Fastest route to architectural consistency.
- Cons:
  - High risk and large scope while platform behavior is still evolving pre-release.
  - Could delay unrelated feature/documentation work.

## Consequences

### Positive

- Establishes a clear direction without forcing high-risk immediate refactors.
- Reduces ambiguity by requiring documentation when patterns differ.
- Supports incremental convergence toward a single creation approach.

### Negative

- Short-term inconsistency remains until follow-up refactors are completed.
- Reviews must explicitly verify whether new code moves toward target state.

### Neutral

- Existing runtime behavior remains unchanged by this ADR.

## Follow-up Actions

- [ ] Add a documentation section describing current platform-specific broadcasting creation paths.
- [ ] Track Apple broadcaster migration to factory-based creation as a dedicated task.
- [ ] Re-evaluate this ADR when Windows broadcasting runtime implementation is completed.

## References

- Code references:
  - `Bluetooth.Maui.Platforms.Droid/Broadcasting/Factories/`
  - `Bluetooth.Maui.Platforms.Apple/Broadcasting/AppleBluetoothBroadcaster.cs`
  - `Bluetooth.Maui.Platforms.Win/Broadcasting/Factories/`
- Related docs:
  - `DOCUMENTATION_ALIGNMENT_REVIEW.md`
  - `Docs/ARCHITECTURE_GUIDELINES.md`
- Related PRs/issues:
  - N/A
