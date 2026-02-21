namespace Bluetooth.Abstractions.Scanning.EventArgs;

/// <summary>
///     Event arguments for when a characteristic value is updated.
/// </summary>
public class ValueUpdatedEventArgs : System.EventArgs
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ValueUpdatedEventArgs" /> class.
    /// </summary>
    /// <param name="newValue">The new value of the characteristic.</param>
    /// <param name="oldValue">The old value of the characteristic.</param>
    public ValueUpdatedEventArgs(ReadOnlyMemory<byte> newValue, ReadOnlyMemory<byte> oldValue)
    {
        NewValue = newValue;
        OldValue = oldValue;
    }

    /// <summary>
    ///     Gets the new value as a read-only memory segment.
    /// </summary>
    public ReadOnlyMemory<byte> NewValue { get; }

    /// <summary>
    ///     Gets the old value as a read-only memory segment.
    /// </summary>
    public ReadOnlyMemory<byte> OldValue { get; }
}