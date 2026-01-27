using Bluetooth.Abstractions.Broadcasting;

namespace Bluetooth.Core.Broadcasting;

/// <inheritdoc />
public record BaseBluetoothBroadcasterStartBroadcastingOptions : IBluetoothBroadcasterStartBroadcastingOptions
{
    /// <inheritdoc />
    public string? LocalDeviceName { get; init; }

    /// <inheritdoc />
    public bool IsConnectable { get; init; }

    /// <inheritdoc />
    public ushort? ManufacturerId { get; init; }

    /// <inheritdoc />
    public ReadOnlyMemory<byte>? ManufacturerData { get; init; }

    /// <inheritdoc />
    public IReadOnlyList<Guid>? AdvertisedServiceUuids { get; init; }
}

/// <inheritdoc />
public record DefaultBluetoothBroadcasterStartBroadcastingOptions : BaseBluetoothBroadcasterStartBroadcastingOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultBluetoothBroadcasterStartBroadcastingOptions"/> class.
    /// </summary>
    public DefaultBluetoothBroadcasterStartBroadcastingOptions()
        : base()
    {
    }
}
