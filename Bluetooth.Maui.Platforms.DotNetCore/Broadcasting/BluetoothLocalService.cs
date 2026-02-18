using Bluetooth.Abstractions.Broadcasting.Factories;

using Microsoft.Extensions.Logging;

namespace Bluetooth.Maui.Platforms.DotNetCore.Broadcasting;

/// <inheritdoc/>
public class BluetoothLocalService : Core.Broadcasting.BaseBluetoothLocalService
{

    /// <inheritdoc/>
    /// <exception cref="PlatformNotSupportedException"></exception>
    public BluetoothLocalService(IBluetoothBroadcaster broadcaster,
        IBluetoothLocalServiceFactory.BluetoothLocalServiceSpec request,
        IBluetoothLocalCharacteristicFactory localCharacteristicFactory,
        ILogger<IBluetoothLocalService>? logger = null) : base(broadcaster, request, localCharacteristicFactory, logger)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }
}
