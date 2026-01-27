namespace Bluetooth.Abstractions.Broadcasting;

/// <summary>
/// Represents a Bluetooth broadcast advertisement configuration.
/// </summary>
public interface IBluetoothBroadcasterStartBroadcastingOptions
{
    /// <summary>
    /// Gets the local device name to be included in the advertisement.
    /// </summary>
    string? LocalDeviceName { get; }

    /// <summary>
    /// Gets a value indicating whether the advertising device is connectable.
    /// </summary>
    bool IsConnectable { get; }

    /// <summary>
    /// Gets the manufacturer identifier included in the advertisement data.
    /// </summary>
    ushort? ManufacturerId { get; }

    /// <summary>
    /// Gets the manufacturer-specific data included in the advertisement.
    /// </summary>
    ReadOnlyMemory<byte>? ManufacturerData { get; }

    /// <summary>
    /// Gets the list of service UUIDs advertised by the device.
    /// </summary>
    IReadOnlyList<Guid>? AdvertisedServiceUuids { get; }
}
