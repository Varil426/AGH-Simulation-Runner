using Domain;
using Microsoft.AspNetCore.Components;

namespace Application.SimulationResults;

public record SimulationResultsDto
{
    public SimulationResultsDto()
    {
    }

    public SimulationResultsDto(SimulationRunAttempt simulationRunAttempt)
    {
        StartTime = simulationRunAttempt.Start;
        EndTime = simulationRunAttempt.End;
        Finished = simulationRunAttempt.End != null;
        RunAttemptNumber = simulationRunAttempt.AttemptNumer;
        
        foreach (var parameterValue in simulationRunAttempt.ParamValues)
            Parameters[parameterValue.SimulationParamTemplate.Name] = parameterValue.IsCollection ? parameterValue.Values.OrderBy(x => x.Index).Select(x => x.Value) : parameterValue.Value ?? string.Empty;

        foreach (var resultValue in simulationRunAttempt.ResultValues)
            Results[resultValue.SimulationResultTemplate.Name] = resultValue.IsCollection ? resultValue.Values.OrderBy(x => x.Index).Select(x => x.Value) : resultValue.Value ?? string.Empty;
    }

    public DateTime? StartTime { get; init; }
    public DateTime? EndTime { get; init; }
    public bool Finished { get; init; }
    public int RunAttemptNumber { get; init; }
    public Dictionary<string, object> Parameters { get; } = new();
    public Dictionary<string, object> Results { get; } = new();
}