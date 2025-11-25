namespace Bluetooth.Core.CharacteristicAccess;

/// <summary>
/// Implementation of characteristic access service for simple types using conversion functions.
/// </summary>
/// <typeparam name="T">The type of the characteristic value.</typeparam>
internal sealed class CharacteristicAccessServiceForSimpleTypes<T> : CharacteristicAccessService<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CharacteristicAccessServiceForSimpleTypes{T}"/> class.
    /// </summary>
    /// <param name="characteristicId">The unique identifier of the characteristic.</param>
    /// <param name="name">The name of the characteristic.</param>
    /// <param name="toBytesConversion">Function to convert from T to bytes.</param>
    /// <param name="fromBytesConversion">Function to convert from bytes to T.</param>
    public CharacteristicAccessServiceForSimpleTypes(Guid characteristicId, string name, Func<T, ReadOnlyMemory<byte>> toBytesConversion, Func<ReadOnlyMemory<byte>, T> fromBytesConversion) : base(characteristicId, name)
    {
        ToBytesConversion = toBytesConversion;
        FromBytesConversion = fromBytesConversion;
    }

    /// <summary>
    /// Gets the function used to convert from T to bytes.
    /// </summary>
    private Func<T, ReadOnlyMemory<byte>> ToBytesConversion { get; }

    /// <summary>
    /// Gets the function used to convert from bytes to T.
    /// </summary>
    private Func<ReadOnlyMemory<byte>, T> FromBytesConversion { get; }

    /// <inheritdoc />
    protected override ReadOnlyMemory<byte> ToBytes(T input)
    {
        return ToBytesConversion(input);
    }

    /// <inheritdoc />
    protected override T FromBytes(ReadOnlyMemory<byte> value)
    {
        return FromBytesConversion(value);
    }
}
