namespace Bluetooth.Abstractions.Scanning;

/// <summary>
///     Interface representing a Bluetooth device, providing properties and methods for interacting with it.
/// </summary>
public partial interface IBluetoothRemoteDevice
{
    #region Advertisement

    /// <summary>
    ///     Gets the latest advertisement information received.
    /// </summary>
    IBluetoothAdvertisement? LastAdvertisement { get; }

    /// <summary>
    ///     Gets the interval between advertisement information.
    /// </summary>
    TimeSpan IntervalBetweenAdvertisement { get; }

    /// <summary>
    ///     Occurs when advertisement information is received.
    /// </summary>
    event EventHandler<AdvertisementReceivedEventArgs> AdvertisementReceived;

    /// <summary>
    ///     Waits for advertisement information asynchronously.
    /// </summary>
    /// <param name="filter">The filter to apply to the advertisement information.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the advertisement information.</returns>
    ValueTask<IBluetoothAdvertisement> WaitForAdvertisementAsync(Func<IBluetoothAdvertisement?, bool> filter, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Waits for advertisement information asynchronously.
    /// </summary>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the advertisement information.</returns>
    ValueTask<IBluetoothAdvertisement> WaitForAdvertisementAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Handles the advertisement information received event.
    /// </summary>
    /// <param name="advertisement">The advertisement information received.</param>
    void OnAdvertisementReceived(IBluetoothAdvertisement advertisement);

    #endregion
}