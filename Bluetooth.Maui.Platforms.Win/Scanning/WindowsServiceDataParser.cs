using System.Buffers.Binary;

namespace Bluetooth.Maui.Platforms.Win.Scanning;

/// <summary>
///     Reads the "Service Data" structures (AD types 0x16, 0x20 and 0x21) of a Windows advertisement.
/// </summary>
/// <remarks>
///     Windows hands every advertisement data section over as its AD type and the bytes that follow the type. For Service Data
///     those bytes are the service UUID, least significant byte first, and then the data of the service. The UUID is 2 bytes
///     long for type 0x16, 4 bytes for 0x20 and 16 bytes for 0x21. This class does not touch any Windows type, so it can be
///     tested on any platform.
/// </remarks>
internal static class WindowsServiceDataParser
{
    private const byte ServiceData16BitUuidType = 0x16;
    private const byte ServiceData32BitUuidType = 0x20;
    private const byte ServiceData128BitUuidType = 0x21;

    private const int Uuid16BitLength = 2;
    private const int Uuid32BitLength = 4;
    private const int Uuid128BitLength = 16;

    /// <summary>
    ///     Tells whether an advertisement data section of the given AD type is a Service Data structure.
    /// </summary>
    /// <param name="dataType">The AD type of the data section.</param>
    /// <returns><see langword="true" /> for the AD types 0x16, 0x20 and 0x21.</returns>
    internal static bool IsServiceDataType(byte dataType)
    {
        return dataType is ServiceData16BitUuidType or ServiceData32BitUuidType or ServiceData128BitUuidType;
    }

    /// <summary>
    ///     Reads one Service Data structure.
    /// </summary>
    /// <param name="dataType">The AD type of the data section.</param>
    /// <param name="section">The bytes of the data section: the service UUID, then the service data.</param>
    /// <param name="uuid">The full 128-bit service UUID; 16-bit and 32-bit UUIDs are expanded with the Bluetooth Base UUID.</param>
    /// <param name="data">The service data that follows the UUID, copied; empty when the structure holds only the UUID.</param>
    /// <returns>
    ///     <see langword="false" /> when <paramref name="dataType" /> is not a Service Data type or when
    ///     <paramref name="section" /> is too short to hold the UUID.
    /// </returns>
    internal static bool TryParse(byte dataType, ReadOnlySpan<byte> section, out Guid uuid, out ReadOnlyMemory<byte> data)
    {
        uuid = Guid.Empty;
        data = ReadOnlyMemory<byte>.Empty;

        int uuidLength;
        switch (dataType)
        {
            case ServiceData16BitUuidType:
                uuidLength = Uuid16BitLength;
                break;
            case ServiceData32BitUuidType:
                uuidLength = Uuid32BitLength;
                break;
            case ServiceData128BitUuidType:
                uuidLength = Uuid128BitLength;
                break;
            default:
                return false;
        }

        if (section.Length < uuidLength)
        {
            return false;
        }

        uuid = uuidLength switch
        {
            Uuid16BitLength => ExpandShortUuid(BinaryPrimitives.ReadUInt16LittleEndian(section)),
            Uuid32BitLength => ExpandShortUuid(BinaryPrimitives.ReadUInt32LittleEndian(section)),
            _ => ReadFullUuid(section[..Uuid128BitLength])
        };
        data = section[uuidLength..].ToArray();
        return true;
    }

    // 0000xxxx-0000-1000-8000-00805F9B34FB, the Bluetooth Base UUID with the short UUID in its first 32 bits.
    private static Guid ExpandShortUuid(uint shortUuid)
    {
        return new Guid((int) shortUuid, 0, 0x1000, 0x80, 0x00, 0x00, 0x80, 0x5F, 0x9B, 0x34, 0xFB);
    }

    // The UUID is sent least significant byte first; a Guid is built from the textual (most significant first) order.
    private static Guid ReadFullUuid(ReadOnlySpan<byte> leastSignificantFirst)
    {
        Span<byte> mostSignificantFirst = stackalloc byte[Uuid128BitLength];
        leastSignificantFirst.CopyTo(mostSignificantFirst);
        mostSignificantFirst.Reverse();
        return new Guid(mostSignificantFirst, bigEndian: true);
    }
}
