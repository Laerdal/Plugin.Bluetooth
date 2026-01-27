Android API :

OnMtuChanged(Android.Bluetooth.BluetoothDevice? device, int mtu)
OnExecuteWrite(Android.Bluetooth.BluetoothDevice ? device, int requestId, bool execute)
OnNotificationSent(Android.Bluetooth.BluetoothDevice ? device, GattStatus status)
OnPhyRead(Android.Bluetooth.BluetoothDevice ? device, ScanSettingsPhy txPhy, ScanSettingsPhy rxPhy, GattStatus status)
OnPhyUpdate(Android.Bluetooth.BluetoothDevice ? device, ScanSettingsPhy txPhy, ScanSettingsPhy rxPhy, GattStatus status)
OnConnectionStateChange(Android.Bluetooth.BluetoothDevice ? device, ProfileState status, ProfileState newState)

OnServiceAdded(GattStatus status, BluetoothGattService ? service)

OnCharacteristicReadRequest(Android.Bluetooth.BluetoothDevice? device, int requestId, int offset, BluetoothGattCharacteristic? characteristic)
OnCharacteristicWriteRequest(Android.Bluetooth.BluetoothDevice? device, int requestId, BluetoothGattCharacteristic? characteristic, bool preparedWrite, bool responseNeeded, int offset, byte[]? value)

OnDescriptorReadRequest(Android.Bluetooth.BluetoothDevice? device, int requestId, int offset, BluetoothGattDescriptor? descriptor)
OnDescriptorWriteRequest(Android.Bluetooth.BluetoothDevice? device, int requestId, BluetoothGattDescriptor? descriptor, bool preparedWrite, bool responseNeeded, int offset, byte[]? value)

iOS/MacCatalyst API :

WillRestoreState(NSDictionary dict)
AdvertisingStarted(NSError ? error)
DidOpenL2CapChannel(NSError ? error, CBL2CapChannel ? channel)
DidPublishL2CapChannel(NSError ? error, ushort psm)
DidUnpublishL2CapChannel(NSError ? error, ushort psm)
StateUpdated()

ServiceAdded(CBService service)

CharacteristicSubscribed(CBCentral central, CBCharacteristic characteristic)
CharacteristicUnsubscribed(CBCentral central, CBCharacteristic characteristic)
ReadRequestReceived(CBATTRequest request)
WriteRequestsReceived(CBATTRequest[] requests)


Windows API :

OnAdvertisementPublisherStatusChanged(BluetoothLEAdvertisementPublisherStatus status)
