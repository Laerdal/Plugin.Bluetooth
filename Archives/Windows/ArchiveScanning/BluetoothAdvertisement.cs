using System.Text;
using Bluetooth.Abstractions.Enums;
using Bluetooth.Abstractions.Scanning;
using Windows.Devices.Bluetooth.Advertisement;

namespace Bluetooth.Maui.Platforms.Windows.Scanning;

/// <summary>
/// Represents a Bluetooth Low Energy advertisement packet received from a Windows device.
/// This readonly struct wraps Windows Bluetooth advertisement data.
/// </summary>
/// <remarks>
/// This is a readonly struct for memory efficiency. Since advertisements arrive by the thousands,
/// using a value type eliminates heap allocations and reduces GC pressure.
/// </remarks>
public readonly record struct BluetoothAdvertisement : IBluetoothAdvertisement
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BluetoothAdvertisement"/> struct from Windows Bluetooth LE event arguments.
    /// For more information, see <see href="https://docs.microsoft.com/en-us/uwp/api/windows.devices.bluetooth.advertisement.bluetoothleadvertisementreceivedeventargs"/>.
    /// </summary>
    /// <param name="args">The Windows Bluetooth LE advertisement received event arguments.</param>
    public BluetoothAdvertisement(BluetoothLEAdvertisementReceivedEventArgs args)
    {
        ArgumentNullException.ThrowIfNull(args);

        DeviceName = ExtractDeviceName(args);
        ServicesGuids = args.Advertisement.ServiceUuids?.ToArray() ?? [];
        IsConnectable = args.AdvertisementType is BluetoothLEAdvertisementType.ConnectableDirected
                                                or BluetoothLEAdvertisementType.ConnectableUndirected;
        RawSignalStrengthInDBm = args.RawSignalStrengthInDBm;
        TransmitPowerLevelInDBm = ExtractTransmitPowerLevel(args);
        BluetoothAddress = ConvertNumericBleAddressToHexBleAddress(args.BluetoothAddress);
        ManufacturerData = ExtractManufacturerData(args);
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

    #region Private Helper Methods

    private static string ExtractDeviceName(BluetoothLEAdvertisementReceivedEventArgs args)
    {
        var data = TryGetSectionData(args, BluetoothLEAdvertisementDataTypes.CompleteLocalName);
        if (data != null)
        {
            return Encoding.UTF8.GetString(data);
        }

        return args.Advertisement.LocalName ?? string.Empty;
    }

    private static int ExtractTransmitPowerLevel(BluetoothLEAdvertisementReceivedEventArgs args)
    {
        var data = TryGetSectionData(args, BluetoothLEAdvertisementDataTypes.TxPowerLevel);
        var firstByteOrDefaultToZero = data?[0] ?? 0;
        return (sbyte)firstByteOrDefaultToZero;
    }

    private static ReadOnlyMemory<byte> ExtractManufacturerData(BluetoothLEAdvertisementReceivedEventArgs args)
    {
        switch (args.AdvertisementType)
        {
            case BluetoothLEAdvertisementType.ConnectableUndirected:
                return TryGetSectionData(args, BluetoothLEAdvertisementDataTypes.ManufacturerSpecificData) ?? ReadOnlyMemory<byte>.Empty;

            case BluetoothLEAdvertisementType.ScanResponse:
                var baseScanResponse = TryGetSectionData(args, BluetoothLEAdvertisementDataTypes.ManufacturerSpecificData);
                // It's perfectly normal that some devices might not advertise manufacturer data at all
                return baseScanResponse ?? ReadOnlyMemory<byte>.Empty;

            case BluetoothLEAdvertisementType.ConnectableDirected:
            case BluetoothLEAdvertisementType.ScannableUndirected:
            case BluetoothLEAdvertisementType.NonConnectableUndirected:
                // Ignoring these advertisement types for manufacturer data
                return ReadOnlyMemory<byte>.Empty;

            default:
                return ReadOnlyMemory<byte>.Empty;
        }
    }

    private static byte[]? TryGetSectionData(BluetoothLEAdvertisementReceivedEventArgs args, byte recType)
    {
        var section = args.Advertisement.DataSections?.FirstOrDefault(x => x.DataType == recType);
        var data = section?.Data.ToArray();
        return data;
    }

    private static string ConvertNumericBleAddressToHexBleAddress(ulong bluetoothAddress)
    {
        // Convert ulong address to hex string format (e.g., "00:11:22:33:44:55")
        var bytes = BitConverter.GetBytes(bluetoothAddress);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }

        // Take only the last 6 bytes (MAC address is 48 bits)
        return string.Join(":", bytes.Skip(2).Select(b => b.ToString("X2")));
    }

    #endregion
}
