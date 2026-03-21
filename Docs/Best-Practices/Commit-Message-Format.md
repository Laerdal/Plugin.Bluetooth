# Commit Message Format

This guide defines a commit message format aligned with the current history of this repository and suitable for Copilot-generated commits.

## 1. Current Pattern Observed In Repository

Most commits follow this subject format:

type (scope): summary

Examples from current history:
- feat (android): implement full Android Bluetooth LE GATT server broadcaster
- fix (maui): standardize logging property names for service identifiers
- refa (core): unify permission options and enhance name resolution for scanning and broadcasting
- docs(api-reference): add comprehensive Plugin.Bluetooth API documentation
- ci: update GitHub Actions artifact and dotnet tool versions

## 2. Standard Format To Use Going Forward

Use:

type (scope): short imperative summary

Optional body (recommended for non-trivial changes):
- Explain what changed.
- Explain why the change was made.
- Add risk, migration, or platform notes when relevant.

## 3. Header Rules

- type:
  - feat, fix, refa, docs, ci, build, chore, test
- scope:
  - Lowercase area name, usually project or domain.
  - Examples: android, apple, windows, maui, core, abstractions, scanning, broadcasting, docs
- summary:
  - Imperative mood (add, remove, rename, implement, refactor, update)
  - No trailing period
  - Keep concise and specific

Recommended limits:
- Header target: up to 100 characters
- Header hard limit: 120 characters
- Body lines: up to 100 characters

## 4. Body Rules

Use body for any change that is not obvious from the header.

Good body structure:
- Paragraph 1: what changed
- Paragraph 2: why this approach
- Paragraph 3 (optional): side effects, limitations, or follow-ups

Include platform notes when behavior differs by platform.

## 5. Consistency Notes

The history currently includes both forms below:
- docs(scope): ...
- docs (scope): ...

And this alias:
- refa

To stay compatible with existing history, this guide keeps:
- a space before scope, as in type (scope):
- refa as an accepted type

If you later want strict Conventional Commits compatibility, migrate in one dedicated cleanup decision:
- type(scope): summary
- refactor instead of refa

## 6. Copilot Prompt Template

Use this prompt when asking Copilot to draft a commit message:

Draft a git commit message for the staged changes in this repository.
Use this exact format:
- Header: type (scope): short imperative summary
- Blank line
- Body: 1-3 short paragraphs describing what changed and why.
Rules:
- Use one of: feat, fix, refa, docs, ci, build, chore, test
- Scope must be lowercase and specific to changed area
- Keep header <= 100 chars
- No trailing period in header
- Mention platform differences if applicable
- Do not use bullet lists unless absolutely necessary

## 7. Copilot Prompt Template (Description Only)

If header is already written and only description is needed:

Write a commit description body for this header:
<PASTE HEADER>

Constraints:
- 1-3 short paragraphs
- Explain what changed, then why
- Mention risks/limitations only when relevant
- Keep each line <= 100 chars
- No marketing language

## 8. Examples

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
