# Contribution Definition of Done

This checklist defines the minimum completion criteria before opening or merging a pull request.

## 1. Change Classification

Select at least one category:
- [ ] Feature
- [ ] Bug fix
- [ ] Refactor
- [ ] Docs only
- [ ] CI/build/tooling

## 2. Code And Behavior

- [ ] Behavior is implemented in the intended layer (Abstractions, Core, Platform, Facade).
- [ ] Public API surface changes are intentional and documented.
- [ ] Unsupported operations use the intended exception type (`NotSupportedException` or `PlatformNotSupportedException`).
- [ ] New async operations expose `CancellationToken cancellationToken = default`.
- [ ] State transitions and callbacks are consistent with existing lifecycle patterns.

## 3. Platform Impact Review

- [ ] Platform matrix impact reviewed for Android, iOS/macOS, Windows, DotNetCore fallback.
- [ ] Platform-specific behavior differences are explicit in code and docs.
- [ ] API-level/version gates are validated where applicable.
- [ ] Fallback behavior is defined for unsupported capability paths.

## 4. Logging And Exceptions

- [ ] New logs use structured logging and appropriate EventId range.
- [ ] EventId allocation does not conflict with existing ranges.
- [ ] Exceptions preserve context (`innerException`, useful message, and relevant identifiers).
- [ ] Error behavior is deterministic across retry and cancellation paths.

## 5. Tests

- [ ] Unit tests updated or added for changed logic.
- [ ] Unsupported-path behavior tested where applicable.
- [ ] Existing tests pass for affected projects.
- [ ] Manual validation performed for affected platforms when automated test coverage is unavailable.

## 6. Documentation

- [ ] Relevant docs are updated in the same PR.
- [ ] Capability tables match implemented behavior.
- [ ] API examples reflect current interfaces and signatures.
- [ ] New docs links resolve to existing files.
- [ ] Docs tone remains factual (no promotional language).

## 7. Dependency Injection And Composition

- [ ] Service registration order changes are reviewed for override effects.
- [ ] Default interface resolution behavior remains intentional.
- [ ] Removal or addition of registrations is documented when it changes runtime composition.

## 8. PR Readiness

- [ ] Commit messages follow repository commit format.
- [ ] PR description states what changed and why.
- [ ] Risk level and rollout considerations are identified.
- [ ] Follow-up tasks are listed when work is intentionally partial.

## 9. Docs-Only Fast Path

For docs-only PRs, minimum required checks:
- [ ] Link validation complete
- [ ] Capability claims verified against current code
- [ ] Terminology and naming aligned with repository conventions
- [ ] No stale references to removed files/features
