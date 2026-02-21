namespace Bluetooth.Abstractions.Scanning;

/// <summary>
///     Interface representing a Bluetooth advertisement, providing properties for accessing advertisement data received from a Bluetooth device.
/// </summary>
public interface IBluetoothAdvertisement
{
    /// <summary>
    ///     A timestamp for the point in time in which each Advertisement was received.
    /// </summary>
    DateTimeOffset DateReceived { get; }

    /// <summary>
    ///     Name of the device sending that advertisement.
    ///     Android : ScanResult.ScanRecord?.DeviceName
    ///     iOS : AdvertisementData.LocalName
    ///     Windows : BluetoothLeAdvertisementReceivedEventArgs.Advertisement?.GetDeviceName()
    /// </summary>
    string DeviceName { get; }

    /// <summary>
    ///     Gets the Bluetooth address of the device sending the Bluetooth LE advertisement.
    ///     Android : ScanResult.ScanRecord?.ServiceUuids
    ///     iOS : AdvertisementData.ServiceUuids
    ///     Windows : BluetoothLeAdvertisementReceivedEventArgs.Advertisement?.GetServiceUuids()
    /// </summary>
    IEnumerable<Guid> ServicesGuids { get; }

    /// <summary>
    ///     Indicates whether the received advertisement is connectable.
    ///     Android : ScanResult.ScanRecord?.IsConnectable [not available prior to API 8.0 (API level 26)]
    ///     iOS : AdvertisementData.IsConnectable
    ///     Windows : BluetoothLeAdvertisementReceivedEventArgs.AdvertisementType is
    ///     BluetoothLEAdvertisementType.ConnectableDirected or BluetoothLEAdvertisementType.ConnectableUndirected
    /// </summary>
    bool IsConnectable { get; }

    /// <summary>
    ///     Raw Signal Strength In DBm.
    ///     Android : ScanResult.Rssi
    ///     iOS : Rssi.Int32Value
    ///     Windows : BluetoothLeAdvertisementReceivedEventArgs.RawSignalStrengthInDBm
    /// </summary>
    int RawSignalStrengthInDBm { get; }

    /// <summary>
    ///     Represents the received transmit power of the advertisement.
    ///     Android : ScanResult.ScanRecord?.TxPowerLevel
    ///     iOS : AdvertisementData.TxPowerLevel?.Int32Value
    ///     Windows : BluetoothLeAdvertisementReceivedEventArgs.Advertisement?.GetTxPower()
    /// </summary>
    int TransmitPowerLevelInDBm { get; }

    /// <summary>
    ///     Gets the Bluetooth address of the device sending the Bluetooth LE advertisement.
    ///     Android : ScanResult.Device?.Address
    ///     iOS : Peripheral.Identifier.AsString()
    ///     Windows : BluetoothLeAdvertisementReceivedEventArgs.BluetoothAddress
    /// </summary>
    string BluetoothAddress { get; }

    /// <summary>
    ///     Gets the BLE advertisement-payload-data that was received.
    ///     <br /><br />
    ///     Note that this byte array is normalized to enforce same byte-layout across all platforms (iOS, Android, Windows).
    ///     <br /><br />
    ///     This property has been introduced for the sake of backwards compatibility back in 2020 when the BLE library was
    ///     being consolidated.
    ///     <br /><br />
    ///     The byte-layout has been inspired by the one that iOS offers natively in its APIs:
    ///     <br /><br />
    ///     [2 bytes of manufacturer-id in reserve order] [one byte of device-id] [... chunks of manufacturer-data here]
    ///     <br /><br />
    ///     Note: Don't resort to byte-sniffing this byte-array unless maybe for debugging purposes. when it comes to this
    ///     particular property
    ///     these libs' maintainers can't and won't provide any guarantees in terms of compatibility.
    /// </summary>
    ReadOnlyMemory<byte> ManufacturerData { get; }

    /// <summary>
    ///     Gets the Primary Manufacturer based on the "Manufacturer Data" sectors in BLE "Advertising-Messages" and BLE
    ///     "Scan-Response-Messages".
    ///     <br /><br />
    ///     Corresponds to the bytes [0] and [1] of the "Manufacturer Data" sector.
    /// </summary>
    Manufacturer Manufacturer { get; }

    /// <summary>
    ///     Gets the ID of the manufacturer.
    /// </summary>
    int ManufacturerId { get; }
}