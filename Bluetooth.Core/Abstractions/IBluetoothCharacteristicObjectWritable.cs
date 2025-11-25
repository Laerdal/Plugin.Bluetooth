namespace Bluetooth.Core.Abstractions;

/// <summary>
/// Interface representing a writable Bluetooth characteristic object, providing a method to convert the object to a byte array.
/// </summary>
public interface IBluetoothCharacteristicObjectWritable
{
    /// <summary>
    /// Converts the object to a byte array.
    /// </summary>
    /// <returns>A byte array representation of the object.</returns>
    ReadOnlyMemory<byte> ToByteArray();
}
