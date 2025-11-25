using System.Text;

using Bluetooth.Maui.PlatformSpecific;

namespace Bluetooth.Maui;

/// <inheritdoc/>
public partial class BluetoothAdvertisement(BluetoothLEAdvertisementReceivedEventArgs args) : BaseBluetoothAdvertisement
{

    /// <summary>
    ///     https://docs.microsoft.com/en-us/uwp/api/windows.devices.bluetooth.advertisement.bluetoothleadvertisementreceivedeventargs
    /// </summary>
    public BluetoothLEAdvertisementReceivedEventArgs BluetoothLeAdvertisementReceivedEventArgs { get; } = args;

    /// <inheritdoc/>
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
    protected override IEnumerable<Guid> InitServicesGuids()
    {
        return BluetoothLeAdvertisementReceivedEventArgs.Advertisement.ServiceUuids;
    }

    /// <inheritdoc/>
    protected override bool InitIsConnectable()
    {
        return BluetoothLeAdvertisementReceivedEventArgs.AdvertisementType is BluetoothLEAdvertisementType.ConnectableDirected or BluetoothLEAdvertisementType.ConnectableUndirected;
    }

    /// <inheritdoc/>
    protected override int InitRawSignalStrengthInDBm()
    {
        return BluetoothLeAdvertisementReceivedEventArgs.RawSignalStrengthInDBm;
    }

    /// <inheritdoc/>
    protected override int InitTransmitPowerLevelInDBm()
    {
        var data = TryGetSectionData(BluetoothLEAdvertisementDataTypes.TxPowerLevel);

        var firstByteOrDefaultToZero = data?[0] ?? 0;

        return (sbyte) firstByteOrDefaultToZero;
    }

    /// <inheritdoc/>
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
    protected override string InitBluetoothAddress()
    {
        return BluetoothLeAdvertisementReceivedEventArgs.BluetoothAddress.ConvertNumericBleAddressToHexBleAddress();
    }

    #region Helpers

    private byte[]? TryGetSectionData(byte recType)
    {
        var section = BluetoothLeAdvertisementReceivedEventArgs.Advertisement.DataSections?.FirstOrDefault(x => x.DataType == recType);
        var data = section?.Data.ToArray();
        return data;
    }

    #endregion
}
