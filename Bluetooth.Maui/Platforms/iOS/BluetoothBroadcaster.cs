using Bluetooth.Core.EventArgs;
using Bluetooth.Maui.PlatformSpecific;

namespace Bluetooth.Maui;

/// <summary>
/// iOS implementation of the Bluetooth broadcaster using Core Bluetooth's CBPeripheralManager.
/// </summary>
/// <remarks>
/// This implementation allows the device to act as a BLE peripheral and advertise services.
/// </remarks>
public partial class BluetoothBroadcaster : BaseBluetoothBroadcaster, CbPeripheralManagerProxy.ICbPeripheralManagerProxyDelegate
{
    /// <summary>
    /// Gets the Core Bluetooth peripheral manager proxy.
    /// </summary>
    public CbPeripheralManagerProxy? CbPeripheralManagerProxy { get; protected set; }

    private readonly Dictionary<string, CBCentral> _connectedCentrals = new();

    #region CbPeripheralManagerProxy.ICbPeripheralManagerProxyDelegate

    /// <summary>
    /// Called when a central device subscribes to a characteristic's notifications.
    /// </summary>
    /// <param name="central">The central device that subscribed.</param>
    /// <param name="characteristic">The characteristic that was subscribed to.</param>
    public void CharacteristicSubscribed(CBCentral central, CBCharacteristic characteristic)
    {
        ArgumentNullException.ThrowIfNull(central);
        ArgumentNullException.ThrowIfNull(characteristic);

        // Track the central if not already tracked
        var centralId = central.Identifier.ToString();
        if (!_connectedCentrals.ContainsKey(centralId))
        {
            _connectedCentrals[centralId] = central;
        }

        // Extract service and characteristic IDs
        // Note: We need to find which service this characteristic belongs to
        var characteristicId = Guid.Parse(characteristic.UUID.ToString());

        // Raise the subscription changed event
        var eventArgs = new CharacteristicSubscriptionChangedEventArgs(
            centralId,
            Guid.Empty, // TODO: Need to determine service ID
            characteristicId,
            isSubscribed: true,
            isNotification: true);

        OnCharacteristicSubscriptionChanged(eventArgs);
    }

    /// <summary>
    /// Called when a central device unsubscribes from a characteristic's notifications.
    /// </summary>
    /// <param name="central">The central device that unsubscribed.</param>
    /// <param name="characteristic">The characteristic that was unsubscribed from.</param>
    public void CharacteristicUnsubscribed(CBCentral central, CBCharacteristic characteristic)
    {
        ArgumentNullException.ThrowIfNull(central);
        ArgumentNullException.ThrowIfNull(characteristic);

        var centralId = central.Identifier.ToString();
        var characteristicId = Guid.Parse(characteristic.UUID.ToString());

        // Raise the subscription changed event
        var eventArgs = new CharacteristicSubscriptionChangedEventArgs(
            centralId,
            Guid.Empty, // TODO: Need to determine service ID
            characteristicId,
            isSubscribed: false,
            isNotification: true);

        OnCharacteristicSubscriptionChanged(eventArgs);
    }

    /// <summary>
    /// Called when a service has been added to the peripheral manager.
    /// </summary>
    /// <param name="service">The service that was added.</param>
    public void ServiceAdded(CBService service)
    {
        ArgumentNullException.ThrowIfNull(service);

        var serviceId = Guid.Parse(service.UUID.ToString());

        // Complete the pending task for this service
        if (_serviceAddedTasks.TryGetValue(serviceId, out var tcs))
        {
            tcs.TrySetResult(true);
        }
    }

    /// <summary>
    /// Called when a read request is received from a central device.
    /// </summary>
    /// <param name="request">The read request from the central.</param>
    public void ReadRequestReceived(CBATTRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(CbPeripheralManagerProxy);

        var centralId = request.Central.Identifier.ToString();
        var characteristicId = Guid.Parse(request.Characteristic.UUID.ToString());

        // Create read request event args
        var eventArgs = new CharacteristicReadRequestEventArgs(
            centralId,
            Guid.Empty, // TODO: Need to determine service ID
            characteristicId,
            (int)request.Offset);

        // Raise the event so the application can provide the value
        OnCharacteristicReadRequested(eventArgs);

        // Respond with the value provided by the event handler
        if (eventArgs.Value != null)
        {
            request.Value = NSData.FromArray(eventArgs.Value);
            CbPeripheralManagerProxy.CbPeripheralManager.RespondToRequest(request, CBATTError.Success);
        }
        else
        {
            CbPeripheralManagerProxy.CbPeripheralManager.RespondToRequest(request, CBATTError.ReadNotPermitted);
        }
    }

    /// <summary>
    /// Called when the peripheral manager is about to restore its state.
    /// </summary>
    /// <param name="dict">A dictionary containing state restoration information.</param>
    /// <remarks>
    /// This is called during state restoration when the app is relaunched in the background.
    /// </remarks>
    public void WillRestoreState(NSDictionary dict)
    {
        // Handle state restoration if needed
        // This is used for background execution scenarios
    }

    /// <summary>
    /// Called when write requests are received from a central device.
    /// </summary>
    /// <param name="requests">The array of write requests from the central.</param>
    public void WriteRequestsReceived(CBATTRequest[] requests)
    {
        ArgumentNullException.ThrowIfNull(requests);
        ArgumentNullException.ThrowIfNull(CbPeripheralManagerProxy);

        foreach (var request in requests)
        {
            var centralId = request.Central.Identifier.ToString();
            var characteristicId = Guid.Parse(request.Characteristic.UUID.ToString());
            var value = request.Value?.ToArray() ?? Array.Empty<byte>();

            // Create write request event args
            var eventArgs = new CharacteristicWriteRequestEventArgs(
                centralId,
                Guid.Empty, // TODO: Need to determine service ID
                characteristicId,
                value,
                (int)request.Offset,
                withResponse: true);

            // Raise the event so the application can accept/reject the write
            OnCharacteristicWriteRequested(eventArgs);

            // Respond based on the event handler's decision
            if (!eventArgs.Accept)
            {
                CbPeripheralManagerProxy.CbPeripheralManager.RespondToRequest(request, CBATTError.WriteNotPermitted);
                return; // Stop processing further requests on rejection
            }
        }

        // Respond with success if all writes were accepted
        CbPeripheralManagerProxy.CbPeripheralManager.RespondToRequest(requests[0], CBATTError.Success);
    }

    /// <summary>
    /// Called when an L2CAP channel has been opened.
    /// </summary>
    /// <param name="error">Error that occurred during opening, or <c>null</c> if successful.</param>
    /// <param name="channel">The L2CAP channel that was opened.</param>
    /// <remarks>
    /// Available on iOS 11 and above.
    /// </remarks>
    public void DidOpenL2CapChannel(NSError? error, CBL2CapChannel? channel)
    {
        // L2CAP channel support - not commonly used for standard BLE operations
        // Could be implemented if needed for advanced use cases
    }

    /// <summary>
    /// Called when an L2CAP channel has been published.
    /// </summary>
    /// <param name="error">Error that occurred during publishing, or <c>null</c> if successful.</param>
    /// <param name="psm">The Protocol/Service Multiplexer assigned to the published channel.</param>
    /// <remarks>
    /// Available on iOS 11 and above.
    /// </remarks>
    public void DidPublishL2CapChannel(NSError? error, ushort psm)
    {
        // L2CAP channel support - not commonly used for standard BLE operations
    }

    /// <summary>
    /// Called when an L2CAP channel has been unpublished.
    /// </summary>
    /// <param name="error">Error that occurred during unpublishing, or <c>null</c> if successful.</param>
    /// <param name="psm">The Protocol/Service Multiplexer of the unpublished channel.</param>
    /// <remarks>
    /// Available on iOS 11 and above.
    /// </remarks>
    public void DidUnpublishL2CapChannel(NSError? error, ushort psm)
    {
        // L2CAP channel support - not commonly used for standard BLE operations
    }

    #endregion

}
