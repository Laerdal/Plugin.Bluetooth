using Bluetooth.Maui.Platforms.Droid.Exceptions;
using Bluetooth.Maui.Platforms.Droid.Scanning.Factories;
using Bluetooth.Maui.Platforms.Droid.Scanning.NativeObjects;
using Bluetooth.Maui.Platforms.Droid.Tools;

using CharacteristicNotFoundException = Bluetooth.Abstractions.Scanning.Exceptions.CharacteristicNotFoundException;
using MultipleCharacteristicsFoundException = Bluetooth.Abstractions.Scanning.Exceptions.MultipleCharacteristicsFoundException;

namespace Bluetooth.Maui.Platforms.Droid.Scanning;

/// <summary>
///     Android implementation of a Bluetooth Low Energy remote service.
///     This class wraps Android's BluetoothGattService, providing platform-specific
///     implementations for characteristic discovery and management.
/// </summary>
public class AndroidBluetoothRemoteService : BaseBluetoothRemoteService,
    BluetoothGattProxy.IBluetoothGattServiceDelegate
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AndroidBluetoothRemoteService" /> class.
    /// </summary>
    /// <param name="device">The Bluetooth device that owns this service.</param>
    /// <param name="request">The factory request containing service information.</param>
    /// <param name="characteristicFactory">The factory for creating characteristics.</param>
    public AndroidBluetoothRemoteService(
        IBluetoothRemoteDevice device,
        IBluetoothServiceFactory.BluetoothServiceFactoryRequest request,
        IBluetoothCharacteristicFactory characteristicFactory)
        : base(device, request, characteristicFactory)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request is not AndroidBluetoothServiceFactoryRequest androidRequest)
        {
            throw new ArgumentException(
                $"Expected request of type {typeof(AndroidBluetoothServiceFactoryRequest)}, but got {request.GetType()}");
        }

        NativeService = androidRequest.NativeService;
    }

    /// <summary>
    ///     Gets the native Android GATT service.
    /// </summary>
    public BluetoothGattService NativeService { get; }

    /// <summary>
    ///     Gets the Bluetooth device to which this service belongs, cast to the Android-specific implementation.
    /// </summary>
    public AndroidBluetoothRemoteDevice AndroidBluetoothRemoteDevice =>
        (AndroidBluetoothRemoteDevice) Device;

    #region BluetoothGattProxy.IBluetoothGattServiceDelegate Implementation

    /// <inheritdoc />
    public BluetoothGattProxy.IBluetoothGattCharacteristicDelegate GetCharacteristic(
        BluetoothGattCharacteristic? nativeCharacteristic)
    {
        if (nativeCharacteristic == null)
        {
            throw new CharacteristicNotFoundException(this);
        }

        try
        {
            var match = GetCharacteristic(characteristic =>
                AreRepresentingTheSameObject(nativeCharacteristic, characteristic));
            return match as BluetoothGattProxy.IBluetoothGattCharacteristicDelegate
                   ?? throw new CharacteristicNotFoundException(this, nativeCharacteristic.Uuid.ToGuid());
        }
        catch (InvalidOperationException e)
        {
            var matches = GetCharacteristics(characteristic =>
                AreRepresentingTheSameObject(nativeCharacteristic, characteristic)).ToArray();
            throw new MultipleCharacteristicsFoundException(this, matches, e);
        }
    }

    private static bool AreRepresentingTheSameObject(
        BluetoothGattCharacteristic native,
        IBluetoothRemoteCharacteristic shared)
    {
        return shared is AndroidBluetoothRemoteCharacteristic androidCharacteristic
               && native.Uuid?.Equals(androidCharacteristic.NativeCharacteristic.Uuid) == true;
    }

    #endregion

    #region Characteristic Discovery

    /// <inheritdoc />
    protected override ValueTask NativeCharacteristicsExplorationAsync(
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // On Android, characteristics are discovered along with services
            // The NativeService.Characteristics property should already contain all characteristics
            var characteristics = NativeService.Characteristics;
            if (characteristics == null || characteristics.Count == 0)
            {
                // No characteristics for this service
                OnCharacteristicsExplorationSucceeded(
                    System.Array.Empty<BluetoothGattCharacteristic>(),
                    ConvertNativeCharacteristicToCharacteristic,
                    AreRepresentingTheSameObject);
            }
            else
            {
                OnCharacteristicsExplorationSucceeded(
                    characteristics,
                    ConvertNativeCharacteristicToCharacteristic,
                    AreRepresentingTheSameObject);
            }
        }
        catch (Exception ex)
        {
            OnCharacteristicsExplorationFailed(ex);
        }

        return ValueTask.CompletedTask;
    }

    private IBluetoothRemoteCharacteristic ConvertNativeCharacteristicToCharacteristic(
        BluetoothGattCharacteristic nativeCharacteristic)
    {
        var characteristicRequest = new AndroidBluetoothCharacteristicFactoryRequest(nativeCharacteristic);
        return CharacteristicFactory.CreateCharacteristic(this, characteristicRequest);
    }

    #endregion
}
