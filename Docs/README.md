# Plugin.Bluetooth Documentation

A comprehensive, cross-platform Bluetooth Low Energy (BLE) plugin for .NET MAUI applications, providing unified access to BLE functionality across iOS, Android, Windows, and macOS.

## 🚀 Quick Start

New to the plugin? Start here:

- **[Getting Started Guide](Getting-Started/README.md)** - Installation, setup, and your first BLE scan
- **[Platform Setup](Getting-Started/Platform-Setup.md)** - Platform-specific configuration
- **[Permissions](Getting-Started/Permissions.md)** - Handling BLE permissions

## 📚 Core Concepts

Learn the fundamental building blocks:

- **[Scanner](Core-Concepts/Scanner.md)** - Discovering BLE devices (Central mode)
- **[Device](Core-Concepts/Device.md)** - Connecting and managing connections
- **[Service](Core-Concepts/Service.md)** - GATT services and discovery
- **[Characteristic](Core-Concepts/Characteristic.md)** - Reading, writing, and notifications
- **[Descriptor](Core-Concepts/Descriptor.md)** - Characteristic metadata
- **[Advertisement](Core-Concepts/Advertisement.md)** - Advertisement data and filtering
- **[Broadcaster](Core-Concepts/Broadcaster.md)** - Advertising your device (Peripheral mode)
- **[Local Service](Core-Concepts/Local-Service.md)** - Creating services for broadcasting
- **[Local Characteristic](Core-Concepts/Local-Characteristic.md)** - Configuring characteristics
- **[Connected Device](Core-Concepts/Connected-Device.md)** - Managing connected centrals

## 🔧 Platform-Specific

Understand platform differences and capabilities:

- **[Platform Overview](Platforms/README.md)** - Feature support matrix
- **[iOS & macOS](Platforms/iOS-macOS.md)** - CoreBluetooth specifics
- **[Android](Platforms/Android.md)** - Android BLE implementation
- **[Windows](Platforms/Windows.md)** - WinRT BLE details
- **[Platform Comparison](Platforms/Comparison.md)** - Detailed feature comparison

## ⚙️ Configuration

Configure the plugin for your needs:

- **[Dependency Injection](Configuration/Dependency-Injection.md)** - Setting up DI
- **[Infrastructure Options](Configuration/Infrastructure-Options.md)** - Global settings
- **[Scanning Options](Configuration/Scanning-Options.md)** - Scan configuration
- **[Connection Options](Configuration/Connection-Options.md)** - Connection settings
- **[L2CAP Options](Configuration/L2CAP-Options.md)** - L2CAP channel configuration
- **[Exploration Options](Configuration/Exploration-Options.md)** - Service discovery settings
- **[Broadcasting Options](Configuration/Broadcasting-Options.md)** - Advertising configuration

## 🎯 Advanced Topics

Deep dives into advanced features:

- **[L2CAP Channels](Advanced/L2CAP-Channels.md)** - Raw data transfer
- **[RSSI & Signal Strength](Advanced/RSSI-Signal-Strength.md)** - Distance estimation
- **[PHY Settings](Advanced/PHY-Settings.md)** - Bluetooth 5.0 PHY modes
- **[MTU Negotiation](Advanced/MTU-Negotiation.md)** - Optimizing throughput
- **[Retry & Resilience](Advanced/Retry-and-Resilience.md)** - Error handling strategies
- **[Logging](Advanced/Logging.md)** - Comprehensive logging system
- **[Factories](Advanced/Factories.md)** - Factory pattern architecture
- **[Threading & Async](Advanced/Threading-And-Async.md)** - Thread safety
- **[Property System](Advanced/Property-System.md)** - Bindable property architecture
- **[Ticker Scheduling](Advanced/Ticker-Scheduling.md)** - Periodic task scheduling
- **[Disposal & Cleanup](Advanced/Disposal-And-Cleanup.md)** - Resource management

## 📖 API Reference

Complete API documentation:

- **[API Overview](API-Reference/README.md)** - API structure and conventions
- **[Abstractions](API-Reference/Abstractions.md)** - Core interfaces
- **[Enums](API-Reference/Enums.md)** - Enumerations and flags
- **[Events](API-Reference/Events.md)** - Event types and handling
- **[Exceptions](API-Reference/Exceptions.md)** - Exception hierarchy

## ✅ Best Practices

Recommended patterns and approaches:

- **[Best Practices Overview](Best-Practices/README.md)**
- **[Connection Management](Best-Practices/Connection-Management.md)**
- **[Battery Optimization](Best-Practices/Battery-Optimization.md)**
- **[Error Handling](Best-Practices/Error-Handling.md)**
- **[Performance](Best-Practices/Performance.md)**
- **[MVVM Integration](Best-Practices/MVVM-Integration.md)**
- **[Testing](Best-Practices/Testing.md)**

## 🔍 Troubleshooting

Solutions to common issues:

- **[Common Issues](Troubleshooting/Common-Issues.md)** - Frequently encountered problems
- **[Debugging](Troubleshooting/Debugging.md)** - Debugging techniques and tools

## 🎯 Feature Highlights

### Central Mode (Scanner)
- ✅ Device discovery with filtering
- ✅ Connection management with retry logic
- ✅ GATT service/characteristic/descriptor exploration
- ✅ Read/write/notify operations
- ✅ RSSI monitoring and signal strength
- ✅ MTU negotiation (Android)
- ✅ L2CAP channels (iOS 11+, Android 10+)
- ✅ PHY settings (Android API 26+)

### Peripheral Mode (Broadcaster)
- ✅ Custom service advertising
- ✅ Characteristic notifications
- ✅ Client connection management
- 🚧 iOS/macOS: Not implemented
- 🚧 Android: Implementation in progress
- 🚧 Windows: Not implemented

### Cross-Platform Features
- ✅ Dependency injection support
- ✅ Comprehensive logging with structured logging
- ✅ INotifyPropertyChanged for MVVM binding
- ✅ Configurable retry strategies
- ✅ Thread-safe operations
- ✅ Async/await throughout

## 📦 Platform Support

| Platform | Scanning | Connection | GATT | L2CAP | Broadcasting |
|----------|:--------:|:----------:|:----:|:-----:|:------------:|
| **iOS** | ✅ | ✅ | ✅ | ✅ (11+) | ❌ |
| **macOS** | ✅ | ✅ | ✅ | ✅ (10.13+) | ❌ |
| **Android** | ✅ | ✅ | ✅ | ✅ (API 29+) | 🚧 |
| **Windows** | ✅ | ✅ | ✅ | ❌ | ❌ |

Legend: ✅ Full support | 🚧 In progress | ❌ Not supported

## 🤝 Contributing

Contributions are welcome! Please see the contribution guidelines in the repository.

## 📄 License

Please refer to the LICENSE file in the repository root.

---

**Need help?** Check the [Troubleshooting](Troubleshooting/) section or open an issue in the repository.
