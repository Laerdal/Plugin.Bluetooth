# Scanning Options

`ScanningOptions` (<xref:Bluetooth.Abstractions.Scanning.Options.ScanningOptions>) controls `IBluetoothScanner.StartScanningAsync()`: permission behavior, advertisement filtering, scan mode/power trade-offs, and platform-specific parameters. See the type's generated page for the full property list — the source XML docs already include per-property platform-support notes.

## Basic Usage

```csharp
var options = new ScanningOptions
{
    ServiceUuids = [myServiceUuid],
    ScanMode = BluetoothScanMode.Balanced,
    IgnoreDuplicateAdvertisements = true
};

await scanner.StartScanningAsync(options);
```

## Filtering

Three filtering mechanisms compose together:
- `ServiceUuids` — native filter on Android and iOS/macOS (fastest, most battery-efficient; the OS applies it); on Windows it is applied in software after each advertisement is received, so it filters but saves no power. A device is discovered when it advertises any of the UUIDs
- `RssiThreshold` — reject weak-signal advertisements before they reach your code
- `AdvertisementFilter` — a `Func<IBluetoothAdvertisement, bool>` predicate for anything hardware/RSSI filtering can't express

`IgnoreDuplicateAdvertisements` + `CallbackType` (<xref:Bluetooth.Abstractions.Scanning.Options.BluetoothScanCallbackType>) together decide whether you get one callback per device (discovery) or one per advertisement packet (RSSI/presence tracking).

## Android-Specific Options

<xref:Bluetooth.Abstractions.Scanning.Options.Android.AndroidScanningOptions> — `MatchMode`/`ScanMatchNumber` (API 23+), `ReportDelay` (batches results instead of delivering them immediately), `Phy` and `Legacy` (API 26+).

## Best Practices

- **Always specify `ServiceUuids`** when you know what you're looking for — scanning for everything costs battery and CPU for no benefit. iOS only scans in the background with service UUIDs.
- **Match `ScanMode` to the use case** — `LowLatency` while a user is actively waiting on a device picker, `LowPower` for background monitoring (see [Enumerations](../API-Reference/Enums.md#scan-mode-trade-offs)).
- **Enable duplicate filtering for pure discovery** (`IgnoreDuplicateAdvertisements = true`, `CallbackType = FirstMatch`); disable it when tracking RSSI/presence over time.
- **Set `ScanStartRetry = RetryOptions.Aggressive`** in production to absorb transient scan-start failures.
- **Use `AndroidScanningOptions.ReportDelay`** to batch results and reduce wakeups when you don't need immediate delivery.
- **Layer filters**: native `ServiceUuids` first, then `RssiThreshold`, then `AdvertisementFilter` for anything more complex.

## Related Documentation

- [Dependency Injection](./Dependency-Injection.md)
- [Connection Options](./Connection-Options.md)
- [Scanner](../Core-Concepts/Scanner.md)
- [Advertisement](../Core-Concepts/Advertisement.md)
