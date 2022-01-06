using Application.Interfaces;
using System.Text.Json;

namespace Application.Simulation;

public class Run
{
    public class Command : IRequest
    {
        public Guid SimulationId { get; set; }
        public List<Dictionary<string, JsonElement>> Parameters { get; set; } = null!;
    }

    public class Handler : IRequestHandler<Command>
    {
        private readonly DataContext _dataContext;
        private readonly IDockerContainerManager _dockerContainerManager;

        public Handler(DataContext dataContext, IDockerContainerManager dockerContainerManager)
        {
            _dataContext = dataContext;
            _dockerContainerManager = dockerContainerManager;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var simulation = await _dataContext.Simulations.FirstOrDefaultAsync(simulation => simulation.Id == request.SimulationId)
                ?? throw new RestException(System.Net.HttpStatusCode.BadRequest, new { Simulation = "Not found" });

            foreach (var parameters in request.Parameters)
            {
                await _dockerContainerManager.RunSimulationAsync(simulation, _dataContext, parameters);
            }

            return Unit.Value;
        }
    }
}