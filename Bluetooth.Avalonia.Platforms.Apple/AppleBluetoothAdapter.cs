namespace Bluetooth.Avalonia.Platforms.Apple;

/// <summary>
///     Apple (iOS / macOS native) implementation of the Bluetooth adapter for Avalonia applications.
/// </summary>
/// <remarks>
///     <para>
///         Both iOS and macOS native use the CoreBluetooth framework.
///         The actual implementation will mirror <c>Bluetooth.Maui.Platforms.Apple.AppleBluetoothAdapter</c>
///         but is declared fresh here to keep the Avalonia layer independent of MAUI packages.
///     </para>
///     <para>
///         Key difference vs MAUI: this adapter targets <c>net10.0-macos</c> (native macOS) in addition
///         to <c>net10.0-ios</c>. MAUI uses <c>net10.0-maccatalyst</c> instead of native macOS.
///     </para>
/// </remarks>
public class AppleBluetoothAdapter : BaseBluetoothAdapter
{
    /// <inheritdoc />
    public AppleBluetoothAdapter(ILogger<IBluetoothAdapter>? logger = null) : base(logger)
    {
    }
}
