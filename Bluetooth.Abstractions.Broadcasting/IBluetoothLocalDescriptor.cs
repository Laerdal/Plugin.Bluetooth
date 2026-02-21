namespace Bluetooth.Abstractions.Broadcasting;

/// <summary>
///     Represents a descriptor in the context of Bluetooth broadcasting.
///     Descriptors provide additional information about a characteristic.
/// </summary>
public partial interface IBluetoothLocalDescriptor : INotifyPropertyChanged, IAsyncDisposable
{
    /// <summary>
    ///     Gets the Bluetooth characteristic hosting this descriptor.
    /// </summary>
    IBluetoothLocalCharacteristic LocalCharacteristic { get; }

    /// <summary>
    ///     Gets the name of the descriptor.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     Gets the unique identifier of the descriptor. This is typically a UUID that distinguishes this descriptor from others in the Bluetooth broadcasting context.
    /// </summary>
    Guid Id { get; }
}