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
    public async Task<ResultsCollection> RetrieveAllSimulationResults(Guid simulationId) => await Mediator.Send(new Query { SimulationId = simulationId });
}