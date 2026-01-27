using Bluetooth.Abstractions.Broadcasting;
using Bluetooth.Abstractions.Enums;

namespace Bluetooth.Core.Broadcasting;

/// <summary>
/// Base class for Bluetooth broadcast characteristics.
/// </summary>
public abstract class BaseBluetoothBroadcastCharacteristic : BaseBindableObject, IBluetoothBroadcastCharacteristic
{
    /// <inheritdoc />
    public IBluetoothBroadcastService Service { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseBluetoothBroadcastCharacteristic"/> class.
    /// </summary>
    protected BaseBluetoothBroadcastCharacteristic(IBluetoothBroadcastService service,
        IBluetoothBroadcastCharacteristicFactory.BluetoothBroadcastCharacteristicFactoryRequest request)
    {
        Service = service;
        ArgumentNullException.ThrowIfNull(request);
        Name = request.Name;
        Permissions = request.Permissions;
        Properties = request.Properties;
        IsPrimary = request.IsPrimary;
        Id = request.Id;
        Value = request.InitialValue ?? new ReadOnlyMemory<byte>([]);
    }

    /// <inheritdoc />
    public Guid Id { get; }

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public bool IsPrimary { get; }

    /// <inheritdoc />
    public CharacteristicProperties Properties { get; }

    /// <inheritdoc />
    public CharacteristicPermissions Permissions { get; }

    /// <inheritdoc />
    public ReadOnlySpan<byte> ValueSpan => Value.Span;

    /// <inheritdoc />
    public ReadOnlyMemory<byte> Value { get; private set; }

    /// <inheritdoc />
    public abstract ValueTask DisposeAsync();

    /// <inheritdoc/>
    public async Task UpdateValueAsync(ReadOnlyMemory<byte> value, bool notifyClients = true, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        await NativeUpdateValueAsync(value, notifyClients, timeout, cancellationToken).ConfigureAwait(false);
        Value = value;
    }

    /// <summary>
    /// Native implementation of updating the characteristic value.
    /// </summary>
    /// <param name="value">The new value to set.</param>
    /// <param name="notifyClients">Indicates whether to notify connected clients of the value change.</param>
    /// <param name="timeout">The timeout for this operation.</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected abstract Task NativeUpdateValueAsync(ReadOnlyMemory<byte> value, bool notifyClients = true, TimeSpan? timeout = null, CancellationToken cancellationToken = default);
}
