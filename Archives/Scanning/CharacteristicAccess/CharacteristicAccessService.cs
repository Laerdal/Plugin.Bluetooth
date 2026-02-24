namespace Bluetooth.Core.Scanning.CharacteristicAccess;

/// <summary>
/// Abstract base class for accessing Bluetooth characteristics, providing common functionality for service and characteristic management.
/// </summary>
public abstract partial class CharacteristicAccessService : Abstractions.Scanning.AccessService.IBluetoothCharacteristicAccessService
{

    /// <summary>
    /// Initializes a new instance of the <see cref="CharacteristicAccessService"/> class.
    /// </summary>
    /// <param name="characteristicId">The unique identifier of the characteristic.</param>
    /// <param name="characteristicName">The name of the characteristic.</param>
    protected CharacteristicAccessService(Guid characteristicId, string characteristicName)
    {
        CharacteristicId = characteristicId;
        CharacteristicName = characteristicName;
        ServiceId = Guid.Empty;
        ServiceName = "Unknown Service";
    }

    #region Properties

    /// <inheritdoc />
    public Guid ServiceId { get; private set; }

    /// <inheritdoc />
    public string ServiceName { get; private set; }

    /// <inheritdoc />
    public Guid CharacteristicId { get; }

    /// <inheritdoc />
    public string CharacteristicName { get; }

    #endregion

    /// <inheritdoc />
    public void SetServiceInformation(Guid serviceId, string serviceName)
    {
        if (serviceId == Guid.Empty)
        {
            throw new ArgumentException("ServiceId cannot be empty", nameof(serviceId));
        }

        if (string.IsNullOrWhiteSpace(serviceName))
        {
            throw new ArgumentException("ServiceName cannot be empty", nameof(serviceName));
        }

        ServiceId = serviceId;
        ServiceName = serviceName;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{ServiceName} ({ServiceId}) > {CharacteristicName} ({CharacteristicId})";
    }
}

/// <summary>
/// Abstract base class for accessing Bluetooth characteristics with single type for both read and write operations.
/// </summary>
/// <typeparam name="TReadWrite">The type used for both read and write operations.</typeparam>
public abstract class CharacteristicAccessService<TReadWrite> : CharacteristicAccessService<TReadWrite, TReadWrite>, IBluetoothCharacteristicAccessService<TReadWrite>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CharacteristicAccessService{TReadWrite}"/> class.
    /// </summary>
    /// <param name="characteristicId">The unique identifier of the characteristic.</param>
    /// <param name="name">The name of the characteristic.</param>
    protected CharacteristicAccessService(Guid characteristicId, string name) : base(characteristicId, name)
    {
    }
}

/// <summary>
/// Abstract base class for accessing Bluetooth characteristics with separate types for read and write operations.
/// </summary>
/// <typeparam name="TRead">The type of object that is read from the characteristic.</typeparam>
/// <typeparam name="TWrite">The type of object that is written to the characteristic.</typeparam>
public abstract partial class CharacteristicAccessService<TRead, TWrite> : CharacteristicAccessService, Abstractions.Scanning.AccessService.IBluetoothCharacteristicAccessService<TRead, TWrite>, IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CharacteristicAccessService{TRead, TWrite}"/> class.
    /// </summary>
    /// <param name="characteristicId">The unique identifier of the characteristic.</param>
    /// <param name="name">The name of the characteristic.</param>
    protected CharacteristicAccessService(Guid characteristicId, string name) : base(characteristicId, name) { }

    /// <inheritdoc />
    public Type ValueTypeRead => typeof(TRead);

    /// <inheritdoc />
    public Type ValueTypeWrite => typeof(TWrite);

    /// <summary>
    /// Converts the input value to a byte array for writing to the characteristic.
    /// </summary>
    /// <param name="input">The input value to convert.</param>
    /// <returns>A read-only span of bytes representing the input value.</returns>
    protected abstract ReadOnlyMemory<byte> ToBytes(TWrite input);

    /// <summary>
    /// Converts a byte array from the characteristic to the output type.
    /// </summary>
    /// <param name="value">The byte array to convert.</param>
    /// <returns>The converted value of type TRead.</returns>
    protected abstract TRead FromBytes(ReadOnlyMemory<byte> value);
}
