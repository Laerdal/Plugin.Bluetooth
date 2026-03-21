# Commit Message Format

This guide defines the canonical commit format for this repository.

## 1. Canonical Rules

Use this exact header format:

type (scope): short imperative summary

Header constraints:

- Max length: 72 characters
- No trailing period
- Lowercase `type` and `scope`

Allowed types (only these):

- feat
- fix
- refa
- perf
- docs
- ci
- chore
- test
- build

Scope rules:

- Scope is required
- Use the most specific scope possible
- Scope should be a folder, file, platform, or topic
- Examples: `maui`, `android`, `ble`, `docs`, `ui`, `ci`, `pkg`, `targets`

Body constraints:

- 1-2 short sentences
- Explain what changed and why
- Do not explain how
- Keep factual tone
- No co-authors
- No emojis
- No issue references

Multiple-scope rule:

- If a change touches multiple scopes, pick the most specific one
- Otherwise collapse to a broader topic scope

## 2. Current Pattern Observed In Repository

Most commits follow this subject format:

type (scope): summary

Examples from current history:

- feat (android): implement full Android Bluetooth LE GATT server broadcaster
- fix (maui): standardize logging property names for service identifiers
- refa (core): unify permission options and enhance name resolution for scanning and broadcasting
- docs(api-reference): add comprehensive Plugin.Bluetooth API documentation
- ci: update GitHub Actions artifact and dotnet tool versions

## 3. Consistency Notes

The history currently includes both forms below:

- docs(scope): ...
- docs (scope): ...

This guide standardizes on:

- type (scope): ...
- `refa` (not `refactor`)

## 4. Copilot Prompt Template

Use this prompt when asking Copilot to draft a commit message:

Draft a git commit message for the staged changes in this repository.
Use this exact format:

- Header: type (scope): short imperative summary
- Blank line
- Body: 1-2 short sentences describing what changed and why.
Rules:
- Use one of: feat, fix, refa, perf, docs, ci, chore, test, build
- Scope must be lowercase and specific to changed area
- Keep header <= 72 chars
- No trailing period in header
- No co-authors
- No emojis
- No issue references
- Keep the body factual and do not explain implementation details

## 5. Copilot Prompt Template (Description Only)

If header is already written and only description is needed:

Write a commit description body for this header:
<PASTE HEADER>

Constraints:

- 1-2 short sentences
- Explain what changed and why
- Do not explain how
- No co-authors, emojis, or issue references
- Keep factual tone

## 6. Examples

Example A:
feat (windows): add advertiser lifecycle logging for broadcaster operations

Add structured logs for broadcaster start, stop, and client updates in the Windows platform.
This improves diagnostics for unsupported and partial implementation paths.

Example B:
refa (maui): remove redundant factory registrations from facade composition

Remove registrations that are superseded by facade defaults and keep explicit platform wiring.
This simplifies DI graphs while preserving current runtime resolution behavior.

Example C:
docs (platforms): align capability matrix with implemented exception behavior

Update platform tables and notes to match actual NotSupportedException and
PlatformNotSupportedException behavior in platform implementations.
