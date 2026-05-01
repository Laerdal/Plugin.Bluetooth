namespace Bluetooth.Linux.Scanning.Factories;

/// <summary>
///     Linux-specific factory spec for creating remote GATT services.
///     Extends the base spec with the native BlueZ <see cref="IGattService1"/> proxy.
/// </summary>
public record LinuxBluetoothRemoteServiceFactorySpec : IBluetoothRemoteServiceFactory.BluetoothRemoteServiceFactorySpec
{
    /// <summary>
    ///     Initialises a new spec from a BlueZ GATT service proxy.
    /// </summary>
    public LinuxBluetoothRemoteServiceFactorySpec(Guid serviceId, IGattService1 nativeService)
        : base(serviceId)
    {
        ArgumentNullException.ThrowIfNull(nativeService);
        NativeService = nativeService;
    }

    /// <summary>
    ///     The native BlueZ GATT service D-Bus proxy.
    /// </summary>
    public IGattService1 NativeService { get; }
}
