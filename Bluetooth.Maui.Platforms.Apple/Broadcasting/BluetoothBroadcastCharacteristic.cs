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

    // High-performance logging using LoggerMessage delegates
    private readonly static Action<ILogger, string, Guid, Exception?> _logCharacteristicSubscribed =
        LoggerMessage.Define<string, Guid>(
            LogLevel.Debug,
            new EventId(1, nameof(CharacteristicSubscribed)),
            "Central {DeviceId} subscribed to characteristic {CharacteristicId}");

    private readonly static Action<ILogger, string, Guid, Exception?> _logCharacteristicUnsubscribed =
        LoggerMessage.Define<string, Guid>(
            LogLevel.Debug,
            new EventId(2, nameof(CharacteristicUnsubscribed)),
            "Central {DeviceId} unsubscribed from characteristic {CharacteristicId}");

    private readonly static Action<ILogger, string, Guid, nint, Exception?> _logReadRequestReceived =
        LoggerMessage.Define<string, Guid, nint>(
            LogLevel.Debug,
            new EventId(3, nameof(ReadRequestReceived)),
            "Central {DeviceId} requested read from characteristic {CharacteristicId} at offset {Offset}");

    private readonly static Action<ILogger, string, Guid, nint, Exception?> _logWriteRequestReceived =
        LoggerMessage.Define<string, Guid, nint>(
            LogLevel.Debug,
            new EventId(4, nameof(WriteRequestsReceived)),
            "Central {DeviceId} requested write to characteristic {CharacteristicId} at offset {Offset}");

    /// <inheritdoc/>
    public BluetoothBroadcastCharacteristic(IBluetoothBroadcastService service, IBluetoothBroadcastCharacteristicFactory.BluetoothBroadcastCharacteristicFactoryRequest request) : base(service, request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request is not BluetoothBroadcastCharacteristicFactoryRequest nativeRequest)
        {
            throw new ArgumentException($"Request must be of type {nameof(BluetoothBroadcastCharacteristicFactoryRequest)}", nameof(request));
        }

        if (nativeRequest.NativeCharacteristic != null)
        {
            NativeCharacteristic = nativeRequest.NativeCharacteristic;
        }
        else
        {
            // Create new characteristic if not provided
            var cbProperties = ConvertProperties(request.Properties);
            var cbPermissions = ConvertPermissions(request.Permissions);
            var initialNsData = request.InitialValue.HasValue ? NSData.FromArray(request.InitialValue.Value.ToArray()) : null;

            NativeCharacteristic = new CBMutableCharacteristic(
                CBUUID.FromString(request.Id.ToString()),
                cbProperties,
                initialNsData,
                cbPermissions);
        }
    }

    /// <inheritdoc/>
    public override ValueTask DisposeAsync()
    {
        // No specific disposal needed for CBMutableCharacteristic
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    protected override Task NativeUpdateValueAsync(ReadOnlyMemory<byte> value, bool notifyClients = true, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        if (Service.Broadcaster is not BluetoothBroadcaster broadcaster)
        {
            throw new InvalidOperationException("Broadcaster is not a BluetoothBroadcaster");
        }

        ArgumentNullException.ThrowIfNull(broadcaster.CbPeripheralManagerWrapper);

        var nsData = NSData.FromArray(value.ToArray());

        if (notifyClients)
        {
            var success = broadcaster.CbPeripheralManagerWrapper.CbPeripheralManager.UpdateValue(nsData, NativeCharacteristic, null);
            if (!success)
            {
                // Queue is full, will need to wait for ready callback
                // For now, we'll just log this
            }
        }
        else
        {
            // Just update the value without notifying
            NativeCharacteristic.Value = nsData;
        }

        return Task.CompletedTask;
    }

    #region CbPeripheralManagerWrapper.ICbCharacteristicDelegate

    /// <inheritdoc/>
    public void CharacteristicSubscribed(CBCentral central, CBCharacteristic characteristic)
    {
        ArgumentNullException.ThrowIfNull(central);
        var deviceId = central.Identifier.ToString();
        if (Logger is not null)
        {
            _logCharacteristicSubscribed(Logger, deviceId, Id, null);
        }
    }

    /// <inheritdoc/>
    public void CharacteristicUnsubscribed(CBCentral central, CBCharacteristic characteristic)
    {
        ArgumentNullException.ThrowIfNull(central);
        var deviceId = central.Identifier.ToString();
        if (Logger is not null)
        {
            _logCharacteristicUnsubscribed(Logger, deviceId, Id, null);
        }
    }

    /// <inheritdoc/>
    public void ReadRequestReceived(CBATTRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        var deviceId = request.Central.Identifier.ToString();
        if (Logger is not null)
        {
            _logReadRequestReceived(Logger, deviceId, Id, request.Offset, null);
        }
        // Respond with the current value
        request.Value = NSData.FromArray(Value.ToArray());
    }

    /// <inheritdoc/>
    public void WriteRequestsReceived(CBATTRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        var deviceId = request.Central.Identifier.ToString();
        if (request.Value != null)
        {
            if (Logger is not null)
            {
                _logWriteRequestReceived(Logger, deviceId, Id, request.Offset, null);
            }
            // Note: The actual value update and response should be handled by the peripheral manager
        }
    }

    #endregion

    #region Helper Methods

    private static CBCharacteristicProperties ConvertProperties(CharacteristicProperties properties)
    {
        CBCharacteristicProperties result = 0;

        if (properties.HasFlag(CharacteristicProperties.Broadcast))
        {
            result |= CBCharacteristicProperties.Broadcast;
        }
        if (properties.HasFlag(CharacteristicProperties.Read))
        {
            result |= CBCharacteristicProperties.Read;
        }
        if (properties.HasFlag(CharacteristicProperties.WriteWithoutResponse))
        {
            result |= CBCharacteristicProperties.WriteWithoutResponse;
        }
        if (properties.HasFlag(CharacteristicProperties.Write))
        {
            result |= CBCharacteristicProperties.Write;
        }
        if (properties.HasFlag(CharacteristicProperties.Notify))
        {
            result |= CBCharacteristicProperties.Notify;
        }
        if (properties.HasFlag(CharacteristicProperties.Indicate))
        {
            result |= CBCharacteristicProperties.Indicate;
        }
        if (properties.HasFlag(CharacteristicProperties.AuthenticatedSignedWrites))
        {
            result |= CBCharacteristicProperties.AuthenticatedSignedWrites;
        }
        if (properties.HasFlag(CharacteristicProperties.ExtendedProperties))
        {
            result |= CBCharacteristicProperties.ExtendedProperties;
        }

        return result;
    }

    private static CBAttributePermissions ConvertPermissions(CharacteristicPermissions permissions)
    {
        CBAttributePermissions result = 0;

        if (permissions.HasFlag(CharacteristicPermissions.Read))
        {
            result |= CBAttributePermissions.Readable;
        }
        if (permissions.HasFlag(CharacteristicPermissions.ReadEncrypted))
        {
            result |= CBAttributePermissions.ReadEncryptionRequired;
        }
        if (permissions.HasFlag(CharacteristicPermissions.Write))
        {
            result |= CBAttributePermissions.Writeable;
        }
        if (permissions.HasFlag(CharacteristicPermissions.WriteEncrypted))
        {
            result |= CBAttributePermissions.WriteEncryptionRequired;
        }

        return result;
    }

    #endregion
}
