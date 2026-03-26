# Plugin.Bluetooth Documentation

This directory documents the current pre-release implementation state of Plugin.Bluetooth.

Goal of these docs:
- Describe behavior as implemented in code
- Document conventions and constraints used in this repository
- Show platform differences explicitly

## Documentation Map

### Start Here
- [Getting Started](Getting-Started/README.md)
- [Platform Setup](Getting-Started/Platform-Setup.md)
- [Permissions](Getting-Started/Permissions.md)

### Architecture And Rules
- [Architecture Guidelines](ARCHITECTURE_GUIDELINES.md)
- [Architecture Diagrams](ARCHITECTURE_DIAGRAMS.md)
- [Facade Pattern Summary](FACADE_PATTERN_SUMMARY.md)
- [Architecture Decision Records (ADR)](Architecture/ADR/README.md)

### Core Concepts
- [Scanner](Core-Concepts/Scanner.md)
- [Device](Core-Concepts/Device.md)
- [Service](Core-Concepts/Service.md)
- [Characteristic](Core-Concepts/Characteristic.md)
- [Descriptor](Core-Concepts/Descriptor.md)
- [Advertisement](Core-Concepts/Advertisement.md)
- [Broadcaster](Core-Concepts/Broadcaster.md)
- [Local Service](Core-Concepts/Local-Service.md)
- [Local Characteristic](Core-Concepts/Local-Characteristic.md)
- [Connected Device](Core-Concepts/Connected-Device.md)

### Configuration
- [Dependency Injection](Configuration/Dependency-Injection.md)
- [Profile Migration](Configuration/Profile-Migration.md)
- [Infrastructure Options](Configuration/Infrastructure-Options.md)
- [Scanning Options](Configuration/Scanning-Options.md)
- [Connection Options](Configuration/Connection-Options.md)
- [Exploration Options](Configuration/Exploration-Options.md)
- [L2CAP Options](Configuration/L2CAP-Options.md)
- [Broadcasting Options](Configuration/Broadcasting-Options.md)

### API Reference
- [API Overview](API-Reference/README.md)
- [Abstractions](API-Reference/Abstractions.md)
- [Enums](API-Reference/Enums.md)
- [Events](API-Reference/Events.md)
- [Exceptions](API-Reference/Exceptions.md)

### Platform Documentation
- [Platforms Overview](Platforms/README.md)
- [Platform Comparison](Platforms/Comparison.md)
- [Android](Platforms/Android.md)
- [iOS and macOS](Platforms/iOS-macOS.md)
- [Windows](Platforms/Windows.md)

### Operational Guidance
- [Best Practices](Best-Practices/README.md)
- [Commit Message Format](Best-Practices/Commit-Message-Format.md)
- [Contribution Definition of Done](Best-Practices/Contribution-DoD.md)
- [Troubleshooting](Troubleshooting/Common-Issues.md)
- [Debugging](Troubleshooting/Debugging.md)

## Layer Model

```mermaid
flowchart LR
	A[Abstractions] --> B[Core]
	B --> C[Platform Implementations]
	C --> D[Bluetooth.Maui Facade]
```

Projects in the layer model:
- Abstractions: Bluetooth.Abstractions, Bluetooth.Abstractions.Scanning, Bluetooth.Abstractions.Broadcasting
- Core: Bluetooth.Core, Bluetooth.Core.Scanning, Bluetooth.Core.Broadcasting
- Platform: Bluetooth.Maui.Platforms.Apple, Bluetooth.Maui.Platforms.Droid, Bluetooth.Maui.Platforms.Win, Bluetooth.Maui.Platforms.DotNetCore
- Facade: Bluetooth.Maui

## Capability Snapshot

Legend:
- ✅ implemented
- ⚠️ implemented with platform limitations
- ❌ not implemented

| Platform | Scanning | Connection | GATT Read/Write/Notify | L2CAP | Broadcasting |
|----------|----------|------------|-------------------------|-------|--------------|
| Android | ✅ | ✅ | ✅ | ✅ API 29+ | ✅ |
| iOS/macOS | ✅ | ✅ | ✅ | ✅ iOS 11+/macOS 10.13+ | ✅ |
| Windows | ✅ | ✅ | ✅ | ❌ | ❌ |
| DotNetCore fallback | ❌ | ❌ | ❌ | ❌ | ❌ |

Notes:
- Windows currently throws NotSupportedException for broadcasting, L2CAP, connected RSSI reads, PHY changes, MTU requests, and connection-priority requests.
- iOS/macOS broadcaster is implemented, but some operations remain restricted by CoreBluetooth behavior.
- DotNetCore fallback implementations throw PlatformNotSupportedException by design.
