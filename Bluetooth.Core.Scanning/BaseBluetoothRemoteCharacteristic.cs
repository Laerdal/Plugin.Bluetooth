namespace Bluetooth.Core.Scanning;

/// <inheritdoc cref="IBluetoothRemoteCharacteristic" />
public abstract partial class BaseBluetoothRemoteCharacteristic : BaseBindableObject, IBluetoothRemoteCharacteristic
{
    /// <summary>
    /// The logger instance used for logging characteristic operations.
    /// </summary>
    private readonly ILogger<IBluetoothRemoteCharacteristic> _logger;

    /// <inheritdoc/>
    public IBluetoothRemoteService RemoteService { get; }

    /// <summary>
    /// The factory responsible for creating descriptors associated with this characteristic.
    /// </summary>
    protected IBluetoothDescriptorFactory DescriptorFactory { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseBluetoothRemoteCharacteristic"/> class.
    /// </summary>
    /// <param name="remoteService">The Bluetooth service associated with this characteristic.</param>
    /// <param name="request">The factory request containing characteristic information.</param>
    /// <param name="descriptorFactory">The factory for creating descriptors for this characteristic.</param>
    /// <param name="logger">The logger instance to use for logging (optional).</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="remoteService"/> is null.</exception>
    protected BaseBluetoothRemoteCharacteristic(IBluetoothRemoteService remoteService, IBluetoothCharacteristicFactory.BluetoothCharacteristicFactoryRequest request, IBluetoothDescriptorFactory descriptorFactory, ILogger<IBluetoothRemoteCharacteristic>? logger = null)
    {
        ArgumentNullException.ThrowIfNull(remoteService);
        ArgumentNullException.ThrowIfNull(descriptorFactory);
        ArgumentNullException.ThrowIfNull(request);

        _logger = logger ?? NullLogger<IBluetoothRemoteCharacteristic>.Instance;
        RemoteService = remoteService;
        DescriptorFactory = descriptorFactory;
        Id = request.CharacteristicId;

        LazyCanRead = new Lazy<bool>(NativeCanRead);
        LazyCanWrite = new Lazy<bool>(NativeCanWrite);
        LazyCanListen = new Lazy<bool>(NativeCanListen);
    }

    /// <inheritdoc/>
    public Guid Id { get; }

    /// <inheritdoc/>
    public string Name { get; } = "Unknown Characteristic";

    #region Dispose

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
        DescriptorsExplorationTcs?.TrySetCanceled();
        BeginReliableWriteTcs?.TrySetCanceled();
        ExecuteReliableWriteTcs?.TrySetCanceled();
        AbortReliableWriteTcs?.TrySetCanceled();

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
    }

    #endregion


    #region ToString

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
        return $"[{Id}] {Name} ({string.Join("/", access.Where(s => !string.IsNullOrEmpty(s)))})";
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

    #endregion

}
