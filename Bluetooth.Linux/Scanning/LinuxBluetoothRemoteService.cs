using Bluetooth.Linux.Scanning.Factories;

namespace Bluetooth.Linux.Scanning;

/// <summary>
///     Linux implementation of a remote GATT service backed by a BlueZ D-Bus <see cref="IGattService1"/> proxy.
/// </summary>
public class LinuxBluetoothRemoteService : BaseBluetoothRemoteService
{
    private readonly IGattService1 _nativeService;

    /// <inheritdoc />
    public LinuxBluetoothRemoteService(
        IBluetoothRemoteDevice parentDevice,
        IBluetoothRemoteServiceFactory.BluetoothRemoteServiceFactorySpec spec,
        IBluetoothRemoteCharacteristicFactory characteristicFactory,
        IBluetoothNameProvider? nameProvider = null,
        ILogger<IBluetoothRemoteService>? logger = null)
        : base(parentDevice, spec, characteristicFactory, nameProvider, logger)
    {
        if (spec is LinuxBluetoothRemoteServiceFactorySpec linuxSpec)
        {
            _nativeService = linuxSpec.NativeService;
        }
        else
        {
            throw new ArgumentException($"Expected {nameof(LinuxBluetoothRemoteServiceFactorySpec)}", nameof(spec));
        }
    }

    /// <inheritdoc />
    protected override async ValueTask NativeCharacteristicsExplorationAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var nativeCharacteristics = await _nativeService.GetCharacteristicsAsync().ConfigureAwait(false);

            // Pre-fetch UUID and Flags asynchronously before entering the synchronous conversion lambda
            var charEntries = await Task.WhenAll(nativeCharacteristics.Select(async ch =>
            {
                var props = await ch.GetAllAsync().ConfigureAwait(false);
                var normalized = BlueZManager.NormalizeUUID(props.UUID ?? string.Empty);
                var id = Guid.TryParse(normalized, out var g) ? g : Guid.Empty;
                return (Native: ch, Id: id, Flags: props.Flags ?? []);
            })).ConfigureAwait(false);

            var validEntries = charEntries.Where(e => e.Id != Guid.Empty).ToList();

            OnCharacteristicsExplorationSucceeded(
                validEntries,
                areRepresentingTheSameObject: (entry, shared) => entry.Id == shared.Id,
                fromInputTypeToOutputTypeConversion: entry =>
                {
                    var spec = new LinuxBluetoothRemoteCharacteristicFactorySpec(entry.Id, entry.Native, entry.Flags);
                    return (CharacteristicFactory ?? throw new InvalidOperationException("CharacteristicFactory must be set")).Create(this, spec);
                });
        }
        catch (Exception ex)
        {
            OnCharacteristicsExplorationFailed(ex);
        }
    }
}
