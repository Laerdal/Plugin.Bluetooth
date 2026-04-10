namespace Bluetooth.Avalonia.Platforms.Apple.NativeObjects;

/// <summary>
///     Options used when initialising the <c>CBCentralManager</c> on iOS and macOS.
/// </summary>
/// <remarks>
///     Mirror of the equivalent type in <c>Bluetooth.Maui.Platforms.Apple</c>, declared
///     independently to keep the Avalonia layer free of MAUI assembly references.
/// </remarks>
public record CbCentralInitOptions
{
    /// <summary>
    ///     Gets or sets a value indicating whether to restore the central manager state on app relaunch.
    /// </summary>
    public bool ShowPowerAlert { get; init; } = true;

    /// <summary>
    ///     Gets or sets the restore identifier for state preservation (iOS background restore).
    /// </summary>
    public string? RestoreIdentifier { get; init; }
}
