using Bluetooth.Maui.Platforms.Apple.Broadcasting.Factories;

namespace Bluetooth.Maui.Platforms.Apple.Broadcasting;

/// <inheritdoc cref="BaseBluetoothConnectedDevice" />
public class AppleBluetoothConnectedDevice : BaseBluetoothConnectedDevice
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AppleBluetoothConnectedDevice" /> class with the specified broadcaster and factory request.
    /// </summary>
    /// <param name="broadcaster">The Bluetooth broadcaster to which this client device is connected.</param>
    /// <param name="request">The factory request containing the information needed to create this client device.</param>
    public AppleBluetoothConnectedDevice(IBluetoothBroadcaster broadcaster, IBluetoothConnectedDeviceFactory.BluetoothConnectedDeviceSpec request) : base(broadcaster, request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request is not AppleBluetoothConnectedDeviceSpec appleRequest)
        {
            throw new ArgumentException($"Expected request of type {typeof(AppleBluetoothConnectedDeviceSpec)}, but got {request.GetType()}");
        }

        CbCentral = appleRequest.CbCentral;
    }

    /// <summary>
    ///     Gets the native iOS Core Bluetooth central device.
    /// </summary>
    public CBCentral CbCentral { get; }

    /// <summary>
    ///     Gets the Bluetooth broadcaster to which this client device is connected, cast to the Apple-specific implementation.
    /// </summary>
    public AppleBluetoothBroadcaster AppleBluetoothBroadcaster => (AppleBluetoothBroadcaster) Broadcaster;

    /// <inheritdoc />
    protected override ValueTask NativeDisconnectAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        // iOS CBPeripheralManager does not provide a direct way to disconnect a specific central device.
        // Disconnection is typically initiated by the central itself or by stopping advertising.
        // As a peripheral, we can't force a central to disconnect - we can only stop providing services.
        throw new NotSupportedException("iOS does not support forcing disconnection of a central device from the peripheral. The central must initiate the disconnection.");
    }
}