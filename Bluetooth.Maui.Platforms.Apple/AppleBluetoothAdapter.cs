using Bluetooth.Maui.Platforms.Apple.Broadcasting.NativeObjects;

namespace Bluetooth.Maui.Platforms.Apple;

/// <summary>
///     iOS implementation of the Bluetooth adapter using Core Bluetooth framework.
/// </summary>
public class AppleBluetoothAdapter : BaseBluetoothAdapter
{
    /// <inheritdoc />
    public AppleBluetoothAdapter(ILogger<IBluetoothAdapter>? logger = null) : base(logger)
    {
    }

    /// <summary>
    ///     Updates <see cref="BaseBluetoothAdapter.IsEnabled" /> from the shared
    ///     <see cref="Scanning.AppleBluetoothScanner" />'s CBCentralManager state.
    /// </summary>
    /// <remarks>
    ///     This adapter has no CBCentralManager of its own - CoreBluetooth power state is only
    ///     observable once one exists, and the scanner already owns the single instance this app
    ///     uses (see <see cref="Scanning.AppleBluetoothScanner.CbCentralManagerWrapper" />). Adding a
    ///     second CBCentralManager purely to track state here would risk interfering with that one's
    ///     background state-restoration setup, so the scanner pushes state into this adapter instead
    ///     (<see cref="Scanning.AppleBluetoothScanner.UpdatedState" />). Consequence: <see cref="BaseBluetoothAdapter.IsEnabled" />
    ///     only reflects reality once scanning has been attempted at least once in this process.
    /// </remarks>
    internal void UpdateIsEnabled(bool isEnabled)
    {
        IsEnabled = isEnabled;
    }
}
