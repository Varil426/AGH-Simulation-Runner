using Application.Simulation;
using BackendAPI.Security.Policies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace BackendAPI.Controllers;

public class SimulationController : BaseController
{
    [HttpGet]
    public async Task<List.SimulationCollection> GetAllSimulations() => await Mediator.Send(new List.Query());

    [HttpPost]
    public async Task<SimulationDto> CreateSimulation([FromForm] Create.Command command) => await Mediator.Send(command);

    [Authorize(Policy = nameof(IsOwnerRequirement))]
    [HttpPost("{simulationId}/run")]
    public async Task RunSimulation(Guid simulationId, List<Dictionary<string, JsonElement>> parameters) => await Mediator.Send(new Run.Command { SimulationId = simulationId, Parameters = parameters });
}