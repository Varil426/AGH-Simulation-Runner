using SimulationStandard;
using SimulationStandard.Interfaces;

namespace MockSimulation;
public class MockSimulation : ISimulation
{
    internal MockSimulation()
    {
    }
    public ISimulationParams Params { get; init; } = null!;

    public void Dispose()
    {
        // Nothing to dispose
    }

    public ISimulationResults Run()
    {
        var rand = new Random();
        var time = new List<double>();
        var values = new List<long>();

        for (var i = 1; i < 11; i++)
        {
            time.Add(rand.NextDouble() * i);
            values.Add(rand.Next(0, i));
        }

        var results = new SimulationResults();
        results.Results[ParamsHelper.OutputParams.OutputLong.ToString()] = Params.Params.Count;
        results.Results[ParamsHelper.OutputParams.OutputListDoubleTime.ToString()] = time;
        results.Results[ParamsHelper.OutputParams.OutputListLongValues.ToString()] = values;
        Thread.Sleep(500);

        return results;
    }
}
