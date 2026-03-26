namespace Bluetooth.Abstractions.Scanning.Profiles;

/// <summary>
///     Defines base behavior for resolving a specific remote Bluetooth characteristic on a device.
/// </summary>
public interface IBluetoothCharacteristicAccessor
{
    /// <summary>
    ///     Gets the UUID of the service that owns the target characteristic.
    /// </summary>
    Guid ServiceId { get; }

    /// <summary>
    ///     Gets the UUID of the target characteristic.
    /// </summary>
    Guid CharacteristicId { get; }

    /// <summary>
    ///     Gets the known service name, if available.
    /// </summary>
    string ServiceName { get; }

    /// <summary>
    ///     Gets the known characteristic name, if available.
    /// </summary>
    string CharacteristicName { get; }

    /// <summary>
    ///     Resolves the remote characteristic on the specified device.
    /// </summary>
    /// <param name="device">The connected remote device that owns the characteristic.</param>
    /// <param name="timeout">Optional timeout for exploration operations.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The resolved remote characteristic.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="device" /> is null.</exception>
    /// <exception cref="Exceptions.CharacteristicAccessorResolutionException">Thrown when the service or characteristic cannot be resolved.</exception>
    ValueTask<IBluetoothRemoteCharacteristic> ResolveCharacteristicAsync(
        IBluetoothRemoteDevice device,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default);
}

/// <summary>
///     Defines typed read and write operations for a specific remote Bluetooth characteristic.
/// </summary>
/// <typeparam name="TRead">The typed value produced when reading the characteristic.</typeparam>
/// <typeparam name="TWrite">The typed value accepted when writing the characteristic.</typeparam>
public interface IBluetoothCharacteristicAccessor<TRead, in TWrite> : IBluetoothCharacteristicAccessor
{
    /// <summary>
    ///     Determines whether the resolved characteristic can be read.
    /// </summary>
    /// <param name="device">The connected remote device that owns the characteristic.</param>
    /// <returns><c>true</c> when the characteristic is already resolved and readable; otherwise <c>false</c>.</returns>
    bool CanRead(IBluetoothRemoteDevice device);

    /// <summary>
    ///     Determines whether the resolved characteristic can be written.
    /// </summary>
    /// <param name="device">The connected remote device that owns the characteristic.</param>
    /// <returns><c>true</c> when the characteristic is already resolved and writable; otherwise <c>false</c>.</returns>
    bool CanWrite(IBluetoothRemoteDevice device);

    /// <summary>
    ///     Determines whether the resolved characteristic supports notifications/indications.
    /// </summary>
    /// <param name="device">The connected remote device that owns the characteristic.</param>
    /// <returns><c>true</c> when the characteristic is already resolved and listenable; otherwise <c>false</c>.</returns>
    bool CanListen(IBluetoothRemoteDevice device);

    /// <summary>
    ///     Reads and decodes the typed value from the characteristic.
    /// </summary>
    /// <param name="device">The connected remote device that owns the characteristic.</param>
    /// <param name="skipIfPreviouslyRead">When true, skips the native read if a previous value is already cached.</param>
    /// <param name="timeout">Optional timeout for exploration and read operations.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The decoded typed value.</returns>
    ValueTask<TRead> ReadAsync(
        IBluetoothRemoteDevice device,
        bool skipIfPreviouslyRead = false,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Encodes and writes the typed value to the characteristic.
    /// </summary>
    /// <param name="device">The connected remote device that owns the characteristic.</param>
    /// <param name="value">The typed value to encode and write.</param>
    /// <param name="skipIfOldValueMatchesNewValue">When true, skips the native write if current and new values match.</param>
    /// <param name="timeout">Optional timeout for exploration and write operations.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    ValueTask WriteAsync(
        IBluetoothRemoteDevice device,
        TWrite value,
        bool skipIfOldValueMatchesNewValue = false,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default);
}
