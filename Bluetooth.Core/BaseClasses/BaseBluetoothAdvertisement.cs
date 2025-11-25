namespace Bluetooth.Core.BaseClasses;

/// <inheritdoc cref="IBluetoothAdvertisement" />
public abstract class BaseBluetoothAdvertisement : BaseBindableObject, IBluetoothAdvertisement
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseBluetoothAdvertisement"/> class.
    /// </summary>
    protected BaseBluetoothAdvertisement()
    {
        DateReceived = DateTimeOffset.UtcNow;
        LazyDeviceName = new Lazy<string>(InitDeviceName);
        LazyServicesGuids = new Lazy<IEnumerable<Guid>>(InitServicesGuids);
        LazyIsConnectable = new Lazy<bool>(InitIsConnectable);
        LazyRawSignalStrengthInDBm = new Lazy<int>(InitRawSignalStrengthInDBm);
        LazyTransmitPowerLevelInDBm = new Lazy<int>(InitTransmitPowerLevelInDBm);
        LazyManufacturerData = new Lazy<byte[]>(InitManufacturerData);
        LazyManufacturer = new Lazy<Manufacturer>(InitManufacturer);
        LazyManufacturerId = new Lazy<int>(InitManufacturerId);
    }

    #region IBluetoothAdvertisement Members

    /// <inheritdoc/>
    public DateTimeOffset DateReceived { get; }

    /// <inheritdoc/>
    public string DeviceName => LazyDeviceName.Value;

    /// <inheritdoc/>
    public IEnumerable<Guid> ServicesGuids => LazyServicesGuids.Value;

    /// <inheritdoc/>
    public bool IsConnectable => LazyIsConnectable.Value;

    /// <inheritdoc/>
    public int RawSignalStrengthInDBm => LazyRawSignalStrengthInDBm.Value;

    /// <inheritdoc/>
    public int TransmitPowerLevelInDBm => LazyTransmitPowerLevelInDBm.Value;

    /// <inheritdoc/>
    public string BluetoothAddress => field ??= InitBluetoothAddress();

    /// <inheritdoc/>
    public ReadOnlyMemory<byte> ManufacturerData => LazyManufacturerData.Value;

    /// <inheritdoc/>
    public Manufacturer Manufacturer => LazyManufacturer.Value;

    /// <inheritdoc/>
    public int ManufacturerId => LazyManufacturerId.Value;

    #endregion

    #region Lazy initializers

    private Lazy<string> LazyDeviceName { get; }

    private Lazy<IEnumerable<Guid>> LazyServicesGuids { get; }

    private Lazy<bool> LazyIsConnectable { get; }

    private Lazy<int> LazyRawSignalStrengthInDBm { get; }

    private Lazy<int> LazyTransmitPowerLevelInDBm { get; }

    private Lazy<byte[]> LazyManufacturerData { get; }

    private Lazy<Manufacturer> LazyManufacturer { get; }

    private Lazy<int> LazyManufacturerId { get; }

    #endregion

    #region Abstract init Methods

    /// <summary>
    /// Initializes and returns the device name from the advertisement data.
    /// </summary>
    /// <returns>The device name, or an empty string if not available.</returns>
    protected abstract string InitDeviceName();

    /// <summary>
    /// Initializes and returns the collection of service GUIDs advertised by the device.
    /// </summary>
    /// <returns>An enumerable collection of service GUIDs.</returns>
    protected abstract IEnumerable<Guid> InitServicesGuids();

    /// <summary>
    /// Initializes and returns whether the device is connectable based on the advertisement data.
    /// </summary>
    /// <returns><c>true</c> if the device is connectable; otherwise, <c>false</c>.</returns>
    protected abstract bool InitIsConnectable();

    /// <summary>
    /// Initializes and returns the raw signal strength (RSSI) in dBm from the advertisement.
    /// </summary>
    /// <returns>The signal strength in dBm.</returns>
    protected abstract int InitRawSignalStrengthInDBm();

    /// <summary>
    /// Initializes and returns the transmit power level in dBm from the advertisement data.
    /// </summary>
    /// <returns>The transmit power level in dBm.</returns>
    protected abstract int InitTransmitPowerLevelInDBm();

    /// <summary>
    /// Initializes and returns the manufacturer-specific data from the advertisement.
    /// </summary>
    /// <returns>A byte array containing the manufacturer data.</returns>
    protected abstract byte[] InitManufacturerData();

    /// <summary>
    /// Initializes and returns the Bluetooth address (MAC address) of the device.
    /// </summary>
    /// <returns>The Bluetooth address as a string.</returns>
    protected abstract string InitBluetoothAddress();

    /// <summary>
    /// Initializes and returns the manufacturer by converting the manufacturer ID to the <see cref="Manufacturer"/> enum.
    /// </summary>
    /// <returns>The manufacturer enum value.</returns>
    protected virtual Manufacturer InitManufacturer()
    {
        return (Manufacturer)ManufacturerId;
    }

    /// <summary>
    /// Initializes and returns the manufacturer ID from the first two bytes of the manufacturer data.
    /// </summary>
    /// <returns>The manufacturer ID, or -1 if the data is insufficient.</returns>
    protected virtual int InitManufacturerId()
    {
        if (ManufacturerData.Length < 2)
        {
            return -1;
        }

        return BitConverter.ToInt16(ManufacturerData[..2].Span);
    }

    #endregion
}
