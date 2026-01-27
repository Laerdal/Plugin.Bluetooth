using Bluetooth.Maui.PlatformSpecific;

namespace Bluetooth.Maui;

/// <summary>
/// Android implementation of the Bluetooth service using Android's BluetoothGatt API.
/// </summary>
/// <remarks>
/// This implementation wraps Android's <see cref="BluetoothGattService"/> to provide access to GATT characteristics.
/// </remarks>
public partial class BluetoothService : BaseBluetoothService, BluetoothGattProxy.IService
{
    /// <summary>
    /// Gets the native Android GATT service.
    /// </summary>
    public BluetoothGattService NativeService { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BluetoothService"/> class.
    /// </summary>
    /// <param name="device">The Bluetooth device associated with this service.</param>
    /// <param name="serviceUuid">The unique identifier of the service.</param>
    /// <param name="nativeService">The native Android GATT service.</param>
    public BluetoothService(IBluetoothDevice device, Guid serviceUuid, BluetoothGattService nativeService) : base(device, serviceUuid)
    {
        NativeService = nativeService;
    }

}
