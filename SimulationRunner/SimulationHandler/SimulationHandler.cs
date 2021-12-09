using Newtonsoft.Json;
using SimulationStandard;
using SimulationStandard.Interfaces;
using System.Reflection;
using System.Runtime.Loader;

namespace SimulationHandler;

public class SimulationHandler : ISimulationHandler
{
    private readonly JsonSerializerSettings _jsonSerializerSettings;

    private readonly JsonSerializer _jsonSerializer;

    private readonly AssemblyLoadContext _assemblyLoadContext;

    private bool _disposed;

    private bool IsValidSimulationBuilderClass(Type type) => type.IsClass && !type.IsAbstract && type.IsAssignableTo(typeof(ISimulationBuilder)) && type.GetConstructors().Any(constructorInfo => !constructorInfo.GetParameters().Any());

    private bool IsValidSimulationClass(Type type) => type.IsClass && !type.IsAbstract && type.IsAssignableTo(typeof(ISimulation));

    public SimulationHandler()
    {
        _assemblyLoadContext = new(null, true);
        _jsonSerializerSettings = new()
        {
            TypeNameHandling = TypeNameHandling.All
        };

        _jsonSerializer = JsonSerializer.CreateDefault(_jsonSerializerSettings);
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

    public Assembly LoadSimulationAssembly(string assemblyPath)
    {
        using var fileStream = new FileStream(assemblyPath, FileMode.Open);
        return _assemblyLoadContext.LoadFromStream(fileStream) ?? throw new Exception("Assembly load failed");
    }

    public ISimulationBuilder CreateSimulationBuilder(string assemblyPath) => CreateSimulationBuilder(LoadSimulationAssembly(assemblyPath));

    public ISimulationBuilder CreateSimulationBuilder(Assembly assembly) => (Activator.CreateInstance(assembly.ExportedTypes.FirstOrDefault(IsValidSimulationBuilderClass) ?? throw new Exception()) as ISimulationBuilder)!;

    public ISimulationParams CreateSimulationParams(string json)
    {
        using var reader = CreateJsonReader(json);
        return _jsonSerializer.Deserialize<SimulationParams>(reader) ?? throw new ArgumentException("Invalid JSON");
    }

    public ISimulationParamsTemplate CreateSimulationParamsTemplate(string json)
    {
        using var reader = CreateJsonReader(json);
        return _jsonSerializer.Deserialize<SimulationParamsTemplate>(reader) ?? throw new ArgumentException("Invalid JSON");
    }

    public ISimulationResults CreateSimulationResults(string json)
    {
        using var reader = CreateJsonReader(json);
        return _jsonSerializer.Deserialize<SimulationResults>(reader) ?? throw new ArgumentException("Invalid JSON");
    }

    public ISimulationResultsTemplate CreateSimulationResultsTemplate(string json)
    {
        using var reader = CreateJsonReader(json);
        return _jsonSerializer.Deserialize<SimulationResultsTemplate>(reader) ?? throw new ArgumentException("Invalid JSON");
    }

    public string ToJson(ISimulationParams simulationParams) => SerializeObject(simulationParams);
    public string ToJson(ISimulationResults simulationResults) => SerializeObject(simulationResults);
    public string ToJson(ISimulationParamsTemplate simulationParamsTemplate) => SerializeObject(simulationParamsTemplate);
    public string ToJson(ISimulationResultsTemplate simulationResultsTemplate) => SerializeObject(simulationResultsTemplate);

    /// <summary>
    /// Unloads all loaded assemblies.
    /// </summary>
    public void UnloadAllLoadedAssemblies() => _assemblyLoadContext.Unload();
    
    public virtual void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private static JsonReader CreateJsonReader(string json) => new JsonTextReader(new StringReader(json));

    private string SerializeObject(object @object)
    {
        using var jsonWriter = new StringWriter();
        _jsonSerializer.Serialize(jsonWriter, @object);
        return jsonWriter.ToString();
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
            }

            _assemblyLoadContext.Unload();
            _disposed = true;
        }
    }

    ~SimulationHandler() => Dispose(false);
}