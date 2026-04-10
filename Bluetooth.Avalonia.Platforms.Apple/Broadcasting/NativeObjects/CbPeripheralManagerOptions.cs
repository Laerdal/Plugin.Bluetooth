namespace Bluetooth.Avalonia.Platforms.Apple.Broadcasting.NativeObjects;

/// <summary>
///     Options used when initialising the <c>CBPeripheralManager</c> on iOS and macOS.
/// </summary>
/// <remarks>
///     Mirror of the equivalent type in <c>Bluetooth.Maui.Platforms.Apple</c>, declared
///     independently to keep the Avalonia layer free of MAUI assembly references.
/// </remarks>
public record CbPeripheralManagerOptions
{
    /// <summary>
    ///     Gets or sets a value indicating whether to show a power alert when Bluetooth is off.
    /// </summary>
    public bool ShowPowerAlert { get; init; } = true;

    /// <summary>
    ///     Gets or sets the restore identifier for state preservation (iOS background restore).
    /// </summary>
    public string? RestoreIdentifier { get; init; }
}
