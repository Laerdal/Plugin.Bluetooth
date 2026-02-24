namespace Bluetooth.Abstractions.Scanning.AccessService;

/// <summary>
/// Interface representing a multi-type readable Bluetooth characteristic object, providing a method to convert the object to its final type.
/// </summary>
public interface IBluetoothCharacteristicObjectReadableMultiType : IBluetoothCharacteristicObjectReadable
{
    /// <summary>
    /// Converts the object to its final type.
    /// </summary>
    /// <returns>The object converted to its final type.</returns>
    IBluetoothCharacteristicObjectReadableMultiType ToFinalObjectType();
}

/// <summary>
/// Generic interface representing a multi-type readable Bluetooth characteristic object with a specific final type.
/// </summary>
/// <typeparam name="TFinalObjectType">The final type of the object.</typeparam>
public interface IBluetoothCharacteristicObjectReadableMultiType<out TFinalObjectType> : IBluetoothCharacteristicObjectReadableMultiType
{
    /// <summary>
    /// Converts the object to its final type.
    /// </summary>
    /// <returns>The object converted to its final type.</returns>
    new TFinalObjectType ToFinalObjectType();
}
