using System.Collections.Concurrent;
using System.Collections.ObjectModel;

using Bluetooth.Abstractions.Broadcasting;
using Bluetooth.Abstractions.Enums;
using Bluetooth.Core.Broadcasting.Exceptions;

namespace Bluetooth.Core.Broadcasting;

/// <summary>
/// Base class for Bluetooth broadcaster services.
/// </summary>
public abstract class BaseBluetoothBroadcastService : BaseBindableObject, IBluetoothBroadcastService
{
    /// <inheritdoc/>
    public IBluetoothBroadcaster Broadcaster { get; }

    /// <inheritdoc/>
    public IBluetoothBroadcastCharacteristicFactory CharacteristicFactory { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseBluetoothBroadcastService"/> class.
    /// </summary>
    /// <param name="broadcaster">The broadcaster that owns this service.</param>
    /// <param name="request">The request for creating the service.</param>
    /// <param name="characteristicFactory">The factory for creating characteristics.</param>
    protected BaseBluetoothBroadcastService(IBluetoothBroadcaster broadcaster,
        IBluetoothBroadcastServiceFactory.BluetoothBroadcastServiceFactoryRequest request,
        IBluetoothBroadcastCharacteristicFactory characteristicFactory)
    {
        Broadcaster = broadcaster;
        CharacteristicFactory = characteristicFactory;
        ArgumentNullException.ThrowIfNull(request);
        Id = request.Id;
        Name = request.Name;
        IsPrimary = request.IsPrimary;
        Characteristics = new ReadOnlyDictionary<Guid, IBluetoothBroadcastCharacteristic>(WritableCharacteristicsList);
    }

    /// <inheritdoc/>
    public Guid Id { get; }

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public bool IsPrimary { get; }

    /// <inheritdoc/>
    public ReadOnlyDictionary<Guid, IBluetoothBroadcastCharacteristic> Characteristics { get; }

    private Dictionary<Guid, IBluetoothBroadcastCharacteristic> WritableCharacteristicsList { get; } = new Dictionary<Guid, IBluetoothBroadcastCharacteristic>();

    /// <inheritdoc/>
    public async Task<IBluetoothBroadcastCharacteristic> AddCharacteristicAsync(Guid id,
        string name,
        CharacteristicProperties properties,
        CharacteristicPermissions permissions,
        ReadOnlyMemory<byte>? initialValue = null,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        if (WritableCharacteristicsList.ContainsKey(id))
        {
            throw new BroadcasterCharacteristicAlreadyExistsException(this, id);
        }
        var newCharacteristicRequest = new IBluetoothBroadcastCharacteristicFactory.BluetoothBroadcastCharacteristicFactoryRequest()
        {
            Id = id,
            Name = name,
            Properties = properties,
            Permissions = permissions,
            InitialValue = initialValue,
        };
        var newCharacteristic = await CharacteristicFactory.CreateBroadcastCharacteristicAsync(this, newCharacteristicRequest, timeout, cancellationToken).ConfigureAwait(false);
        lock (WritableCharacteristicsList)
        {
            WritableCharacteristicsList.Add(id, newCharacteristic);
        }
        return newCharacteristic;
    }

    /// <inheritdoc/>
    public Task RemoveCharacteristicAsync(Guid id, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        if (!WritableCharacteristicsList.TryGetValue(id, out var characteristic))
        {
            throw new BroadcasterCharacteristicNotFoundException(this, id);
        }
        return RemoveCharacteristicAsync(characteristic, timeout, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task RemoveCharacteristicAsync(IBluetoothBroadcastCharacteristic characteristic, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(characteristic);
        var id = characteristic.Id;
        if (!WritableCharacteristicsList.ContainsKey(id))
        {
            throw new BroadcasterCharacteristicNotFoundException(this, id);
        }
        await characteristic.DisposeAsync().ConfigureAwait(false);
        WritableCharacteristicsList.Remove(characteristic.Id);
    }

    /// <inheritdoc/>
    public async Task RemoveAllCharacteristicsAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        foreach (var characteristic in WritableCharacteristicsList.Values.ToList())
        {
            await RemoveCharacteristicAsync(characteristic, timeout, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public async virtual ValueTask DisposeAsync()
    {
        await RemoveAllCharacteristicsAsync().ConfigureAwait(false);
            }
}
