using Plugin.ByteArrays;

namespace Bluetooth.Core.Scanning.CharacteristicAccess;

/// <summary>
/// Factory class for creating characteristic access services for various data types.
/// </summary>
public static class CharacteristicAccessServiceFactory
{
    /// <summary>
    /// Creates a characteristic access service for a predefined object type.
    /// </summary>
    /// <typeparam name="TObject">The type of the object, must be a class with parameterless constructor.</typeparam>
    /// <param name="characteristicId">The unique identifier of the characteristic.</param>
    /// <param name="name">The name of the characteristic.</param>
    /// <returns>A characteristic access service for the specified type.</returns>
    public static CharacteristicAccessService<TObject> Create<TObject>(Guid characteristicId, string name) where TObject : class, new()
    {
        return new CharacteristicAccessServiceForPredefinedTypes<TObject>(characteristicId, name);
    }

    /// <summary>
    /// Creates a characteristic access service for separate input and output object types.
    /// </summary>
    /// <typeparam name="TObjectIn">The type for reading, must implement <see cref="IBluetoothCharacteristicObjectReadable"/>.</typeparam>
    /// <typeparam name="TObjectOut">The type for writing, must implement <see cref="IBluetoothCharacteristicObjectWritable"/>.</typeparam>
    /// <param name="characteristicId">The unique identifier of the characteristic.</param>
    /// <param name="name">The name of the characteristic.</param>
    /// <returns>A characteristic access service for the specified types.</returns>
    public static CharacteristicAccessService<TObjectIn, TObjectOut> Create<TObjectIn, TObjectOut>(Guid characteristicId, string name) where TObjectIn : class, IBluetoothCharacteristicObjectReadable, new()
                                                                                                                                       where TObjectOut : class, IBluetoothCharacteristicObjectWritable
    {
        return new CharacteristicAccessServiceForPredefinedTypes<TObjectIn, TObjectOut>(characteristicId, name);
    }

    /// <summary>
    /// Creates a characteristic access service for an enum type.
    /// </summary>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    /// <param name="characteristicId">The unique identifier of the characteristic.</param>
    /// <param name="name">The name of the characteristic.</param>
    /// <returns>A characteristic access service for the enum type.</returns>
    public static CharacteristicAccessService<TEnum> CreateForEnum<TEnum>(Guid characteristicId, string name) where TEnum : struct, Enum
    {
        return new CharacteristicAccessServiceForSimpleTypes<TEnum>(characteristicId, name, arg => arg.ToByteArray(), static arg => arg.Span.ToEnum<TEnum>());
    }

    /// <summary>
    /// Creates a characteristic access service for signed byte values.
    /// </summary>
    /// <param name="characteristicId">The unique identifier of the characteristic.</param>
    /// <param name="name">The name of the characteristic.</param>
    /// <returns>A characteristic access service for signed byte values.</returns>
    public static CharacteristicAccessService<sbyte> CreateForSByte(Guid characteristicId, string name)
    {
        return new CharacteristicAccessServiceForSimpleTypes<sbyte>(characteristicId, name, arg => arg.ToByteArray(), arg => arg.Span.ToSByte());
    }

    /// <summary>
    /// Creates a characteristic access service for byte values.
    /// </summary>
    /// <param name="characteristicId">The unique identifier of the characteristic.</param>
    /// <param name="name">The name of the characteristic.</param>
    /// <returns>A characteristic access service for byte values.</returns>
    public static CharacteristicAccessService<byte> CreateForByte(Guid characteristicId, string name)
    {
        return new CharacteristicAccessServiceForSimpleTypes<byte>(characteristicId, name, arg => arg.ToByteArray(), arg => arg.Span.ToByte());
    }

    /// <summary>
    /// Creates a characteristic access service for boolean values.
    /// </summary>
    /// <param name="characteristicId">The unique identifier of the characteristic.</param>
    /// <param name="name">The name of the characteristic.</param>
    /// <returns>A characteristic access service for boolean values.</returns>
    public static CharacteristicAccessService<bool> CreateForBoolean(Guid characteristicId, string name)
    {
        return new CharacteristicAccessServiceForSimpleTypes<bool>(characteristicId, name, arg => arg.ToByteArray(), arg => arg.Span.ToBoolean());
    }

    /// <summary>
    /// Creates a characteristic access service for 16-bit signed integer values.
    /// </summary>
    /// <param name="characteristicId">The unique identifier of the characteristic.</param>
    /// <param name="name">The name of the characteristic.</param>
    /// <returns>A characteristic access service for 16-bit signed integer values.</returns>
    public static CharacteristicAccessService<short> CreateForInt16(Guid characteristicId, string name)
    {
        return new CharacteristicAccessServiceForSimpleTypes<short>(characteristicId, name, arg => arg.ToByteArray(), arg => arg.Span.ToInt16());
    }

    /// <summary>
    /// Creates a characteristic access service for 16-bit unsigned integer values.
    /// </summary>
    /// <param name="characteristicId">The unique identifier of the characteristic.</param>
    /// <param name="name">The name of the characteristic.</param>
    /// <returns>A characteristic access service for 16-bit unsigned integer values.</returns>
    public static CharacteristicAccessService<ushort> CreateForUInt16(Guid characteristicId, string name)
    {
        return new CharacteristicAccessServiceForSimpleTypes<ushort>(characteristicId, name, arg => arg.ToByteArray(), arg => arg.Span.ToUInt16());
    }

    /// <summary>
    /// Creates a characteristic access service for 32-bit signed integer values.
    /// </summary>
    /// <param name="characteristicId">The unique identifier of the characteristic.</param>
    /// <param name="name">The name of the characteristic.</param>
    /// <returns>A characteristic access service for 32-bit signed integer values.</returns>
    public static CharacteristicAccessService<int> CreateForInt32(Guid characteristicId, string name)
    {
        return new CharacteristicAccessServiceForSimpleTypes<int>(characteristicId, name, arg => arg.ToByteArray(), arg => arg.Span.ToInt32());
    }

    /// <summary>
    /// Creates a characteristic access service for 32-bit unsigned integer values.
    /// </summary>
    /// <param name="characteristicId">The unique identifier of the characteristic.</param>
    /// <param name="name">The name of the characteristic.</param>
    /// <returns>A characteristic access service for 32-bit unsigned integer values.</returns>
    public static CharacteristicAccessService<uint> CreateForUInt32(Guid characteristicId, string name)
    {
        return new CharacteristicAccessServiceForSimpleTypes<uint>(characteristicId, name, arg => arg.ToByteArray(), arg => arg.Span.ToUInt32());
    }

    /// <summary>
    /// Creates a characteristic access service for 64-bit signed integer values.
    /// </summary>
    /// <param name="characteristicId">The unique identifier of the characteristic.</param>
    /// <param name="name">The name of the characteristic.</param>
    /// <returns>A characteristic access service for 64-bit signed integer values.</returns>
    public static CharacteristicAccessService<long> CreateForInt64(Guid characteristicId, string name)
    {
        return new CharacteristicAccessServiceForSimpleTypes<long>(characteristicId, name, arg => arg.ToByteArray(), arg => arg.Span.ToInt64());
    }

    /// <summary>
    /// Creates a characteristic access service for 64-bit unsigned integer values.
    /// </summary>
    /// <param name="characteristicId">The unique identifier of the characteristic.</param>
    /// <param name="name">The name of the characteristic.</param>
    /// <returns>A characteristic access service for 64-bit unsigned integer values.</returns>
    public static CharacteristicAccessService<ulong> CreateForUInt64(Guid characteristicId, string name)
    {
        return new CharacteristicAccessServiceForSimpleTypes<ulong>(characteristicId, name, arg => arg.ToByteArray(), arg => arg.Span.ToUInt64());
    }

    /// <summary>
    /// Creates a characteristic access service for character values.
    /// </summary>
    /// <param name="characteristicId">The unique identifier of the characteristic.</param>
    /// <param name="name">The name of the characteristic.</param>
    /// <returns>A characteristic access service for character values.</returns>
    public static CharacteristicAccessService<char> CreateForChar(Guid characteristicId, string name)
    {
        return new CharacteristicAccessServiceForSimpleTypes<char>(characteristicId, name, arg => arg.ToByteArray(), arg => arg.Span.ToChar());
    }

    /// <summary>
    /// Creates a characteristic access service for double precision floating-point values.
    /// </summary>
    /// <param name="characteristicId">The unique identifier of the characteristic.</param>
    /// <param name="name">The name of the characteristic.</param>
    /// <returns>A characteristic access service for double precision floating-point values.</returns>
    public static CharacteristicAccessService<double> CreateForDouble(Guid characteristicId, string name)
    {
        return new CharacteristicAccessServiceForSimpleTypes<double>(characteristicId, name, arg => arg.ToByteArray(), arg => arg.Span.ToDouble());
    }

    /// <summary>
    /// Creates a characteristic access service for single precision floating-point values.
    /// </summary>
    /// <param name="characteristicId">The unique identifier of the characteristic.</param>
    /// <param name="name">The name of the characteristic.</param>
    /// <returns>A characteristic access service for single precision floating-point values.</returns>
    public static CharacteristicAccessService<float> CreateForSingle(Guid characteristicId, string name)
    {
        return new CharacteristicAccessServiceForSimpleTypes<float>(characteristicId, name, arg => arg.ToByteArray(), arg => arg.Span.ToSingle());
    }

    /// <summary>
    /// Creates a characteristic access service for UTF-8 encoded string values.
    /// </summary>
    /// <param name="characteristicId">The unique identifier of the characteristic.</param>
    /// <param name="name">The name of the characteristic.</param>
    /// <returns>A characteristic access service for UTF-8 encoded string values.</returns>
    public static CharacteristicAccessService<string> CreateForUtf8String(Guid characteristicId, string name)
    {
        return new CharacteristicAccessServiceForSimpleTypes<string>(characteristicId, name, arg => arg.ToByteArray(), arg => arg.Span.ToUtf8String());
    }

    /// <summary>
    /// Creates a characteristic access service for version values (read-only).
    /// </summary>
    /// <param name="characteristicId">The unique identifier of the characteristic.</param>
    /// <param name="name">The name of the characteristic.</param>
    /// <returns>A characteristic access service for version values.</returns>
    public static CharacteristicAccessService<Version> CreateForVersion(Guid characteristicId, string name)
    {
        return new CharacteristicAccessServiceForSimpleTypes<Version>(characteristicId, name, arg => throw new NotSupportedException($"We don't write Versions, can't write {arg}"), arg => arg.Span.ToVersionOrDefault(defaultValue: new Version()));
    }

    /// <summary>
    /// Creates a characteristic access service for byte array values.
    /// </summary>
    /// <param name="characteristicId">The unique identifier of the characteristic.</param>
    /// <param name="name">The name of the characteristic.</param>
    /// <returns>A characteristic access service for byte array values.</returns>
    public static CharacteristicAccessService<byte[]> CreateForByteArray(Guid characteristicId, string name)
    {
        return new CharacteristicAccessServiceForSimpleTypes<byte[]>(characteristicId, name, o => o, value => value.ToArray());
    }
}

/// <summary>
/// Implementation of characteristic access service for predefined types that implement required interfaces.
/// </summary>
/// <typeparam name="TReadWrite">The type for both read and write operations, must have a parameterless constructor.</typeparam>
public class CharacteristicAccessServiceForPredefinedTypes<TReadWrite> : CharacteristicAccessService<TReadWrite> where TReadWrite : new()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CharacteristicAccessServiceForPredefinedTypes{TReadWrite}"/> class.
    /// </summary>
    /// <param name="characteristicId">The unique identifier of the characteristic.</param>
    /// <param name="name">The name of the characteristic.</param>
    public CharacteristicAccessServiceForPredefinedTypes(Guid characteristicId, string name) : base(characteristicId, name)
    {
    }

    /// <inheritdoc />
    protected override ReadOnlyMemory<byte> ToBytes(TReadWrite input)
    {
        if (input is not IBluetoothCharacteristicObjectWritable writable)
        {
            throw new CharacteristicValueConversionException(this, Array.Empty<byte>(), typeof(TReadWrite));
        }

        ReadOnlyMemory<byte> output;
        try
        {
            output = writable.ToByteArray();
        }
        catch (Exception e)
        {
            throw new CharacteristicValueConversionException(this,Array.Empty<byte>(), typeof(TReadWrite), e);

        }

        return output;
    }

    /// <inheritdoc />
    protected override TReadWrite FromBytes(ReadOnlyMemory<byte> value)
    {
        var output = new TReadWrite();

        if (output is not IBluetoothCharacteristicObjectReadable readable)
        {
            throw new CharacteristicValueConversionException(this, value.ToArray(), typeof(TReadWrite));
        }

        try
        {
            readable.FromByteArray(value);
            if (readable is IBluetoothCharacteristicObjectReadableMultiType multiType)
            {
                output = (TReadWrite) multiType.ToFinalObjectType();
            }
        }
        catch (Exception e)
        {
            throw new CharacteristicValueConversionException(this, value.ToArray(), typeof(TReadWrite),  e);
        }

        return output;
    }
}
