using SimulationStandard;
using SimulationStandard.Interfaces;

namespace MockSimulation;

public class SimulationBuilder : ISimulationBuilder
{
    public ISimulation CreateSimulation(ISimulationParams simulationParams) => new MockSimulation { Params = simulationParams };

    public SimulationParamsTemplate CreateSimulationParamsTemplate() => new SimulationParamsTemplate
    {
        [ParamsHelper.InputParams.InputDouble.ToString()] = typeof(double),
        [ParamsHelper.InputParams.InputLong.ToString()] = typeof(long),
        [ParamsHelper.InputParams.InputString.ToString()] = typeof(string),
        [ParamsHelper.InputParams.InputListLong.ToString()] = typeof(IList<long>)
    };

    public SimulationResultsTemplate CreateSimulationResultsTemplate() => new SimulationResultsTemplate
    {
        [ParamsHelper.OutputParams.OutputLong.ToString()] = typeof(long),
        [ParamsHelper.OutputParams.OutputListDoubleTime.ToString()] = typeof(IList<double>),
        [ParamsHelper.OutputParams.OutputListLongValues.ToString()] = typeof(IList<long>)
    };
}