namespace SimulationStandard;

/// <summary>
/// Defines a simulation.
/// </summary>
public interface ISimulation : IDisposable
{
    /// <summary>
    /// Params used by a simulation.
    /// </summary>
    public ISimulationParams Params { get; init; }

    /// <summary>
    /// Starts a simulation.
    /// </summary>
    /// <returns>Simulation results.</returns>
    public ISimulationResults Run();

    /// <summary>
    /// Creates <see cref="ISimulationResults"/> template.
    /// </summary>
    /// <returns>Results template.</returns>
    public ISimulationResults CreateSimulationResultsTemplate();
}