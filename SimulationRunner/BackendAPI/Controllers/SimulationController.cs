using Application.Simulation;
using Domain;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace BackendAPI.Controllers;

public class SimulationController : BaseController
{
    [HttpGet]
    public async Task<List.SimulationCollection> GetAllSimulations() => await Mediator.Send(new List.Query());

    [HttpPost]
    public async Task<SimulationDto> CreateSimulation([FromForm] Create.Command command) => await Mediator.Send(command);

    // TODO Add policy IsSimulationOwner
    [HttpPost("{simulationId}/run")]
    public async Task RunSimulation(Guid simulationId, List<Dictionary<string, JsonElement>> parameters) => await Mediator.Send(new Run.Command { SimulationId = simulationId, Parameters = parameters });
}