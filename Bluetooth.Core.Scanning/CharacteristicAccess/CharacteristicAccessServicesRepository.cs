using System.Reflection;

using Bluetooth.Abstractions.AccessService;

namespace Bluetooth.Core.Scanning.CharacteristicAccess;

/// <summary>
/// Repository for managing Bluetooth characteristic access services and service definitions.
/// </summary>
public class CharacteristicAccessServicesRepository : BaseBindableObject, IBluetoothCharacteristicAccessServicesRepository
{
    /// <summary>
    /// Gets the dictionary of service names indexed by service ID.
    /// </summary>
    private Dictionary<Guid, string> ServiceNames { get; } = new Dictionary<Guid, string>();

    /// <summary>
    /// Gets the dictionary of characteristic access services indexed by characteristic ID.
    /// </summary>
    private Dictionary<Guid, IBluetoothCharacteristicAccessService> CharacteristicsAccessServices { get; } = new Dictionary<Guid, IBluetoothCharacteristicAccessService>();

    #region ADD

    /// <inheritdoc />
    public void AddAllServiceDefinitionsInCurrentAssembly()
    {
        AddAllServiceDefinitionsInAssembly(Assembly.GetCallingAssembly());
    }

    /// <inheritdoc />
    public ValueTask AddAllServiceDefinitionsInCurrentAssemblyAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        return AddAllServiceDefinitionsInAssemblyAsync(Assembly.GetCallingAssembly(), timeout, cancellationToken);
    }

    /// <inheritdoc />
    public void AddAllServiceDefinitionsInAssembly(string assemblyName)
    {
        AddAllServiceDefinitionsInAssembly(GetAssemblyFromName(assemblyName));
    }

    /// <inheritdoc />
    public ValueTask AddAllServiceDefinitionsInAssemblyAsync(string assemblyName, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        return AddAllServiceDefinitionsInAssemblyAsync(GetAssemblyFromName(assemblyName), timeout, cancellationToken);
    }

    /// <inheritdoc />
    public void AddAllServiceDefinitionsInAssembly(Assembly assembly)
    {
        // Find all classes with the ServiceDefinition attribute
        // It returns the type of the class and the attribute
        var serviceDefinitions = ServiceDefinitionAttribute.GetAllServiceDefinitionsInAssemblyOf(assembly);

        foreach ((var serviceDefinition, var serviceType) in serviceDefinitions)
        {
            AddServiceDefinition(serviceType, serviceDefinition.IdField, serviceDefinition.NameField);
        }
    }

    /// <inheritdoc />
    public async ValueTask AddAllServiceDefinitionsInAssemblyAsync(Assembly assembly, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        // Find all classes with the ServiceDefinition attribute
        // It returns the type of the class and the attribute
        var serviceDefinitions = await ServiceDefinitionAttribute.GetAllServiceDefinitionsInAssemblyOfAsync(assembly, timeout, cancellationToken: cancellationToken).ConfigureAwait(false);

        foreach ((var serviceDefinition, var serviceType) in serviceDefinitions)
        {
            AddServiceDefinition(serviceType, serviceDefinition.IdField, serviceDefinition.NameField);
        }
    }

    /// <summary>
    /// Adds a service definition from a type that contains service information.
    /// </summary>
    /// <param name="serviceType">The type containing service definition.</param>
    /// <param name="idFieldName">The name of the field containing the service ID.</param>
    /// <param name="nameFieldName">The name of the field containing the service name.</param>
    private void AddServiceDefinition(Type serviceType, string idFieldName = "Id", string nameFieldName = "Name")
    {
        // Read the Service Name
        var serviceName = serviceType.GetField(nameFieldName)?.GetValue(null) as string ?? string.Empty;

        // Read the Service Id
        var serviceId = serviceType.GetField(idFieldName)?.GetValue(null) as Guid? ?? Guid.Empty;

        AddKnownServiceName(serviceId, serviceName);

        // Read the characteristics information
        var characteristicAccessServices = serviceType.GetFields().Select(f => f.GetValue(null)).Where(f => f is CharacteristicAccessService).Cast<IBluetoothCharacteristicAccessService>();

        foreach (var characteristicAccessService in characteristicAccessServices)
        {
            characteristicAccessService.SetServiceInformation(serviceId, serviceName);
            AddKnownCharacteristicAccessService(characteristicAccessService);
        }
    }

    /// <inheritdoc />
    public void AddKnownServiceName(Guid serviceId, string serviceName)
    {
        if (string.IsNullOrWhiteSpace(serviceName))
        {
            throw new ArgumentException($"Error while reading service definition of {serviceId} : Service Name can't be empty", nameof(serviceName));
        }

        if (serviceId == Guid.Empty)
        {
            throw new ArgumentException($"Error while reading service definition of {serviceName} : Service Id can't be empty", nameof(serviceId));
        }

        if (!ServiceNames.TryAdd(serviceId, serviceName))
        {
            if (ServiceNames[serviceId] != serviceName)
            {
                // LOG : WARNING - Service definition already exists
            }
        }
        else
        {
            // LOG : TRACE - Service definition added
        }
    }

    /// <inheritdoc />
    public void AddKnownCharacteristicAccessService(IBluetoothCharacteristicAccessService characteristicAccessService)
    {
        ArgumentNullException.ThrowIfNull(characteristicAccessService);

        if (characteristicAccessService.CharacteristicId == Guid.Empty)
        {
            throw new ArgumentException("Characteristic Id can't be empty", nameof(characteristicAccessService));
        }

        if (characteristicAccessService.CharacteristicName == "Unknown Characteristic")
        {
            throw new ArgumentException("Characteristic Name can't be empty", nameof(characteristicAccessService));
        }

        if (characteristicAccessService.ServiceId == Guid.Empty)
        {
            throw new ArgumentException("Service Id can't be empty", nameof(characteristicAccessService));
        }

        if (characteristicAccessService.ServiceName == "Unknown Service")
        {
            throw new ArgumentException("Service Name can't be empty", nameof(characteristicAccessService));
        }

        if (!CharacteristicsAccessServices.TryAdd(characteristicAccessService.CharacteristicId, characteristicAccessService))
        {
            var preexistingService = CharacteristicsAccessServices[characteristicAccessService.CharacteristicId];
            if (preexistingService != characteristicAccessService)
            {
                // LOG : WARNING - Characteristic access service already exists for ID
            }
        }
        else
        {
            // LOG : TRACE - Characteristic access service added
        }
    }

    #endregion

    #region GET

    /// <inheritdoc />
    public string GetServiceName(Guid serviceId)
    {
        if (serviceId == Guid.Empty)
        {
            // LOG : WARNING - No Service Name for Guid.Empty
            return "Unknown Service";
        }

        if (!ServiceNames.TryGetValue(serviceId, out var output))
        {
            // LOG : WARNING - Service Name not found
            return "Unknown Service";
        }

        // LOG : TRACE - Service Name found
        return output;
    }

    /// <inheritdoc />
    public IBluetoothCharacteristicAccessService GetCharacteristicAccessService(Guid characteristicId)
    {
        return TryGetCharacteristicAccessService(characteristicId) ?? new UnknownCharacteristicAccessService();
    }

    /// <inheritdoc />
    public IBluetoothCharacteristicAccessService? TryGetCharacteristicAccessService(Guid characteristicId)
    {
        if (characteristicId == Guid.Empty)
        {
            // LOG : WARNING - No CharacteristicAccessService for Guid.Empty
            return null;
        }

        if (!CharacteristicsAccessServices.TryGetValue(characteristicId, out var output))
        {
            // LOG : WARNING - CharacteristicAccessService not found
            return null;
        }

        // LOG : TRACE - CharacteristicAccessService found
        return output;
    }

    #endregion

    /// <summary>
    ///   Gets an assembly from its name.
    /// </summary>
    /// <param name="assemblyName"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    protected static Assembly GetAssemblyFromName(string assemblyName)
    {
        ArgumentNullException.ThrowIfNull(assemblyName);

        return AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name == assemblyName)
               ?? throw new InvalidOperationException($"Assembly with name '{assemblyName}' not found in current AppDomain.");
    }
}
