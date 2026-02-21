namespace Bluetooth.Maui.Platforms.Win.NativeObjects;

/// <summary>
///     Interface for the Radio wrapper to provide abstraction and easier testing of Bluetooth radio operations.
/// </summary>
public interface IRadioWrapper
{
    /// <summary>
    ///     Gets the Bluetooth radio, ensuring that only one instance is created and shared across the application.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the Bluetooth radio.</returns>
    ValueTask<Windows.Devices.Radios.Radio> GetRadioAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets the current state of the Bluetooth radio.
    /// </summary>
    RadioState RadioState { get; }

    /// <summary>
    ///     Gets the kind of the Bluetooth radio.
    /// </summary>
    RadioKind RadioKind { get; }

    /// <summary>
    ///     Gets the name of the Bluetooth radio.
    /// </summary>
    string RadioName { get; }
}
