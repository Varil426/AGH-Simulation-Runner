using SimulationStandard.Interfaces;
using System.Reflection;

namespace SimulationHandler;

public interface ISimulationHandler : IDisposable
{
    static string SimulationFileFileName => "simulation";
    static string SimulationParametersFileName => "parameters";
    static string SimulationResultsFileName => "results";
    static string SimulationParametersTemplateFileName => "parametersTemplate";
    static string SimulationResultsTemplateFileName => "resultsTemplate";
    static string JsonFileExtension => "json";

    ISimulationResultsTemplate CreateSimulationResultsTemplate(string json);
    ISimulationParamsTemplate CreateSimulationParamsTemplate(string json);
    ISimulationParams CreateSimulationParams(string json, ISimulationParamsTemplate simulationParamsTemplate);
    ISimulationResults CreateSimulationResults(string json, ISimulationResultsTemplate simulationResultsTemplate);
    string ToJson(ISimulationParams simulationParams);
    string ToJson(ISimulationResults simulationResults);
    string ToJson(ISimulationParamsTemplate simulationParamsTemplate);
    string ToJson(ISimulationResultsTemplate simulationResultsTemplate);

    /// <summary>
    /// Checks simulation assemby.
    /// </summary>
    /// <param name="assemblyPath">Path to assembly.</param>
    /// <param name="errors">List of errors.</param>
    /// <returns><see langword="true"/> if there are no errors; <see langword="false"/> otherwise.</returns>
    bool CheckSimulationAssembly(string assemblyPath, out List<string> errors);
    
    ISimulationBuilder CreateSimulationBuilder(Assembly assembly);
    ISimulationBuilder CreateSimulationBuilder(string assemblyPath);

    Assembly LoadSimulationAssembly(string assemblyPath);

    void UnloadAllLoadedAssemblies();
}