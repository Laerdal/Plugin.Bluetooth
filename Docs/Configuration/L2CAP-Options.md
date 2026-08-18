# L2CAP Options

`L2CapChannelOptions` (<xref:Bluetooth.Abstractions.Scanning.Options.L2CapChannelOptions>) configures the L2CAP channel represented by <xref:Bluetooth.Abstractions.Scanning.IBluetoothRemoteL2CapChannel> — a connection-oriented channel useful for higher-throughput bulk transfer than individual GATT characteristic writes. It controls MTU sizing, whether a background read loop is used, write-flush behavior, and the background read buffer size. DI-configured via `IOptions<L2CapChannelOptions>`.

**It does not configure per-operation timeouts** — `OpenAsync`, `CloseAsync`, `ReadAsync`, and `WriteAsync` each accept their own optional `timeout` parameter directly.

## Basic Configuration

```csharp
builder.Services.Configure<L2CapChannelOptions>(options =>
{
    options.Mtu = null; // let the platform negotiate; falls back to DefaultMtu
    options.DefaultMtu = 512;
    options.EnableBackgroundReading = true;
    options.AutoFlushWrites = true;
    options.ReadBufferSize = 512;
});
```

```csharp
await channel.OpenAsync(TimeSpan.FromSeconds(30), cancellationToken);
channel.DataReceived += (_, args) => ProcessReceivedData(args.Data);
await channel.WriteAsync(data, TimeSpan.FromSeconds(10), cancellationToken);
await channel.CloseAsync(cancellationToken: cancellationToken);
```

## Bulk Transfer (Firmware Update / File Transfer)

```csharp
builder.Services.Configure<L2CapChannelOptions>(options =>
{
    options.DefaultMtu = 4096;        // larger MTU for throughput
    options.AutoFlushWrites = false;  // let the platform batch writes where it can
    options.ReadBufferSize = 4096;
});

await channel.OpenAsync(TimeSpan.FromSeconds(60), cancellationToken);
await channel.WriteAsync(largeChunk, TimeSpan.FromMinutes(2), cancellationToken);
```

## Pull-Based Reading

```csharp
builder.Services.Configure<L2CapChannelOptions>(options => options.EnableBackgroundReading = false);

var buffer = new byte[1024];
var bytesRead = await channel.ReadAsync(buffer, TimeSpan.FromSeconds(10), cancellationToken);
```

## Platform Considerations

- **iOS/macOS**: real implementation, but CoreBluetooth doesn't expose the real negotiated L2CAP MTU — `DefaultMtu` is always used in its place.
- **Android**: real implementation, negotiated MTU is reported correctly.
- **Windows**: **not supported.** `NativeOpenL2CapChannelAsync` throws `NotSupportedException`.

## Related Documentation

- [Dependency Injection](./Dependency-Injection.md)
- [Connection Options](./Connection-Options.md)
- [Scanning Options](./Scanning-Options.md)
