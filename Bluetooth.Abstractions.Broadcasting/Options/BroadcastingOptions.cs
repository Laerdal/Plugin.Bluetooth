namespace Bluetooth.Abstractions.Broadcasting.Options;

/// <summary>
///     Represents broadcasting configuration options.
/// </summary>
public record BroadcastingOptions
{
    /// <summary>
    ///     Gets the local device name to be included in the advertisement.
    /// </summary>
    /// <platforms>
    ///     <platform>Apple</platform>
    /// </platforms>
    public string? LocalDeviceName { get; init; }

    /// <summary>
    ///     Gets a value indicating whether to include the device name in the advertisement.
    /// </summary>
    /// <platforms>
    ///     <platform>Apple</platform>
    /// </platforms>
    public bool IncludeDeviceName { get; init; }

    /// <summary>
    ///     Gets the list of service UUIDs advertised by the device.
    /// </summary>
    /// <platforms>
    ///     <platform>Apple</platform>
    /// </platforms>
    public IReadOnlyList<Guid>? AdvertisedServiceUuids { get; init; }
}
