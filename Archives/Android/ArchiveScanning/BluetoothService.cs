using Bluetooth.Abstractions.Scanning.Exceptions;
using Bluetooth.Abstractions.Scanning.Factories;
using Bluetooth.Maui.Platforms.Droid.Scanning.NativeObjects;
using Bluetooth.Maui.Platforms.Droid.Tools;

namespace Bluetooth.Maui.Platforms.Droid.Scanning;

/// <inheritdoc/>
public partial class BluetoothService : BaseBluetoothService, BluetoothGattProxy.IService
{
    /// <summary>
    /// Gets the native Android Bluetooth GATT service.
    /// </summary>
    public BluetoothGattService NativeService { get; }

    /// <summary>
    /// Gets the repository for characteristic access services.
    /// </summary>
    private IBluetoothCharacteristicAccessServicesRepository CharacteristicAccessServicesRepository { get; }

    /// <inheritdoc/>
    public BluetoothService(IBluetoothDevice device,
        IBluetoothServiceFactory.BluetoothServiceFactoryRequest request,
        IBluetoothCharacteristicFactory characteristicFactory,
        IBluetoothCharacteristicAccessServicesRepository characteristicAccessServicesRepository) : base(device, request, characteristicFactory, characteristicAccessServicesRepository)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request is not BluetoothServiceFactoryRequest nativeRequest)
        {
            throw new ArgumentException($"Request must be of type {nameof(BluetoothServiceFactoryRequest)}", nameof(request));
        }

        NativeService = nativeRequest.NativeService ?? throw new ArgumentNullException(nameof(nativeRequest.NativeService));
        CharacteristicAccessServicesRepository = characteristicAccessServicesRepository;
    }

    /// <inheritdoc/>
    protected override ValueTask NativeCharacteristicsExplorationAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        if (NativeService.Characteristics == null)
        {
            throw new CharacteristicExplorationException("NativeService.Characteristics == null");

            // MIGHT NEED Retries and/or delays
        }

        // on android, characteristics are scanned at the same time the Service is discovered
        OnCharacteristicsExplorationSucceeded(NativeService.Characteristics, FromInputTypeToOutputTypeConversion, AreRepresentingTheSameObject);

        return ValueTask.CompletedTask;

        IBluetoothCharacteristic FromInputTypeToOutputTypeConversion(BluetoothGattCharacteristic native)
        {
            var characteristicId = native.Uuid?.ToGuid()
                ?? throw new InvalidOperationException("Characteristic UUID is null");

            var accessService = CharacteristicAccessServicesRepository.GetCharacteristicAccessService(characteristicId);
            var request = new BluetoothCharacteristicFactoryRequest
            {
                AccessService = accessService
            };

            // Note: This blocks, but is necessary since OnCharacteristicsExplorationSucceeded expects a synchronous function
            return CharacteristicFactory.CreateCharacteristic(this, request);
        }

        bool AreRepresentingTheSameObject(BluetoothGattCharacteristic native, IBluetoothCharacteristic shared)
        {
            return shared is BluetoothCharacteristic androidCharacteristic &&
                   native.Uuid?.ToString() == androidCharacteristic.NativeCharacteristic.Uuid?.ToString() &&
                   native.InstanceId == androidCharacteristic.NativeCharacteristic.InstanceId;
        }
    }

    #region BluetoothGattProxy.IService Implementation

    /// <inheritdoc/>
    public BluetoothGattProxy.ICharacteristic GetCharacteristic(BluetoothGattCharacteristic? nativeCharacteristic)
    {
        ArgumentNullException.ThrowIfNull(nativeCharacteristic);

        var characteristicId = Java.Util.UUID.FromString(nativeCharacteristic.Uuid?.ToString())?.ToGuid()
            ?? throw new InvalidOperationException("Characteristic UUID is null");

        var characteristic = GetCharacteristicOrDefault(characteristicId);
        if (characteristic is BluetoothGattProxy.ICharacteristic androidCharacteristic)
        {
            return androidCharacteristic;
        }

        throw new InvalidOperationException($"Characteristic {characteristicId} not found or is not an Android characteristic");
    }

    #endregion
}
