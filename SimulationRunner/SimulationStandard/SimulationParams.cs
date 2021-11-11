using SimulationStandard.Interfaces;

namespace SimulationStandard;

/// <summary>
/// Default implementation of <see cref="ISimulationParams"/>.
/// </summary>
public class SimulationParams : ISimulationParams
{
    public SimulationParams()
    {
        Params = new();
    }

    public SimulationValuesDictionary Params { get; init; }
}
