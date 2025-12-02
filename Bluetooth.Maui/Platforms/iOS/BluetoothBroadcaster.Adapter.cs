using Bluetooth.Maui.PlatformSpecific;

namespace Bluetooth.Maui;

public partial class BluetoothBroadcaster
{
    /// <summary>
    /// Called when the peripheral manager's state is updated.
    /// </summary>
    /// <exception cref="NotImplementedException">This method is not yet implemented.</exception>
    public void StateUpdated()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    /// <exception cref="NotImplementedException">This method is not yet implemented.</exception>
    protected override ValueTask NativeInitializeAsync()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    /// <exception cref="NotImplementedException">This method is not yet implemented.</exception>
    protected override void NativeRefreshIsBluetoothOn()
    {
        throw new NotImplementedException();
    }
}
