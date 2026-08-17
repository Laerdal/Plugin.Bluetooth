# Broadcasting Options

`BroadcastingOptions` (<xref:Bluetooth.Abstractions.Broadcasting.Options.BroadcastingOptions>) configures the advertisement content used by `IBluetoothBroadcaster.StartBroadcastingAsync()`. It's intentionally small today — exactly three properties (`LocalDeviceName`, `IncludeDeviceName`, `AdvertisedServiceUuids`), and **all three are currently scoped to Apple platforms only** per their source XML doc annotations.

Permission handling is a **separate** parameter (`PermissionOptions`), not a property of `BroadcastingOptions` — see [Permission Handling](#permission-handling) below.

## Basic Usage

```csharp
var options = new BroadcastingOptions
{
    LocalDeviceName = "MyDevice",
    IncludeDeviceName = true,
    AdvertisedServiceUuids = new[] { MyServiceUuid }
};

await broadcaster.StartBroadcastingAsync(options, cancellationToken: cancellationToken);
```

## Permission Handling

Permission behavior is controlled by <xref:Bluetooth.Abstractions.Broadcasting.Options.PermissionOptions>`.PermissionStrategy`, passed as `StartBroadcastingAsync`'s **second** argument — not a property of `BroadcastingOptions`.

```csharp
// Automatic (default if you pass no PermissionOptions at all)
await broadcaster.StartBroadcastingAsync(
    options,
    new PermissionOptions { PermissionStrategy = PermissionRequestStrategy.RequestAutomatically },
    cancellationToken: cancellationToken);

// Custom permission flow
try
{
    await broadcaster.StartBroadcastingAsync(
        options,
        new PermissionOptions { PermissionStrategy = PermissionRequestStrategy.ThrowIfNotGranted },
        cancellationToken: cancellationToken);
}
catch (BluetoothPermissionException)
{
    await RequestPermissionsManually();
}
```

## Platform Support

`BroadcastingOptions` has no cross-platform way to control connectability, TX power, manufacturer data, or extended-advertising/PHY settings — if you need platform-specific advertisement knobs beyond device name and service UUIDs, check the platform broadcaster implementation directly (`AppleBluetoothBroadcaster`, `AndroidBluetoothBroadcaster`, `WindowsBluetoothBroadcaster`) rather than assuming a documented option exists for it.

**Broadcasting itself is not Apple-only** — Android and Windows both have real, non-stub `IBluetoothBroadcaster` implementations (see [Windows](../Platforms/Windows.md)). Only `BroadcastingOptions`' current three properties happen to be Apple-scoped.

## Related Documentation

- [Dependency Injection](./Dependency-Injection.md)
- [Scanning Options](./Scanning-Options.md)
- [Connection Options](./Connection-Options.md)
- [Broadcaster](../Core-Concepts/Broadcaster.md)
