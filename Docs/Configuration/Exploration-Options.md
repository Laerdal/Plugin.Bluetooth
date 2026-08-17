# Exploration Options

GATT exploration discovers the structure of services, characteristics, and descriptors on a connected device. Three option classes — <xref:Bluetooth.Abstractions.Scanning.Options.ServiceExplorationOptions>, <xref:Bluetooth.Abstractions.Scanning.Options.CharacteristicExplorationOptions>, <xref:Bluetooth.Abstractions.Scanning.Options.DescriptorExplorationOptions> — control depth, caching, and UUID filtering. See each type's generated page for the full property list.

## GATT Hierarchy

```
Device
├── Service (e.g., Heart Rate Service)
│   ├── Characteristic (e.g., Heart Rate Measurement)
│   │   ├── Descriptor (e.g., Client Characteristic Configuration)
│   │   └── Descriptor (e.g., Characteristic User Description)
│   └── Characteristic (e.g., Body Sensor Location)
│       └── Descriptor
└── Service (e.g., Battery Service)
    └── Characteristic (e.g., Battery Level)
        └── Descriptor
```

<xref:Bluetooth.Abstractions.Scanning.Options.ExplorationDepth> (`ServicesOnly` / `Characteristics` / `Descriptors`) controls how far down this tree exploration goes; `ServiceExplorationOptions.Depth` is a convenience property computed from the underlying `ExploreCharacteristics`/`ExploreDescriptors` booleans.

## Static Factory Methods

Prefer these over manually constructing options:

```csharp
await device.ExploreServicesAsync(ServiceExplorationOptions.ServicesOnly);
await device.ExploreServicesAsync(ServiceExplorationOptions.WithCharacteristics);
await device.ExploreServicesAsync(ServiceExplorationOptions.Full); // + descriptors
```

## Best Practices

- **Only explore as deep as you need** — `WithCharacteristics` instead of `Full` if you don't need descriptors.
- **Filter services when possible** via `ServiceUuidFilter` — targeted exploration is faster than exploring everything.
- **Rely on caching** — repeated calls with the same options reuse the cached result rather than re-exploring.
- **Explore step-by-step for conditional logic** — discover services first, then explore characteristics only on the ones you actually care about:
  ```csharp
  await device.ExploreServicesAsync(ServiceExplorationOptions.ServicesOnly);

  foreach (var service in device.GetServices())
  {
      if (IsRequiredService(service.Uuid))
          await service.ExploreCharacteristicsAsync(CharacteristicExplorationOptions.Full);
  }
  ```
- **Validate required services after discovery**:
  ```csharp
  await device.ExploreServicesAsync(ServiceExplorationOptions.ServicesOnly);

  var missing = requiredServiceUuids.Where(uuid => !device.GetServices().Any(s => s.Uuid == uuid)).ToList();
  if (missing.Count > 0)
      throw new InvalidOperationException($"Device missing required services: {string.Join(", ", missing)}");
  ```

## Related Documentation

- [Dependency Injection](./Dependency-Injection.md)
- [Service Definitions and Profiles](../Core-Concepts/Service-Definitions-And-Profiles.md)
- [Service](../Core-Concepts/Service.md)
