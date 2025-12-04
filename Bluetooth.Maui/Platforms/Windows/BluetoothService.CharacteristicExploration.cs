using Bluetooth.Maui.PlatformSpecific.Exceptions;

namespace Bluetooth.Maui;

public partial class BluetoothService
{
    /// <inheritdoc/>
    /// <remarks>
    /// On Windows, this queries characteristics using <see cref="GattDeviceService.GetCharacteristicsAsync()"/> with uncached mode.
    /// </remarks>
    /// <exception cref="WindowsNativeBluetoothException">Thrown when the GATT communication fails or returns an error status.</exception>
    protected async override ValueTask NativeCharacteristicsExplorationAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await NativeServiceProxy.GattDeviceService.GetCharacteristicsAsync(BluetoothCacheMode.Uncached).AsTask(cancellationToken).ConfigureAwait(false);

            if (result.Status != GattCommunicationStatus.Success)
            {
                throw new WindowsNativeBluetoothException(result.Status, result.ProtocolError);
            }

            OnCharacteristicsExplorationSucceeded<GattCharacteristic>(result.Characteristics.ToList(), FromInputTypeToOutputTypeConversion, AreRepresentingTheSameObject);
            return;

            BluetoothCharacteristic FromInputTypeToOutputTypeConversion(GattCharacteristic native)
            {
                return new BluetoothCharacteristic(this, native.Uuid, native);
            }
        }
        catch (Exception e)
        {
            OnCharacteristicsExplorationFailed(e);
        }
    }

    /// <summary>
    /// Compares a native characteristic to a shared characteristic to determine if they represent the same object.
    /// </summary>
    /// <param name="native">Native Windows GATT characteristic.</param>
    /// <param name="shared">Shared characteristic interface.</param>
    /// <returns><c>true</c> if the shared characteristic wraps the same native characteristic; otherwise, <c>false</c>.</returns>
    /// <remarks>
    /// Comparison is based on UUID and attribute handle equality.
    /// </remarks>
    private static bool AreRepresentingTheSameObject(GattCharacteristic native, IBluetoothCharacteristic shared)
    {
        return shared is BluetoothCharacteristic s && native.Uuid.Equals(s.GattCharacteristicProxy.GattCharacteristic.Uuid) && native.AttributeHandle.Equals(s.GattCharacteristicProxy.GattCharacteristic.AttributeHandle);
    }

}
