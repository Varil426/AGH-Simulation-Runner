namespace SimulationStandard.Interfaces;

/// <summary>
/// Simulation Builder Interface. Should provide parameterless constructor.
/// </summary>
public interface ISimulationBuilder
{
    /// <summary>
    /// Create an instance of a simulation.
    /// </summary>
    /// <param name="simulationParams">Params to be used by a simulation.</param>
    /// <returns>Simulation instance.</returns>
    ISimulation CreateSimulation(ISimulationParams simulationParams);

    /// <summary>
    /// Creates <see cref="ISimulationParams"/> template.
    /// </summary>
    /// <returns>Params template.</returns>
    public SimulationParamsTemplate CreateSimulationParamsTemplate();

    /// <summary>
    /// Creates <see cref="ISimulationResults"/> template.
    /// </summary>
    /// <returns>Results template.</returns>
    public SimulationResultsTemplate CreateSimulationResultsTemplate();
}