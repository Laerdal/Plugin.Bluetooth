
namespace Plugin.Bluetooth.Maui.Helpers;

/// <summary>
///   Helper methods for working with assemblies.
/// </summary>
public static class AssemblyHelpers
{
    /// <summary>
    ///   Gets an assembly from its name.
    /// </summary>
    /// <param name="assemblyName"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static Assembly GetAssemblyFromName(string assemblyName)
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name == assemblyName)
               ?? throw new InvalidOperationException($"Assembly with name '{assemblyName}' not found in current AppDomain.");
    }
}
