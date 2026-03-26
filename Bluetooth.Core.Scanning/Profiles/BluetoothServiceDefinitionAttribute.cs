namespace Bluetooth.Core.Scanning.Profiles;

/// <summary>
///     Marks a static type as a Bluetooth service definition that can be auto-registered.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class BluetoothServiceDefinitionAttribute : Attribute
{
}
