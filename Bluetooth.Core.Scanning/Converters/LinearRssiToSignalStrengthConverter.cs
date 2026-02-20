namespace Bluetooth.Core.Scanning.Converters;

/// <summary>
///     A linear implementation of <see cref="IBluetoothRssiToSignalStrengthConverter" /> that maps RSSI values to signal strength percentages.
/// </summary>
public class LinearRssiToSignalStrengthConverter : IBluetoothRssiToSignalStrengthConverter
{
    private readonly Lock _lock = new();
    private double _closeRssiValue = -50;
    private double _farRssiValue = -100;

    /// <inheritdoc />
    public double Convert(double rssi)
    {
        lock (_lock)
        {
            if (rssi <= _farRssiValue)
            {
                _farRssiValue = rssi;
                return 0;
            }

            if (rssi >= _closeRssiValue)
            {
                _closeRssiValue = rssi;
                return 1;
            }

            return (rssi - _farRssiValue) / (_closeRssiValue - _farRssiValue);
        }
    }
}