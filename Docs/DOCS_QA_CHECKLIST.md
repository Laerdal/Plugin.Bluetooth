# Documentation QA Checklist

Use this checklist before merging documentation changes to keep docs aligned with current code behavior.

---

## ✅ Capability Accuracy

- [ ] Every capability table reflects what is actually implemented in code, not what is planned.
- [ ] Any operation that throws `NotSupportedException` or `PlatformNotSupportedException` is explicitly documented as not supported on that platform.
- [ ] No operation is described as "no-op" when the implementation actually throws.
- [ ] Android API-level guards are stated where they exist (`API 21+`, `API 26+`, `API 29+`).
- [ ] iOS/macOS version guards are stated where they exist (`iOS 11+`, `macOS 10.13+`).

## ✅ Platform Coverage

- [ ] Platform matrix tables cover all four targets: Android, iOS/macOS, Windows, DotNetCore fallback.
- [ ] DotNetCore fallback is noted as throwing `PlatformNotSupportedException` at runtime.
- [ ] Windows-specific unsupported paths are noted: broadcasting, L2CAP, MTU request, connection-priority request, PHY request, connected RSSI read.

## ✅ Code Examples

- [ ] Code examples use the current public API surface (interface names, method signatures, option record names).
- [ ] Code examples that apply only to specific platforms are either labeled or wrapped in a platform check.
- [ ] No example uses a class or method that has been renamed or removed.
- [ ] DI setup examples use `AddBluetoothServices()` as the single entry point from `Bluetooth.Maui`.

## ✅ Links

- [ ] No links point to files in `Docs/Advanced/` (that directory does not exist).
- [ ] No links point to `L2CAP_ADDITIONAL_OPTIONS.md`, `SCANNING.md`, `CONNECTING.md`, `BROADCASTING.md`, or `TIMEOUT_ANALYSIS.md` (those files do not exist).
- [ ] No links point to `../Core-Concepts/README.md` or `../Troubleshooting/README.md` (no top-level index files in those folders).
- [ ] All local `../` relative links resolve to a file that actually exists.

## ✅ Architecture Alignment

- [ ] Project names use the correct suffixes: `Bluetooth.Maui.Platforms.Win` (not `Windows`), `Bluetooth.Maui.Platforms.Droid` (not `Android`).
- [ ] The three-layer model (Abstractions → Core → Platform → Facade) is described correctly wherever it appears.
- [ ] Facade pattern description matches the current `BluetoothScanner` / `BluetoothBroadcaster` wrappers in `Bluetooth.Maui`.
- [ ] DI registration order is described correctly: core → platform → facade override.

## ✅ Exception Contracts

- [ ] `NotSupportedException` vs `PlatformNotSupportedException` usage is correct (see platform code for which is thrown where).
- [ ] Exception hierarchy diagrams match what is in `Bluetooth.Abstractions/Exceptions/` and `Bluetooth.Abstractions.Scanning/Exceptions/`.

## ✅ Tone

- [ ] No promotional or superlative language: "excellent", "robust", "clean" (as quality claim), "seamless", "powerful", "comprehensive", "crucial", "amazing", "perfect".
- [ ] Intro paragraphs describe content (what the page covers), not value claims (why it is great).
- [ ] No "Winner" / "🏆" / "Best in class" comparisons.
- [ ] Section headers are descriptive (e.g. "Role of Advertisements") not promotional (e.g. "Why X is Amazing").
- [ ] Code examples show behavior, not testimonials.

## ✅ EventId / Logging Range Accuracy

- [ ] If a logging EventId range is documented, it matches the actual ranges in platform logging files:
  - 1000–1999: scanner
  - 2000–2999: connection
  - 3000–3999: discovery
  - 4000–4999: GATT read/write
  - 5000–5999: notifications/indications
  - 6000–6999: broadcaster / MTU (platform-dependent)
  - 7000–7999: platform-dependent advanced operations
  - 8000–8999: Apple L2CAP

## ✅ Known Pending Items (check DOCUMENTATION_ALIGNMENT_REVIEW.md)

- [ ] Broadcasting factory pattern status noted correctly (mixed across platforms).
- [ ] Facade DI override behavior is described where relevant.
- [ ] Core Broadcasting DI extension is not described as providing active shared registrations.
