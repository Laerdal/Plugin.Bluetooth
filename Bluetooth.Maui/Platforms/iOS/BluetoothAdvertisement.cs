using Bluetooth.Maui.PlatformSpecific;

namespace Bluetooth.Maui;

/// <inheritdoc/>
public class BluetoothAdvertisement(CBPeripheral peripheral, NSDictionary advertisementData, NSNumber rssi) : BaseBluetoothAdvertisement
{

    /// <summary>
    /// The peripheral device.
    /// </summary>
    public CBPeripheral Peripheral { get; } = peripheral;

    /// <summary>
    /// RSSI value of the advertisement.
    /// </summary>
    public NSNumber Rssi { get; } = rssi;

    /// <summary>
    /// The advertisement data.
    /// </summary>
    public AdvertisementData AdvertisementData { get; } = new AdvertisementData(advertisementData);

    /// <inheritdoc/>
    protected override string InitDeviceName()
    {
        return AdvertisementData.LocalName ?? string.Empty;
    }

    /// <inheritdoc/>
    protected override IEnumerable<Guid> InitServicesGuids()
    {
        return AdvertisementData.ServiceUuids?.Select(serviceUuid => serviceUuid.ToGuid()) ?? [];
    }

    /// <inheritdoc/>
    protected override bool InitIsConnectable()
    {
        return AdvertisementData.IsConnectable ?? false;
    }

    /// <inheritdoc/>
    protected override int InitRawSignalStrengthInDBm()
    {
        return Rssi.Int32Value;
    }

    /// <inheritdoc/>
    protected override int InitTransmitPowerLevelInDBm()
    {
        return AdvertisementData.TxPowerLevel?.Int32Value ?? 0;
    }

    /// <inheritdoc/>
    protected override byte[] InitManufacturerData()
    {
        return AdvertisementData.ManufacturerData?.ToArray() ?? [];
    }

    /// <inheritdoc/>
    protected override string InitBluetoothAddress()
    {
        return Peripheral.Identifier.AsString();
    }

}
