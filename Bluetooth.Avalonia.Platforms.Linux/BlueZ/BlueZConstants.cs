namespace Bluetooth.Avalonia.Platforms.Linux.BlueZ;

/// <summary>
///     D-Bus service name, interface, and path constants for BlueZ.
/// </summary>
internal static class BlueZConstants
{
    // ==================== Service ====================

    /// <summary>BlueZ D-Bus well-known name.</summary>
    public const string ServiceName = "org.bluez";

    // ==================== Standard interfaces ====================

    /// <summary>D-Bus ObjectManager interface — used to enumerate all BlueZ objects.</summary>
    public const string ObjectManagerInterface = "org.freedesktop.DBus.ObjectManager";

    /// <summary>D-Bus Properties interface — used to read and watch properties.</summary>
    public const string PropertiesInterface = "org.freedesktop.DBus.Properties";

    // ==================== BlueZ interfaces ====================

    /// <summary>BlueZ adapter interface — power management and discovery.</summary>
    public const string Adapter1Interface = "org.bluez.Adapter1";

    /// <summary>BlueZ device interface — connection and pairing.</summary>
    public const string Device1Interface = "org.bluez.Device1";

    /// <summary>BlueZ GATT service interface.</summary>
    public const string GattService1Interface = "org.bluez.GattService1";

    /// <summary>BlueZ GATT characteristic interface.</summary>
    public const string GattCharacteristic1Interface = "org.bluez.GattCharacteristic1";

    /// <summary>BlueZ GATT descriptor interface.</summary>
    public const string GattDescriptor1Interface = "org.bluez.GattDescriptor1";

    /// <summary>BlueZ LE advertising manager interface — peripheral role advertising.</summary>
    public const string LEAdvertisingManager1Interface = "org.bluez.LEAdvertisingManager1";

    /// <summary>BlueZ GATT manager interface — peripheral role GATT server registration.</summary>
    public const string GattManager1Interface = "org.bluez.GattManager1";

    // ==================== Paths ====================

    /// <summary>D-Bus root path — ObjectManager is exposed here on org.bluez.</summary>
    public const string RootPath = "/";

    // ==================== Property names ====================

    /// <summary>Adapter1 property: whether the adapter is powered on.</summary>
    public const string PropPowered = "Powered";

    /// <summary>Adapter1 property: whether discovery is currently running.</summary>
    public const string PropDiscovering = "Discovering";

    /// <summary>Device1 property: Bluetooth MAC address string.</summary>
    public const string PropAddress = "Address";

    /// <summary>Device1 property: device display name (Alias preferred, then Name).</summary>
    public const string PropAlias = "Alias";

    /// <summary>Device1 property: device Bluetooth name.</summary>
    public const string PropName = "Name";

    /// <summary>Device1 property: RSSI in dBm (int16, only present during advertising).</summary>
    public const string PropRSSI = "RSSI";

    /// <summary>Device1 property: transmit power level in dBm (int16).</summary>
    public const string PropTxPower = "TxPower";

    /// <summary>Device1 property: whether the device is currently connected.</summary>
    public const string PropConnected = "Connected";

    /// <summary>Device1 property: whether GATT service discovery is complete.</summary>
    public const string PropServicesResolved = "ServicesResolved";

    /// <summary>Device1 property: whether the device is connectable (from advertisement).</summary>
    public const string PropConnectable = "Connectable";

    /// <summary>Device1 property: array of advertised service UUID strings.</summary>
    public const string PropUUIDs = "UUIDs";

    /// <summary>Device1 property: manufacturer-specific data dict (uint16 company ID → byte array).</summary>
    public const string PropManufacturerData = "ManufacturerData";

    /// <summary>GattService1 property: service UUID string.</summary>
    public const string PropUUID = "UUID";

    /// <summary>GattService1 property: whether the service is primary.</summary>
    public const string PropPrimary = "Primary";

    /// <summary>GattCharacteristic1 property: characteristic flags array of strings.</summary>
    public const string PropFlags = "Flags";

    /// <summary>GattCharacteristic1 property: current cached value (byte array).</summary>
    public const string PropValue = "Value";

    /// <summary>GattCharacteristic1 property: whether notifications are currently enabled.</summary>
    public const string PropNotifying = "Notifying";

    // ==================== Characteristic flags ====================

    /// <summary>Flag string indicating the characteristic supports reading.</summary>
    public const string FlagRead = "read";

    /// <summary>Flag string indicating the characteristic supports writing (with response).</summary>
    public const string FlagWrite = "write";

    /// <summary>Flag string indicating the characteristic supports writing without response.</summary>
    public const string FlagWriteWithoutResponse = "write-without-response";

    /// <summary>Flag string indicating the characteristic supports notifications.</summary>
    public const string FlagNotify = "notify";

    /// <summary>Flag string indicating the characteristic supports indications.</summary>
    public const string FlagIndicate = "indicate";

    // ==================== D-Bus method names ====================

    /// <summary>ObjectManager method to retrieve all managed objects and their properties.</summary>
    public const string MethodGetManagedObjects = "GetManagedObjects";

    /// <summary>Properties interface method to get a single property value.</summary>
    public const string MethodPropertiesGet = "Get";

    /// <summary>Properties interface method to get all properties of an interface.</summary>
    public const string MethodPropertiesGetAll = "GetAll";

    /// <summary>Adapter1 method to start BLE device discovery.</summary>
    public const string MethodStartDiscovery = "StartDiscovery";

    /// <summary>Adapter1 method to stop BLE device discovery.</summary>
    public const string MethodStopDiscovery = "StopDiscovery";

    /// <summary>Device1 method to initiate a connection.</summary>
    public const string MethodConnect = "Connect";

    /// <summary>Device1 method to terminate a connection.</summary>
    public const string MethodDisconnect = "Disconnect";

    /// <summary>GattCharacteristic1 method to read the characteristic value.</summary>
    public const string MethodReadValue = "ReadValue";

    /// <summary>GattCharacteristic1 method to write the characteristic value.</summary>
    public const string MethodWriteValue = "WriteValue";

    /// <summary>GattCharacteristic1 method to enable characteristic notifications.</summary>
    public const string MethodStartNotify = "StartNotify";

    /// <summary>GattCharacteristic1 method to disable characteristic notifications.</summary>
    public const string MethodStopNotify = "StopNotify";

    // ==================== D-Bus signal names ====================

    /// <summary>ObjectManager signal emitted when objects are added.</summary>
    public const string SignalInterfacesAdded = "InterfacesAdded";

    /// <summary>ObjectManager signal emitted when objects are removed.</summary>
    public const string SignalInterfacesRemoved = "InterfacesRemoved";

    /// <summary>Properties signal emitted when properties change.</summary>
    public const string SignalPropertiesChanged = "PropertiesChanged";
}
