using Bluetooth.Maui.Platforms.Apple.Broadcasting.Factories;
using Bluetooth.Maui.Platforms.Apple.Broadcasting.NativeObjects;

namespace Bluetooth.Maui.Platforms.Apple.Broadcasting;

/// <inheritdoc cref="BaseBluetoothLocalCharacteristic" />
public class AppleBluetoothLocalCharacteristic : BaseBluetoothLocalCharacteristic, CbPeripheralManagerWrapper.ICbCharacteristicDelegate
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AppleBluetoothLocalCharacteristic" /> class with the specified service, factory request, and descriptor factory.
    /// </summary>
    /// <param name="localService">The Bluetooth service to which this characteristic belongs.</param>
    /// <param name="request">The factory request containing the information needed to create this characteristic.</param>
    /// <param name="localDescriptorFactory">The factory used to create descriptors for this characteristic.</param>
    public AppleBluetoothLocalCharacteristic(IBluetoothLocalService localService, IBluetoothLocalCharacteristicFactory.BluetoothLocalCharacteristicSpec request, IBluetoothLocalDescriptorFactory localDescriptorFactory) : base(localService,
        request, localDescriptorFactory)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request is not AppleBluetoothCharacteristicSpec appleRequest)
        {
            throw new ArgumentException($"Expected request of type {typeof(AppleBluetoothCharacteristicSpec)}, but got {request.GetType()}");
        }

        CbCharacteristic = appleRequest.CbCharacteristic;
    }

    /// <summary>
    ///     Gets the native iOS Core Bluetooth mutable characteristic.
    /// </summary>
    public CBMutableCharacteristic CbCharacteristic { get; }

    /// <summary>
    ///     Gets the Bluetooth service to which this characteristic belongs, cast to the Apple-specific implementation.
    /// </summary>
    public AppleBluetoothLocalService AppleBluetoothLocalService => (AppleBluetoothLocalService) LocalService;

    /// <inheritdoc />
    public void CharacteristicSubscribed(CBCentral central, CBCharacteristic characteristic)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(central);
            var device = GetOrCreateClientDevice(central);
            OnCharacteristicSubscribed(device);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc />
    public void CharacteristicUnsubscribed(CBCentral central, CBCharacteristic characteristic)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(central);
            var device = GetOrCreateClientDevice(central);
            OnCharacteristicUnsubscribed(device);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc />
    public void ReadRequestReceived(CBATTRequest request)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(request.Central);

            var device = GetOrCreateClientDevice(request.Central);
            var value = OnReadRequestReceived(device);
            var bytes = value.ToArray();
            request.Value = new ReadOnlyMemory<byte>(bytes).ToNSData();

            AppleBluetoothLocalService.AppleBluetoothBroadcaster.CbPeripheralManagerWrapper.CbPeripheralManager.RespondToRequest(request, CBATTError.Success);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
            AppleBluetoothLocalService.AppleBluetoothBroadcaster.CbPeripheralManagerWrapper.CbPeripheralManager.RespondToRequest(request, CBATTError.RequestNotSupported);
        }
    }

    /// <inheritdoc />
    public async void WriteRequestsReceived(CBATTRequest request)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(request.Central);
            ArgumentNullException.ThrowIfNull(request.Value);

            var device = GetOrCreateClientDevice(request.Central);
            await OnWriteRequestReceivedAsync(device, request.Value.ToArray()).ConfigureAwait(false);

            AppleBluetoothLocalService.AppleBluetoothBroadcaster.CbPeripheralManagerWrapper.CbPeripheralManager.RespondToRequest(request, CBATTError.Success);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
            AppleBluetoothLocalService.AppleBluetoothBroadcaster.CbPeripheralManagerWrapper.CbPeripheralManager.RespondToRequest(request, CBATTError.RequestNotSupported);
        }
    }

    /// <inheritdoc />
    protected override ValueTask NativeUpdateValueAsync(ReadOnlyMemory<byte> value, bool notifyClients, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        var nsData = value.ToNSData();
        var centralsToNotify = notifyClients ? SubscribedDevices.Cast<AppleBluetoothConnectedDevice>().Select(d => d.CbCentral).ToArray() : null;

        var result = AppleBluetoothLocalService.AppleBluetoothBroadcaster.CbPeripheralManagerWrapper.CbPeripheralManager.UpdateValue(nsData, CbCharacteristic, centralsToNotify);

        if (!result)
        {
            throw new InvalidOperationException("Failed to update characteristic value on iOS peripheral manager. The queue may be full.");
        }

        return ValueTask.CompletedTask;
    }

    /// <summary>
    ///     Gets an existing client device or creates a new one for the specified central.
    /// </summary>
    /// <param name="central">The Core Bluetooth central device.</param>
    /// <returns>The client device corresponding to the central.</returns>
    private IBluetoothConnectedDevice GetOrCreateClientDevice(CBCentral central)
    {
        return AppleBluetoothLocalService.AppleBluetoothBroadcaster.GetOrCreateClientDevice(central);
    }
}
