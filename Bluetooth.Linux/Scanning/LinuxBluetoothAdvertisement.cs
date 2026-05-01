namespace Bluetooth.Linux.Scanning;

/// <summary>
///     Represents a BLE advertisement discovered via BlueZ on Linux.
///     Wraps the device properties exposed by BlueZ over D-Bus at discovery time.
/// </summary>
/// <remarks>
///     BlueZ does not surface raw advertisement PDUs — it exposes parsed properties
///     (Name, RSSI, ServiceUUIDs, ManufacturerData) on the D-Bus Device1 interface.
///     This struct assembles an <see cref="IBluetoothAdvertisement"/> from those properties.
/// </remarks>
public readonly record struct LinuxBluetoothAdvertisement : IBluetoothAdvertisement
{
    /// <summary>
    ///     Initialises a new <see cref="LinuxBluetoothAdvertisement"/> from BlueZ device properties.
    /// </summary>
    /// <param name="address">The device MAC address in "XX:XX:XX:XX:XX:XX" format as reported by BlueZ.</param>
    /// <param name="name">Optional device local name.</param>
    /// <param name="rssi">RSSI in dBm.</param>
    /// <param name="txPower">Transmit power in dBm (0 if unavailable).</param>
    /// <param name="serviceUuids">Advertised service UUIDs.</param>
    /// <param name="manufacturerData">Raw manufacturer-specific data bytes (may be empty).</param>
    /// <param name="isConnectable">Whether the device is connectable.</param>
    public LinuxBluetoothAdvertisement(
        string address,
        string? name,
        short rssi,
        short txPower,
        IEnumerable<Guid>? serviceUuids,
        ReadOnlyMemory<byte> manufacturerData,
        bool isConnectable)
    {
        BluetoothAddress = address;
        DeviceName = name ?? string.Empty;
        RawSignalStrengthInDBm = rssi;
        TransmitPowerLevelInDBm = txPower;
        ServicesGuids = serviceUuids?.ToArray() ?? [];
        ManufacturerData = manufacturerData;
        IsConnectable = isConnectable;
        DateReceived = DateTimeOffset.UtcNow;
    }

    /// <inheritdoc />
    public DateTimeOffset DateReceived { get; }

    /// <inheritdoc />
    public string DeviceName { get; }

    /// <inheritdoc />
    public IEnumerable<Guid> ServicesGuids { get; }

    /// <inheritdoc />
    public bool IsConnectable { get; }

    /// <inheritdoc />
    public int RawSignalStrengthInDBm { get; }

    /// <inheritdoc />
    public int TransmitPowerLevelInDBm { get; }

    /// <inheritdoc />
    public string BluetoothAddress { get; }

    /// <inheritdoc />
    public ReadOnlyMemory<byte> ManufacturerData { get; }

    /// <inheritdoc />
    public Manufacturer Manufacturer => ManufacturerData.Length >= 2
        ? (Manufacturer) ManufacturerId
        : (Manufacturer) (-1);

    /// <inheritdoc />
    public int ManufacturerId
    {
        get
        {
            if (ManufacturerData.Length < 2)
                return -1;

            return BitConverter.ToInt16(ManufacturerData[..2].Span);
        }
    }
}
