namespace Bluetooth.Maui;

/// <summary>
///     Optional Logging flags to enable/disable parts of the logs
/// </summary>
public static class LoggingFlags
{
    /// <summary>
    /// Logging flag for Bluetooth adapter operations.
    /// </summary>
    public const string BluetoothAdapter = nameof(BluetoothAdapter);

    /// <summary>
    /// Logging flag for Bluetooth scanner operations.
    /// </summary>
    public const string Scanner = nameof(Scanner);

    /// <summary>
    /// Logging flag for Bluetooth scanner ping operations.
    /// </summary>
    public const string ScannerPing = nameof(ScannerPing);

    /// <summary>
    /// Logging flag for native Bluetooth scanner operations.
    /// </summary>
    public const string ScannerNative = nameof(ScannerNative);

    /// <summary>
    /// Logging flag for Bluetooth scanner advertisement operations.
    /// </summary>
    public const string ScannerAdvertisement = nameof(ScannerAdvertisement);

    /// <summary>
    /// Logging flag for Bluetooth broadcaster operations.
    /// </summary>
    public const string Broadcaster = nameof(Broadcaster);

    /// <summary>
    /// Logging flag for native Bluetooth broadcaster operations.
    /// </summary>
    public const string BroadcasterNative = nameof(BroadcasterNative);

    /// <summary>
    /// Logging flag for Bluetooth device operations.
    /// </summary>
    public const string Device = nameof(Device);

    /// <summary>
    /// Logging flag for native Bluetooth device operations.
    /// </summary>
    public const string DeviceNative = nameof(DeviceNative);

    /// <summary>
    /// Logging flag for Bluetooth device ping operations.
    /// </summary>
    public const string DevicePing = nameof(DevicePing);

    /// <summary>
    /// Logging flag for Bluetooth device advertisement operations.
    /// </summary>
    public const string DeviceAdvertisement = nameof(DeviceAdvertisement);

    /// <summary>
    /// Logging flag for Bluetooth device pairing operations.
    /// </summary>
    public const string DevicePairing = nameof(DevicePairing);

    /// <summary>
    /// Logging flag for Bluetooth device connection operations.
    /// </summary>
    public const string DeviceConnection = nameof(DeviceConnection);

    /// <summary>
    /// Logging flag for Bluetooth device battery level operations.
    /// </summary>
    public const string DeviceBatteryLevel = nameof(DeviceBatteryLevel);

    /// <summary>
    /// Logging flag for Bluetooth device signal strength operations.
    /// </summary>
    public const string DeviceSignalStrength = nameof(DeviceSignalStrength);

    /// <summary>
    /// Logging flag for Bluetooth device serial number operations.
    /// </summary>
    public const string DeviceSerialNumber = nameof(DeviceSerialNumber);

    /// <summary>
    /// Logging flag for Bluetooth device name operations.
    /// </summary>
    public const string DeviceName = nameof(DeviceName);

    /// <summary>
    /// Logging flag for Bluetooth device MTU operations.
    /// </summary>
    public const string DeviceMtu = nameof(DeviceMtu);

    /// <summary>
    /// Logging flag for Bluetooth device version operations.
    /// </summary>
    public const string DeviceVersion = nameof(DeviceVersion);

    /// <summary>
    /// Logging flag for Bluetooth device exploration operations.
    /// </summary>
    public const string DeviceExploration = nameof(DeviceExploration);

    /// <summary>
    /// Logging flag for Bluetooth service operations.
    /// </summary>
    public const string Service = nameof(Service);

    /// <summary>
    /// Logging flag for native Bluetooth service operations.
    /// </summary>
    public const string ServiceNative = nameof(ServiceNative);

    /// <summary>
    /// Logging flag for Bluetooth service exploration operations.
    /// </summary>
    public const string ServiceExploration = nameof(ServiceExploration);

    /// <summary>
    /// Logging flag for Bluetooth characteristic operations.
    /// </summary>
    public const string Characteristic = nameof(Characteristic);

    /// <summary>
    /// Logging flag for native Bluetooth characteristic operations.
    /// </summary>
    public const string CharacteristicNative = nameof(CharacteristicNative);

    /// <summary>
    /// Logging flag for Bluetooth characteristic access operations.
    /// </summary>
    public const string CharacteristicAccess = nameof(CharacteristicAccess);

    /// <summary>
    /// Logging flag for Bluetooth characteristic exploration operations.
    /// </summary>
    public const string CharacteristicExploration = nameof(CharacteristicExploration);

    /// <summary>
    /// Logging flag for known services and characteristics operations.
    /// </summary>
    public const string KnownServicesAndCharacteristics = nameof(KnownServicesAndCharacteristics);

    /// <summary>
    /// Logging flag for configuration operations.
    /// </summary>
    public const string Configuration = nameof(Configuration);

    /// <summary>
    /// Logging flag for permission operations.
    /// </summary>
    public const string Permission = nameof(Permission);

    /// <summary>
    /// Logging flag for file system operations.
    /// </summary>
    public const string FileSystem = nameof(FileSystem);

    /// <summary>
    /// Logging flag for native exception operations.
    /// </summary>
    public const string NativeException = nameof(NativeException);
}
