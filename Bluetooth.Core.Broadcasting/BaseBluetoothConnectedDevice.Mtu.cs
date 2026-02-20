namespace Bluetooth.Core.Broadcasting;

public abstract partial class BaseBluetoothConnectedDevice
{
    /// <inheritdoc />
    public int Mtu
    {
        get => GetValue(23); // Default MTU is 23 bytes
        protected set => SetValue(value);
    }

    /// <inheritdoc />
    public event EventHandler<MtuChangedEventArgs>? MtuChanged;

    /// <summary>
    ///     Called when MTU changes for this client connection.
    /// </summary>
    protected void OnMtuChanged(int mtu)
    {
        var oldMtu = Mtu;
        Mtu = mtu;
        LogMtuChanged(Id, oldMtu, mtu);
        MtuChanged?.Invoke(this, new MtuChangedEventArgs(this, mtu));
    }
}