using Bluetooth.Maui.Platforms.Win.Exceptions;
using Bluetooth.Maui.Platforms.Win.Scanning.Factories;
using Bluetooth.Maui.Platforms.Win.Scanning.NativeObjects;

namespace Bluetooth.Maui.Platforms.Win.Scanning;

/// <summary>
///     Windows implementation of the Bluetooth service using Windows.Devices.Bluetooth APIs.
///     This implementation wraps Windows's GattDeviceService to provide access to GATT characteristics.
/// </summary>
public class WindowsBluetoothRemoteService : BaseBluetoothRemoteService, GattDeviceServiceProxy.IBluetoothServiceProxyDelegate
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="WindowsBluetoothRemoteService" /> class.
    /// </summary>
    /// <param name="device">The Bluetooth device associated with this service.</param>
    /// <param name="request">The factory request containing service information.</param>
    /// <param name="characteristicFactory">The factory for creating characteristics.</param>
    public WindowsBluetoothRemoteService(
        IBluetoothRemoteDevice device,
        IBluetoothServiceFactory.BluetoothServiceFactoryRequest request,
        IBluetoothCharacteristicFactory characteristicFactory)
        : base(device, request, characteristicFactory)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request is not WindowsBluetoothServiceFactoryRequest windowsRequest)
        {
            throw new ArgumentException(
                $"Expected request of type {typeof(WindowsBluetoothServiceFactoryRequest)}, but got {request.GetType()}");
        }

        NativeServiceProxy = new NativeObjects.GattDeviceServiceProxy(windowsRequest.NativeService, this);
    }

    /// <summary>
    ///     Gets the native Windows GATT device service proxy.
    /// </summary>
    public NativeObjects.GattDeviceServiceProxy NativeServiceProxy { get; }

    #region Delegate Callbacks - GattDeviceServiceProxy

    /// <summary>
    ///     Called when the access status of the GATT device service changes.
    /// </summary>
    /// <param name="argsId">The device ID.</param>
    /// <param name="argsStatus">The new access status.</param>
    public void OnAccessChanged(string argsId, DeviceAccessStatus argsStatus)
    {
        // Handle permission changes if needed
        // Currently just a placeholder
    }

    #endregion

    /// <inheritdoc />
    protected override ValueTask DisposeAsyncCore()
    {
        NativeServiceProxy.Dispose();
        return base.DisposeAsyncCore();
    }

    #region Characteristic Exploration

    /// <inheritdoc />
    protected async override ValueTask NativeCharacteristicsExplorationAsync(
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await NativeServiceProxy.GattDeviceService
                .GetCharacteristicsAsync(BluetoothCacheMode.Uncached)
                .AsTask(cancellationToken)
                .ConfigureAwait(false);

            WindowsNativeGattCommunicationStatusException.ThrowIfNotSuccess(result.Status);

            // Call OnCharacteristicsExplorationSucceeded with conversion function
            OnCharacteristicsExplorationSucceeded(
                result.Characteristics.ToList(),
                ConvertNativeCharacteristicToCharacteristic,
                AreRepresentingTheSameObject);
        }
        catch (Exception e)
        {
            OnCharacteristicsExplorationFailed(e);
        }
    }

    private IBluetoothRemoteCharacteristic ConvertNativeCharacteristicToCharacteristic(GattCharacteristic nativeCharacteristic)
    {
        var characteristicRequest = new WindowsBluetoothCharacteristicFactoryRequest(nativeCharacteristic);
        return CharacteristicFactory.CreateCharacteristic(this, characteristicRequest);
    }

    private static bool AreRepresentingTheSameObject(GattCharacteristic native, IBluetoothRemoteCharacteristic shared)
    {
        // Compare by UUID - more detailed comparison can be done once BluetoothCharacteristic is implemented
        return native.Uuid.Equals(shared.Id);
    }

    #endregion
}