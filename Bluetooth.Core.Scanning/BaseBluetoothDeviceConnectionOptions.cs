using Bluetooth.Abstractions.Scanning;

namespace Bluetooth.Core.Scanning;

/// <inheritdoc />
public record BaseBluetoothDeviceConnectionOptions : IBluetoothDeviceConnectionOptions
{
    /// <inheritdoc />
    public bool WaitForAdvertisementBeforeConnecting { get; init; }
}
