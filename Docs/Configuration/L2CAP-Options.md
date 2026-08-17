# L2CAP Options

`L2CapChannelOptions` configures the L2CAP (Logical Link Control and Adaptation Protocol) channel represented by `IBluetoothRemoteL2CapChannel`. It controls MTU sizing, whether a background read loop is used, write-flush behavior, and the background read buffer size.

**It does not configure per-operation timeouts.** `OpenAsync`, `CloseAsync`, `ReadAsync`, and `WriteAsync` each accept their own optional `timeout` parameter directly — there is no `OpenTimeout`/`CloseTimeout`/`ReadTimeout`/`WriteTimeout` on the options object.

## Table of Contents

- [Overview](#overview)
- [Configuration](#configuration)
- [Properties](#properties)
  - [Mtu](#mtu)
  - [DefaultMtu](#defaultmtu)
  - [EnableBackgroundReading](#enablebackgroundreading)
  - [AutoFlushWrites](#autoflushwrites)
  - [ReadBufferSize](#readbuffersize)
- [Usage Examples](#usage-examples)
- [Platform Considerations](#platform-considerations)
- [Related Documentation](#related-documentation)

---

## Overview

L2CAP channels provide a connection-oriented data channel over Bluetooth, useful for higher-throughput bulk transfer than individual GATT characteristic writes/notifications. `L2CapChannelOptions` controls:

- The MTU to request, and the fallback value used when the platform can't report the real negotiated MTU
- Whether a background loop pushes incoming data via the `DataReceived` event, or you pull data manually via `ReadAsync`
- Whether writes are flushed immediately after each call
- The buffer size used by the background read loop

**Namespace**: `Bluetooth.Abstractions.Scanning.Options`

**Configuration via**: `IOptions<L2CapChannelOptions>` (DI-configured)

**Platform support**: Android and iOS/macOS have real L2CAP implementations. Windows throws `NotSupportedException` — L2CAP channels are not currently supported on that platform.

---

## Configuration

### Basic Configuration

Configure in your `MauiProgram.cs`:

```csharp
public static MauiApp CreateMauiApp()
{
    var builder = MauiApp.CreateBuilder();

    builder.Services.AddBluetoothServices();

    builder.Services.Configure<L2CapChannelOptions>(options =>
    {
        options.Mtu = null; // let the platform negotiate; falls back to DefaultMtu
        options.DefaultMtu = 512;
        options.EnableBackgroundReading = true;
        options.AutoFlushWrites = true;
        options.ReadBufferSize = 512;
    });

    return builder.Build();
}
```

### Configuration from appsettings.json

```json
{
  "Bluetooth": {
    "L2CAP": {
      "Mtu": null,
      "DefaultMtu": 512,
      "EnableBackgroundReading": true,
      "AutoFlushWrites": true,
      "ReadBufferSize": 512
    }
  }
}
```

Bind in `MauiProgram.cs`:

```csharp
builder.Services.Configure<L2CapChannelOptions>(
    builder.Configuration.GetSection("Bluetooth:L2CAP"));
```

### Accessing in Your Code

`L2CapChannelOptions` is consumed by the channel implementation itself (via its constructor); application code doesn't normally need to read it back. Per-operation timeouts are passed explicitly to each call instead:

```csharp
public async Task UseChannelAsync(IBluetoothRemoteL2CapChannel channel, CancellationToken cancellationToken)
{
    await channel.OpenAsync(TimeSpan.FromSeconds(30), cancellationToken);

    channel.DataReceived += (_, args) => ProcessReceivedData(args.Data);

    await channel.WriteAsync(data, TimeSpan.FromSeconds(10), cancellationToken);

    await channel.CloseAsync(cancellationToken: cancellationToken);
}
```

---

## Properties

### Mtu

```csharp
public int? Mtu { get; init; }
```

**Default**: `null`

The maximum transmission unit (MTU) for the L2CAP channel. If `null`, a platform-determined default is used (see [`DefaultMtu`](#defaultmtu)).

### DefaultMtu

```csharp
public int DefaultMtu { get; init; } = 512;
```

**Default**: `512 bytes`

The default MTU to use when the platform cannot determine it automatically. This matters most on iOS/macOS, where CoreBluetooth does not expose the real negotiated L2CAP MTU to the caller — `DefaultMtu` is used there unconditionally rather than as a fallback.

Larger values generally mean fewer packets and higher throughput for bulk transfers, at the cost of requiring the remote device to support that MTU. Smaller values are more broadly compatible.

### EnableBackgroundReading

```csharp
public bool EnableBackgroundReading { get; init; } = true;
```

**Default**: `true`

Controls whether a background loop raises `DataReceived` events as data arrives (push model).

**When `true` (default)**: a background loop continuously reads from the channel and raises `DataReceived` — you don't call `ReadAsync()` yourself.

**When `false`**: you must call `ReadAsync()` explicitly (pull model); `DataReceived` is not raised.

### AutoFlushWrites

```csharp
public bool AutoFlushWrites { get; init; } = true;
```

**Default**: `true`

Controls whether the output stream is automatically flushed after each write. There is no public method to flush manually — this is the only control over flush timing. Real-time/low-latency use cases generally want this `true`; bulk transfers may benefit from `false` where the platform honors it.

### ReadBufferSize

```csharp
public int ReadBufferSize { get; init; } = 512;
```

**Default**: `512 bytes`

The buffer size in bytes used for reading data in the background read loop. A value of `0` falls back to [`DefaultMtu`](#defaultmtu).

---

## Usage Examples

### Standard Configuration

```csharp
builder.Services.Configure<L2CapChannelOptions>(options =>
{
    options.DefaultMtu = 512;
    options.EnableBackgroundReading = true;
    options.AutoFlushWrites = true;
    options.ReadBufferSize = 512;
});
```

### Bulk Transfer (Firmware Update / File Transfer)

```csharp
builder.Services.Configure<L2CapChannelOptions>(options =>
{
    // Larger MTU for throughput
    options.DefaultMtu = 4096;

    // Let the platform batch writes where it can
    options.AutoFlushWrites = false;

    // Larger read buffer to match
    options.ReadBufferSize = 4096;
});
```

Pass generous per-call timeouts explicitly for this kind of workload rather than configuring them here:

```csharp
await channel.OpenAsync(TimeSpan.FromSeconds(60), cancellationToken);
await channel.WriteAsync(largeChunk, TimeSpan.FromMinutes(2), cancellationToken);
```

### Pull-Based Reading

```csharp
builder.Services.Configure<L2CapChannelOptions>(options =>
{
    options.EnableBackgroundReading = false;
});

// Elsewhere:
var buffer = new byte[1024];
var bytesRead = await channel.ReadAsync(buffer, TimeSpan.FromSeconds(10), cancellationToken);
```

---

## Platform Considerations

### iOS/macOS

- CoreBluetooth does not expose the real negotiated L2CAP MTU — `DefaultMtu` is always used in its place.
- Has a real L2CAP channel implementation (`Bluetooth.Maui.Platforms.Apple/Scanning/AppleBluetoothRemoteL2CapChannel.cs`).

### Android

- Has a real L2CAP channel implementation (`Bluetooth.Maui.Platforms.Droid/Scanning/AndroidBluetoothRemoteL2CapChannel.cs`).

### Windows

- **Not supported.** `NativeOpenL2CapChannelAsync` throws `NotSupportedException("L2CAP channels are not currently supported on Windows.")`. Don't attempt to use L2CAP channels on Windows targets.

---

## Related Documentation

- [Dependency-Injection.md](./Dependency-Injection.md) - DI configuration guide
- [Connection-Options.md](./Connection-Options.md) - Connection configuration
- [Scanning-Options.md](./Scanning-Options.md) - Scanner configuration
