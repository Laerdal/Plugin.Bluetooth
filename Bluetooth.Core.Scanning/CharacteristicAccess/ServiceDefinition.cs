using System.Reflection;

using Plugin.BaseTypeExtensions;

namespace Bluetooth.Core.Scanning.CharacteristicAccess;

/// <summary>
/// Attribute used to automatically declare a Bluetooth service definition.
/// </summary>
/// <example>
/// [ServiceDefinitionAttribute]
/// public static class BatteryServiceDefinition
/// {
///     public static readonly Guid Id = new Guid("0000180F-0000-1000-8000-00805F9B34FB");
///     public static readonly string Name = "Battery Service";
/// }
/// </example>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ServiceDefinitionAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the name of the string property containing the service's name.
    /// </summary>
    /// <value>The default value is "Name".</value>
    public string NameField { get; set; } = "Name";

    /// <summary>
    /// Gets or sets the name of the Guid property containing the service's identifier.
    /// </summary>
    /// <value>The default value is "Id".</value>
    public string IdField { get; set; } = "Id";

    /// <summary>
    /// Finds all classes with the ServiceDefinition attribute in the specified assembly.
    /// </summary>
    /// <param name="assembly">The assembly to search in.</param>
    /// <returns>An enumerable of tuples containing the ServiceDefinitionAttribute and the associated Type.</returns>
    public static IEnumerable<(ServiceDefinitionAttribute, Type)> GetAllServiceDefinitionsInAssemblyOf(Assembly assembly)
    {
        // Find all classes with the ServiceDefinition attribute
        // It returns the type of the class and the attribute
        return assembly.GetTypesWithAttribute<ServiceDefinitionAttribute>();
    }

    /// <summary>
    /// Asynchronously finds all classes with the ServiceDefinition attribute in the specified assembly.
    /// </summary>
    /// <param name="assembly">The assembly to search in.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task representing the asynchronous operation, containing a list of tuples with the ServiceDefinitionAttribute and the associated Type.</returns>
    public static Task<List<(ServiceDefinitionAttribute, Type)>> GetAllServiceDefinitionsInAssemblyOfAsync(Assembly assembly, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        // Find all classes with the ServiceDefinition attribute
        // It returns the type of the class and the attribute
        return assembly.GetTypesWithAttributeAsync<ServiceDefinitionAttribute>().WaitBetterAsync(timeout, cancellationToken);
    }
}
