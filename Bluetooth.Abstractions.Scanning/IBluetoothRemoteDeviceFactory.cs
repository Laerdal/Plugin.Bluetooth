namespace Bluetooth.Abstractions.Scanning;

/// <summary>
///     Factory for creating platform-specific Bluetooth remote devices.
/// </summary>
/// <remarks>
///     TODO: This is a stub interface. Full factory infrastructure implementation pending.
/// </remarks>
public interface IBluetoothRemoteDeviceFactory
{
    /// <summary>
    ///     Specification for creating a Bluetooth remote device.
    /// </summary>
    public record BluetoothRemoteDeviceFactorySpec(string DeviceId, Manufacturer Manufacturer)
    {
        /// <summary>
        ///     Initializes a new instance from a Bluetooth advertisement.
        /// </summary>
        public BluetoothRemoteDeviceFactorySpec(IBluetoothAdvertisement advertisement)
            : this((advertisement ?? throw new ArgumentNullException(nameof(advertisement))).BluetoothAddress, advertisement.Manufacturer)
        {
        }
    }

    /// <summary>
    ///     Creates a platform-specific Bluetooth remote device.
    /// </summary>
    IBluetoothRemoteDevice Create(IBluetoothScanner scanner, BluetoothRemoteDeviceFactorySpec spec);
}

/// <summary>
///     Factory for creating platform-specific Bluetooth remote services.
/// </summary>
/// <remarks>
///     TODO: This is a stub interface. Full factory infrastructure implementation pending.
/// </remarks>
public interface IBluetoothRemoteServiceFactory
{
    /// <summary>
    ///     Specification for creating a Bluetooth remote service.
    /// </summary>
    public record BluetoothRemoteServiceFactorySpec(Guid ServiceId);

    /// <summary>
    ///     Creates a platform-specific Bluetooth remote service.
    /// </summary>
    IBluetoothRemoteService Create(IBluetoothRemoteDevice device, BluetoothRemoteServiceFactorySpec spec);
}

/// <summary>
///     Factory for creating platform-specific Bluetooth remote characteristics.
/// </summary>
/// <remarks>
///     TODO: This is a stub interface. Full factory infrastructure implementation pending.
/// </remarks>
public interface IBluetoothRemoteCharacteristicFactory
{
    /// <summary>
    ///     Specification for creating a Bluetooth remote characteristic.
    /// </summary>
    public record BluetoothRemoteCharacteristicFactorySpec(Guid CharacteristicId);

    /// <summary>
    ///     Creates a platform-specific Bluetooth remote characteristic.
    /// </summary>
    IBluetoothRemoteCharacteristic Create(IBluetoothRemoteService service, BluetoothRemoteCharacteristicFactorySpec spec);
}

/// <summary>
///     Factory for creating platform-specific Bluetooth remote descriptors.
/// </summary>
/// <remarks>
///     TODO: This is a stub interface. Full factory infrastructure implementation pending.
/// </remarks>
public interface IBluetoothRemoteDescriptorFactory
{
    /// <summary>
    ///     Specification for creating a Bluetooth remote descriptor.
    /// </summary>
    public record BluetoothRemoteDescriptorFactorySpec(Guid DescriptorId);

    /// <summary>
    ///     Creates a platform-specific Bluetooth remote descriptor.
    /// </summary>
    IBluetoothRemoteDescriptor Create(IBluetoothRemoteCharacteristic characteristic, BluetoothRemoteDescriptorFactorySpec spec);
}

/// <summary>
///     Factory for creating platform-specific Bluetooth L2CAP channels.
/// </summary>
/// <remarks>
///     TODO: This is a stub interface. Full factory infrastructure implementation pending.
/// </remarks>
public interface IBluetoothRemoteL2CapChannelFactory
{
    /// <summary>
    ///     Specification for creating a Bluetooth L2CAP channel.
    /// </summary>
    public record BluetoothRemoteL2CapChannelFactorySpec(int Psm);

    /// <summary>
    ///     Creates a platform-specific Bluetooth L2CAP channel.
    /// </summary>
    IBluetoothRemoteL2CapChannel Create(IBluetoothRemoteDevice device, BluetoothRemoteL2CapChannelFactorySpec spec);
}
