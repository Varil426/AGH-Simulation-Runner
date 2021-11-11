namespace SimulationStandard.Interfaces;

/// <summary>
/// Defines results returned by <see cref="ISimulation"/>.
/// </summary>
public interface ISimulationResults
{
    /// <summary>
    /// Simulation results.
    /// </summary>
    SimulationValuesDictionary Results { get; init; }
}
