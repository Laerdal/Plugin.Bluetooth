using Bluetooth.Abstractions.Exceptions;
using Bluetooth.Abstractions.Scanning.Exceptions;
using Bluetooth.Core.Scanning.CharacteristicAccess;

using Plugin.BaseTypeExtensions;

namespace Bluetooth.Maui.Platforms.Apple.Scanning;

public class BluetoothCharacteristicRepository : BaseBluetoothObjectRepository<IBluetoothCharacteristic>
{
    private readonly CBService _nativeService;

    public BluetoothCharacteristicRepository(CBService nativeService)
    {
        _nativeService = nativeService;
    }

    protected override ValueTask NativeItemsExplorationAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(_nativeService);
        ArgumentNullException.ThrowIfNull(_nativeService.Peripheral, nameof(_nativeService.Peripheral));

        _nativeService.Peripheral.DiscoverCharacteristics(_nativeService);
        return ValueTask.CompletedTask;
    }


    /// <summary>
    /// Called when characteristic exploration succeeds. Updates the Characteristics collection and completes the exploration task.
    /// </summary>
    /// <typeparam name="TNativeCharacteristicType">The platform-specific characteristic type.</typeparam>
    /// <param name="characteristics">The list of native characteristics discovered.</param>
    /// <param name="fromInputTypeToOutputTypeConversion">Function to convert from native characteristic type to IBluetoothCharacteristic.</param>
    /// <param name="areRepresentingTheSameObject">Function to determine if a native characteristic and IBluetoothCharacteristic represent the same object.</param>
    /// <exception cref="UnexpectedCharacteristicExplorationException">Thrown when the task completion source is not in the expected state.</exception>
    protected void OnCharacteristicsExplorationSucceeded<TNativeCharacteristicType>(IList<TNativeCharacteristicType> characteristics)
    {
        OnItemsExplorationSucceeded(characteristics, fromInputTypeToOutputTypeConversion, areRepresentingTheSameObject);
    }

    /// <summary>
    /// Called when characteristic exploration fails. Completes the exploration task with an exception or dispatches to the unhandled exception listener.
    /// </summary>
    /// <param name="e">The exception that occurred during characteristic exploration.</param>
    /// <remarks>
    /// If the task completion source accepts the exception, it is propagated to waiting tasks.
    /// Otherwise, the exception is dispatched to the <see cref="BluetoothUnhandledExceptionListener"/>.
    /// </remarks>
    protected void OnCharacteristicsExplorationFailed(Exception e)
    {
        // Attempt to dispatch exception to the TaskCompletionSource
        var success = CharacteristicsExplorationTcs?.TrySetException(e) ?? false;
        if (success)
        {
            return;
        }

        // If the TaskCompletionSource was already completed, dispatch the exception to the listener
        BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
    }


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
