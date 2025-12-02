using Bluetooth.Maui.PlatformSpecific;

namespace Bluetooth.Maui;

/// <summary>
/// iOS implementation of the Bluetooth broadcaster using Core Bluetooth's CBPeripheralManager.
/// </summary>
/// <remarks>
/// This implementation allows the device to act as a BLE peripheral and advertise services.
/// Most functionality is not yet implemented.
/// </remarks>
public partial class BluetoothBroadcaster : BaseBluetoothBroadcaster, CbPeripheralManagerProxy.ICbPeripheralManagerProxyDelegate
{
    /// <summary>
    /// Gets the Core Bluetooth peripheral manager proxy.
    /// </summary>
    public CbPeripheralManagerProxy? CbPeripheralManagerProxy { get; protected set; }

    #region CbPeripheralManagerProxy.ICbPeripheralManagerProxyDelegate

    /// <summary>
    /// Called when a central device subscribes to a characteristic's notifications.
    /// </summary>
    /// <param name="central">The central device that subscribed.</param>
    /// <param name="characteristic">The characteristic that was subscribed to.</param>
    /// <exception cref="NotImplementedException">This method is not yet implemented.</exception>
    public void CharacteristicSubscribed(CBCentral central, CBCharacteristic characteristic)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Called when a central device unsubscribes from a characteristic's notifications.
    /// </summary>
    /// <param name="central">The central device that unsubscribed.</param>
    /// <param name="characteristic">The characteristic that was unsubscribed from.</param>
    /// <exception cref="NotImplementedException">This method is not yet implemented.</exception>
    public void CharacteristicUnsubscribed(CBCentral central, CBCharacteristic characteristic)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Called when a service has been added to the peripheral manager.
    /// </summary>
    /// <param name="service">The service that was added.</param>
    /// <exception cref="NotImplementedException">This method is not yet implemented.</exception>
    public void ServiceAdded(CBService service)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Called when a read request is received from a central device.
    /// </summary>
    /// <param name="request">The read request from the central.</param>
    /// <exception cref="NotImplementedException">This method is not yet implemented.</exception>
    public void ReadRequestReceived(CBATTRequest request)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Called when the peripheral manager is about to restore its state.
    /// </summary>
    /// <param name="dict">A dictionary containing state restoration information.</param>
    /// <remarks>
    /// This is called during state restoration when the app is relaunched in the background.
    /// </remarks>
    /// <exception cref="NotImplementedException">This method is not yet implemented.</exception>
    public void WillRestoreState(NSDictionary dict)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Called when write requests are received from a central device.
    /// </summary>
    /// <param name="requests">The array of write requests from the central.</param>
    /// <exception cref="NotImplementedException">This method is not yet implemented.</exception>
    public void WriteRequestsReceived(CBATTRequest[] requests)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Called when an L2CAP channel has been opened.
    /// </summary>
    /// <param name="error">Error that occurred during opening, or <c>null</c> if successful.</param>
    /// <param name="channel">The L2CAP channel that was opened.</param>
    /// <remarks>
    /// Available on iOS 11 and above.
    /// </remarks>
    /// <exception cref="NotImplementedException">This method is not yet implemented.</exception>
    public void DidOpenL2CapChannel(NSError? error, CBL2CapChannel? channel)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Called when an L2CAP channel has been published.
    /// </summary>
    /// <param name="error">Error that occurred during publishing, or <c>null</c> if successful.</param>
    /// <param name="psm">The Protocol/Service Multiplexer assigned to the published channel.</param>
    /// <remarks>
    /// Available on iOS 11 and above.
    /// </remarks>
    /// <exception cref="NotImplementedException">This method is not yet implemented.</exception>
    public void DidPublishL2CapChannel(NSError? error, ushort psm)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Called when an L2CAP channel has been unpublished.
    /// </summary>
    /// <param name="error">Error that occurred during unpublishing, or <c>null</c> if successful.</param>
    /// <param name="psm">The Protocol/Service Multiplexer of the unpublished channel.</param>
    /// <remarks>
    /// Available on iOS 11 and above.
    /// </remarks>
    /// <exception cref="NotImplementedException">This method is not yet implemented.</exception>
    public void DidUnpublishL2CapChannel(NSError? error, ushort psm)
    {
        throw new NotImplementedException();
    }

    #endregion

}
