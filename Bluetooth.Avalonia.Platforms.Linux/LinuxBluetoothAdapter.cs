namespace Bluetooth.Avalonia.Platforms.Linux;

/// <summary>
///     Linux / plain .NET implementation of the Bluetooth adapter.
/// </summary>
/// <remarks>
///     BlueZ integration for Linux is not yet implemented.
///     This adapter is a scaffold placeholder that throws <see cref="PlatformNotSupportedException"/>
///     on all operations. PRs adding BlueZ support via a native interop layer are welcome.
/// </remarks>
public class LinuxBluetoothAdapter : BaseBluetoothAdapter
{
    /// <inheritdoc />
    protected LinuxBluetoothAdapter(ILogger<IBluetoothAdapter>? logger = null) : base(logger)
    {
    }
}
