# Interfaces and Abstractions

Every interface's members, XML doc descriptions, and platform-specific `<remarks>` are generated from source in the API reference (see <xref:Bluetooth.Abstractions.Scanning> and <xref:Bluetooth.Abstractions.Broadcasting>) — this page is a map to find the right one, not a duplicate listing (the previous version of this file had drifted badly out of sync with the actual interfaces; see the file's git history if you need the old content for reference).

## Bluetooth.Abstractions

<xref:Bluetooth.Abstractions.IBluetoothAdapter> is an empty marker interface (`INotifyPropertyChanged` only) used for platform DI registration — it does not expose `CreateScanner()`/`CreateBroadcaster()` or adapter-state properties. Scanners and broadcasters are constructor-injected directly (`IBluetoothScanner`, `IBluetoothBroadcaster`), not created from an adapter factory.

## Bluetooth.Abstractions.Scanning — client (central) role

| Interface | Covers |
|-----------|--------|
| <xref:Bluetooth.Abstractions.Scanning.IBluetoothScanner> | Starting/stopping scans, the discovered-device list, advertisement filtering |
| <xref:Bluetooth.Abstractions.Scanning.IBluetoothRemoteDevice> | Connect/disconnect, service discovery, signal strength, MTU/PHY, pairing |
| <xref:Bluetooth.Abstractions.Scanning.IBluetoothRemoteService> | A discovered GATT service and its characteristics |
| <xref:Bluetooth.Abstractions.Scanning.IBluetoothRemoteCharacteristic> | Read/write/listen on a discovered characteristic |
| <xref:Bluetooth.Abstractions.Scanning.IBluetoothRemoteDescriptor> | Read/write on a discovered descriptor |
| <xref:Bluetooth.Abstractions.Scanning.IBluetoothRemoteL2CapChannel> | An open L2CAP channel (Android/iOS/macOS only) |
| <xref:Bluetooth.Abstractions.Scanning.IBluetoothPairingManager> | OS-level pairing/bonding |
| <xref:Bluetooth.Abstractions.Scanning.IBluetoothAdvertisement> | A single received advertisement packet — device name, RSSI, manufacturer data, service UUIDs |
| <xref:Bluetooth.Abstractions.Scanning.IBluetoothNameProvider> | Overriding device name resolution |

Factory interfaces (`IBluetoothRemoteDeviceFactory`, `IBluetoothRemoteServiceFactory`, etc.) are platform-implementation details, not something app code typically calls directly — see <xref:Bluetooth.Abstractions.Scanning> if you're implementing a new platform.

## Bluetooth.Abstractions.Scanning.Profiles

BT SIG service/characteristic profile registration and typed codec access — see <xref:Bluetooth.Abstractions.Scanning.Profiles> for `IBluetoothServiceDefinitionRegistry`, `ICharacteristicCodec<TIn, TOut>`, and the characteristic-accessor interfaces. Covered conceptually in [Service Definitions and Profiles](../Core-Concepts/Service-Definitions-And-Profiles.md).

## Bluetooth.Abstractions.Broadcasting — server (peripheral) role

| Interface | Covers |
|-----------|--------|
| <xref:Bluetooth.Abstractions.Broadcasting.IBluetoothBroadcaster> | Starting/stopping advertising, local service creation, connected-client list |
| <xref:Bluetooth.Abstractions.Broadcasting.IBluetoothConnectedDevice> | A remote central connected to this device while broadcasting |
| <xref:Bluetooth.Abstractions.Broadcasting.IBluetoothLocalService> | A locally-hosted GATT service and its characteristics |
| <xref:Bluetooth.Abstractions.Broadcasting.IBluetoothLocalCharacteristic> | Handling read/write requests, sending notifications on a local characteristic |
| <xref:Bluetooth.Abstractions.Broadcasting.IBluetoothLocalDescriptor> | Handling read/write requests on a local descriptor |

## See Also

- [Overview and Conventions](./README.md)
- [Enumerations](./Enums.md)
- [Events](./Events.md)
- [Exceptions](./Exceptions.md)
- [Generated API Reference](xref:Bluetooth.Abstractions)
