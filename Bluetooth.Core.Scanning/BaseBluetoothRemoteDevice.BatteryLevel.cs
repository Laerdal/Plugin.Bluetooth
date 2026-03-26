namespace Bluetooth.Core.Scanning;

using Bluetooth.Core.Scanning.Profiles.BluetoothSig;

public abstract partial class BaseBluetoothRemoteDevice
{
    /// <inheritdoc />
    public double? BatteryLevelPercent
    {
        get => GetValue<double?>(null);
        private set => SetValue(value);
    }

    /// <inheritdoc />
    public async ValueTask<double?> ReadBatteryLevelAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        var batteryLevel = await BatteryProfile.BatteryLevel.ReadAsync(this, false, timeout, cancellationToken).ConfigureAwait(false);
        BatteryLevelPercent = batteryLevel / 100d;
        return BatteryLevelPercent;
    }
}