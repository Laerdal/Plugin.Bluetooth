namespace Bluetooth.Core.Scanning.CharacteristicAccess;

/// <summary>
/// Repository for managing Bluetooth characteristic access services and service definitions.
/// </summary>
public partial class CharacteristicAccessServicesRepository : BaseBindableObject, IBluetoothCharacteristicAccessServicesRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CharacteristicAccessServicesRepository"/> class.
    /// </summary>
    /// <param name="logger">Optional logger instance for tracking repository operations.</param>
    public CharacteristicAccessServicesRepository(ILogger<CharacteristicAccessServicesRepository>? logger = null)
        : base(logger)
    {
    }

    #region Logging

    // Service Name operations
    [LoggerMessage(Level = LogLevel.Trace, Message = "Service definition added: {ServiceId} - {ServiceName}")]
    private static partial void LogServiceDefinitionAdded(ILogger logger, Guid serviceId, string serviceName);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Service definition already exists with different name: {ServiceId} - Existing: '{ExistingName}', New: '{NewName}'")]
    private static partial void LogServiceDefinitionConflict(ILogger logger, Guid serviceId, string existingName, string newName);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Cannot find service name for Guid.Empty")]
    private static partial void LogServiceNameEmptyGuid(ILogger logger);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Service name not found for service ID: {ServiceId}")]
    private static partial void LogServiceNameNotFound(ILogger logger, Guid serviceId);

    [LoggerMessage(Level = LogLevel.Trace, Message = "Service name found: {ServiceId} - {ServiceName}")]
    private static partial void LogServiceNameFound(ILogger logger, Guid serviceId, string serviceName);

    // Characteristic Access Service operations
    [LoggerMessage(Level = LogLevel.Trace, Message = "Characteristic access service added: {CharacteristicId} - {CharacteristicName} (Service: {ServiceId} - {ServiceName})")]
    private static partial void LogCharacteristicAccessServiceAdded(ILogger logger, Guid characteristicId, string characteristicName, Guid serviceId, string serviceName);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Characteristic access service already exists for ID: {CharacteristicId} - {CharacteristicName}")]
    private static partial void LogCharacteristicAccessServiceConflict(ILogger logger, Guid characteristicId, string characteristicName);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Cannot find characteristic name for Guid.Empty")]
    private static partial void LogCharacteristicNameEmptyGuid(ILogger logger);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Characteristic name not found for characteristic ID: {CharacteristicId}")]
    private static partial void LogCharacteristicNameNotFound(ILogger logger, Guid characteristicId);

    [LoggerMessage(Level = LogLevel.Trace, Message = "Characteristic name found: {CharacteristicId} - {CharacteristicName}")]
    private static partial void LogCharacteristicNameFound(ILogger logger, Guid characteristicId, string characteristicName);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Cannot find characteristic access service for Guid.Empty")]
    private static partial void LogCharacteristicAccessServiceEmptyGuid(ILogger logger);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Characteristic access service not found for characteristic ID: {CharacteristicId}")]
    private static partial void LogCharacteristicAccessServiceNotFound(ILogger logger, Guid characteristicId);

    [LoggerMessage(Level = LogLevel.Trace, Message = "Characteristic access service found: {CharacteristicId} - {CharacteristicName}")]
    private static partial void LogCharacteristicAccessServiceFound(ILogger logger, Guid characteristicId, string characteristicName);

    #endregion

    /// <summary>
    /// Gets the dictionary of service names indexed by service ID.
    /// </summary>
    private Dictionary<Guid, string> ServiceNames { get; } = new Dictionary<Guid, string>();

    /// <summary>
    /// Gets the dictionary of characteristic access services indexed by characteristic ID.
    /// </summary>
    private Dictionary<Guid, Abstractions.Scanning.AccessService.IBluetoothCharacteristicAccessService> CharacteristicsAccessServices { get; } =
        new Dictionary<Guid, Abstractions.Scanning.AccessService.IBluetoothCharacteristicAccessService>();

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
        var characteristicAccessServices = serviceType.GetFields().Select(f => f.GetValue(null)).Where(f => f is CharacteristicAccessService).Cast<Abstractions.Scanning.AccessService.IBluetoothCharacteristicAccessService>();

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
                if (Logger is not null)
                {
                    LogServiceDefinitionConflict(Logger, serviceId, ServiceNames[serviceId], serviceName);
                }
            }
        }
        else
        {
            if (Logger is not null)
            {
                LogServiceDefinitionAdded(Logger, serviceId, serviceName);
            }
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
                if (Logger is not null)
                {
                    LogCharacteristicAccessServiceConflict(Logger, characteristicAccessService.CharacteristicId, characteristicAccessService.CharacteristicName);
                }
            }
        }
        else
        {
            if (Logger is not null)
            {
                LogCharacteristicAccessServiceAdded(Logger, characteristicAccessService.CharacteristicId, characteristicAccessService.CharacteristicName, characteristicAccessService.ServiceId, characteristicAccessService.ServiceName);
            }
        }
    }

    #endregion

    #region GET

    /// <inheritdoc />
    public string GetServiceName(Guid serviceId)
    {
        if (serviceId == Guid.Empty)
        {
            if (Logger is not null)
            {
                LogServiceNameEmptyGuid(Logger);
            }
            return "Unknown Service";
        }

        if (!ServiceNames.TryGetValue(serviceId, out var output))
        {
            if (Logger is not null)
            {
                LogServiceNameNotFound(Logger, serviceId);
            }
            return "Unknown Service";
        }

        if (Logger is not null)
        {
            LogServiceNameFound(Logger, serviceId, output);
        }
        return output;
    }

    /// <inheritdoc />
    public string GetCharacteristicName(Guid characteristicId)
    {
        if (characteristicId == Guid.Empty)
        {
            if (Logger is not null)
            {
                LogCharacteristicNameEmptyGuid(Logger);
            }
            return "Unknown Characteristic";
        }

        if (!CharacteristicsAccessServices.TryGetValue(characteristicId, out var accessService))
        {
            if (Logger is not null)
            {
                LogCharacteristicNameNotFound(Logger, characteristicId);
            }
            return "Unknown Characteristic";
        }

        if (Logger is not null)
        {
            LogCharacteristicNameFound(Logger, characteristicId, accessService.CharacteristicName);
        }
        return accessService.CharacteristicName;
    }

    /// <inheritdoc />
    public Abstractions.Scanning.AccessService.IBluetoothCharacteristicAccessService GetCharacteristicAccessService(Guid characteristicId)
    {
        return GetCharacteristicAccessServiceOrDefault(characteristicId) ?? new UnknownCharacteristicAccessService();
    }

    /// <inheritdoc />
    public Abstractions.Scanning.AccessService.IBluetoothCharacteristicAccessService? GetCharacteristicAccessServiceOrDefault(Guid characteristicId)
    {
        if (characteristicId == Guid.Empty)
        {
            if (Logger is not null)
            {
                LogCharacteristicAccessServiceEmptyGuid(Logger);
            }
            return null;
        }

        if (!CharacteristicsAccessServices.TryGetValue(characteristicId, out var output))
        {
            if (Logger is not null)
            {
                LogCharacteristicAccessServiceNotFound(Logger, characteristicId);
            }
            return null;
        }

        if (Logger is not null)
        {
            LogCharacteristicAccessServiceFound(Logger, characteristicId, output.CharacteristicName);
        }
        return output;
    }

    #endregion

    /// <summary>
    ///   Gets an assembly from its name.
    /// </summary>
    /// <param name="assemblyName"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private static Assembly GetAssemblyFromName(string assemblyName)
    {
        ArgumentNullException.ThrowIfNull(assemblyName);

        return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == assemblyName) ?? throw new InvalidOperationException($"Assembly with name '{assemblyName}' not found in current AppDomain.");
    }
}