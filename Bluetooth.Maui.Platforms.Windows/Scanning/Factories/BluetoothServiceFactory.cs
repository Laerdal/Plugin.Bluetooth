using Bluetooth.Abstractions.Scanning.Factories;
using Bluetooth.Core.Scanning.Factories;

namespace Bluetooth.Maui.Platforms.Windows.Scanning.Factories;

/// <inheritdoc/>
public class BluetoothServiceFactory : BaseBluetoothServiceFactory, Abstractions.Scanning.Factories.IBluetoothServiceFactory
{
    /// <inheritdoc/>
    /// <exception cref="PlatformNotSupportedException"></exception>
    public BluetoothServiceFactory(IBluetoothCharacteristicFactory characteristicFactory) : base(characteristicFactory)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
       }

    /// <inheritdoc/>
    /// <exception cref="PlatformNotSupportedException"></exception>
    public override Abstractions.Scanning.IBluetoothRemoteService CreateService(IBluetoothRemoteDevice device, IBluetoothServiceFactory.BluetoothServiceFactoryRequest request)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }
}
