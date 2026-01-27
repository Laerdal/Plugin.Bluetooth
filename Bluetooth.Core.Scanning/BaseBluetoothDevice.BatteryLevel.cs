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
    public async Task<double?> ReadBatteryLevelAsync()
    {
        if (await BatteryServiceDefinition.BatteryLevel.CanReadAsync(this).ConfigureAwait(false))
        {
            var batteryLevel = await BatteryServiceDefinition.BatteryLevel.ReadAsync(this).ConfigureAwait(false);
            BatteryLevelPercent = (double) batteryLevel / 100;
        }

        return BatteryLevelPercent;
    }
}
