namespace Bluetooth.Core.BaseClasses;

public abstract partial class BaseBluetoothScanner
{
    /// <summary>
    /// Cleans up multiple devices by disconnecting them and removing them from the device list.
    /// </summary>
    /// <param name="devices">The devices to clean up.</param>
    /// <returns>A task that completes when all devices have been cleaned up.</returns>
    public async ValueTask CleanAsync(IEnumerable<IBluetoothDevice> devices)
    {
        ArgumentNullException.ThrowIfNull(devices);
        
        foreach (var device in devices)
        {
            await CleanAsync(device).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Cleans up a single device by disconnecting it and removing it from the device list.
    /// </summary>
    /// <param name="device">The device to clean up.</param>
    /// <returns>A task that completes when the device has been cleaned up.</returns>
    public async ValueTask CleanAsync(IBluetoothDevice? device)
    {
        if (device == null)
        {
            return;
        }

        if (device.IsConnected)
        {
            await device.DisconnectAsync().ConfigureAwait(false);
        }

        lock (Devices)
        {
            Devices.Remove(device);
        }
    }

    /// <summary>
    /// Cleans up a device with the specified ID by disconnecting it and removing it from the device list.
    /// </summary>
    /// <param name="deviceId">The ID of the device to clean up.</param>
    /// <returns>A task that completes when the device has been cleaned up.</returns>
    public ValueTask CleanAsync(string deviceId)
    {
        return CleanAsync(GetDeviceOrDefault(deviceId));
    }

    /// <summary>
    /// Cleans up all devices by disconnecting them and removing them from the device list.
    /// </summary>
    /// <returns>A task that completes when all devices have been cleaned up.</returns>
    public async ValueTask CleanAsync()
    {
        var devices = GetDevices().ToList();
        foreach (var device in devices)
        {
            await CleanAsync(device).ConfigureAwait(false);
        }
    }
}
