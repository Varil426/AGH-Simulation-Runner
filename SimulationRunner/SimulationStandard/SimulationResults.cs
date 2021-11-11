using SimulationStandard.Interfaces;

namespace SimulationStandard;

/// <summary>
/// Default implementation of <see cref="ISimulationResults"/>.
/// </summary>
public class SimulationResults : ISimulationResults
{

    public SimulationResults()
    {
        Results = new();
    }

    public SimulationValuesDictionary Results { get; init; }
}