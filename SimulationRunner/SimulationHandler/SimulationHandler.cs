using SimulationStandard;
using SimulationStandard.Interfaces;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;

namespace SimulationHandler;

public class SimulationHandler : ISimulationHandler
{
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    private readonly AssemblyLoadContext _assemblyLoadContext;

    private bool _disposed;

    private bool IsValidSimulationBuilderClass(Type type) => type.IsClass && !type.IsAbstract && type.IsAssignableTo(typeof(ISimulationBuilder)) && type.GetConstructors().Any(constructorInfo => !constructorInfo.GetParameters().Any());

    private bool IsValidSimulationClass(Type type) => type.IsClass && !type.IsAbstract && type.IsAssignableTo(typeof(ISimulation));

    public SimulationHandler()
    {
        _assemblyLoadContext = new(null, true);
        _jsonSerializerOptions = new();
        _jsonSerializerOptions.Converters.Add(new JsonConverters.TypeJsonConverter());
    }

    public bool CheckSimulationAssembly(string assemblyPath, out List<string> errors)
    {
        errors = new();

        try
        {
            var assembly = LoadSimulationAssembly(assemblyPath);
            var exportedTypes = assembly.ExportedTypes;

            if (exportedTypes.Where(IsValidSimulationClass).Count() != 1)
                errors.Add($"Missing implementation of {typeof(ISimulation)} or more than 1 implementation.");

            if (exportedTypes.Where(IsValidSimulationBuilderClass).Count() != 1)
                errors.Add($"Missing implementation of {typeof(ISimulationBuilder)}, more than 1 implementation or missing parameterless constructor.");
        
            
        }
        catch (Exception ex)
        {
            errors.Add(ex.Message);
        }

        return !errors.Any();
    }

    public Assembly LoadSimulationAssembly(string assemblyPath) => _assemblyLoadContext.LoadFromAssemblyPath(assemblyPath) ?? throw new Exception("Assembly load failed");

    public ISimulationBuilder CreateSimulationBuilder(string assemblyPath) => CreateSimulationBuilder(LoadSimulationAssembly(assemblyPath));

    public ISimulationBuilder CreateSimulationBuilder(Assembly assembly) => (Activator.CreateInstance(assembly.ExportedTypes.FirstOrDefault(IsValidSimulationBuilderClass) ?? throw new Exception()) as ISimulationBuilder)!;

    public ISimulationParams CreateSimulationParams(string json, ISimulationParamsTemplate simulationParamsTemplate) => JsonSerializer.Deserialize<ISimulationParams>(json, _jsonSerializerOptions) ?? throw new ArgumentException("Invalid JSON");

    public ISimulationParamsTemplate CreateSimulationParamsTemplate(string json) => JsonSerializer.Deserialize<ISimulationParamsTemplate>(json, _jsonSerializerOptions) ?? throw new ArgumentException("Invalid JSON");

    public ISimulationResults CreateSimulationResults(string json, ISimulationResultsTemplate simulationResultsTemplate) => JsonSerializer.Deserialize<ISimulationResults>(json, _jsonSerializerOptions) ?? throw new ArgumentException("Invalid JSON");

    public ISimulationResultsTemplate CreateSimulationResultsTemplate(string json) => JsonSerializer.Deserialize<ISimulationResultsTemplate>(json, _jsonSerializerOptions) ?? throw new ArgumentException("Invalid JSON");

    public string ToJson(ISimulationParams simulationParams) => JsonSerializer.Serialize(simulationParams, _jsonSerializerOptions);

    public string ToJson(ISimulationResults simulationResults) => JsonSerializer.Serialize(simulationResults, _jsonSerializerOptions);

    public string ToJson(ISimulationParamsTemplate simulationParamsTemplate) => JsonSerializer.Serialize(simulationParamsTemplate, _jsonSerializerOptions);

    public string ToJson(ISimulationResultsTemplate simulationResultsTemplate) => JsonSerializer.Serialize(simulationResultsTemplate, _jsonSerializerOptions);

    /// <summary>
    /// Unloads all loaded assemblies.
    /// </summary>
    public void UnloadAllLoadedAssemblies() => _assemblyLoadContext.Unload();

    public virtual void Dispose() => Dispose(true);

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _assemblyLoadContext.Unload();
            }

            _disposed = true;
        }
    }

    ~SimulationHandler() => Dispose(false);
}