namespace Bluetooth.Maui.PlatformSpecific;

/// <summary>
/// Extension methods for converting Bluetooth addresses between different formats.
/// Provides utilities for converting between numeric and hexadecimal string representations
/// of Bluetooth addresses used by the Windows Bluetooth API.
/// </summary>
public static class BluetoothAddressExtensions
{
    /// <summary>
    /// Converts a numeric Bluetooth address to a colon-separated hexadecimal string format.
    /// </summary>
    /// <param name="numericBleAddress">The numeric Bluetooth address to convert.</param>
    /// <returns>
    /// A string representation of the Bluetooth address in the format "XX:XX:XX:XX:XX:XX"
    /// where each XX is a two-digit hexadecimal value.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method converts Windows Bluetooth numeric addresses to the standard MAC address format.
    /// The conversion process:
    /// <list type="number">
    /// <item>Converts the numeric value to a 12-character hexadecimal string</item>
    /// <item>Splits the string into 2-character chunks from right to left</item>
    /// <item>Reverses the order and joins with colons</item>
    /// </list>
    /// </para>
    /// <para>
    /// Examples:
    /// <list type="bullet">
    /// <item>1181 → "00:00:00:00:04:9D"</item>
    /// <item>279164741971468 → "FD:E6:1B:47:6E:0C"</item>
    /// </list>
    /// </para>
    /// </remarks>
    public static string ConvertNumericBleAddressToHexBleAddress(this ulong numericBleAddress)
    {
        // Convert to 12-character hexadecimal string (e.g., 1181 → "00000000049D")
        var hexString = numericBleAddress.ToString("X12", CultureInfo.InvariantCulture);

        // Split into 2-character chunks from end to start and reverse order
        // (e.g., "00000000049D" → ["00", "00", "00", "00", "04", "9D"])
        var hexStringChunks = ChunkStringFromEndToStart(hexString, 2).Reverse();

        // Join with colons to create standard MAC address format
        // (e.g., ["00", "00", "00", "00", "04", "9D"] → "00:00:00:00:04:9D")
        var bluetoothAddress = string.Join(":", hexStringChunks);

        return bluetoothAddress;
    }

    /// <summary>
    /// Splits a string into chunks of the specified size, starting from the end of the string.
    /// Each chunk is padded with leading zeros if necessary to ensure consistent length.
    /// </summary>
    /// <param name="input">The string to split into chunks.</param>
    /// <param name="chunkSize">The size of each chunk.</param>
    /// <returns>
    /// An enumerable of string chunks, each padded to the specified chunk size with leading zeros.
    /// </returns>
    /// <remarks>
    /// This method processes the string from right to left to ensure proper byte boundary alignment
    /// for hexadecimal representations. Single-character chunks are automatically padded with a
    /// leading zero (e.g., "9" becomes "09").
    /// </remarks>
    private static IEnumerable<string> ChunkStringFromEndToStart(string input, int chunkSize)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            yield break;
        }

        for (var i = input.Length; i > 0; i -= chunkSize)
        {
            var startIndex = Math.Max(i - chunkSize, 0);
            var length = Math.Min(chunkSize, i);

            var chunk = input
                .Substring(startIndex, length)
                .PadLeft(chunkSize, '0'); // Ensure consistent chunk size with leading zeros

            yield return chunk;
        }
    }
}
