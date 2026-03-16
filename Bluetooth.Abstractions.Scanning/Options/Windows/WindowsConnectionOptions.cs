namespace Bluetooth.Abstractions.Scanning.Options.Windows;

/// <summary>
///     Windows platform-specific connection options.
/// </summary>
/// <remarks>
///     Windows currently does not expose connection options through the WinRT API.
///     Connection parameters are managed automatically by the Windows Bluetooth stack.
///     This class is provided for consistency and future extensibility.
/// </remarks>
public record WindowsConnectionOptions
{
    // Reserved for future Windows-specific connection options
}