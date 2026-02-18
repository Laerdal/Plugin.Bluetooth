namespace Bluetooth.Maui.Platforms.Apple.NativeObjects;

/// <summary>
/// Interface for providing DispatchQueue instances for CoreBluetooth managers.
/// </summary>
public interface IDispatchQueueProvider
{
    /// <summary>
    /// Gets a  DispatchQueue for the CBPeripheralManager with the specified label.
    /// </summary>
    /// <returns>A DispatchQueue instance.</returns>
    DispatchQueue GetCbPeripheralManagerDispatchQueue();

    /// <summary>
    /// Gets a DispatchQueue for the CBCentralManager with the specified label.
    /// </summary>
    /// <returns>A DispatchQueue instance.</returns>
    DispatchQueue GetCbCentralManagerDispatchQueue();
}