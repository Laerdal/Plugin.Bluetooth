namespace Bluetooth.Abstractions.Scanning.AccessService;

/// <summary>
/// Interface representing a service for accessing Bluetooth characteristics, providing methods for reading, writing, and listening to characteristics.
/// </summary>
public partial interface IBluetoothCharacteristicAccessService
{
    /// <summary>
    /// Gets the unique identifier of the service.
    /// </summary>
    Guid ServiceId { get; }
    
    /// <summary>
    /// Gets the name of the service.
    /// </summary>
    string ServiceName { get; }
    
    /// <summary>
    /// Sets the service information.
    /// </summary>
    /// <param name="serviceId">The unique identifier of the service.</param>
    /// <param name="serviceName">The name of the service.</param>
    void SetServiceInformation(Guid serviceId, string serviceName);

    /// <summary>
    /// Gets the unique identifier of the characteristic.
    /// </summary>
    Guid CharacteristicId { get; }
    
    /// <summary>
    /// Gets the name of the characteristic.
    /// </summary>
    string CharacteristicName { get; }
}

/// <summary>
/// Generic interface representing a service for accessing Bluetooth characteristics with specific input and output types.
/// </summary>
/// <typeparam name="T">The type of the characteristic value.</typeparam>
public interface IBluetoothCharacteristicAccessService<T> : IBluetoothCharacteristicAccessService<T, T>
{
}

/// <summary>
/// Generic interface representing a service for accessing Bluetooth characteristics with different input and output types.
/// </summary>
/// <typeparam name="TRead">The type of the input value.</typeparam>
/// <typeparam name="TWrite">The type of the output value.</typeparam>
public partial interface IBluetoothCharacteristicAccessService<TRead, TWrite> : IBluetoothCharacteristicAccessService
{
    /// <summary>
    /// Gets the type of the input value.
    /// </summary>
    Type ValueTypeRead { get; }

    /// <summary>
    /// Gets the type of the output value.
    /// </summary>
    Type ValueTypeWrite { get; }
}