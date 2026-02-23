using System.Diagnostics.CodeAnalysis;

namespace Bluetooth.Maui.Platforms.Apple.Broadcasting.Factories;

/// <summary>
///     Apple-specific specification for creating a connected client device.
/// </summary>
public record AppleBluetoothConnectedDeviceSpec : IBluetoothConnectedDeviceFactory.BluetoothConnectedDeviceSpec
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AppleBluetoothConnectedDeviceSpec" /> record.
    /// </summary>
    /// <param name="cbCentral">The native iOS Core Bluetooth central device.</param>
    public AppleBluetoothConnectedDeviceSpec(CBCentral cbCentral) : base(cbCentral?.Identifier.ToString() ?? throw new ArgumentNullException(nameof(cbCentral)))
    {
        CbCentral = cbCentral;
        MaxUpdateValueLength = (int) cbCentral.MaximumUpdateValueLength;
    }

    /// <summary>
    ///     Gets the native iOS Core Bluetooth central device.
    /// </summary>
    public CBCentral CbCentral { get; init; }
}
