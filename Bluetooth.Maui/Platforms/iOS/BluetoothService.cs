using Bluetooth.Maui.PlatformSpecific;
using Bluetooth.Maui.PlatformSpecific.Exceptions;

namespace Bluetooth.Maui;

/// <summary>
/// iOS implementation of the Bluetooth service using Core Bluetooth framework.
/// </summary>
/// <remarks>
/// This implementation wraps iOS's <see cref="CBService"/> to provide access to GATT characteristics.
/// </remarks>
public partial class BluetoothService : BaseBluetoothService, CbPeripheralProxy.ICbServiceDelegate
{
    /// <summary>
    /// Gets the native iOS Core Bluetooth service.
    /// </summary>
    public CBService NativeService { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BluetoothService"/> class.
    /// </summary>
    /// <param name="device">The Bluetooth device associated with this service.</param>
    /// <param name="serviceUuid">The unique identifier of the service.</param>
    /// <param name="nativeService">The native iOS Core Bluetooth service.</param>
    public BluetoothService(IBluetoothDevice device, Guid serviceUuid, CBService nativeService) : base(device, serviceUuid)
    {
        NativeService = nativeService;
    }

    #region CbPeripheralProxy.ICbServiceDelegate

    /// <summary>
    /// Called when included services are discovered for this service.
    /// </summary>
    /// <param name="error">Error that occurred during discovery, or <c>null</c> if successful.</param>
    /// <param name="service">The service for which included services were discovered.</param>
    /// <exception cref="AppleNativeBluetoothException">Thrown when an error occurred during included service discovery.</exception>
    public void DiscoveredIncludedService(NSError? error, CBService service)
    {
        AppleNativeBluetoothException.ThrowIfError(error);
    }

    #endregion
}
