namespace Bluetooth.Maui.Platforms.Win.NativeObjects;

public interface IRadioWrapper
{
    ValueTask<Windows.Devices.Radios.Radio> GetRadioAsync(CancellationToken cancellationToken = default);
    
    RadioState RadioState { get; }
    RadioKind RadioKind { get; }
    string RadioName { get; }
}