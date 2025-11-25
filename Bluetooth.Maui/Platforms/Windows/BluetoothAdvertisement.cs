using System.Text;

using Bluetooth.Maui.PlatformSpecific;

namespace Bluetooth.Maui;

/// <summary>
/// Represents a Bluetooth Low Energy advertisement packet received from a Windows device.
/// This class wraps Windows's BluetoothLEAdvertisementReceivedEventArgs and provides access to
/// advertisement data including device information, services, signal strength, and manufacturer-specific data.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="BluetoothAdvertisement"/> class from Windows Bluetooth LE event arguments.
/// For more information, see <see href="https://docs.microsoft.com/en-us/uwp/api/windows.devices.bluetooth.advertisement.bluetoothleadvertisementreceivedeventargs"/>.
/// </remarks>
/// <param name="args">The Windows Bluetooth LE advertisement received event arguments.</param>
public partial class BluetoothAdvertisement(BluetoothLEAdvertisementReceivedEventArgs args) : BaseBluetoothAdvertisement
{

    /// <summary>
    /// Gets the native Windows Bluetooth LE advertisement received event arguments containing all advertisement data.
    /// </summary>
    /// <remarks>
    /// For more information, see <see href="https://docs.microsoft.com/en-us/uwp/api/windows.devices.bluetooth.advertisement.bluetoothleadvertisementreceivedeventargs"/>.
    /// </remarks>
    public BluetoothLEAdvertisementReceivedEventArgs BluetoothLeAdvertisementReceivedEventArgs { get; } = args;

    /// <inheritdoc/>
    /// <remarks>
    /// Attempts to retrieve the device name from the CompleteLocalName data section first.
    /// If not available, falls back to the LocalName property of the advertisement.
    /// </remarks>
    protected override string InitDeviceName()
    {
        var data = TryGetSectionData(BluetoothLEAdvertisementDataTypes.CompleteLocalName);
        if (data == null)
        {
            return BluetoothLeAdvertisementReceivedEventArgs.Advertisement.LocalName;
        }

        var name = Encoding.UTF8.GetString(data);
        return name;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Retrieves the list of service UUIDs directly from the Windows advertisement object.
    /// </remarks>
    protected override IEnumerable<Guid> InitServicesGuids()
    {
        return BluetoothLeAdvertisementReceivedEventArgs.Advertisement.ServiceUuids;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Determines if the device is connectable based on the advertisement type.
    /// Returns <c>true</c> for ConnectableDirected or ConnectableUndirected types.
    /// </remarks>
    protected override bool InitIsConnectable()
    {
        return BluetoothLeAdvertisementReceivedEventArgs.AdvertisementType is BluetoothLEAdvertisementType.ConnectableDirected or BluetoothLEAdvertisementType.ConnectableUndirected;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Returns the RSSI (Received Signal Strength Indicator) value in dBm from the Windows event arguments.
    /// </remarks>
    protected override int InitRawSignalStrengthInDBm()
    {
        return BluetoothLeAdvertisementReceivedEventArgs.RawSignalStrengthInDBm;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Extracts the transmit power level from the TxPowerLevel data section.
    /// Returns 0 if the data is not available. The value is cast to a signed byte.
    /// </remarks>
    protected override int InitTransmitPowerLevelInDBm()
    {
        var data = TryGetSectionData(BluetoothLEAdvertisementDataTypes.TxPowerLevel);

        var firstByteOrDefaultToZero = data?[0] ?? 0;

        return (sbyte) firstByteOrDefaultToZero;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Extracts manufacturer-specific data based on the advertisement type.
    /// ConnectableUndirected and ScanResponse types may contain manufacturer data.
    /// Other advertisement types return an empty array.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the advertisement type is not a recognized <see cref="BluetoothLEAdvertisementType"/> value.
    /// </exception>
    protected override byte[] InitManufacturerData()
    {
        switch (BluetoothLeAdvertisementReceivedEventArgs.AdvertisementType)
        {
            case BluetoothLEAdvertisementType.ConnectableUndirected:
                return TryGetSectionData(BluetoothLEAdvertisementDataTypes.ManufacturerSpecificData) ?? [];

            case BluetoothLEAdvertisementType.ScanResponse:
                var baseScanResponse = TryGetSectionData(BluetoothLEAdvertisementDataTypes.ManufacturerSpecificData);
                return baseScanResponse ?? [];

            case BluetoothLEAdvertisementType.ConnectableDirected:
            case BluetoothLEAdvertisementType.ScannableUndirected:
            case BluetoothLEAdvertisementType.NonConnectableUndirected:
                return [];

            default:
                throw new ArgumentOutOfRangeException(nameof(BluetoothLeAdvertisementReceivedEventArgs.AdvertisementType));
        }
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Converts the Windows numeric Bluetooth address to a hexadecimal string format (MAC address).
    /// </remarks>
    protected override string InitBluetoothAddress()
    {
        return BluetoothLeAdvertisementReceivedEventArgs.BluetoothAddress.ConvertNumericBleAddressToHexBleAddress();
    }

    #region Helpers

    /// <summary>
    /// Attempts to retrieve data from a specific advertisement data section.
    /// </summary>
    /// <param name="recType">The type of data section to retrieve (e.g., CompleteLocalName, TxPowerLevel, ManufacturerSpecificData).</param>
    /// <returns>A byte array containing the section data, or <c>null</c> if the section is not found.</returns>
    private byte[]? TryGetSectionData(byte recType)
    {
        var section = BluetoothLeAdvertisementReceivedEventArgs.Advertisement.DataSections?.FirstOrDefault(x => x.DataType == recType);
        var data = section?.Data.ToArray();
        return data;
    }

    #endregion
}
