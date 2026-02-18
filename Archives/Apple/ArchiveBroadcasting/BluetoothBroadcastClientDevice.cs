using Bluetooth.Abstractions.Broadcasting.Factories;

namespace Bluetooth.Maui.Platforms.Apple.Broadcasting;

/// <summary>
/// Represents an iOS-specific central device connected to the peripheral.
/// This wraps iOS Core Bluetooth's CBCentral.
/// </summary>
public class BluetoothBroadcastClientDevice : BaseBluetoothBroadcastClientDevice
{
    /// <summary>
    /// Gets the native iOS central device.
    /// </summary>
    public CBCentral NativeCentral { get; }

    /// <inheritdoc/>
    public BluetoothBroadcastClientDevice(IBluetoothBroadcaster broadcaster, IBluetoothBroadcastClientDeviceFactory.BluetoothBroadcastClientDeviceFactoryRequest request) : base(broadcaster, request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request is not BluetoothBroadcastClientDeviceFactoryRequest nativeRequest)
        {
            throw new ArgumentException($"Request must be of type {nameof(BluetoothBroadcastClientDeviceFactoryRequest)}", nameof(request));
        }

        ArgumentNullException.ThrowIfNull(nativeRequest.NativeCentral);
        NativeCentral = nativeRequest.NativeCentral;
    }

    /// <inheritdoc/>
    public override ValueTask DisposeAsync()
    {
        // No specific disposal needed for CBCentral
        return ValueTask.CompletedTask;
    }
}
