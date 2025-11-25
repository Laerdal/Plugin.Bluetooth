using Bluetooth.Maui.PlatformSpecific.Exceptions;

namespace Bluetooth.Maui;

public partial class BluetoothService
{

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

    private static bool AreRepresentingTheSameObject(GattCharacteristic native, IBluetoothCharacteristic shared)
    {
        return shared is BluetoothCharacteristic s && native.Uuid.Equals(s.GattCharacteristicProxy.GattCharacteristic.Uuid) && native.AttributeHandle.Equals(s.GattCharacteristicProxy.GattCharacteristic.AttributeHandle);
    }

}
