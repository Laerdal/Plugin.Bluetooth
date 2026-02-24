# GitHub Copilot Instructions for Plugin.Bluetooth

> **Note**: This file is a reference pointer. The comprehensive coding standards, architectural guidelines, and best practices are maintained in the main documentation file.

## 📚 Complete Documentation

For the full, detailed coding instructions, please refer to:

**[`../Docs/COPILOT_INSTRUCTIONS.md`](../Docs/COPILOT_INSTRUCTIONS.md)**

This comprehensive guide includes:

- **Architecture** - Project structure, design patterns, core components
- **Code Conventions** - Naming, documentation standards, file organization
- **Logging Standards** - Structured logging with EventId ranges (1000-8999)
- **Exception Handling** - Custom hierarchy, static helpers, platform-specific types
- **CancellationToken Standards** - Async cancellation best practices
- **TaskCompletionSource Patterns** - Callback-based async operations
- **Options Pattern** - Record types, platform-specific options, DI
- **Thread Safety** - Locking strategies, defensive copies, SemaphoreSlim
- **Platform Guidelines** - Android, iOS/macOS, Windows specific APIs
- **Code Review Checklist** - 60+ items across 10 categories

## Quick Reference

### Key Principles

1. **Use structured, nullable logging**: `Logger?.LogEvent(param1, param2)`
2. **Throw specific exceptions**: Custom domain exceptions, not generic `Exception`
3. **Support cancellation**: `CancellationToken cancellationToken = default` on all async
4. **Thread-safe collections**: Lock on private objects, return defensive copies
5. **Platform abstraction**: `Native*` methods for platform overrides
6. **Options pattern**: Record types with init-only properties and sensible defaults
7. **Retry infrastructure**: Use `RetryTools` with configurable `RetryOptions`

### Naming Conventions

- **Interfaces**: `I*` (e.g., `IBluetoothDevice`)
- **Base Classes**: `Base*` (e.g., `BaseBluetoothScanner`)
- **Platform Implementations**: `Android*`, `Apple*`, `Windows*`
- **Options**: `*Options` (e.g., `ConnectionOptions`)
- **Factories**: `*Factory`, specs: `*FactorySpec`
- **Callbacks**: `On*` (e.g., `OnConnectSucceeded`)
- **Platform Methods**: `Native*` (e.g., `NativeConnectAsync`)
- **Retry Helpers**: `*Internal` (e.g., `ConnectInternalAsync`)

### EventId Ranges

- **1000-1999**: Scanner operations
- **2000-2999**: Connection operations
- **3000-3999**: Service/Characteristic discovery
- **4000-4999**: GATT operations
- **5000-5999**: Notifications/Indications
- **6000-6999**: Broadcaster/MTU (platform-specific)
- **7000-7999**: L2CAP (Android), Adapter init (Windows), MTU (Apple)
- **8000-8999**: L2CAP (Apple/iOS)

---

*For complete details, see [`../Docs/COPILOT_INSTRUCTIONS.md`](../Docs/COPILOT_INSTRUCTIONS.md)*
