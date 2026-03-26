namespace Bluetooth.Core.Scanning;

public abstract partial class BaseBluetoothRemoteDevice
{
    /// <inheritdoc />
    public bool IsStale
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    /// <summary>
    ///     Updates the stale state of the device.
    /// </summary>
    /// <param name="isStale">True if the device is stale; otherwise false.</param>
    internal void SetStaleState(bool isStale)
    {
        IsStale = isStale;
    }
}
