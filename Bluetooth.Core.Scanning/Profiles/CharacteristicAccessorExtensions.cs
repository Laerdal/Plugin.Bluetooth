using Bluetooth.Abstractions.Scanning;
using Bluetooth.Abstractions.Scanning.Exceptions;
using Bluetooth.Abstractions.Scanning.Profiles;

namespace Bluetooth.Core.Scanning.Profiles;

/// <summary>
///     Convenience helpers for resilient characteristic accessor operations.
/// </summary>
public static class CharacteristicAccessorExtensions
{
    /// <summary>
    ///     Reads a typed value and returns <paramref name="defaultValue" /> when the target service/characteristic
    ///     cannot be resolved or does not support reading.
    /// </summary>
    /// <typeparam name="TRead">The typed value produced when reading the characteristic.</typeparam>
    /// <typeparam name="TWrite">The typed value accepted when writing the characteristic.</typeparam>
    /// <param name="accessor">The accessor describing the target characteristic.</param>
    /// <param name="device">The connected remote device.</param>
    /// <param name="defaultValue">The value returned when reading is unavailable.</param>
    /// <param name="skipIfPreviouslyRead">When true, skips the native read if a previous value is already cached.</param>
    /// <param name="timeout">Optional timeout for exploration and read operations.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>
    ///     The decoded typed value when readable; otherwise <paramref name="defaultValue" />.
    /// </returns>
    public static ValueTask<TRead> ReadValueOrDefaultAsync<TRead, TWrite>(
        this IBluetoothCharacteristicAccessor<TRead, TWrite> accessor,
        IBluetoothRemoteDevice device,
        TRead defaultValue = default!,
        bool skipIfPreviouslyRead = false,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        return accessor.ReadOrDefaultAsync(device, defaultValue, skipIfPreviouslyRead, timeout, cancellationToken);
    }

    /// <summary>
    ///     Reads a typed value and returns <paramref name="defaultValue" /> when the target service/characteristic
    ///     cannot be resolved or does not support reading.
    /// </summary>
    /// <typeparam name="TRead">The typed value produced when reading the characteristic.</typeparam>
    /// <typeparam name="TWrite">The typed value accepted when writing the characteristic.</typeparam>
    /// <param name="accessor">The accessor describing the target characteristic.</param>
    /// <param name="device">The connected remote device.</param>
    /// <param name="defaultValue">The value returned when reading is unavailable.</param>
    /// <param name="skipIfPreviouslyRead">When true, skips the native read if a previous value is already cached.</param>
    /// <param name="timeout">Optional timeout for exploration and read operations.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>
    ///     The decoded typed value when readable; otherwise <paramref name="defaultValue" />.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="accessor" /> or <paramref name="device" /> is null.</exception>
    /// <exception cref="CharacteristicAccessorException">
    ///     Propagates non-resolution accessor/codec failures (for example decode failures) to avoid masking data issues.
    /// </exception>
    public static async ValueTask<TRead> ReadOrDefaultAsync<TRead, TWrite>(
        this IBluetoothCharacteristicAccessor<TRead, TWrite> accessor,
        IBluetoothRemoteDevice device,
        TRead defaultValue = default!,
        bool skipIfPreviouslyRead = false,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(accessor);
        ArgumentNullException.ThrowIfNull(device);

        try
        {
            var characteristic = await accessor.ResolveCharacteristicAsync(device, timeout, cancellationToken).ConfigureAwait(false);
            if (!characteristic.CanRead)
            {
                return defaultValue;
            }

            return await accessor.ReadAsync(device, skipIfPreviouslyRead, timeout, cancellationToken).ConfigureAwait(false);
        }
        catch (CharacteristicAccessorResolutionException)
        {
            return defaultValue;
        }
    }

    /// <summary>
    ///     Attempts to write a typed value and returns <c>false</c> when the target service/characteristic
    ///     cannot be resolved or does not support writing.
    /// </summary>
    /// <typeparam name="TRead">The typed value produced when reading the characteristic.</typeparam>
    /// <typeparam name="TWrite">The typed value accepted when writing the characteristic.</typeparam>
    /// <param name="accessor">The accessor describing the target characteristic.</param>
    /// <param name="device">The connected remote device.</param>
    /// <param name="value">The typed value to encode and write.</param>
    /// <param name="skipIfOldValueMatchesNewValue">When true, skips native write if current and new values match.</param>
    /// <param name="timeout">Optional timeout for exploration and write operations.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns><c>true</c> when the value was written; otherwise <c>false</c>.</returns>
    public static ValueTask<bool> TryWriteValueAsync<TRead, TWrite>(
        this IBluetoothCharacteristicAccessor<TRead, TWrite> accessor,
        IBluetoothRemoteDevice device,
        TWrite value,
        bool skipIfOldValueMatchesNewValue = false,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        return accessor.TryWriteAsync(device, value, skipIfOldValueMatchesNewValue, timeout, cancellationToken);
    }

    /// <summary>
    ///     Attempts to write a typed value and returns <c>false</c> when the target service/characteristic
    ///     cannot be resolved or does not support writing.
    /// </summary>
    /// <typeparam name="TRead">The typed value produced when reading the characteristic.</typeparam>
    /// <typeparam name="TWrite">The typed value accepted when writing the characteristic.</typeparam>
    /// <param name="accessor">The accessor describing the target characteristic.</param>
    /// <param name="device">The connected remote device.</param>
    /// <param name="value">The typed value to encode and write.</param>
    /// <param name="skipIfOldValueMatchesNewValue">When true, skips native write if current and new values match.</param>
    /// <param name="timeout">Optional timeout for exploration and write operations.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns><c>true</c> when the value was written; otherwise <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="accessor" /> or <paramref name="device" /> is null.</exception>
    /// <exception cref="CharacteristicAccessorException">
    ///     Propagates non-resolution accessor/codec failures (for example encode failures) to avoid masking data issues.
    /// </exception>
    public static async ValueTask<bool> TryWriteAsync<TRead, TWrite>(
        this IBluetoothCharacteristicAccessor<TRead, TWrite> accessor,
        IBluetoothRemoteDevice device,
        TWrite value,
        bool skipIfOldValueMatchesNewValue = false,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(accessor);
        ArgumentNullException.ThrowIfNull(device);

        try
        {
            var characteristic = await accessor.ResolveCharacteristicAsync(device, timeout, cancellationToken).ConfigureAwait(false);
            if (!characteristic.CanWrite)
            {
                return false;
            }

            await accessor.WriteAsync(device, value, skipIfOldValueMatchesNewValue, timeout, cancellationToken).ConfigureAwait(false);
            return true;
        }
        catch (CharacteristicAccessorResolutionException)
        {
            return false;
        }
    }
}
