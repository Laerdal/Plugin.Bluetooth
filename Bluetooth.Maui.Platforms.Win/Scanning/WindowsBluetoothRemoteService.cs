using Bluetooth.Maui.Platforms.Win.Exceptions;
using Bluetooth.Maui.Platforms.Win.Logging;
using Bluetooth.Maui.Platforms.Win.Scanning.Factories;
using Bluetooth.Maui.Platforms.Win.Scanning.NativeObjects;

namespace Bluetooth.Maui.Platforms.Win.Scanning;

/// <summary>
///     Windows implementation of the Bluetooth service using Windows.Devices.Bluetooth APIs.
///     This implementation wraps Windows's GattDeviceService to provide access to GATT characteristics.
/// </summary>
/// <remarks>
///     <seealso href="https://learn.microsoft.com/en-us/uwp/api/windows.devices.bluetooth.genericattributeprofile.gattdeviceservice">GattDeviceService</seealso>
///     <seealso href="https://learn.microsoft.com/en-us/uwp/api/windows.devices.bluetooth.genericattributeprofile.gattcharacteristicsresult">GattCharacteristicsResult</seealso>
/// </remarks>
public class WindowsBluetoothRemoteService : BaseBluetoothRemoteService, GattDeviceServiceProxy.IBluetoothServiceProxyDelegate
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="WindowsBluetoothRemoteService" /> class.
    /// </summary>
    /// <param name="device">The Bluetooth device associated with this service.</param>
    /// <param name="spec">The factory spec containing service information.</param>
    /// <param name="characteristicFactory">The factory for creating characteristics.</param>
    /// <param name="logger">Optional logger for logging service operations.</param>
    public WindowsBluetoothRemoteService(IBluetoothRemoteDevice device, IBluetoothRemoteServiceFactory.BluetoothRemoteServiceFactorySpec spec, IBluetoothRemoteCharacteristicFactory characteristicFactory, ILogger<IBluetoothRemoteService>? logger = null) :
        base(device, spec, characteristicFactory, logger)
    {
        ArgumentNullException.ThrowIfNull(spec);
        if (spec is not WindowsBluetoothRemoteServiceFactorySpec nativeSpec)
        {
            throw new ArgumentException($"Expected spec of type {typeof(WindowsBluetoothRemoteServiceFactorySpec)}, but got {spec.GetType()}");
        }

        NativeServiceProxy = new NativeObjects.GattDeviceServiceProxy(nativeSpec.NativeService, this);
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
    protected async override ValueTask NativeCharacteristicsExplorationAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        Logger?.LogCharacteristicDiscoveryStarting(Id, Device.Id);

        try
        {
            var result = await NativeServiceProxy.GattDeviceService.GetCharacteristicsAsync(BluetoothCacheMode.Uncached).AsTask(cancellationToken).ConfigureAwait(false);

            WindowsNativeGattCommunicationStatusException.ThrowIfNotSuccess(result.Status);

            Logger?.LogCharacteristicDiscoveryCompleted(Id, Device.Id, result.Characteristics.Count);

            // Call OnCharacteristicsExplorationSucceeded with conversion function
            OnCharacteristicsExplorationSucceeded(result.Characteristics.ToList(), AreRepresentingTheSameObject, FromInputTypeToOutputTypeConversion);
        }
        catch (Exception e)
        {
            Logger?.LogCharacteristicDiscoveryError(Id, Device.Id, e.Message, e);
            OnCharacteristicsExplorationFailed(e);
        }
        return;

        IBluetoothRemoteCharacteristic FromInputTypeToOutputTypeConversion(GattCharacteristic nativeCharacteristic)
        {
            var spec = new WindowsBluetoothRemoteCharacteristicFactorySpec(nativeCharacteristic);
            return CharacteristicFactory.Create(this, spec);
        }
    }

    private static bool AreRepresentingTheSameObject(GattCharacteristic native, IBluetoothRemoteCharacteristic shared)
    {
        // Compare by UUID - more detailed comparison can be done once BluetoothCharacteristic is implemented
        return native.Uuid.Equals(shared.Id);
    }

    #endregion

}
