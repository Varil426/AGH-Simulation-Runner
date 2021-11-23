using Application.Simulation;
using Microsoft.AspNetCore.Mvc;

namespace BackendAPI.Controllers;

public class SimulationController : BaseController
{
    [HttpGet]
    public async Task<List.SimulationCollection> GetAllSimulations() => await Mediator.Send(new List.Query());

}