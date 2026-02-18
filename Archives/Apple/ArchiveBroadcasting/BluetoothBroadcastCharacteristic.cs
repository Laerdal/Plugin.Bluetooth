using Bluetooth.Abstractions.Broadcasting.Exceptions;
using Bluetooth.Abstractions.Broadcasting.Exceptions.CharacteristicExceptions.UpdateValueExceptions;
using Bluetooth.Abstractions.Broadcasting.Factories;
using Bluetooth.Abstractions.Exceptions;
using Bluetooth.Maui.Platforms.Apple.Broadcasting.NativeObjects;

using Microsoft.Extensions.Logging;

namespace Bluetooth.Maui.Platforms.Apple.Broadcasting;

/// <summary>
/// iOS implementation of a mutable Bluetooth characteristic for the broadcaster/peripheral role.
/// </summary>
/// <remarks>
/// This implementation wraps iOS's <see cref="CBMutableCharacteristic"/> for hosting GATT characteristics.
/// Unlike <see cref="Scanning.BluetoothCharacteristic"/>, this is used when the device acts as a peripheral.
/// </remarks>
public class BluetoothBroadcastCharacteristic : BaseBluetoothBroadcastCharacteristic, CbPeripheralManagerWrapper.ICbCharacteristicDelegate
{
    /// <summary>
    /// Gets the native iOS mutable characteristic.
    /// </summary>
    private CBMutableCharacteristic NativeCharacteristic { get; }

    /// <inheritdoc/>
    public BluetoothBroadcastCharacteristic(IBluetoothBroadcastService service, IBluetoothBroadcastCharacteristicFactory.BluetoothBroadcastCharacteristicFactoryRequest request) : base(service, request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request is not BluetoothBroadcastCharacteristicFactoryRequest nativeRequest)
        {
            throw new ArgumentException($"Request must be of type {nameof(BluetoothBroadcastCharacteristicFactoryRequest)}", nameof(request));
        }

        ArgumentNullException.ThrowIfNull(nativeRequest.NativeCharacteristic);
        NativeCharacteristic = nativeRequest.NativeCharacteristic;
    }

    /// <inheritdoc/>
    protected override ValueTask NativeUpdateValueAsync(ReadOnlyMemory<byte> value, bool notifyClients, TimeSpan? timeout, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(Service);
        ArgumentNullException.ThrowIfNull(Service.Broadcaster);
        if (Service.Broadcaster is not BluetoothBroadcaster nativeBroadcaster)
        {
            throw new InvalidCastException($"Service.Broadcaster must be of type {nameof(BluetoothBroadcaster)}");
        }
        ArgumentNullException.ThrowIfNull(nativeBroadcaster.CbPeripheralManagerWrapper);
        var nsData = NSData.FromArray(value.ToArray());
        var devicesToBeNotified = SubscribedDevices.Cast<BluetoothBroadcastClientDevice>().Select(d => d.NativeCentral).ToArray();

        var result = nativeBroadcaster.CbPeripheralManagerWrapper.CbPeripheralManager.UpdateValue(nsData, NativeCharacteristic, devicesToBeNotified);

        if (!result)
        {
            throw new BroadcastCharacteristicUpdateValueException(this, value, "Failed to update characteristic value on iOS peripheral manager");
        }
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    public override ValueTask DisposeAsync()
    {
        // No specific disposal needed for CBMutableCharacteristic
        return ValueTask.CompletedTask;
    }

    #region CbPeripheralManagerWrapper.ICbCharacteristicDelegate

    /// <inheritdoc/>
    public void CharacteristicSubscribed(CBCentral central, CBCharacteristic characteristic)
    {
        ArgumentNullException.ThrowIfNull(central);
        var device = Service.Broadcaster.GetClientDevice(central.Identifier.ToString());
        OnCharacteristicSubscribed(device);
    }

    /// <inheritdoc/>
    public void CharacteristicUnsubscribed(CBCentral central, CBCharacteristic characteristic)
    {
        ArgumentNullException.ThrowIfNull(central);
        var device = Service.Broadcaster.GetClientDevice(central.Identifier.ToString());
        OnCharacteristicUnsubscribed(device);
    }

    /// <inheritdoc/>
    public void ReadRequestReceived(CBATTRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        var device = Service.Broadcaster.GetClientDevice(request.Central.Identifier.ToString());
        var value = OnReadRequestReceived(device);
        request.Value = NSData.FromArray(value.ToArray());
    }

    /// <inheritdoc/>
    public async void WriteRequestsReceived(CBATTRequest request)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(request);
            var device = Service.Broadcaster.GetClientDevice(request.Central.Identifier.ToString());

            ArgumentNullException.ThrowIfNull(request.Value);
            await OnWriteRequestReceivedAsync(device, request.Value.ToArray()).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    #endregion

}
