# Connection Options

`ConnectionOptions` (<xref:Bluetooth.Abstractions.Scanning.Options.ConnectionOptions>) controls how `IBluetoothRemoteDevice.ConnectAsync()` establishes a connection: permission behavior, retry logic, whether to wait for an advertisement first, and platform-specific parameters. It's passed per call, not DI-configured — see the type's generated page for the full property list.

## Basic Usage

```csharp
var options = new ConnectionOptions
{
    PermissionStrategy = PermissionRequestStrategy.RequestAutomatically,
    ConnectionRetry = RetryOptions.Default,
    WaitForAdvertisementBeforeConnecting = false
};

await device.ConnectAsync(options);
```

## Platform-Specific Options

- **Android** (<xref:Bluetooth.Abstractions.Scanning.Options.Android.AndroidConnectionOptions>): `AutoConnect`, `ConnectionPriority`, `TransportType`, `PreferredPhy` (API 26+), and per-operation `ServiceDiscoveryRetry`/`GattWriteRetry`/`GattReadRetry`.
- **Apple** (<xref:Bluetooth.Abstractions.Scanning.Options.Apple.AppleConnectionOptions>): `NotifyOnConnection`/`NotifyOnDisconnection`/`NotifyOnNotification`, `EnableTransportBridging` and `RequiresAncs` (iOS 13+).
- **Windows** (<xref:Bluetooth.Abstractions.Scanning.Options.Windows.WindowsConnectionOptions>): see the generated reference — Windows exposes fewer connection-time knobs than Android/Apple.

## Best Practices

- **Always set retry logic.** Connection failures are common, especially Android GATT error 133 — prefer `RetryOptions.Aggressive` over `RetryOptions.None` in production.
- **Disable `AutoConnect` on Android.** Direct connections are faster and more predictable; `AutoConnect = true` is rarely what you want.
- **Match `ConnectionPriority` to the use case** — `High` for real-time audio/control, `LowPower` for periodic sensor reads (see [Enumerations](../API-Reference/Enums.md#connection-priority-trade-offs) for the trade-off table).
- **Add GATT operation retries on Android** (`ServiceDiscoveryRetry`, `GattWriteRetry`, `GattReadRetry`) for unreliable devices.
- **Set `WaitForAdvertisementBeforeConnecting = true`** for devices with intermittent advertising.
- **Use conditional platform blocks** (`options.Android = ...` / `options.Apple = ...`) rather than assuming one platform's option shape works everywhere.

## Related Documentation

- [Dependency Injection](./Dependency-Injection.md)
- [Scanning Options](./Scanning-Options.md)
- [Device](../Core-Concepts/Device.md)
- [Connection Management](../Best-Practices/Connection-Management.md)
