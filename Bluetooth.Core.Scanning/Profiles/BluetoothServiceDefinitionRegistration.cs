namespace Bluetooth.Core.Scanning.Profiles;

/// <summary>
///     Delegate used to register one or more Bluetooth service definitions into a service definition registry.
/// </summary>
/// <param name="registry">The registry to populate.</param>
public delegate void BluetoothServiceDefinitionRegistration(IBluetoothServiceDefinitionRegistry registry);
