namespace Bluetooth.Linux.Scanning.Factories;

/// <summary>
///     Linux-specific factory spec for creating remote devices.
///     Extends the base spec with the native BlueZ <see cref="Device"/> object.
/// </summary>
public record LinuxBluetoothRemoteDeviceFactorySpec : IBluetoothRemoteDeviceFactory.BluetoothRemoteDeviceFactorySpec
{
    /// <summary>
    ///     Initialises a new spec from a BlueZ <paramref name="device"/> and the advertisement that triggered discovery.
    /// </summary>
    public LinuxBluetoothRemoteDeviceFactorySpec(Device device, IBluetoothAdvertisement advertisement)
        : base(advertisement)
    {
        ArgumentNullException.ThrowIfNull(device);
        NativeDevice = device;
    }

    /// <summary>
    ///     The native BlueZ device object. Used for connect, disconnect, and service discovery.
    /// </summary>
    public Device NativeDevice { get; }
}
