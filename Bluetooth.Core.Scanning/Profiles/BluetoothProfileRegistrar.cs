namespace Bluetooth.Core.Scanning.Profiles;

/// <summary>
///     Delegate used to register one or more Bluetooth profile definitions into a profile registry.
/// </summary>
/// <param name="registry">The registry to populate.</param>
public delegate void BluetoothProfileRegistrar(IBluetoothProfileRegistry registry);
