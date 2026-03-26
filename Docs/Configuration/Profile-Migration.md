# Profile Migration

This document explains how to move from the archived characteristic access model to the current profile-based architecture.

## Why This Changed

The archived implementation centered around:
- `CharacteristicAccessService<T>` and related factory helpers
- reflection-based `[ServiceDefinition]` discovery
- UUID-only characteristic lookup
- mutable service metadata assigned after construction

The current implementation replaces that with:
- immutable profile definitions
- pair-keyed characteristic registration by `(serviceId, characteristicId)`
- explicit DI registration through `BluetoothProfileRegistrar`
- typed access through `IBluetoothCharacteristicAccessor<TRead, TWrite>`
- typed listen orchestration through `IBluetoothCharacteristicAccessorListener<TRead>`
- explicit codecs through `ICharacteristicCodec<TRead, TWrite>`

The new model removes ambiguity, is easier to reason about under AOT, and aligns better with the current `IBluetoothRemote*` abstraction surface.

## Concept Mapping

| Archived model | Current model |
|---|---|
| `CharacteristicAccessService<T>` | `IBluetoothCharacteristicAccessor<TRead, TWrite>` |
| `CharacteristicAccessServiceFactory` | `CharacteristicCodecFactory` + `CharacteristicAccessor<TRead, TWrite>` |
| `[ServiceDefinition]` discovery | `BluetoothProfileRegistrar` in DI |
| `CharacteristicAccessServicesRepository` | `IBluetoothProfileRegistry` |
| UUID-only characteristic name lookup | `(serviceId, characteristicId)` lookup |
| `OnNotificationReceived` callback registration | `IBluetoothCharacteristicAccessorListener<TRead>` |

## Registering Profiles

Archived code often depended on automatic discovery. The new model is explicit.

For the built-in SIG profiles:

```csharp
builder.Services.AddBluetoothSigProfiles();
builder.Services.AddBluetoothCoreScanningServices();
```

If you use `AddBluetoothServices()` from `Bluetooth.Maui`, SIG profiles are already included before the core scanning registrations are finalized.

For custom profiles, register a `BluetoothProfileRegistrar`:

```csharp
builder.Services.AddSingleton<BluetoothProfileRegistrar>(_ => registry =>
{
    registry.Register(new BluetoothServiceDefinition(MyServiceId, "My Service"));
    registry.Register(new BluetoothCharacteristicDefinition(
        MyServiceId,
        MyCharacteristicId,
        "My Characteristic"));
});
```

## Defining Typed Accessors

Archived code often looked like this:

```csharp
public readonly static CharacteristicAccessService<byte> BatteryLevel =
    CharacteristicAccessServiceFactory.CreateForByte(batteryLevelId, "Battery Level");
```

The current equivalent is an explicit accessor:

```csharp
public static IBluetoothCharacteristicAccessor<byte, byte> BatteryLevel { get; } =
    new CharacteristicAccessor<byte, byte>(
        serviceId,
        batteryLevelId,
        CharacteristicCodecFactory.ForByte(),
        "Battery Service",
        "Battery Level");
```

This separates:
- profile registration and naming
- typed value conversion
- runtime characteristic resolution

## Reading And Writing

Archived usage:

```csharp
var level = await BatteryServiceDefinition.BatteryLevel.ReadAsync(device, false);
```

Current usage:

```csharp
var level = await BatteryProfile.BatteryLevel.ReadAsync(device, false);
```

Writes follow the same pattern:

```csharp
await myAccessor.WriteAsync(device, value);
```

## Listening For Notifications

The archived callback pattern has been replaced with a listener orchestrator that owns subscription lifecycle.

```csharp
var listener = new CharacteristicAccessorListener<byte>(
    serviceId,
    characteristicId,
    CharacteristicCodecFactory.ForByte(),
    "Battery Service",
    "Battery Level");

await listener.SubscribeAsync(device, value =>
{
    Console.WriteLine($"Battery: {value}");
});
```

Important differences:
- listeners are fan-out callbacks, not bool-return handlers
- notification payloads are decoded from event args, not characteristic cache state
- start/stop listening is tied to first/last subscriber

## Name Resolution

Archived code resolved characteristic names by UUID only, which breaks when the same characteristic UUID appears in multiple services.

The current implementation resolves names with both service and characteristic UUIDs first, then falls back to UUID-only lookup only when that is unambiguous.

This means custom profiles should always register both:
- `BluetoothServiceDefinition`
- `BluetoothCharacteristicDefinition` with the owning service ID

## Device Convenience APIs

The current codebase keeps device-level convenience methods, but they now route through profile accessors instead of the archived repository:
- `ReadBatteryLevelAsync()`
- `ReadFirmwareVersionAsync()`
- `ReadSoftwareVersionAsync()`
- `ReadHardwareVersionAsync()`
- `ReadVersionsAsync()`

If you had custom convenience APIs in subclasses, prefer implementing them in terms of profile accessors rather than direct characteristic traversal.

## What Did Not Carry Forward

These archived behaviors were intentionally not preserved:
- reflection-based service definition discovery by default
- repository fallback objects like `UnknownCharacteristicAccessService`
- silent capability probing that swallowed exceptions
- mutable service metadata assignment after accessor construction

If you still need archive-style object conversion, build a codec adapter around `ICharacteristicCodec<TRead, TWrite>` instead of reintroducing the old repository model.

## Recommended Migration Steps

1. Replace each archived static service definition with a profile class that exposes typed accessors.
2. Register service and characteristic names through `BluetoothProfileRegistrar`.
3. Replace `CharacteristicAccessServiceFactory.*` calls with `CharacteristicCodecFactory.*` plus `CharacteristicAccessor<TRead, TWrite>`.
4. Replace notification callback registration with `CharacteristicAccessorListener<TRead>`.
5. Remove any reliance on reflection-based discovery or UUID-only characteristic identity.
