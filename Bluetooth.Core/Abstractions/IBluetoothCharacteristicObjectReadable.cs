namespace Bluetooth.Core.Abstractions;

/// <summary>
/// Interface representing a readable Bluetooth characteristic object, providing a method to convert a byte array to the object.
/// </summary>
public interface IBluetoothCharacteristicObjectReadable
{
    /// <summary>
    /// Converts a byte array to the object.
    /// </summary>
    /// <param name="byteArray">The byte array to convert.</param>
    void FromByteArray(ReadOnlyMemory<byte> byteArray);
}
