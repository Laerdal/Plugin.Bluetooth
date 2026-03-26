namespace Bluetooth.Abstractions.Scanning.Profiles;

/// <summary>
///     Defines conversion logic between raw Bluetooth characteristic bytes and typed values.
/// </summary>
/// <typeparam name="TRead">The typed value produced when decoding bytes from a characteristic read.</typeparam>
/// <typeparam name="TWrite">The typed value accepted when encoding bytes for a characteristic write.</typeparam>
public interface ICharacteristicCodec<TRead, in TWrite>
{
    /// <summary>
    ///     Converts raw characteristic bytes into a typed value.
    /// </summary>
    /// <param name="bytes">The raw bytes read from the characteristic.</param>
    /// <returns>The decoded typed value.</returns>
    TRead FromBytes(ReadOnlyMemory<byte> bytes);

    /// <summary>
    ///     Converts a typed value into raw characteristic bytes.
    /// </summary>
    /// <param name="value">The typed value to encode.</param>
    /// <returns>The encoded bytes to write to the characteristic.</returns>
    ReadOnlyMemory<byte> ToBytes(TWrite value);
}
