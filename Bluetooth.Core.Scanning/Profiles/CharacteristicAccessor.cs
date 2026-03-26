namespace Bluetooth.Core.Scanning.Profiles;

/// <summary>
///     Typed accessor for reading and writing a known Bluetooth characteristic.
/// </summary>
/// <typeparam name="TRead">The typed value produced when reading the characteristic.</typeparam>
/// <typeparam name="TWrite">The typed value accepted when writing the characteristic.</typeparam>
public class CharacteristicAccessor<TRead, TWrite> : IBluetoothCharacteristicAccessor<TRead, TWrite>
{
    private readonly ICharacteristicCodec<TRead, TWrite> _codec;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicAccessor{TRead, TWrite}" /> class.
    /// </summary>
    /// <param name="serviceId">The UUID of the service that owns the target characteristic.</param>
    /// <param name="characteristicId">The UUID of the target characteristic.</param>
    /// <param name="codec">The codec used to decode reads and encode writes.</param>
    /// <param name="serviceName">Optional known service name used for diagnostics.</param>
    /// <param name="characteristicName">Optional known characteristic name used for diagnostics.</param>
    public CharacteristicAccessor(
        Guid serviceId,
        Guid characteristicId,
        ICharacteristicCodec<TRead, TWrite> codec,
        string? serviceName = null,
        string? characteristicName = null)
    {
        ArgumentNullException.ThrowIfNull(codec);

        ServiceId = serviceId;
        CharacteristicId = characteristicId;
        _codec = codec;

        ServiceName = string.IsNullOrWhiteSpace(serviceName) ? "Unknown Service" : serviceName;
        CharacteristicName = string.IsNullOrWhiteSpace(characteristicName) ? "Unknown Characteristic" : characteristicName;
    }

    /// <inheritdoc />
    public Guid ServiceId { get; }

    /// <inheritdoc />
    public Guid CharacteristicId { get; }

    /// <inheritdoc />
    public string ServiceName { get; }

    /// <inheritdoc />
    public string CharacteristicName { get; }

    /// <inheritdoc />
    public bool CanRead(IBluetoothRemoteDevice device)
    {
        return TryGetResolvedCharacteristic(device)?.CanRead == true;
    }

    /// <inheritdoc />
    public bool CanWrite(IBluetoothRemoteDevice device)
    {
        return TryGetResolvedCharacteristic(device)?.CanWrite == true;
    }

    /// <inheritdoc />
    public bool CanListen(IBluetoothRemoteDevice device)
    {
        return TryGetResolvedCharacteristic(device)?.CanListen == true;
    }

    /// <inheritdoc />
    public async ValueTask<IBluetoothRemoteCharacteristic> ResolveCharacteristicAsync(
        IBluetoothRemoteDevice device,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(device);

        var service = device.GetServiceOrDefault(ServiceId);
        if (service == null)
        {
            await device.ExploreServicesAsync(ServiceExplorationOptions.WithCharacteristics, timeout, cancellationToken).ConfigureAwait(false);
            service = device.GetServiceOrDefault(ServiceId);
        }

        if (service == null)
        {
            throw new CharacteristicAccessorResolutionException(
                ServiceId,
                CharacteristicId,
                $"Could not resolve service '{ServiceName}' ({ServiceId}) on device '{device.Id}'.");
        }

        var characteristic = service.GetCharacteristicOrDefault(CharacteristicId);
        if (characteristic == null)
        {
            await service.ExploreCharacteristicsAsync(null, timeout, cancellationToken).ConfigureAwait(false);
            characteristic = service.GetCharacteristicOrDefault(CharacteristicId);
        }

        if (characteristic == null)
        {
            throw new CharacteristicAccessorResolutionException(
                ServiceId,
                CharacteristicId,
                $"Could not resolve characteristic '{CharacteristicName}' ({CharacteristicId}) in service '{ServiceName}' ({ServiceId}) on device '{device.Id}'.");
        }

        return characteristic;
    }

    /// <inheritdoc />
    public async ValueTask<TRead> ReadAsync(
        IBluetoothRemoteDevice device,
        bool skipIfPreviouslyRead = false,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        var characteristic = await ResolveCharacteristicAsync(device, timeout, cancellationToken).ConfigureAwait(false);
        var bytes = await characteristic.ReadValueAsync(skipIfPreviouslyRead, timeout, cancellationToken).ConfigureAwait(false);

        try
        {
            return _codec.FromBytes(bytes);
        }
        catch (CharacteristicCodecException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new CharacteristicCodecException(
                typeof(TRead),
                bytes,
                $"Failed to decode read payload for characteristic '{CharacteristicName}' ({CharacteristicId}).",
                ex);
        }
    }

    /// <inheritdoc />
    public async ValueTask WriteAsync(
        IBluetoothRemoteDevice device,
        TWrite value,
        bool skipIfOldValueMatchesNewValue = false,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        var characteristic = await ResolveCharacteristicAsync(device, timeout, cancellationToken).ConfigureAwait(false);

        ReadOnlyMemory<byte> encodedValue;
        try
        {
            encodedValue = _codec.ToBytes(value);
        }
        catch (CharacteristicCodecException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new CharacteristicCodecException(
                typeof(TWrite),
                ReadOnlyMemory<byte>.Empty,
                $"Failed to encode write payload for characteristic '{CharacteristicName}' ({CharacteristicId}).",
                ex);
        }

        await characteristic.WriteValueAsync(encodedValue, skipIfOldValueMatchesNewValue, timeout, cancellationToken).ConfigureAwait(false);
    }

    private IBluetoothRemoteCharacteristic? TryGetResolvedCharacteristic(IBluetoothRemoteDevice device)
    {
        ArgumentNullException.ThrowIfNull(device);

        var service = device.GetServiceOrDefault(ServiceId);
        return service?.GetCharacteristicOrDefault(CharacteristicId);
    }
}
