namespace Bluetooth.Abstractions.Scanning;

/// <summary>
///     Interface representing a Bluetooth device, providing properties and methods for interacting with it.
/// </summary>
public partial interface IBluetoothRemoteDevice
{
    /// <summary>
    ///     Gets the battery level as a value between 0 and 1.
    /// </summary>
    double? BatteryLevelPercent { get; }

    /// <summary>
    ///     Reads the battery level asynchronously.
    /// </summary>
    /// <param name="timeout">Optional timeout for service exploration and read operations.</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the battery level percentage.</returns>
    ValueTask<double?> ReadBatteryLevelAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);
}
