namespace Bluetooth.Core.Scanning;

/// <inheritdoc cref="IBluetoothAdvertisement" />
/// <remarks>
/// This is a readonly struct for memory efficiency. Advertisements are immutable snapshots
/// that are created frequently (thousands per second), so using a value type reduces heap allocations
/// and GC pressure significantly.
/// </remarks>
public readonly record struct BaseBluetoothAdvertisement : IBluetoothAdvertisement
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseBluetoothAdvertisement"/> struct.
    /// </summary>
    /// <param name="deviceName">The device name from the advertisement.</param>
    /// <param name="servicesGuids">The collection of service GUIDs advertised.</param>
    /// <param name="isConnectable">Whether the device is connectable.</param>
    /// <param name="rawSignalStrengthInDBm">The raw signal strength (RSSI) in dBm.</param>
    /// <param name="transmitPowerLevelInDBm">The transmit power level in dBm.</param>
    /// <param name="bluetoothAddress">The Bluetooth address of the device.</param>
    /// <param name="manufacturerData">The manufacturer-specific data.</param>
    public BaseBluetoothAdvertisement(
        string? deviceName,
        IEnumerable<Guid>? servicesGuids,
        bool isConnectable,
        int rawSignalStrengthInDBm,
        int transmitPowerLevelInDBm,
        string? bluetoothAddress,
        ReadOnlyMemory<byte> manufacturerData)
    {
        DeviceName = deviceName ?? string.Empty;
        ServicesGuids = servicesGuids ?? [];
        IsConnectable = isConnectable;
        RawSignalStrengthInDBm = rawSignalStrengthInDBm;
        TransmitPowerLevelInDBm = transmitPowerLevelInDBm;
        BluetoothAddress = bluetoothAddress ?? string.Empty;
        ManufacturerData = manufacturerData;
        DateReceived = DateTimeOffset.UtcNow;
    }

    #region IBluetoothAdvertisement Members

    /// <inheritdoc/>
    public DateTimeOffset DateReceived { get; }

    /// <inheritdoc/>
    public string DeviceName { get; }

    /// <inheritdoc/>
    public IEnumerable<Guid> ServicesGuids { get; }

    /// <inheritdoc/>
    public bool IsConnectable { get; }

    /// <inheritdoc/>
    public int RawSignalStrengthInDBm { get; }

    /// <inheritdoc/>
    public int TransmitPowerLevelInDBm { get; }

    /// <inheritdoc/>
    public string BluetoothAddress { get; }

    /// <inheritdoc/>
    public ReadOnlyMemory<byte> ManufacturerData { get; }

    /// <inheritdoc/>
    public Manufacturer Manufacturer => ManufacturerData.Length >= 2
        ? (Manufacturer)ManufacturerId
        : (Manufacturer)(-1);

    /// <inheritdoc/>
    public int ManufacturerId
    {
        get
        {
            if (ManufacturerData.Length < 2)
            {
                return -1;
            }

            return BitConverter.ToInt16(ManufacturerData[..2].Span);
        }
    }

    #endregion
}
