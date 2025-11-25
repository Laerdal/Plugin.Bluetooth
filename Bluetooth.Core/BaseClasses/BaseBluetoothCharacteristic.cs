

namespace Bluetooth.Core.BaseClasses;

/// <inheritdoc cref="IBluetoothCharacteristic" />
public abstract partial class BaseBluetoothCharacteristic : BaseBindableObject, IBluetoothCharacteristic
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseBluetoothCharacteristic"/> class.
    /// </summary>
    /// <param name="service">The Bluetooth service associated with this characteristic.</param>
    /// <param name="id">The unique identifier of the characteristic.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="service"/> is null.</exception>
    /// <exception cref="CharacteristicFoundInWrongServiceException">Thrown when the characteristic is defined for a different service than the one provided.</exception>
    protected BaseBluetoothCharacteristic(IBluetoothService service, Guid id)
    {
        ArgumentNullException.ThrowIfNull(service);
        Service = service;
        Id = id;
        AccessService = service.Device.Scanner.KnownServicesAndCharacteristicsRepository.GetCharacteristicAccessService(id);
        Name = AccessService.CharacteristicName;
        if (AccessService is not UnknownCharacteristicAccessService && AccessService.ServiceId != service.Id)
        {
            // Characteristic defined somewhere, but for another service
            throw new CharacteristicFoundInWrongServiceException(AccessService, this, service.Id);
        }

        LazyCanRead = new Lazy<bool>(NativeCanRead);
        LazyCanWrite = new Lazy<bool>(NativeCanWrite);
        LazyCanListen = new Lazy<bool>(NativeCanListen);
    }



    /// <summary>
    /// Performs the core disposal logic for the characteristic, including stopping listening and cleaning up resources.
    /// This method is called during disposal to ensure proper cleanup of the characteristic's resources.
    /// </summary>
    /// <returns>A task that represents the asynchronous disposal operation.</returns>
    /// <remarks>
    /// This method will attempt to stop listening if the characteristic is currently listening for notifications.
    /// Any exceptions during the stop listening process will be handled by the unhandled exception listener.
    /// </remarks>
    protected async virtual ValueTask DisposeAsyncCore()
    {
        // Stop listening if active
        if (CanListen && IsListening)
        {
            try
            {
                await StopListeningAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, ex);
            }
        }

        // Cancel any pending operations
        ReadValueTcs?.TrySetCanceled();
        WriteValueTcs?.TrySetCanceled();
        ReadIsListeningTcs?.TrySetCanceled();
        WriteIsListeningTcs?.TrySetCanceled();

        // Dispose semaphores
        WriteIsListeningSemaphoreSlim.Dispose();
        WriteSemaphoreSlim.Dispose();

        // Unsubscribe from events
        ValueUpdated = null;
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources asynchronously.</summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public IBluetoothCharacteristicAccessService AccessService { get; }

    /// <inheritdoc/>
    public IBluetoothService Service { get; }

    /// <inheritdoc/>
    public Guid Id { get; }

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public ReadOnlySpan<byte> ValueSpan => Value.Span;

    /// <inheritdoc/>
    public ReadOnlyMemory<byte> Value
    {
        get => GetValue<ReadOnlyMemory<byte>>(default);
        protected set => SetValue(value);
    }


    /// <inheritdoc/>
    public event EventHandler<ValueUpdatedEventArgs>? ValueUpdated;

    /// <summary>
    ///     Returns a string representation of the Bluetooth characteristic, including its ID and access capabilities.
    /// </summary>
    /// <remarks>
    ///     The returned string includes a short description of the characteristic, its unique ID, and a shorthand notation
    ///     for its access capabilities:
    ///     <list type="bullet">
    ///         <item>
    ///             <description><c>R</c> if the characteristic is readable.</description>
    ///         </item>
    ///         <item>
    ///             <description><c>W</c> if the characteristic is writable.</description>
    ///         </item>
    ///         <item>
    ///             <description><c>N*</c> if the characteristic supports notifications and is actively listening.</description>
    ///         </item>
    ///         <item>
    ///             <description><c>N</c> if the characteristic supports notifications but is not currently listening.</description>
    ///         </item>
    ///     </list>
    /// </remarks>
    /// <returns>
    ///     A formatted string that includes the characteristic's short description, ID, and access permissions.
    /// </returns>
    /// <example>
    ///     <code>
    /// var characteristicString = characteristic.ToString();
    /// Console.WriteLine(characteristicString); // Output example: CharacteristicName (CharacteristicId) (R/W/N*)
    /// </code>
    /// </example>
    public override string ToString()
    {
        var access = new List<string>
        {
            ToReadString(),
            ToWriteString(),
            ToListenString()
        };
        return $"{Name} ({Id}) ({string.Join("/", access.Where(s => !string.IsNullOrEmpty(s)))})";
    }

    /// <summary>
    /// Gets the read capability string representation for the characteristic.
    /// </summary>
    /// <returns>Returns "R" if the characteristic can be read, otherwise an empty string.</returns>
    protected virtual string ToReadString()
    {
        return CanRead ? "R" : string.Empty;
    }

    /// <summary>
    /// Gets the write capability string representation for the characteristic.
    /// </summary>
    /// <returns>Returns "W" if the characteristic can be written to, otherwise an empty string.</returns>
    protected virtual string ToWriteString()
    {
        return CanWrite ? "W" : string.Empty;
    }

    /// <summary>
    /// Gets the notification capability string representation for the characteristic.
    /// </summary>
    /// <returns>Returns "N*" if listening, "N" if notifications are supported but not listening, otherwise an empty string.</returns>
    protected virtual string ToListenString()
    {
        return CanListen ? IsListening ? "N*" : "N" : string.Empty;
    }
}
