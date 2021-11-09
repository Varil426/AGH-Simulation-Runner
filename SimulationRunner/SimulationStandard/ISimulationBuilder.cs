namespace SimulationStandard;

/// <summary>
/// Simulation Builder Interface.
/// </summary>
public interface ISimulationBuilder
{
    /// <summary>
    /// Create an instance of a simulation.
    /// </summary>
    /// <param name="simulationParams">Params to be used by a simulation.</param>
    /// <returns>Simulation instance.</returns>
    ISimulation CreateSimulation(ISimulationParams simulationParams);
}