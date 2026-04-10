using Bluetooth.Avalonia.Platforms.Linux.Scanning.Factories;

namespace Bluetooth.Avalonia.Platforms.Linux.Scanning;

/// <summary>
///     Linux BLE remote GATT service backed by a BlueZ <c>org.bluez.GattService1</c> D-Bus object.
/// </summary>
public class LinuxBluetoothRemoteService : BaseBluetoothRemoteService
{
    private readonly LinuxBluetoothAdapter _adapter;

    /// <inheritdoc />
    public LinuxBluetoothRemoteService(
        IBluetoothRemoteDevice parentDevice,
        IBluetoothRemoteServiceFactory.BluetoothRemoteServiceFactorySpec spec,
        IBluetoothRemoteCharacteristicFactory characteristicFactory,
        IBluetoothNameProvider? nameProvider = null,
        ILogger<IBluetoothRemoteService>? logger = null)
        : base(parentDevice, spec, characteristicFactory, nameProvider, logger)
    {
        ArgumentNullException.ThrowIfNull(spec);

        if (spec is not LinuxBluetoothRemoteServiceFactorySpec linuxSpec)
        {
            throw new ArgumentException(
                $"Expected {nameof(LinuxBluetoothRemoteServiceFactorySpec)} but got {spec.GetType()}.",
                nameof(spec));
        }

        ObjectPath = linuxSpec.ObjectPath;

        if (parentDevice is not LinuxBluetoothRemoteDevice linuxDevice)
        {
            throw new ArgumentException(
                "Parent device must be a LinuxBluetoothRemoteDevice.", nameof(parentDevice));
        }

        _adapter = linuxDevice.Adapter;
    }

    /// <summary>
    ///     Gets the D-Bus object path of this GATT service.
    /// </summary>
    internal string ObjectPath { get; }

    // ==================== Native overrides ====================

    /// <inheritdoc />
    protected override async ValueTask NativeCharacteristicsExplorationAsync(
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var connection = _adapter.Connection;

            var objects = await BlueZObjectManager
                .GetManagedObjectsAsync(connection, cancellationToken)
                .ConfigureAwait(false);

            var characteristicObjects = objects
                .Where(o => o.HasInterface(BlueZConstants.GattCharacteristic1Interface)
                            && o.Path.StartsWith(ObjectPath + "/", StringComparison.Ordinal))
                .ToList();

            OnCharacteristicsExplorationSucceeded(
                characteristicObjects,
                (native, existing) => existing is LinuxBluetoothRemoteCharacteristic c && c.ObjectPath == native.Path,
                native =>
                {
                    var spec = new LinuxBluetoothRemoteCharacteristicFactorySpec(native);
                    return (CharacteristicFactory
                            ?? throw new InvalidOperationException(
                                "CharacteristicFactory must be set via the spec-based constructor."))
                        .Create(this, spec);
                });
        }
        catch (Exception ex)
        {
            OnCharacteristicsExplorationFailed(ex);
        }
    }
}

