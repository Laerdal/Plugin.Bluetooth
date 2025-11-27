using Bluetooth.Maui.PlatformSpecific;

namespace Bluetooth.Maui;

/// <summary>
/// Represents a Bluetooth Low Energy advertisement packet received from an iOS device.
/// This class wraps iOS's CBPeripheral and advertisement data, providing access to
/// device information, services, signal strength, and manufacturer-specific data.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="BluetoothAdvertisement"/> class from iOS Core Bluetooth objects.
/// </remarks>
/// <param name="peripheral">The Core Bluetooth peripheral that sent the advertisement.</param>
/// <param name="advertisementData">The native iOS advertisement data dictionary.</param>
/// <param name="rssi">The received signal strength indicator value.</param>
public class BluetoothAdvertisement(CBPeripheral peripheral, NSDictionary advertisementData, NSNumber rssi) : BaseBluetoothAdvertisement
{

    /// <summary>
    /// Gets the native iOS Core Bluetooth peripheral device that sent this advertisement.
    /// </summary>
    public CBPeripheral Peripheral { get; } = peripheral;

    /// <summary>
    /// Gets the RSSI (Received Signal Strength Indicator) value in dBm as an NSNumber.
    /// </summary>
    public NSNumber Rssi { get; } = rssi;

    /// <summary>
    /// Gets the parsed advertisement data containing device name, service UUIDs,
    /// manufacturer data, transmit power level, and connectivity status.
    /// </summary>
    public AdvertisementData AdvertisementData { get; } = new AdvertisementData(advertisementData);

    /// <inheritdoc/>
    /// <remarks>
    /// Retrieves the local name from the iOS advertisement data.
    /// Returns an empty string if no local name is available in the advertisement.
    /// </remarks>
    protected override string InitDeviceName()
    {
        return AdvertisementData.LocalName ?? string.Empty;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Extracts service UUIDs from the iOS advertisement data and converts them to GUIDs.
    /// Returns an empty collection if no service UUIDs are advertised.
    /// </remarks>
    protected override IEnumerable<Guid> InitServicesGuids()
    {
        return AdvertisementData.ServiceUuids?.Select(serviceUuid => serviceUuid.ToGuid()) ?? [];
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Returns the connectable status from the iOS advertisement data.
    /// Returns <c>false</c> if the connectable status is not available.
    /// </remarks>
    protected override bool InitIsConnectable()
    {
        return AdvertisementData.IsConnectable ?? false;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Returns the RSSI (Received Signal Strength Indicator) value in dBm from the iOS NSNumber.
    /// </remarks>
    protected override int InitRawSignalStrengthInDBm()
    {
        return Rssi.Int32Value;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Returns the transmit power level from the iOS advertisement data, or 0 if not available.
    /// </remarks>
    protected override int InitTransmitPowerLevelInDBm()
    {
        return AdvertisementData.TxPowerLevel?.Int32Value ?? 0;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Returns the manufacturer-specific data from the iOS advertisement data.
    /// Returns an empty array if no manufacturer data is present.
    /// </remarks>
    protected override byte[] InitManufacturerData()
    {
        return AdvertisementData.ManufacturerData?.ToArray() ?? [];
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Returns the peripheral's unique identifier as a string.
    /// On iOS, this is a UUID rather than a Bluetooth MAC address.
    /// </remarks>
    protected override string InitBluetoothAddress()
    {
        return Peripheral.Identifier.AsString();
    }

    #region Equality

    /// <summary>
    /// Determines whether the specified object is equal to the current advertisement.
    /// </summary>
    /// <param name="obj">The object to compare with the current advertisement.</param>
    /// <returns><c>true</c> if the specified object is equal to the current advertisement; otherwise, <c>false</c>.</returns>
    /// <remarks>
    /// Two advertisements are considered equal if they have the same peripheral identifier and
    /// the native advertisement data dictionaries are equal.
    /// </remarks>
    public override bool Equals(object? obj)
    {
        if (obj is not BluetoothAdvertisement other)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return AdvertisementData.GetHashCode() == other.AdvertisementData.GetHashCode();
    }

    /// <summary>
    /// Returns a hash code for the current advertisement.
    /// </summary>
    /// <returns>A hash code for the current advertisement.</returns>
    public override int GetHashCode()
    {
        return AdvertisementData.GetHashCode();
    }

    #endregion

}
