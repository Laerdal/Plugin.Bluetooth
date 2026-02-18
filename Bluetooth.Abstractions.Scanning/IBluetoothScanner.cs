namespace Bluetooth.Abstractions.Scanning;

/// <summary>
/// Interface for managing and scanning Bluetooth devices.
/// </summary>
public partial interface IBluetoothScanner : IAsyncDisposable
{

    #region Advertisement

    /// <summary>
    /// Event triggered when a Bluetooth advertisement is received.
    /// </summary>
    event EventHandler<AdvertisementReceivedEventArgs> AdvertisementReceived;

    #endregion

}