namespace SimulationStandard;

/// <summary>
/// Defines parameters used by <see cref="ISimulation"/>.
/// </summary>
public interface ISimulationParams
{
    /// <summary>
    /// Simulation parameters.
    /// </summary>
    SimulationValuesDictionary Params { get; init; }
}