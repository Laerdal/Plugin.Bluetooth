# Profile Migration

This document explains how to move from the archived characteristic access model to the current service-definition-based architecture.

## Why This Changed

The archived implementation centered around:

- `CharacteristicAccessService<T>` and related factory helpers
- reflection-based `[ServiceDefinition]` discovery
- UUID-only characteristic lookup
- mutable service metadata assigned after construction

The current implementation replaces that with:

- immutable service definitions
- pair-keyed characteristic registration by `(serviceId, characteristicId)`
- explicit DI registration through `BluetoothServiceDefinitionRegistrarDelegate`
- typed access through `IBluetoothCharacteristicAccessor<TRead, TWrite>`
- typed listen orchestration through `IBluetoothCharacteristicAccessorListener<TRead>`
- explicit codecs through `ICharacteristicCodec<TRead, TWrite>`

The new model removes ambiguity, is easier to reason about under AOT, and aligns better with the current `IBluetoothRemote*` abstraction surface.

## Concept Mapping

| Archived model | Current model |
| --- | --- |
| `CharacteristicAccessService<T>` | `IBluetoothCharacteristicAccessor<TRead, TWrite>` |
| `CharacteristicAccessServiceFactory` | `CharacteristicCodecFactory` + `CharacteristicAccessor<TRead, TWrite>` |
| `[ServiceDefinition]` discovery | `BluetoothServiceDefinitionRegistrarDelegate` in DI |
| `CharacteristicAccessServicesRepository` | `IBluetoothServiceDefinitionRegistry` |
| UUID-only characteristic name lookup | `(serviceId, characteristicId)` lookup |
| `OnNotificationReceived` callback registration | `IBluetoothCharacteristicAccessorListener<TRead>` |

## Registering Profiles

Archived code often depended on automatic discovery. The new model is explicit.

For the built-in SIG service definitions:

```csharp
builder.Services.AddBluetoothSigProfiles();
builder.Services.AddBluetoothCoreScanningServices();
```

If you use `AddBluetoothServices()` from `Bluetooth.Maui`, SIG service definitions are already included before the core scanning registrations are finalized.

For custom service definitions, register the definition type through `BluetoothServiceDefinitionRegistrar`:

```csharp
builder.Services.AddSingleton<BluetoothServiceDefinitionRegistration>(_ => registry =>
{
    BluetoothServiceDefinitionRegistrar.Register(registry, typeof(MyServiceDefinition));
});
```

## Defining Typed Accessors

Archived code often looked like this:

```csharp
public readonly static CharacteristicAccessService<byte> BatteryLevel =
    CharacteristicAccessServiceFactory.CreateForByte(batteryLevelId, "Battery Level");
```

The current equivalent can stay concise while keeping direct static access:

```csharp
[BluetoothServiceDefinition]
public static class BatteryServiceDefinition
{
    public static readonly string Name = "Battery Service";
    public static readonly Guid Id = BluetoothServiceDefinitions.FromShortId(0x180F);
    public static readonly IBluetoothCharacteristicAccessor<byte, byte> BatteryLevel =
        BluetoothServiceDefinitions.ByteCharacteristic(Id, 0x2A19, Name, "Battery Level");
}
```

This separates:

- service definition registration and naming
- typed value conversion
- runtime characteristic resolution

while preserving direct static access such as `await BatteryServiceDefinition.BatteryLevel.ReadAsync(device)`

## Reading And Writing

Archived usage:

```csharp
var level = await BatteryServiceDefinition.BatteryLevel.ReadAsync(device, false);
```

Current usage remains intuitive:

```csharp
var level = await BatteryServiceDefinition.BatteryLevel.ReadAsync(device, false);
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

The current codebase keeps device-level convenience methods, but they now route through service definition accessors instead of the archived repository:

- `ReadBatteryLevelAsync()`
- `ReadFirmwareVersionAsync()`
- `ReadSoftwareVersionAsync()`
- `ReadHardwareVersionAsync()`
- `ReadVersionsAsync()`

If you had custom convenience APIs in subclasses, prefer implementing them in terms of service definition accessors rather than direct characteristic traversal.

## What Did Not Carry Forward

These archived behaviors were intentionally not preserved:

- reflection-based service definition discovery by default
- repository fallback objects like `UnknownCharacteristicAccessService`
- silent capability probing that swallowed exceptions
- mutable service metadata assignment after accessor construction

If you still need archive-style object conversion, build a codec adapter around `ICharacteristicCodec<TRead, TWrite>` instead of reintroducing the old repository model.

## Recommended Migration Steps

1. Replace each archived static service definition with a service definition class that exposes typed accessors.
2. Register service and characteristic names through `BluetoothServiceDefinitionRegistrarDelegate`.
3. Replace `CharacteristicAccessServiceFactory.*` calls with `CharacteristicCodecFactory.*` plus `CharacteristicAccessor<TRead, TWrite>`.
4. Replace notification callback registration with `CharacteristicAccessorListener<TRead>`.
5. Remove any reliance on reflection-based discovery or UUID-only characteristic identity.
