namespace Bluetooth.Linux.Scanning.Factories;

/// <summary>
///     Linux-specific factory spec for creating remote GATT descriptors.
///     Extends the base spec with the native BlueZ proxy.
/// </summary>
public record LinuxBluetoothRemoteDescriptorFactorySpec : IBluetoothRemoteDescriptorFactory.BluetoothRemoteDescriptorFactorySpec
{
    /// <summary>
    ///     Initialises a new spec from a BlueZ descriptor proxy.
    /// </summary>
    /// <param name="descriptorId">The descriptor UUID.</param>
    /// <param name="nativeDescriptor">The native BlueZ D-Bus proxy.</param>
    /// <param name="flags">
    ///     The BlueZ flags string array (e.g. "read", "write").
    ///     Pre-fetched to avoid async calls inside constructors.
    /// </param>
    public LinuxBluetoothRemoteDescriptorFactorySpec(Guid descriptorId, IGattDescriptor1 nativeDescriptor, string[] flags)
        : base(descriptorId)
    {
        ArgumentNullException.ThrowIfNull(nativeDescriptor);
        NativeDescriptor = nativeDescriptor;
        Flags = flags ?? [];
    }

    /// <summary>
    ///     The native BlueZ GATT descriptor D-Bus proxy.
    /// </summary>
    public IGattDescriptor1 NativeDescriptor { get; }

    /// <summary>
    ///     BlueZ property flags such as "read", "write", "encrypt-read", "encrypt-write".
    /// </summary>
    public string[] Flags { get; }
}
