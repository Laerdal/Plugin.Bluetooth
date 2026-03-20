namespace Bluetooth.Abstractions.Broadcasting;

/// <summary>
///     Factory for creating platform-specific Bluetooth local services.
/// </summary>
/// <remarks>
///     TODO: This is a stub interface. Full factory infrastructure implementation pending.
/// </remarks>
public interface IBluetoothLocalServiceFactory
{
    /// <summary>
    ///     Specification for creating a Bluetooth local service.
    /// </summary>
    public record BluetoothLocalServiceSpec(Guid ServiceId, string? Name, bool IsPrimary);

    /// <summary>
    ///     Creates a platform-specific Bluetooth local service.
    /// </summary>
    IBluetoothLocalService Create(IBluetoothBroadcaster broadcaster, BluetoothLocalServiceSpec spec);
}

/// <summary>
///     Factory for creating platform-specific Bluetooth local characteristics.
/// </summary>
/// <remarks>
///     TODO: This is a stub interface. Full factory infrastructure implementation pending.
/// </remarks>
public interface IBluetoothLocalCharacteristicFactory
{
    /// <summary>
    ///     Specification for creating a Bluetooth local characteristic.
    /// </summary>
    public record BluetoothLocalCharacteristicSpec(Guid CharacteristicId, BluetoothCharacteristicProperties Properties, BluetoothCharacteristicPermissions Permissions, string? Name);

    /// <summary>
    ///     Creates a platform-specific Bluetooth local characteristic.
    /// </summary>
    IBluetoothLocalCharacteristic Create(IBluetoothLocalService service, BluetoothLocalCharacteristicSpec spec);
}

/// <summary>
///     Factory for creating platform-specific Bluetooth local descriptors.
/// </summary>
/// <remarks>
///     TODO: This is a stub interface. Full factory infrastructure implementation pending.
/// </remarks>
public interface IBluetoothLocalDescriptorFactory
{
    /// <summary>
    ///     Specification for creating a Bluetooth local descriptor.
    /// </summary>
    public record BluetoothLocalDescriptorSpec(Guid DescriptorId, string? Name);

    /// <summary>
    ///     Creates a platform-specific Bluetooth local descriptor.
    /// </summary>
    IBluetoothLocalDescriptor Create(IBluetoothLocalCharacteristic characteristic, BluetoothLocalDescriptorSpec spec);
}

/// <summary>
///     Factory for creating platform-specific Bluetooth connected devices (when operating as broadcaster/server).
/// </summary>
/// <remarks>
///     TODO: This is a stub interface. Full factory infrastructure implementation pending.
/// </remarks>
public interface IBluetoothConnectedDeviceFactory
{
    /// <summary>
    ///     Specification for creating a Bluetooth connected device.
    /// </summary>
    public record BluetoothConnectedDeviceSpec(string DeviceId);

    /// <summary>
    ///     Creates a platform-specific Bluetooth connected device.
    /// </summary>
    IBluetoothConnectedDevice Create(IBluetoothBroadcaster broadcaster, BluetoothConnectedDeviceSpec spec);
}
