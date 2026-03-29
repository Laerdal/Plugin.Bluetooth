namespace Bluetooth.Core.Scanning;

using Bluetooth.Core.Scanning.Profiles;
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
        const byte unreadableSentinel = byte.MaxValue;
        var batteryLevel = await BatteryServiceDefinition.BatteryLevel
            .ReadValueOrDefaultAsync(this, defaultValue: unreadableSentinel, skipIfPreviouslyRead: false, timeout, cancellationToken)
            .ConfigureAwait(false);
        if (batteryLevel == unreadableSentinel)
        {
            return BatteryLevelPercent;
        }

        BatteryLevelPercent = batteryLevel / 100d;
        return BatteryLevelPercent;
    }
}
