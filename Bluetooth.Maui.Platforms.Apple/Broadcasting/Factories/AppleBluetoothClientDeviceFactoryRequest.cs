
using System.Diagnostics.CodeAnalysis;

using Bluetooth.Abstractions.Broadcasting.Factories;

namespace Bluetooth.Maui.Platforms.Apple.Broadcasting.Factories;

/// <inheritdoc/>
public record AppleBluetoothConnectedDeviceSpec : IBluetoothConnectedDeviceFactory.BluetoothConnectedDeviceSpec
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppleBluetoothConnectedDeviceSpec"/> class with the specified Core Bluetooth central device.
    /// </summary>
    /// <param name="cbCentral">The native iOS Core Bluetooth central device from which to create the factory request.</param>
    public AppleBluetoothConnectedDeviceSpec([NotNull]CBCentral cbCentral) : base(cbCentral.Identifier.ToString())
    {
        CbCentral = cbCentral;
        MaxUpdateValueLength = (int)cbCentral.MaximumUpdateValueLength;
    }

    /// <summary>
    /// Gets the native iOS Core Bluetooth central device.
    /// </summary>
    public CBCentral CbCentral { get; init; }
}
