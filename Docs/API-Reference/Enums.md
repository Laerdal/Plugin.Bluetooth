# Enumerations

Every enum's members and XML doc descriptions are generated from source in the API reference (see <xref:Bluetooth.Abstractions.Enums>) — this page only covers the handful of decision-guidance details (timing/power trade-offs) that don't fit a member-level XML doc comment.

## Core Enums

| Enum | Namespace | What it's for |
|------|-----------|----------------|
| <xref:Bluetooth.Abstractions.Enums.CharacteristicProperties> | `Bluetooth.Abstractions.Enums` | GATT characteristic properties (Read/Write/Notify/Indicate/...) as advertised by a remote characteristic |
| <xref:Bluetooth.Abstractions.Enums.CharacteristicPermissions> | `Bluetooth.Abstractions.Enums` | Local (peripheral-role) characteristic access permissions |
| <xref:Bluetooth.Abstractions.Enums.PhyMode> | `Bluetooth.Abstractions.Enums` | Bluetooth 5 PHY (1M/2M/Coded) |
| <xref:Bluetooth.Abstractions.Enums.Manufacturer> | `Bluetooth.Abstractions.Enums` | BT SIG assigned company identifiers, parsed from advertisement manufacturer data. `short`-backed, `Unknown = -1` |
| <xref:Bluetooth.Abstractions.PermissionRequestStrategy> | `Bluetooth.Abstractions` | Controls whether the library requests OS permissions automatically or leaves it to the app |
| <xref:Bluetooth.Abstractions.Enums.ConnectionPriority> | `Bluetooth.Abstractions.Enums` | See [comparison table](#connection-priority-trade-offs) below |
| <xref:Bluetooth.Abstractions.Enums.TransportType> | `Bluetooth.Abstractions.Enums` | BR/EDR vs LE transport selection (Android-specific concept) |

## Scanning Enums

| Enum | Namespace | What it's for |
|------|-----------|----------------|
| <xref:Bluetooth.Abstractions.Scanning.Options.BluetoothScanMode> | `Bluetooth.Abstractions.Scanning.Options` | See [comparison table](#scan-mode-trade-offs) below |
| <xref:Bluetooth.Abstractions.Scanning.Options.BluetoothScanCallbackType> | `Bluetooth.Abstractions.Scanning.Options` | Android scan callback matching behavior |
| <xref:Bluetooth.Abstractions.Scanning.Options.BluetoothDeviceDisappearanceBehavior> | `Bluetooth.Abstractions.Scanning.Options` | What happens to a device in the scanner's list once it stops advertising |

## Broadcasting Enums

| Enum | Namespace | What it's for |
|------|-----------|----------------|
| <xref:Bluetooth.Abstractions.Broadcasting.Enums.BluetoothCharacteristicProperties> | `Bluetooth.Abstractions.Broadcasting.Enums` | GATT properties to advertise on a local (peripheral-role) characteristic |
| <xref:Bluetooth.Abstractions.Broadcasting.Enums.BluetoothCharacteristicPermissions> | `Bluetooth.Abstractions.Broadcasting.Enums` | Access permissions for a local characteristic |
| <xref:Bluetooth.Abstractions.Broadcasting.Enums.BluetoothDescriptorPermissions> | `Bluetooth.Abstractions.Broadcasting.Enums` | Access permissions for a local descriptor |

`BroadcastingOptions` is a record, not an enum — see [Broadcasting-Options.md](../Configuration/Broadcasting-Options.md).

## Decision Guidance

### Scan mode trade-offs

`ScanningOptions.ScanMode` (<xref:Bluetooth.Abstractions.Scanning.Options.BluetoothScanMode>) is a hint most fully honored on Android; iOS/macOS and Windows manage actual scan timing themselves.

| Mode | Android scan interval | Power | Best for |
|------|------------------------|-------|----------|
| `LowPower` | ~5 s | Very low | Background monitoring |
| `Balanced` | ~2 s | Moderate | General use (default) |
| `LowLatency` | Continuous/near-continuous | High | Active device search, pairing |
| `Opportunistic` | Piggybacks on other apps' scans (Android 7+ only, falls back to `Balanced` elsewhere) | Minimal | Very low power scenarios |

`ScanningOptions` is immutable (init-only) — to change scan mode, stop and restart with new options:

```csharp
await scanner.StopScanningAsync();
await scanner.StartScanningAsync(options with { ScanMode = BluetoothScanMode.LowPower });
```

### Connection priority trade-offs

`ConnectionPriority` maps to the standard Android GATT connection-interval constants:

| Priority | Interval | Best for |
|----------|----------|----------|
| `LowPower` | ~100-125 ms | Idle/background connections |
| `Balanced` | ~30-50 ms | General use (default) |
| `High` | ~11.25-15 ms | Bulk data transfer, firmware updates |

```csharp
await device.RequestConnectionPriorityAsync(BluetoothConnectionPriority.High);
```

## See Also

- [Overview and Conventions](./README.md)
- [Interfaces and Abstractions](./Abstractions.md)
- [Events](./Events.md)
- [Generated API Reference](xref:Bluetooth.Abstractions)
