namespace Bluetooth.Maui.Platforms.Apple.Broadcasting.NativeObjects;

// Mapping native APIs leads to unclean interfaces, ignoring warnings here
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
#pragma warning disable CA1716 // Identifiers should not match keywords

public partial class CbPeripheralManagerWrapper
{
    /// <summary>
    ///     Delegate interface for CoreBluetooth service callbacks, extending the base Bluetooth broadcast service interface.
    /// </summary>
    public interface ICbServiceDelegate
    {
        /// <summary>
        ///     Gets the characteristic delegate for the specified CoreBluetooth characteristic.
        /// </summary>
        /// <param name="characteristic">The CoreBluetooth characteristic to get the delegate for.</param>
        /// <returns>The characteristic delegate for the specified characteristic.</returns>
        ICbCharacteristicDelegate GetCharacteristic(CBCharacteristic? characteristic);
    }
}

#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
#pragma warning restore CA1716 // Identifiers should not match keywords