namespace Bluetooth.Abstractions.Broadcasting.Factories;

/// <summary>
///     Factory interface for creating connected client device instances for a local GATT server (broadcaster).
/// </summary>
public interface IBluetoothConnectedDeviceFactory
{
    /// <summary>
    ///     Creates a representation of a remote client (central) currently connected to the local GATT server.
    /// </summary>
    /// <param name="broadcaster">The local GATT server that the client is connected to.</param>
    /// <param name="spec">The specification describing the connected client.</param>
    /// <returns>A connected client device instance.</returns>
    IBluetoothConnectedDevice CreateConnectedDevice(
        IBluetoothBroadcaster broadcaster,
        BluetoothConnectedDeviceSpec spec);

    /// <summary>
    ///     Specification describing a connected client (central) that is interacting with the local GATT server.
    /// </summary>
    record BluetoothConnectedDeviceSpec
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BluetoothConnectedDeviceSpec" /> record.
        /// </summary>
        /// <param name="clientId">
        ///     An opaque, platform-defined identifier for the connected client. This value is stable only within
        ///     the current runtime/session unless a platform guarantees otherwise.
        /// </param>
        public BluetoothConnectedDeviceSpec(string clientId)
        {
            ClientId = clientId;
        }

        /// <summary>
        ///     An opaque, platform-defined identifier for the connected client.
        /// </summary>
        public string ClientId { get; init; }

        /// <summary>
        ///     Optional human-readable name if the platform can provide it (often unavailable).
        /// </summary>
        public string? DisplayName { get; init; }

        /// <summary>
        ///     Maximum payload size, in bytes, that the server should use when sending notifications/indications
        ///     to this client (e.g., iOS <c>CBCentral.MaximumUpdateValueLength</c>).
        /// </summary>
        public int? MaxUpdateValueLength { get; init; }

        /// <summary>
        ///     Maximum payload size, in bytes, that the client can write in a single GATT write request.
        ///     If unknown, leave null.
        /// </summary>
        public int? MaxWriteValueLength { get; init; }

        /// <summary>
        ///     Timestamp when the client was observed as connected, if known.
        /// </summary>
        public DateTimeOffset? ConnectedAt { get; init; }

        /// <summary>
        ///     Optional bag of platform-provided metadata (addresses, transport hints, etc.).
        ///     Keys should be stable and documented by the platform adapter.
        /// </summary>
        public IReadOnlyDictionary<string, string>? Metadata { get; init; }
    }
}
