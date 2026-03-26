using System.Buffers.Binary;

namespace Bluetooth.Core.Scanning.Profiles;

/// <summary>
///     Provides reusable codecs for common characteristic value types.
/// </summary>
public static class CharacteristicCodecFactory
{
    /// <summary>
    ///     Creates a codec for byte values.
    /// </summary>
    public static ICharacteristicCodec<byte, byte> ForByte()
    {
        return new DelegateCharacteristicCodec<byte, byte>(
            bytes =>
            {
                if (bytes.Length < 1)
                {
                    throw new CharacteristicCodecException(typeof(byte), bytes, "Cannot decode byte from an empty payload.");
                }

                return bytes.Span[0];
            },
            value => new[] { value });
    }

    /// <summary>
    ///     Creates a codec for unsigned 16-bit integer values using little-endian byte order.
    /// </summary>
    public static ICharacteristicCodec<ushort, ushort> ForUInt16()
    {
        return new DelegateCharacteristicCodec<ushort, ushort>(
            bytes =>
            {
                EnsureLength(bytes, sizeof(ushort), typeof(ushort));
                return BinaryPrimitives.ReadUInt16LittleEndian(bytes.Span);
            },
            value =>
            {
                var buffer = new byte[sizeof(ushort)];
                BinaryPrimitives.WriteUInt16LittleEndian(buffer, value);
                return buffer;
            });
    }

    /// <summary>
    ///     Creates a codec for unsigned 32-bit integer values using little-endian byte order.
    /// </summary>
    public static ICharacteristicCodec<uint, uint> ForUInt32()
    {
        return new DelegateCharacteristicCodec<uint, uint>(
            bytes =>
            {
                EnsureLength(bytes, sizeof(uint), typeof(uint));
                return BinaryPrimitives.ReadUInt32LittleEndian(bytes.Span);
            },
            value =>
            {
                var buffer = new byte[sizeof(uint)];
                BinaryPrimitives.WriteUInt32LittleEndian(buffer, value);
                return buffer;
            });
    }

    /// <summary>
    ///     Creates a codec for signed 16-bit integer values using little-endian byte order.
    /// </summary>
    public static ICharacteristicCodec<short, short> ForInt16()
    {
        return new DelegateCharacteristicCodec<short, short>(
            bytes =>
            {
                EnsureLength(bytes, sizeof(short), typeof(short));
                return BinaryPrimitives.ReadInt16LittleEndian(bytes.Span);
            },
            value =>
            {
                var buffer = new byte[sizeof(short)];
                BinaryPrimitives.WriteInt16LittleEndian(buffer, value);
                return buffer;
            });
    }

    /// <summary>
    ///     Creates a codec for signed 32-bit integer values using little-endian byte order.
    /// </summary>
    public static ICharacteristicCodec<int, int> ForInt32()
    {
        return new DelegateCharacteristicCodec<int, int>(
            bytes =>
            {
                EnsureLength(bytes, sizeof(int), typeof(int));
                return BinaryPrimitives.ReadInt32LittleEndian(bytes.Span);
            },
            value =>
            {
                var buffer = new byte[sizeof(int)];
                BinaryPrimitives.WriteInt32LittleEndian(buffer, value);
                return buffer;
            });
    }

    /// <summary>
    ///     Creates a codec for boolean values.
    /// </summary>
    public static ICharacteristicCodec<bool, bool> ForBool()
    {
        return new DelegateCharacteristicCodec<bool, bool>(
            bytes =>
            {
                if (bytes.Length < 1)
                {
                    throw new CharacteristicCodecException(typeof(bool), bytes, "Cannot decode bool from an empty payload.");
                }

                return bytes.Span[0] != 0;
            },
            value => new[] { value ? (byte) 1 : (byte) 0 });
    }

    /// <summary>
    ///     Creates a codec for strings using UTF-8 by default.
    /// </summary>
    /// <param name="encoding">Optional text encoding to use. Defaults to UTF-8 when null.</param>
    public static ICharacteristicCodec<string, string> ForString(Encoding? encoding = null)
    {
        var effectiveEncoding = encoding ?? Encoding.UTF8;
        return new DelegateCharacteristicCodec<string, string>(
            bytes => effectiveEncoding.GetString(bytes.Span),
            value => effectiveEncoding.GetBytes(value ?? string.Empty));
    }

    /// <summary>
    ///     Creates a codec for enum values using the enum's underlying integral type.
    /// </summary>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    public static ICharacteristicCodec<TEnum, TEnum> ForEnum<TEnum>() where TEnum : struct, Enum
    {
        var enumType = typeof(TEnum);
        var underlyingType = Enum.GetUnderlyingType(enumType);

        return new DelegateCharacteristicCodec<TEnum, TEnum>(
            bytes =>
            {
                var numeric = ReadNumericValue(bytes, underlyingType, enumType);
                var enumValue = Enum.ToObject(enumType, numeric);
                if (enumValue is not TEnum typedValue)
                {
                    throw new CharacteristicCodecException(enumType, bytes, $"Decoded value could not be converted to enum type '{enumType.Name}'.");
                }

                return typedValue;
            },
            value => WriteNumericValue(Convert.ChangeType(value, underlyingType, CultureInfo.InvariantCulture), underlyingType, enumType));
    }

    /// <summary>
    ///     Creates a pass-through codec for raw byte payloads.
    /// </summary>
    public static ICharacteristicCodec<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>> ForBytes()
    {
        return new DelegateCharacteristicCodec<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>>(
            bytes => bytes,
            value => value);
    }

    private static void EnsureLength(ReadOnlyMemory<byte> bytes, int expectedLength, Type targetType)
    {
        if (bytes.Length < expectedLength)
        {
            throw new CharacteristicCodecException(targetType, bytes, $"Cannot decode type '{targetType.Name}' from a payload with length {bytes.Length}. Expected at least {expectedLength} bytes.");
        }
    }

    private static object ReadNumericValue(ReadOnlyMemory<byte> bytes, Type underlyingType, Type enumType)
    {
        if (underlyingType == typeof(byte))
        {
            EnsureLength(bytes, sizeof(byte), enumType);
            return bytes.Span[0];
        }

        if (underlyingType == typeof(sbyte))
        {
            EnsureLength(bytes, sizeof(sbyte), enumType);
            return unchecked((sbyte) bytes.Span[0]);
        }

        if (underlyingType == typeof(short))
        {
            EnsureLength(bytes, sizeof(short), enumType);
            return BinaryPrimitives.ReadInt16LittleEndian(bytes.Span);
        }

        if (underlyingType == typeof(ushort))
        {
            EnsureLength(bytes, sizeof(ushort), enumType);
            return BinaryPrimitives.ReadUInt16LittleEndian(bytes.Span);
        }

        if (underlyingType == typeof(int))
        {
            EnsureLength(bytes, sizeof(int), enumType);
            return BinaryPrimitives.ReadInt32LittleEndian(bytes.Span);
        }

        if (underlyingType == typeof(uint))
        {
            EnsureLength(bytes, sizeof(uint), enumType);
            return BinaryPrimitives.ReadUInt32LittleEndian(bytes.Span);
        }

        if (underlyingType == typeof(long))
        {
            EnsureLength(bytes, sizeof(long), enumType);
            return BinaryPrimitives.ReadInt64LittleEndian(bytes.Span);
        }

        if (underlyingType == typeof(ulong))
        {
            EnsureLength(bytes, sizeof(ulong), enumType);
            return BinaryPrimitives.ReadUInt64LittleEndian(bytes.Span);
        }

        throw new CharacteristicCodecException(enumType, bytes, $"Enum underlying type '{underlyingType.Name}' is not supported.");
    }

    private static ReadOnlyMemory<byte> WriteNumericValue(object numericValue, Type underlyingType, Type enumType)
    {
        try
        {
            if (underlyingType == typeof(byte))
            {
                return new[] { Convert.ToByte(numericValue, CultureInfo.InvariantCulture) };
            }

            if (underlyingType == typeof(sbyte))
            {
                return new[] { unchecked((byte) Convert.ToSByte(numericValue, CultureInfo.InvariantCulture)) };
            }

            if (underlyingType == typeof(short))
            {
                var buffer = new byte[sizeof(short)];
                BinaryPrimitives.WriteInt16LittleEndian(buffer, Convert.ToInt16(numericValue, CultureInfo.InvariantCulture));
                return buffer;
            }

            if (underlyingType == typeof(ushort))
            {
                var buffer = new byte[sizeof(ushort)];
                BinaryPrimitives.WriteUInt16LittleEndian(buffer, Convert.ToUInt16(numericValue, CultureInfo.InvariantCulture));
                return buffer;
            }

            if (underlyingType == typeof(int))
            {
                var buffer = new byte[sizeof(int)];
                BinaryPrimitives.WriteInt32LittleEndian(buffer, Convert.ToInt32(numericValue, CultureInfo.InvariantCulture));
                return buffer;
            }

            if (underlyingType == typeof(uint))
            {
                var buffer = new byte[sizeof(uint)];
                BinaryPrimitives.WriteUInt32LittleEndian(buffer, Convert.ToUInt32(numericValue, CultureInfo.InvariantCulture));
                return buffer;
            }

            if (underlyingType == typeof(long))
            {
                var buffer = new byte[sizeof(long)];
                BinaryPrimitives.WriteInt64LittleEndian(buffer, Convert.ToInt64(numericValue, CultureInfo.InvariantCulture));
                return buffer;
            }

            if (underlyingType == typeof(ulong))
            {
                var buffer = new byte[sizeof(ulong)];
                BinaryPrimitives.WriteUInt64LittleEndian(buffer, Convert.ToUInt64(numericValue, CultureInfo.InvariantCulture));
                return buffer;
            }
        }
        catch (Exception ex)
        {
            throw new CharacteristicCodecException(enumType, ReadOnlyMemory<byte>.Empty, $"Failed to encode enum type '{enumType.Name}'.", ex);
        }

        throw new CharacteristicCodecException(enumType, ReadOnlyMemory<byte>.Empty, $"Enum underlying type '{underlyingType.Name}' is not supported.");
    }

    private sealed class DelegateCharacteristicCodec<TRead, TWrite> : ICharacteristicCodec<TRead, TWrite>
    {
        private readonly Func<ReadOnlyMemory<byte>, TRead> _fromBytes;
        private readonly Func<TWrite, ReadOnlyMemory<byte>> _toBytes;

        public DelegateCharacteristicCodec(
            Func<ReadOnlyMemory<byte>, TRead> fromBytes,
            Func<TWrite, ReadOnlyMemory<byte>> toBytes)
        {
            ArgumentNullException.ThrowIfNull(fromBytes);
            ArgumentNullException.ThrowIfNull(toBytes);

            _fromBytes = fromBytes;
            _toBytes = toBytes;
        }

        public TRead FromBytes(ReadOnlyMemory<byte> bytes)
        {
            return _fromBytes(bytes);
        }

        public ReadOnlyMemory<byte> ToBytes(TWrite value)
        {
            return _toBytes(value);
        }
    }
}
