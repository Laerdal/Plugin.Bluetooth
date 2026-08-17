# Broadcasting Options

`BroadcastingOptions` configures the advertisement content used by `IBluetoothBroadcaster.StartBroadcastingAsync()`. It is intentionally small today — it has exactly three properties, and all three are currently scoped to Apple platforms.

Permission handling is a **separate** parameter (`PermissionOptions`), not a property of `BroadcastingOptions` — see [Permission Handling](#permission-handling) below.

## Table of Contents

- [Overview](#overview)
- [Basic Usage](#basic-usage)
- [Properties](#properties)
  - [LocalDeviceName](#localdevicename)
  - [IncludeDeviceName](#includedevicename)
  - [AdvertisedServiceUuids](#advertisedserviceuuids)
- [Permission Handling](#permission-handling)
- [Platform Support](#platform-support)
- [Related Documentation](#related-documentation)

---

## Overview

`StartBroadcastingAsync` takes both a `BroadcastingOptions?` and a `PermissionOptions?` argument:

```csharp
ValueTask StartBroadcastingAsync(
    BroadcastingOptions? broadcastingOptions = null,
    PermissionOptions? permissionOptions = null,
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);
```

`BroadcastingOptions` currently controls:
- The local device name included in the advertisement
- Whether that name is included at all
- The service UUIDs advertised

**Namespace**: `Bluetooth.Abstractions.Broadcasting.Options`

**Usage pattern**: passed to the method call (not DI-configured)

---

## Basic Usage

```csharp
public class MyBluetoothService
{
    private readonly IBluetoothBroadcaster _broadcaster;

    public MyBluetoothService(IBluetoothBroadcaster broadcaster)
    {
        _broadcaster = broadcaster;
    }

    public async Task StartAdvertisingAsync(CancellationToken cancellationToken)
    {
        var options = new BroadcastingOptions
        {
            LocalDeviceName = "MyDevice",
            IncludeDeviceName = true,
            AdvertisedServiceUuids = new[] { MyServiceUuid }
        };

        await _broadcaster.StartBroadcastingAsync(options, cancellationToken: cancellationToken);
    }
}
```

---

## Properties

### LocalDeviceName

```csharp
public string? LocalDeviceName { get; init; }
```

**Default**: `null`

The local device name to be included in the advertisement.

**Platform support**: Apple only.

### IncludeDeviceName

```csharp
public bool IncludeDeviceName { get; init; }
```

**Default**: `false`

Whether to include the device name in the advertisement.

**Platform support**: Apple only.

### AdvertisedServiceUuids

```csharp
public IReadOnlyList<Guid>? AdvertisedServiceUuids { get; init; }
```

**Default**: `null`

The list of service UUIDs advertised by the device.

**Platform support**: Apple only.

---

## Permission Handling

Permission behavior is controlled by `PermissionOptions.PermissionStrategy` (`Bluetooth.Abstractions.Broadcasting.Options.PermissionOptions`), passed as `StartBroadcastingAsync`'s second argument — **not** a property of `BroadcastingOptions`.

```csharp
public enum PermissionRequestStrategy
{
    RequestAutomatically = 0, // default — requests permissions automatically before broadcasting
    ThrowIfNotGranted = 1,    // throws BluetoothPermissionException if not already granted
    AssumeGranted = 2         // skips all checks; only use if you requested permissions elsewhere
}
```

```csharp
// Automatic (recommended, and the default if you pass no PermissionOptions at all)
await _broadcaster.StartBroadcastingAsync(
    options,
    new PermissionOptions { PermissionStrategy = PermissionRequestStrategy.RequestAutomatically },
    cancellationToken: cancellationToken);

// Custom permission flow
try
{
    await _broadcaster.StartBroadcastingAsync(
        options,
        new PermissionOptions { PermissionStrategy = PermissionRequestStrategy.ThrowIfNotGranted },
        cancellationToken: cancellationToken);
}
catch (BluetoothPermissionException)
{
    await RequestPermissionsManually();
}
```

---

## Platform Support

Today, `BroadcastingOptions` only exposes the three properties above, and all three are Apple-only per their source XML doc annotations. There is currently no cross-platform way through `BroadcastingOptions` itself to control connectability, TX power, manufacturer data, or extended-advertising/PHY settings — if you need platform-specific advertisement knobs beyond device name and service UUIDs, check the platform broadcaster implementation directly (`Bluetooth.Maui.Platforms.Apple/Broadcasting/AppleBluetoothBroadcaster.cs`, `.Droid/Broadcasting/AndroidBluetoothBroadcaster.cs`, `.Win/Broadcasting/WindowsBluetoothBroadcaster.cs`) rather than assuming a documented option exists for it.

Note that Android and Windows both have real, non-stub `IBluetoothBroadcaster` implementations — don't assume broadcasting itself is Apple-only just because `BroadcastingOptions`' current properties are.

---

## Related Documentation

- [Dependency-Injection.md](./Dependency-Injection.md) - DI configuration guide
- [Scanning-Options.md](./Scanning-Options.md) - Scanner configuration
- [Connection-Options.md](./Connection-Options.md) - Connection configuration
