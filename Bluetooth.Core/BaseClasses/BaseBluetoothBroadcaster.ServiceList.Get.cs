namespace Bluetooth.Core.BaseClasses;

public abstract partial class BaseBluetoothBroadcaster
{
    /// <inheritdoc/>
    public IBluetoothService? GetServiceOrDefault(Guid serviceId)
    {
        lock (HostedServicesInternal)
        {
            return HostedServicesInternal.SingleOrDefault(s => s.Id == serviceId);
        }
    }
}
