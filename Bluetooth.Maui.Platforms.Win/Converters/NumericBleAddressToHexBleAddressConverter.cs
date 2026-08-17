namespace Bluetooth.Maui.Platforms.Win.Converters;

internal static class NumericBleAddressToHexBleAddressConverter
{
    public static string Convert(ulong bluetoothAddress)
    {
        // Convert ulong address to hex string format (e.g., "00:11:22:33:44:55")
        var bytes = BitConverter.GetBytes(bluetoothAddress);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }

        // Take only the last 6 bytes (MAC address is 48 bits)
        return string.Join(":", bytes.Skip(2).Select(b => b.ToString("X2", CultureInfo.InvariantCulture)));
    }
}
