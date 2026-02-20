namespace Bluetooth.Maui.Platforms.Apple.Tools;

/// <summary>
///     Extension/utility methods for converting between .NET byte buffers and Objective-C Foundation types.
/// </summary>
public static class FoundationDataExtensions
{
    /// <summary>
    ///     Converts a <see cref="ReadOnlyMemory{T}" /> of bytes to an <see cref="NSData" />.
    /// </summary>
    public static NSData ToNSData(this ReadOnlyMemory<byte> value)
    {
        // CoreBluetooth expects NSData.
        // ReadOnlyMemory<byte> may not be backed by a byte[] so we materialize.
        return NSData.FromArray(value.ToArray());
    }

    /// <summary>
    ///     Converts an Objective-C <see cref="NSObject" /> (typically <see cref="NSData" />) to a .NET byte buffer.
    /// </summary>
    public static ReadOnlyMemory<byte> ToReadOnlyMemoryBytes(this NSObject? value)
    {
        if (value is null)
        {
            return ReadOnlyMemory<byte>.Empty;
        }

        // For most BLE descriptor/characteristic values, iOS returns NSData.
        if (value is NSData data)
        {
            return data.ToArray();
        }

        // Some profiles may surface NSNumber/NSString etc. Keep strict to avoid silent corruption.
        throw new NotSupportedException($"Unsupported value type for byte conversion: {value.GetType()}");
    }
}