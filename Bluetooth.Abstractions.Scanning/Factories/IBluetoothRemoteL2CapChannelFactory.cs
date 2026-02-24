namespace Bluetooth.Abstractions.Scanning.Factories;

/// <summary>
///     Factory interface for creating Bluetooth L2CAP channel instances.
/// </summary>
public interface IBluetoothRemoteL2CapChannelFactory
{
    /// <summary>
    ///     Creates a new instance of a Bluetooth L2CAP channel.
    /// </summary>
    /// <param name="device">The Bluetooth device to which the channel will be associated.</param>
    /// <param name="request">The request containing information needed to create the channel.</param>
    /// <returns>A new instance of <see cref="IBluetoothL2CapChannel" />.</returns>
    IBluetoothL2CapChannel CreateL2CapChannel(
        IBluetoothRemoteDevice device,
        BluetoothRemoteL2CapChannelFactoryRequest request);

    /// <summary>
    ///     Request object for creating Bluetooth L2CAP channel instances.
    /// </summary>
    record BluetoothRemoteL2CapChannelFactoryRequest
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BluetoothRemoteL2CapChannelFactoryRequest" /> class.
        /// </summary>
        /// <param name="psm">The Protocol/Service Multiplexer identifying the L2CAP service.</param>
        protected BluetoothRemoteL2CapChannelFactoryRequest(int psm)
        {
            if (psm <= 0)
                throw new ArgumentOutOfRangeException(nameof(psm), "PSM must be positive");

            Psm = psm;
        }

        /// <summary>
        ///     Gets the Protocol/Service Multiplexer (PSM) for the L2CAP channel.
        /// </summary>
        public int Psm { get; }
    }
}
