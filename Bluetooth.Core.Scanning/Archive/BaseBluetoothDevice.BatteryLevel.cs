using Bluetooth.Core.Scanning.BluetoothSigSpecific.ServiceDefinitions;

namespace Bluetooth.Core.Scanning;

public abstract partial class BaseBluetoothDevice
{
    /// <inheritdoc/>
    public double? BatteryLevelPercent
    {
        get => GetValue<double?>(null);
        private set => SetValue(value);
    }

    /// <inheritdoc/>
    public async ValueTask<double?> ReadBatteryLevelAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        if (await BatteryServiceDefinition.BatteryLevel.CanReadAsync(this).ConfigureAwait(false))
        {
            var batteryLevel = await BatteryServiceDefinition.BatteryLevel.ReadAsync(this, false, timeout, cancellationToken).ConfigureAwait(false);
            BatteryLevelPercent = (double) batteryLevel / 100;
        }

        return BatteryLevelPercent;
    }
}
