using Application.SimulationResults;
using BackendAPI.Security.Policies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Application.SimulationResults.List;

namespace BackendAPI.Controllers;

public class ResultsController : BaseController
{

    [Authorize(Policy = nameof(IsOwnerRequirement))]
    [HttpGet("{simulationId}")]
    public async Task<ResultsCollection> RetrieveAllSimulationResults(Guid simulationId) => await Mediator.Send(new List.Query { SimulationId = simulationId });

    [Authorize(Policy = nameof(IsOwnerRequirement))]
    [HttpGet("{simulationId}/{runAttemptNumber}")]
    public async Task<SimulationResultsDto> RetrieveAttemptNumber(Guid simulationId, int runAttemptNumber) => await Mediator.Send(new Get.Query { SimulationId = simulationId, AttemptNumber = runAttemptNumber});
}